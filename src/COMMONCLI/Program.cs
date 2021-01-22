using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.data.database;
using gov.sandia.sld.common.data.wmi;
using gov.sandia.sld.common.requestresponse;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace COMMONCLI
{
    class Program
    {
        static Stopwatch watch;

        static void Main(string[] args)
        {
            //gov.sandia.sld.common.logging.EventLog.GlobalSource = "COMMONCLI";

            string ip_address = string.Empty;
            string username = string.Empty;
            string password = string.Empty;
            string connection_string = string.Empty;
            EDatabaseType db_type = EDatabaseType.Unknown;
            int repeat_count = 1;
            int pause_seconds_between_repeats = 1;
            List<string> to_collect = new List<string>();
            bool show_usage = false;

            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "/u":
                        if (i + 1 < args.Length)
                            username = args[++i].Trim();
                        break;
                    case "/p":
                        if (i + 1 < args.Length)
                            password = args[++i].Trim();
                        break;
                    case "/i":
                        if (i + 1 < args.Length)
                            ip_address = args[++i].Trim();
                        break;
                    case "/d":
                        {
                            switch(args[++i].Trim().ToLower())
                            {
                                case "sqlserver":
                                    db_type = EDatabaseType.SqlServer;
                                    break;
                                case "oracle":
                                    db_type = EDatabaseType.Oracle;
                                    break;
                                case "postgres":
                                    db_type = EDatabaseType.Postgres;
                                    break;
                            }
                            break;
                        }
                    case "/c":
                        connection_string = args[++i];
                        break;
                    case "/r":
                        int.TryParse(args[++i], out repeat_count);
                        break;
                    case "/s":
                        int.TryParse(args[++i], out pause_seconds_between_repeats);
                        break;
                    case "/?":
                        show_usage = true;
                        break;
                    default:
                        to_collect.Add(args[i]);
                        break;
                }
            }

            RequestBus.Instance.Subscribe(new PingResponder(ip_address));
            //RequestBus.Instance.Subscribe(new SystemErrorsInfoResponder());

            Remote r = null;
            string device_name = "local";

            if (string.IsNullOrEmpty(ip_address) == false &&
                string.IsNullOrEmpty(username) == false &&
                string.IsNullOrEmpty(password) == false)
            {
                r = new Remote(ip_address, username, password);
                device_name = ip_address;
            }

            CollectorID c_id = new CollectorID(-1, device_name);
            Dictionary<string, DataCollector> collector_map = new Dictionary<string, DataCollector>();
            collector_map["disk"] = new DiskUsageCollector(c_id, r);
            collector_map["disknames"] = new DiskNameCollector(c_id, r);
            collector_map["smart"] = new SMARTCollector(c_id, r);
            collector_map["memory"] = new MemoryUsageCollector(c_id, r);
            collector_map["cpu"] = new CPUUsageCollector(c_id, r);
            collector_map["nic"] = new NICUsageCollector(c_id, r);
            collector_map["uptime"] = new UptimeCollector(c_id, r);
            collector_map["boot"] = new LastBootTimeCollector(c_id, r);
            collector_map["processes"] = new ProcessesCollector(c_id, r);
            collector_map["applications"] = new ApplicationsCollector(c_id, r);
            collector_map["services"] = new ServicesCollector(c_id, r);
            collector_map["ups"] = new UPSCollector(c_id, r);
            collector_map["database"] = new DatabaseSizeCollector(c_id, false, new DatabaseCollectorFactory())
            {
                DBType = db_type,
                ConnectionString = connection_string
            };
            //collector_map["antivirus"] = new AntiVirusCollector(c_id, r);
            //collector_map["firewall"] = new FirewallCollector(c_id, r);
            collector_map["ping"] = new PingCollector(c_id);
            collector_map["systemerrors"] = new SystemErrorLogCollector(c_id, r);
            collector_map["applicationerrors"] = new ApplicationErrorLogCollector(c_id, r);

            if (show_usage)
            {
                ShowUsage(collector_map.Keys);
                return;
            }

            if (to_collect.Count == 0)
            {
                foreach (string collector in collector_map.Keys)
                    to_collect.Add(collector);
            }

            List<DataCollector> c = new List<DataCollector>();
            foreach (string collector in to_collect)
            {
                if (collector_map.TryGetValue(collector, out DataCollector c2))
                {
                    c2.AttachDataAcquiredHandler(OnDataAcquired);
                    c.Add(c2);
                }
                else
                    WriteLine($"Unknown collector: {collector}");
            }

            if(c.Count == 0)
            {
                ShowUsage(collector_map.Keys);
            }
            else
            {
                GlobalIsRunning.Start();

                for (int i = 0; i < repeat_count; ++i)
                {
                    foreach (DataCollector collector in c)
                    {
                        if(collector is PingCollector)
                            WriteLine($"Pinging {ip_address}");
                        else
                            WriteLine($"Collecting {collector.Context.Name}:");

                        watch = Stopwatch.StartNew();
                        collector.Acquire();
                    }

                    if ((i + 1) < repeat_count && pause_seconds_between_repeats > 0)
                        Thread.Sleep(pause_seconds_between_repeats * 1000);
                }
            }
        }

        static void OnDataAcquired(CollectedData data)
        {
            watch.Stop();
            long ms = watch.ElapsedMilliseconds;
            if(data == null)
            {
                WriteLine("Null collected data");
            }
            else if (data.DataIsCollected)
            {
                foreach (Data d in data.D)
                    HandleData(d);
            }
            else
            {
                WriteLine("Data collection failure");
                if (string.IsNullOrEmpty(data.Message) == false)
                    WriteLine(data.Message);
            }
            WriteLine($"Collection took {ms} ms");
        }

        static void HandleData(Data d)
        {
            WriteLine(JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented,
                new Newtonsoft.Json.JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }));

            switch (d.Type)
            {
                case ECollectorType.Memory:
                    break;
                case ECollectorType.Disk:
                    if (d is GenericDictionaryData<DiskUsage>)
                    {
                        Dictionary<string, DiskUsage> value = ((GenericDictionaryData<DiskUsage>)d).Data;
                        DiskSpaceStatus ds_status = new DiskSpaceStatus(80, 90);
                        EStatusType status = EStatusType.AdequateDiskSpace;
                        foreach (string drive in value.Keys)
                        {
                            DiskUsage data = value[drive];
                            Tuple<EStatusType, double> drive_status = ds_status.GetStatus(data.UsedNum, data.CapacityNum);
                            status = status.DiskSpaceCompare(drive_status.Item1);
                            WriteLine($"{drive} -- {drive_status.Item2:0.0} %");
                        }
                        WriteLine($"Status: {status}");
                    }
                    break;
                case ECollectorType.CPUUsage:
                    break;
                case ECollectorType.NICUsage:
                    break;
                case ECollectorType.Uptime:
                    break;
                case ECollectorType.LastBootTime:
                    break;
                case ECollectorType.Processes:
                    GenericDictionaryData<gov.sandia.sld.common.data.wmi.Process> processes = d as GenericDictionaryData<gov.sandia.sld.common.data.wmi.Process>;
                    ulong working_set = 0;
                    ulong working_set_private = 0;
                    ulong private_bytes = 0;
                    ulong memory = 0;
                    foreach(gov.sandia.sld.common.data.wmi.Process process in processes.Data.Values)
                    {
                        memory += process.MemoryNum;
                        working_set += process.WorkingSetNum;
                        working_set_private += process.WorkingSetPrivateNum;
                        private_bytes += process.PrivateBytesNum;
                    }

                    WriteLine($"Total memory: {memory} bytes, {memory / (float)(1024 * 1024)} MB");
                    WriteLine($"Total working set: {working_set} bytes, {working_set / (float)(1024 * 1024)} MB");
                    WriteLine($"Total private working set: {working_set_private} bytes, {working_set_private / (float)(1024 * 1024)} MB");
                    WriteLine($"Total private bytes: {private_bytes} bytes, {private_bytes / (float)(1024 * 1024)} MB");

                    break;
                case ECollectorType.Ping:
                    break;
                case ECollectorType.InstalledApplications:
                    break;
                case ECollectorType.Services:
                    break;
                case ECollectorType.SystemErrors:
                    break;
                case ECollectorType.ApplicationErrors:
                    break;
                case ECollectorType.DatabaseSize:
                    break;
                case ECollectorType.UPS:
                    break;
                case ECollectorType.DiskSpeed:
                    break;
                case ECollectorType.Configuration:
                    break;
                case ECollectorType.SMART:
                    break;
                //case CollectorType.AntiVirus:
                //    break;
                //case CollectorType.Firewall:
                //    break;
                case ECollectorType.Unknown:
                    break;
                default:
                    break;
            }
        }

        static void ShowUsage(IEnumerable<string> collectors)
        {
            WriteLine("Usage:");
            WriteLine(" common collector(s) [/i ip-address]");
            WriteLine("                     [/u username]");
            WriteLine("                     [/p password]");
            WriteLine("                     [/d database-type]");
            WriteLine("                     [/c connection-string]");
            WriteLine("                     [/r repeat-count (default 1)]");
            WriteLine("                     [/s sleep seconds between repeats (default 1)]");
            WriteLine("                     [/?]");
            WriteLine("Where:");
            WriteLine(" collector is one or more of:");
            foreach (string collector in collectors)
                WriteLine("  " + collector);
            WriteLine(" if no collector is specified, all will be collected if possible");
            WriteLine("ip-address, username, and password are used when collecting from a");
            WriteLine(" remote machine, and should be unused when collecting from the");
            WriteLine(" local machine");
            WriteLine("When collecting database, the IP address, database type, and connection");
            WriteLine(" string must be provided");
            WriteLine("  database-type must be one of: sqlserver, oracle, postgres");
            WriteLine("When collecting ping, the IP address must be provided");
            WriteLine("/? shows this usage information");
        }

        static public void WriteLine(string line)
        {
            Trace.WriteLine(line);
            Console.WriteLine(line);
        }
    }

    class PingResponder : Responder
    {
        public string IP { get; private set; }

        public PingResponder(string ip)
        {
            IP = ip;
        }

        public override void HandleRequest(Request request)
        {
            if(request is IPAddressRequest)
            {
                IPAddressRequest ip = request as IPAddressRequest;

                if (string.IsNullOrEmpty(IP) == false)
                {
                    ip.IPAddresses.Add(Tuple.Create(IP, string.Empty));
                    ip.Handled();
                }
                else
                    Program.WriteLine("Invalid IP address for ping");
            }
        }
    }

    //class SystemErrorsInfoResponder : Responder
    //{
    //    public SystemErrorsInfoResponder()
    //    {
    //    }

    //    public override void HandleRequest(Request request)
    //    {
    //        if(request is SystemErrorsInfoRequest)
    //        {
    //            SystemErrorsInfoRequest sys_errors = request as SystemErrorsInfoRequest;

    //            EventLogData d = new EventLogData() { MaxRecordNumber = 284377 };

    //            sys_errors.LogData.Assign(d);
    //            sys_errors.LogData.MaxRecordNumber = 234;

    //            request.Handled();
    //        }
    //        else if (request is SystemErrorsUpdateRequest)
    //        {
    //            SystemErrorsUpdateRequest sys_errors = request as SystemErrorsUpdateRequest;

    //            Program.WriteLine(JsonConvert.SerializeObject(sys_errors));
    //        }
    //    }
    //}
}
