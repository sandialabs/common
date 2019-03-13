using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.reports;
using Nancy;
using System.Collections.Generic;
using Newtonsoft.Json;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using System.Text;
using System;
using System.Data.SQLite;
using gov.sandia.sld.common.db.models;

namespace COMMONWeb
{
    public class MainModule : NancyModule
    {
        private IDataStore m_repo;

        public MainModule(IDataStore repo) : base()
        {
            m_repo = repo;

            COMMONDatabaseBootstrapper bootstrapper = new COMMONDatabaseBootstrapper();

            Get["/"] = _ => View["index.html", bootstrapper];

            Get["/devicestatus"] = _ => ToJson(GetDeviceStatus());
            Get["/configurationdata"] = _ => ToJson(GetConfigurationData());
            Get["/allcollectors"] = _ => ToJson(GetAllCollectors());
            Get["/networkstatus/{starting}/{ending}"] = p => ToJson(GetNetworkStatus(new StartStopTime(p.starting, p.ending)));
            Get["/processhistory/{deviceID:int}/{name}"] = p => ToJson(GetProcessHistory(p.deviceID, p.name));
            Get["/allprocesses/{deviceID:int}/{starting}/{ending}"] = p => ToJson(GetAllProcesses(p.deviceID, new StartStopTime(p.starting, p.ending)));
            Get["/xyz/{deviceID:int}/{starting?}/{ending?}"] = p => ToJson(GetAppChanges(p.deviceID, new StartStopTime(p.starting, p.ending)));
            Get["/apphistory/{deviceID:int}/{appName}"] = p => ToJson(GetAppHistory(p.deviceID, p.appName));
            Get["/servicesdata/{deviceID:int}"] = p => ToJson(GetServicesData(p.deviceID));
            Get["/databasehistory/{deviceID:int}/{name}"] = p => ToJson(GetDatabaseHistory(p.deviceID, p.name));
            Get["/devicedetails/{deviceID:int}"] = p => ToJson(GetDeviceDetails(p.deviceID));
            Get["/smartdata/{deviceID:int}"] = p => ToJson(GetSMARTData(p.deviceID));

            Get["/machinedata/{deviceID:int}/{machineParts}/{starting}/{ending}"] = p => ToJson(GetMachineData(p.deviceID, p.machineParts, new StartStopTime(p.starting, p.ending)));

            Post["/collectnow/{collectorID:int}"] = p => ToJson(CollectNow(p.collectorID));

            // Reports
            Get["/machinesubreport/{deviceID:int}/{reportTypes}/{starting}/{ending}"] = p => ToJson(GetCPUReport(p.deviceID, p.reportTypes, new StartStopTime(p.starting, p.ending)));
        }

        private FullDeviceStatus GetDeviceStatus()
        {
            FullDeviceStatus status = m_repo.GetDeviceStatuses(-1);
            return status;
        }

        private SystemConfiguration GetConfigurationData()
        {
            Database db = new Database();
            using (SQLiteConnection conn = db.Connection)
            {
                conn.Open();

                SystemConfiguration config = SystemConfigurationStore.Get(true, conn);

                // Make sure we scrub the username/password data
                config.devices.ForEach(d => d.username = d.password = string.Empty);

                return config;
            }
        }

        private List<CollectorInfo> GetAllCollectors()
        {
            List<CollectorInfo> collectors = m_repo.GetAllCollectors();
            return collectors;
        }

        private List<NetworkStatus> GetNetworkStatus(IStartEndTime start_end)
        {
            List<NetworkStatus> statuses = m_repo.GetNetworkStatuses(start_end);
            return statuses;
        }

        private MachineData GetMachineData(int deviceID, string machineParts, IStartEndTime start_end)
        {
            EMachinePart[] parts = JsonConvert.DeserializeObject<EMachinePart[]>(machineParts);
            MachineData md = new MachineData(deviceID, start_end);
            Database db = new Database();

            foreach (EMachinePart part in parts)
            {
                switch (part)
                {
                    case EMachinePart.CPU:
                        md.cpu = GetDeviceData($"GetCPUData {deviceID}", deviceID, ECollectorType.CPUUsage, start_end);
                        break;
                    case EMachinePart.Memory:
                        md.memory = GetMemoryData($"GetMemoryData {deviceID}", deviceID, start_end, db);
                        break;
                    case EMachinePart.DiskUsage:
                        {
                            Dictionary<string, List<DiskUsageData>> data = db.GetDiskData(deviceID, start_end);
                            List<HardDisk> smart = db.GetSMARTData(deviceID);

                            Dictionary<string, DiskUsageWithSmartData> disk_dict = new Dictionary<string, DiskUsageWithSmartData>();
                            foreach (KeyValuePair<string, List<DiskUsageData>> dvp in data)
                            {
                                string drive_letter = dvp.Key.Trim();
                                DiskUsageWithSmartData disk = new DiskUsageWithSmartData()
                                {
                                    driveLetter = dvp.Key,
                                    diskUsage = dvp.Value,
                                };
                                HardDisk hd = smart.Find(s => s.DriveLetters.Contains(drive_letter));
                                if (hd != null)
                                    disk.smartData = hd;

                                disk_dict[drive_letter] = disk;
                            }
                            md.diskUsage = disk_dict;
                        }
                        break;
                    case EMachinePart.DiskPerformance:
                        {
                            List<DeviceData> dataList = GetDeviceData($"DiskSpeed {deviceID}", deviceID, ECollectorType.DiskSpeed, start_end);
                            Dictionary<string, List<DeviceData>> dictData = new Dictionary<string, List<DeviceData>>();
                            foreach (var data in dataList)
                            {
                                DictionaryData d = JsonConvert.DeserializeObject<DictionaryData>(data.value);
                                string diskName = string.Empty;
                                if (d != null && d.Data.TryGetValue("Disk Name", out diskName))
                                {
                                    List<DeviceData> datalist = null;
                                    if (dictData.TryGetValue(diskName, out datalist) == false)
                                    {
                                        datalist = new List<DeviceData>();
                                        dictData[diskName] = datalist;
                                    }
                                    datalist.Add(data);
                                }
                            }
                            md.diskPerformance = dictData;
                        }
                        break;
                    case EMachinePart.NIC:
                        md.nic = GetDeviceData($"GetNICData {deviceID}", deviceID, ECollectorType.NICUsage, start_end);
                        break;
                    case EMachinePart.SystemErrors:
                        md.systemErrors = db.GetSystemErrors(deviceID, start_end);
                        break;
                    case EMachinePart.ApplicationErrors:
                        md.applicationErrors = db.GetApplicationErrors(deviceID, start_end);
                        break;
                    case EMachinePart.Processes:
                        md.processes = db.GetDeviceProcesses(deviceID);
                        break;
                    case EMachinePart.Database:
                        md.database = db.GetMostRecentValueForDevice(deviceID, ECollectorType.DatabaseSize);
                        break;
                    case EMachinePart.Applications:
                        md.applications = db.GetDeviceApplications(deviceID);
                        break;
                    case EMachinePart.Services:
                        {
                            ValueInfo v = db.GetMostRecentValueForDevice(deviceID, ECollectorType.Services);
                            var definition = new { Value = new List<string>() };
                            var value = JsonConvert.DeserializeAnonymousType(v.value, definition);

                            md.services = new Services(value.Value) { timestamp = v.timestamp };
                        }
                        break;
                    case EMachinePart.UPS:
                        md.ups = GetDeviceData($"GetUpsData {deviceID}", deviceID, ECollectorType.UPS, start_end);
                        break;
                    case EMachinePart.NumMachineParts:
                        break;
                    default:
                        break;
                }
            }

            return md;
        }

        private ReportData GetCPUReport(int deviceID, string reportTypes, IStartEndTime start_end)
        {
            EReportSubType[] types = JsonConvert.DeserializeObject<EReportSubType[]>(reportTypes);
            ReportData rd = new ReportData(deviceID, start_end);
            Database db = new Database();

            foreach(EReportSubType type in types)
            {
                switch (type)
                {
                    case EReportSubType.Memory:
                        Memory m = new Memory(db);
                        rd.memory = m.GetReport(deviceID, start_end);
                        break;
                    case EReportSubType.Disk:
                        Disk d = new Disk(db);
                        rd.disk = d.GetReport(deviceID, start_end);
                        break;
                    case EReportSubType.CPU:
                        CPU c = new CPU(db);
                        rd.cpu = c.GetReport(deviceID, start_end);
                        break;
                    case EReportSubType.NIC:
                        NIC n = new NIC(db);
                        rd.nic = n.GetReport(deviceID, start_end);
                        break;
                    default:
                        break;
                }
            }

            return rd;
        }

        private ProcessHistory GetProcessHistory(int deviceID, string processName)
        {
            Database db = new Database();
            ProcessHistory p = db.GetProcessHistory(deviceID, processName);
            return p;
        }

        private List<string> GetAllProcesses(int deviceID, IStartEndTime start_end)
        {
            Database db = new Database();
            List<string> processes = db.GetAllDeviceProcesses(deviceID, start_end);
            return processes;
        }

        private AllApplicationsHistory GetAppChanges(int deviceID, IStartEndTime start_end)
        {
            Database db = new Database();
            AllApplicationsHistory changes = db.GetApplicationChanges(deviceID, start_end);
            return changes;
        }

        private ApplicationHistory GetAppHistory(int deviceID, string app)
        {
            Database db = new Database();
            ApplicationHistory history = db.GetApplicationHistory(deviceID, app);
            return history;
        }

        private Services GetServicesData(int deviceID)
        {
            Database db = new Database();
            ValueInfo v = db.GetMostRecentValueForDevice(deviceID, ECollectorType.Services);
            var definition = new { Value = new List<string>() };
            var value = JsonConvert.DeserializeAnonymousType(v.value, definition);

            Services services = new Services(value.Value) { timestamp = v.timestamp };
            return services;
        }

        private DatabaseHistory GetDatabaseHistory(int deviceID, string databaseName)
        {
            // The database name might have invalid characters for a URL
            // (like in C:\\COMMON\\common.sqlite), so it's base64 encoded at the client,
            // and we decode it here.
            string db_name = Encoding.UTF8.GetString(Convert.FromBase64String(databaseName));
            Database db = new Database();
            DatabaseHistory d = db.GetDatabaseHistory(deviceID, db_name);
            return d;
        }

        private DeviceDetails GetDeviceDetails(int deviceID)
        {
            Database db = new Database();
            DeviceDetails details = db.GetDeviceDetails(deviceID);
            return details;
        }

        private List<HardDisk> GetSMARTData(int deviceID)
        {
            Database db = new Database();
            List<HardDisk> smart = db.GetSMARTData(deviceID);
            return smart;
        }

        private CollectorInfo CollectNow(int collectorID)
        {
            Database db = new Database();
            using (SQLiteConnection conn = db.Connection)
            {
                conn.Open();
                CollectionTime ct = new CollectionTime(conn);
                CollectorInfo collector_info = ct.CollectNow(collectorID);
                return collector_info;
            }
        }
        
        private List<DeviceData> GetDeviceData(string header, long deviceID, ECollectorType collectorType, IStartEndTime start_end)
        {
            //Stopwatch watch = Stopwatch.StartNew();
            List<DeviceData> data = m_repo.GetDeviceData(deviceID, collectorType, start_end);
            return data;
        }

        private List<DeviceData> GetMemoryData(string header, long deviceID, IStartEndTime start_end, Database db)
        {
            List<DeviceData> data = db.GetMemoryData(deviceID, start_end);
            return data;
        }

        private Nancy.Response ToJson(object o)
        {
            Response r = Response.AsJson(o);
            //Trace.WriteLine(r.ToString());
            return r;
        }
    }
}
