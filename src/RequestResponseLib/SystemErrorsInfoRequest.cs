using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.requestresponse
{
    public class EventLogData
    {
        public ulong? MaxRecordNumber { get; set; }
        public Dictionary<DateTime, List<ulong>> MaxDateToRecordNumbers { get; set; }
        [JsonIgnore]
        public DateTime? MaxDate
        {
            get
            {
                DateTime? max = null;
                foreach (DateTime date in MaxDateToRecordNumbers.Keys)
                {
                    if (max.HasValue == false || date > max.Value)
                        max = date;
                }
                return max;
            }
        }

        public EventLogData()
        {
            MaxRecordNumber = null;
            MaxDateToRecordNumbers = new Dictionary<DateTime, List<ulong>>();
        }

        public void Assign(EventLogData id)
        {
            MaxRecordNumber = id.MaxRecordNumber;
            MaxDateToRecordNumbers = new Dictionary<DateTime, List<ulong>>(id.MaxDateToRecordNumbers);
        }

        public bool ContainsRecordNumber(ulong record_number)
        {
            bool contains = MaxRecordNumber.HasValue && record_number == MaxRecordNumber.Value;
            if (contains == false)
            {
                foreach (KeyValuePair<DateTime, List<ulong>> records in MaxDateToRecordNumbers)
                {
                    contains = records.Value.Contains(record_number);
                    if (contains)
                        break;
                }
            }
            return contains;
        }

        public void Insert(DateTime date, ulong record_number)
        {
            if(MaxDateToRecordNumbers.TryGetValue(date, out List<ulong> record_numbers) == false)
            {
                record_numbers = new List<ulong>();
                MaxDateToRecordNumbers[date] = record_numbers;
            }

            if(record_numbers.Contains(record_number) == false)
                record_numbers.Add(record_number);
        }

        public void Cleanup()
        {
            MaxRecordNumber = null;
            List<DateTime> dates = new List<DateTime>(MaxDateToRecordNumbers.Keys);
            // Reverse sort, then remove everything but the first one
            dates.Sort((a, b) => b.CompareTo(a));
            while(dates.Count > 1)
            {
                DateTime date = dates[1];
                dates.RemoveAt(1);
                MaxDateToRecordNumbers.Remove(date);
            }
        }
    }

    /// <summary>
    /// Request made to get the highest system/application event # from
    /// the event lot. Is used by the DataLib when it's collecting
    /// the system/application error logs so it doesn't collect logs
    /// that have already been collected.
    /// 
    /// The system/application log event #s are stored in the database,
    /// but we didn't want the DataLib to have a dependency upon the
    /// database lib.
    /// </summary>
    public class SystemErrorsInfoRequest : Request
    {
        public enum EType
        {
            System,
            Application,
        }

        public string MachineName { get; private set; }
        public EType Type { get; private set; }
        public EventLogData LogData { get; private set; }

        public SystemErrorsInfoRequest(string machine_name, EType type)
            : base("SystemErrorsInfoRequest")
        {
            MachineName = machine_name;
            Type = type;
            LogData = new EventLogData();
        }
    }

    /// <summary>
    /// Request made to update the highest system/application event #s
    /// 
    /// Is used when the DataLib has collected the log data but needs to store
    /// the highest values it has collected.
    /// </summary>
    public class SystemErrorsUpdateRequest : Request
    {
        public enum EType
        {
            System,
            Application,
        }

        public string MachineName { get; private set; }
        public EType Type { get; private set; }
        public EventLogData LogData { get; set; }

        public SystemErrorsUpdateRequest(string machine_name, EType type)
            : base("SystemErrorsUpdateRequest")
        {
            MachineName = machine_name;
            Type = type;
            LogData = new EventLogData();
        }
    }
}
