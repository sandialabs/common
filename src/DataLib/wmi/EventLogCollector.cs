using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.requestresponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Text;

namespace gov.sandia.sld.common.data.wmi
{
    public class EventLogRecord
    {
        private string _timeGeneratedString;
        private DateTime _timeGenerated;

        public ulong RecordNumber { get; private set; }
        public string Message { get; private set; }
        public string SourceName { get; private set; }
        public uint EventCode { get; private set; }
        public string TimeGenerated { get { return _timeGenerated.ToString("o"); } }

        // The 'raw' date time we get from the EventLog.
        [JsonIgnore]
        public string TimeGeneratedString
        {
            get
            {
                return _timeGeneratedString;
            }
            set
            {
                _timeGeneratedString = value;
                _timeGenerated = ManagementDateTimeConverter.ToDateTime(_timeGeneratedString);
            }
        }
        [JsonIgnore]
        public DateTime TimeGeneratedAsDateTimeOffset { get { return _timeGenerated; } }
        [JsonIgnore]
        public uint EventType { get; private set; }

        public EventLogRecord(Dictionary<string, object> dict)
        {
            if (dict.TryGetValue("RecordNumber", out object record_number) &&
                dict.TryGetValue("TimeGenerated", out object time) &&
                dict.TryGetValue("SourceName", out object source) &&
                dict.TryGetValue("EventCode", out object event_code) &&
                dict.TryGetValue("EventType", out object event_type))
            {
                string record_number_str = record_number.ToString();
                if (ulong.TryParse(record_number_str, out ulong r))
                {
                    RecordNumber = r;
                    TimeGeneratedString = time.ToString();
                    SourceName = source.ToString();

                    // Sometimes "Message" is missing, which should be OK
                    Message = dict.TryGetValue("Message", out object message) ? message.ToString() : string.Empty;
                    EventCode = uint.TryParse(event_code.ToString(), out uint ecode) ? ecode : 0;
                    EventType = uint.TryParse(event_type.ToString(), out uint type) ? type : 0;
                }
                else
                    throw new Exception($"EventLogRecord: invalid RecordNumber {record_number_str}");
            }
            else
                throw new Exception($"EventLogRecord: unable to extract from dictionary");
        }

        public Data AsData(DataCollectorContext context)
        {
            DictionaryData d = new DictionaryData(context);
            d.Data["Message"] = Message;
            d.Data["TimeGenerated"] = TimeGeneratedAsDateTimeOffset.ToString("o");
            d.Data["Source"] = SourceName;
            d.Data["RecordNumber"] = RecordNumber.ToString();
            d.Data["EventCode"] = EventCode.ToString();
            return d;
        }
    }

    public class GenericEventLogCollector
    {
        public enum ELogs
        {
            System,
            Application,
        }

        public WMIContext Context { get; private set; }
        public ELogs Log { get; private set; }

        public GenericEventLogCollector(Remote remote_info, ELogs log, bool use_event_type_clause, EventLogData el_data)
        {
            Context = new WMIContext("Win32_NTLogEvent", "RecordNumber,Message,TimeGenerated,SourceName,EventCode,EventType", remote_info);
            Log = log;

            GenerateWhereClause(use_event_type_clause, el_data);
        }

        public List<EventLogRecord> Retrieve()
        {
            List<EventLogRecord> records = new List<EventLogRecord>();
            WMIRetrieverOptions options = new WMIRetrieverOptions();
            WMIRetriever retriever = new WMIRetriever(Context, options);

            Stopwatch watch = Stopwatch.StartNew();
            WMIRetriever.RetrievalContext retrieval_context = retriever.Retrieve(null);
            long retrieval = watch.ElapsedMilliseconds;

            watch.Restart();
            if (retrieval_context != null)
            {
                lock (retrieval_context.RetrievedData)
                {
                    retrieval_context.RetrievedData.ForEach(r =>
                    {
                        try
                        {
                            records.Add(new EventLogRecord(r));
                        }
                        catch (Exception)
                        {
                        }
                    });
                }
            }

            //ApplicationEventLog log = new ApplicationEventLog();
            //log.LogInformation($"GenericEventLogCollector: retrieval took {retrieval} ms and building list took {watch.ElapsedMilliseconds} ms for {records.Count} records");

            return records;
        }

        private void GenerateWhereClause(bool use_event_type_clause, EventLogData el_data)
        {
            string cutoff = el_data.MaxDate.HasValue ? ManagementDateTimeConverter.ToDmtfDateTime(el_data.MaxDate.Value) : string.Empty;
            if (string.IsNullOrEmpty(cutoff))
            {
                // When we first start COMMON, it will collect errors from ages ago. To keep that from happening, collect
                // errors from at most 45 days ago.
                DateTime cutoff_dt = DateTime.Now - TimeSpan.FromDays(45);
                cutoff = ManagementDateTimeConverter.ToDmtfDateTime(cutoff_dt);
            }

            Context.Where = $"TimeGenerated >= '{cutoff}' AND LogFile = '{Log.ToString()}'";

            // EventType of 1 is errors
            // https://msdn.microsoft.com/en-us/library/aa394226.aspx
            if (use_event_type_clause)
            {
                Context.Where += " AND EventType = 1";

                if (el_data.MaxRecordNumber.HasValue)
                    Context.Where += $" AND RecordNumber >= {el_data.MaxRecordNumber.Value}";
            }

            Trace.WriteLine($"Where: {Context.Where}");
        }
    }

    public class EventLogCollector : DataCollector
    {
        public ECollectorType CollectorType { get; private set; }
        public Remote RemoteInfo { get; private set; }

        public EventLogCollector(CollectorID id, Remote remote_info, ECollectorType type)
            : base(new DataCollectorContext(id, type))
        {
            RemoteInfo = remote_info;

            m_machine_name = Context.ID.Name;
            CollectorType = type;

            if (type != ECollectorType.SystemErrors && type != ECollectorType.ApplicationErrors)
                throw new Exception($"EventLogCollector: invalid collector type {type}");

            // So we don't keep getting log entries that have already been retrieved, keep track
            // of the RecordNumber, which is an ever-increasing number for each of the different
            // types of logfiles (System, or Application), and the dates.

            // We need to keep the dates because if a particular machine is rebuilt, the RecordNumber
            // will get reset and we'll stop getting any errors from the event log.
            // Originally, we just kept the record number, but that's not good enough, so we
            // switched to using the date as the primary clause.

            m_log_data = new EventLogData();

            SystemErrorsInfoRequest.EType req_type = SystemErrorsInfoRequest.EType.System;
            switch (CollectorType)
            {
                case ECollectorType.ApplicationErrors:
                    req_type = SystemErrorsInfoRequest.EType.Application;
                    break;
                case ECollectorType.SystemErrors:
                    req_type = SystemErrorsInfoRequest.EType.System;
                    break;
                default:
                    throw new Exception($"EventLogCollector: failure to handle type conversion from {CollectorType}");
            }

            SystemErrorsInfoRequest request = new SystemErrorsInfoRequest(m_machine_name, req_type);
            SystemBus.Instance.MakeRequest(request);
            if (request.IsHandled)
                m_log_data.Assign(request.LogData);
        }

        public override CollectedData OnAcquire()
        {
            List<Data> data = new List<Data>();

            GenericEventLogCollector.ELogs logfile = GenericEventLogCollector.ELogs.System;
            if (Context.Type == ECollectorType.ApplicationErrors)
                logfile = GenericEventLogCollector.ELogs.Application;

            // In order to figure out if the query is failing (due to a credentials problem or the machine
            // being offline, for example), or there just not being any new errors since the last
            // time we queried, ask for RecordNumber >= the max record # we've
            // already received. This way, we should get at least 1 record that we can ignore if
            // we've already received it.
            // https://docs.microsoft.com/en-us/windows/desktop/WmiSdk/wql-operators

            Stopwatch watch = Stopwatch.StartNew();

            GenericEventLogCollector collector = new GenericEventLogCollector(RemoteInfo, logfile, true, m_log_data);
            List<EventLogRecord> records = collector.Retrieve();

            if(records.Count == 0)
            {
                // Nothing new...why? I've seen two conditions:
                // 1) The machine has been rebuilt and the records numbers on that machine are
                //  lower than what we've already received. As long as the time field is used
                //  that shouldn't be a problem.
                // 2) There haven't been any system/application errors since the date or record number
                //  we got when we did the SystemErrorsInfoRequest.
                // To solve this let's re-gather events, but don't limit the query to EventType=1 (errors)
                // and just grab the most recent event of whatever type.
                collector = new GenericEventLogCollector(RemoteInfo, logfile, false, m_log_data);
                records = collector.Retrieve();
            }

            records.ForEach(r =>
            {
                // If somehow the RecordNumber < the max record # we
                // have so far, then it's likely the machine was rebuilt
                // or the record number rolled over (hard to do when the max
                // is 18,446,744,073,709,551,615, that means if there were 100
                // events per second, the number would roll over after
                // 5.8 billion years...not gonna worry about that).

                Trace.WriteLine(JsonConvert.SerializeObject(r));

                // Make sure we always insert the record into m_log_data even if it's
                // not an error record. But don't put it in data unless it's an error record.
                if (r.EventType == 1 && m_log_data.ContainsRecordNumber(r.RecordNumber) == false)
                    data.Add(r.AsData(Context));

                m_log_data.Insert(r.TimeGeneratedAsDateTimeOffset, r.RecordNumber);
            });

            long elapsed = watch.ElapsedMilliseconds;
            long ms_per_record = elapsed / (data.Count == 0 ? 1 : data.Count);
            string msg = $"EventLogCollector [{m_machine_name} -- {logfile}]: collected {data.Count} of {records.Count} in {elapsed} ms ({ms_per_record} ms per record)";
            m_log.Debug(msg);

            //ApplicationEventLog log = new ApplicationEventLog();
            //log.LogInformation(msg);

            m_log_data.Cleanup();

            SystemErrorsUpdateRequest.EType req_type = SystemErrorsUpdateRequest.EType.System;
            switch (CollectorType)
            {
                case ECollectorType.ApplicationErrors:
                    req_type = SystemErrorsUpdateRequest.EType.Application;
                    break;
                case ECollectorType.SystemErrors:
                    req_type = SystemErrorsUpdateRequest.EType.System;
                    break;
                default:
                    throw new Exception($"EventLogCollector.OnAcquire: failure to handle type conversion from {CollectorType}");
            }

            SystemErrorsUpdateRequest request = new SystemErrorsUpdateRequest(m_machine_name, req_type);
            request.LogData.Assign(m_log_data);
            SystemBus.Instance.MakeRequest(request);

            // We want to report that we collected data if we got something, even if it wasn't an error record
            return new CollectedData(Context, records.Count > 0, data);
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            DictionaryData d = new DictionaryData(context);
            var definition = new { Value = new Dictionary<string, string>() };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d.Data = data.Value;
            return d;
        }

        private readonly string m_machine_name;
        private EventLogData m_log_data;
    }

    public class SystemErrorLogCollector : EventLogCollector
    {
        public SystemErrorLogCollector(CollectorID id, Remote remote_info)
            : base(id, remote_info, ECollectorType.SystemErrors)
        {
        }
    }

    public class ApplicationErrorLogCollector : EventLogCollector
    {
        public ApplicationErrorLogCollector(CollectorID id, Remote remote_info)
            : base(id, remote_info, ECollectorType.ApplicationErrors)
        {
        }
    }
}
