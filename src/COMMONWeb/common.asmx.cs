using COMMONWeb.Properties;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Services;

namespace COMMONWeb
{
    /// <summary>
    /// Summary description for common
    /// </summary>
    [WebService(Namespace = "http://localhost/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class common : System.Web.Services.WebService
    {
        //[WebMethod]
        //public string GetDeviceStatus()
        //{
        //    FullDeviceStatus status = Database.Instance.GetDeviceStatuses(-1);
        //    return ToJson("GetDeviceStatus", status);
        //}

        //[WebMethod]
        //public string GetSingleDeviceStatus(int deviceID)
        //{
        //    FullDeviceStatus status = Database.Instance.GetDeviceStatuses(deviceID);
        //    SingleDeviceStatus device_status = new SingleDeviceStatus();

        //    // There should be only one...
        //    foreach(int key in status.fullStatus.Keys)
        //    {
        //        List<DeviceStatus> single_status = status.fullStatus[key];
        //        single_status.ForEach(s => device_status.AddStatus(s.status, s.isAlarm == 1));
        //    }
        //    return ToJson("GetSingleDeviceStatus", device_status);
        //}

        //[WebMethod]
        //public string GetNetworkStatus()
        //{
        //    List<NetworkStatus> statuses = Database.Instance.GetNetworkStatuses();
        //    return ToJson("GetNetworkStatus", statuses);
        //}

        //[WebMethod]
        //public string GetDeviceNetworkStatus(string ipAddress, int daysToRetrieve)
        //{
        //    NetworkStatus status = Database.Instance.GetPingData(ipAddress, daysToRetrieve);
        //    return ToJson("GetDeviceNetworkStatus", status);
        //}

        //[WebMethod]
        //public string GetMemoryData(int deviceID, int daysToRetrieve)
        //{
        //    return GetJsonDeviceData(string.Format("GetMemoryData {0}", deviceID), deviceID, (int)CollectorType.Memory, daysToRetrieve);
        //}

        /// <summary>
        /// Gets information about the disks on the specified device
        /// 
        /// Returned data looks like:
        /// {
        ///     "C:": [
        ///         {
        ///             "dataID":37969,
        ///             "collectorID":3,
        ///             "value":"{\"Value\":{\"Capacity\":\"128324718592\",\"Free\":\"36206804992\",\"Used\":\"92117913600\"}}",
        ///             "timeStamp":"2015-10-08T13:59:26.620083-06:00",
        ///         },
        ///         More just like that
        ///     ],
        ///     "E:": [
        ///         {
        ///             "dataID":37970,
        ///             "collectorID":3,
        ///             "value":"{\"Value\":{\"Capacity\":\"128324718592\",\"Free\":\"36206804992\",\"Used\":\"92117913600\"}}",
        ///             "timeStamp":"2015-10-08T13:59:26.620083-06:00",
        ///         },
        ///         More just like that
        ///     ]
        /// }
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        //[WebMethod]
        //public string GetDiskData(int deviceID, int daysToRetrieve)
        //{
        //    Dictionary<string, List<DeviceData>> data = Database.Instance.GetDiskData(deviceID, daysToRetrieve);
        //    string d = ToJson("GetDiskData " + deviceID, data);
        //    return d;
        //}

        //[WebMethod]
        //public string GetDiskSpeed(int deviceID, int daysToRetrieve)
        //{
        //    // We get a list of dictionaries back, so let's convert that into a dictionary mapping the drive
        //    // name to the list of DeviceDatas for that disk.
        //    IEnumerable<DeviceData> dataList = GetDeviceData(deviceID, CollectorType.DiskSpeed, daysToRetrieve);
        //    Dictionary<string, List<DeviceData>> dictData = new Dictionary<string, List<DeviceData>>();
        //    foreach (var data in dataList)
        //    {
        //        DictionaryData d = JsonConvert.DeserializeObject<DictionaryData>(data.value);
        //        if (d != null && d.Data.ContainsKey("Disk Name"))
        //        {
        //            string diskName = d.Data["Disk Name"];
        //            if (dictData.ContainsKey(diskName) == false)
        //                dictData[diskName] = new List<DeviceData>();
        //            dictData[diskName].Add(data);
        //        }
        //    }
        //    string jsonData = ToJson("GetDiskSpeed " + deviceID, dictData);
        //    return jsonData;
        //}

        //[WebMethod]
        //public string GetCPUData(int deviceID, int daysToRetrieve)
        //{
        //    return GetJsonDeviceData(string.Format("GetCPUData {0}", deviceID), deviceID, CollectorType.CPUUsage, daysToRetrieve);
        //}

        //[WebMethod]
        //public string GetNICData(int deviceID, int daysToRetrieve)
        //{
        //    return GetJsonDeviceData(string.Format("GetNICData {0}", deviceID), deviceID, CollectorType.NICUsage, daysToRetrieve);
        //}

        //[WebMethod]
        //public string GetSystemData(int deviceID, int daysToRetrieve)
        //{
        //    NetworkStatus data = Database.Instance.GetDevicePingData(deviceID, daysToRetrieve);
        //    return ToJson("GetSystemData", data);
        //}

        //[WebMethod]
        //public string GetProcessData(int deviceID)
        //{
        //    DeviceProcessInfo info = Database.Instance.GetDeviceProcesses(deviceID);
        //    return ToJson("GetProcessData", info);
        //}

        //[WebMethod]
        //public string GetProcessHistory(int deviceID, string processName)
        //{
        //    ProcessHistory p = Database.Instance.GetProcessHistory(deviceID, processName);
        //    return ToJson("GetProcessHistory", p);
        //}
        //[WebMethod]
        //public string GetAllProcesses(int deviceID, int daysToRetrieve)
        //{
        //    List<string> processes = Database.Instance.GetAllDeviceProcesses(deviceID, daysToRetrieve);
        //    return ToJson("GetAllProcesses", processes);
        //}

        //[WebMethod]
        //public string GetAppData(int deviceID)
        //{
        //    DeviceApplications apps = Database.Instance.GetDeviceApplications(deviceID);
        //    return ToJson("GetAppData", apps);
        //}

        //[WebMethod]
        //public string GetAppChanges(int deviceID, int daysToRetrieve)
        //{
        //    var changes = Database.Instance.GetApplicationChanges(deviceID, daysToRetrieve);
        //    return ToJson("GetAppChanges", changes);
        //}

        //[WebMethod]
        //public string GetAppHistory(int deviceID, string app)
        //{
        //    var history = Database.Instance.GetApplicationHistory(deviceID, app);
        //    return ToJson("GetAppHistory", history);
        //}

        //[WebMethod]
        //public string GetServicesData(int deviceID)
        //{
        //    ValueInfo v = Database.Instance.GetMostRecentValueForDevice(deviceID, CollectorType.Services);
        //    var definition = new { Value = new List<string>() };
        //    var value = JsonConvert.DeserializeAnonymousType(v.value, definition);
        //    Services services = new Services();
        //    value.Value.ForEach(s => services.services.Add(s));
        //    services.timestamp = v.timestamp;

        //    return ToJson("GetServicesData", services);
        //}

        //[WebMethod]
        //public string GetDatabaseData(int deviceID)
        //{
        //    ValueInfo v = Database.Instance.GetMostRecentValueForDevice(deviceID, CollectorType.DatabaseSize);
        //    return ToJson("GetDatabaseData", v);
        //}

        //[WebMethod]
        //public string GetDatabaseHistory(int deviceID, string databaseName)
        //{
        //    DatabaseHistory d = Database.Instance.GetDatabaseHistory(deviceID, databaseName);
        //    return ToJson("GetDatabaseHistory", d);
        //}

        //[WebMethod]
        //public string GetUPSStatus(int deviceID)
        //{
        //    UPSStatus ups = Database.Instance.GetUPSStatus(deviceID);
        //    return ToJson("GetUPSStatus", ups);
        //}

        //[WebMethod]
        //public string GetConfigurationData()
        //{
        //    SystemConfiguration config = Database.Instance.GetSystemConfiguration(true);

        //    // Make sure we scrub the username/password data
        //    config.devices.ForEach(d => d.username = d.password = string.Empty);

        //    return ToJson("GetConfigurationData", config);
        //}

        //[WebMethod]
        //public string SetConfigurationData(string data)
        //{
        //    Trace.WriteLine(data);
        //    try
        //    {
        //        SystemConfiguration config = JsonConvert.DeserializeObject<SystemConfiguration>(data);
        //        Database.Instance.SetSystemConfiguration(config);
        //    }
        //    catch (Exception)
        //    {
        //    }
            
        //    return GetConfigurationData();
        //}

        //[WebMethod]
        //public string GetDeviceDetails(int deviceID)
        //{
        //    DeviceDetails details = Database.Instance.GetDeviceDetails(deviceID);
        //    return ToJson("GetDeviceDetails", details);
        //}

        //[WebMethod]
        //public string GetSystemErrors(int deviceID, int daysToRetrieve)
        //{
        //    DeviceErrors errors = Database.Instance.GetSystemErrors(deviceID, daysToRetrieve);
        //    return ToJson("GetSystemErrors", errors);
        //}

        //[WebMethod]
        //public string GetApplicationErrors(int deviceID, int daysToRetrieve)
        //{
        //    DeviceErrors errors = Database.Instance.GetApplicationErrors(deviceID, daysToRetrieve);
        //    return ToJson("GetApplicationErrors", errors);
        //}

        //[WebMethod]
        //public string CheckUniqueDeviceName(string name)
        //{
        //    UniqueDeviceName unique = Database.Instance.CheckUniqueDeviceName(name);
        //    return ToJson("CheckUniqueDeviceName", unique);
        //}

        //[WebMethod]
        //public string GetMemoryReport(int deviceID, int daysToRetrieve)
        //{
        //    MemoryReport report = Database.Instance.GetMemoryReport(deviceID, daysToRetrieve);
        //    return ToJson("GetMemoryReport", report);
        //}

        //[WebMethod]
        //public string GetDiskReport(int deviceID, int daysToRetrieve)
        //{
        //    DiskReport report = Database.Instance.GetDiskReport(deviceID, daysToRetrieve);
        //    return ToJson("GetDiskReport", report);
        //}

        //[WebMethod]
        //public string GetCPUReport(int deviceID, int daysToRetrieve)
        //{
        //    CPUReport report = Database.Instance.GetCPUReport(deviceID, daysToRetrieve);
        //    return ToJson("GetCPUReport", report);
        //}

        //[WebMethod]
        //public string GetNICReport(int deviceID, int daysToRetrieve)
        //{
        //    NICReport report = Database.Instance.GetNICReport(deviceID, daysToRetrieve);
        //    return ToJson("GetNICReport", report);
        //}

        //[WebMethod]
        //public string DownloadReport(string html)
        //{
        //    var reportBytes = Report.PdfSharpConvert(html).ToList();
        //    return ToJson("DownloadReport", reportBytes);
        //}

        //        private string GetJsonDeviceData(string header, int deviceID, CollectorType collectorType, int daysToRetrieve)
        //        {
        //            Stopwatch watch = Stopwatch.StartNew();

        //            IEnumerable<DeviceData> data = GetDeviceData(deviceID, collectorType, daysToRetrieve);
        //            string json = ToJson(header, data);

        //#if LOG_PERFORMANCE
        //            m_performance_log.Append(string.Format("GetJsonDeviceData {0}/{1}/{2}/{3} took {4} ms",
        //                header, deviceID, collectorType, daysToRetrieve, watch.ElapsedMilliseconds));
        //#endif

        //            return json;
        //        }

        //private IEnumerable<DeviceData> GetDeviceData(int deviceID, CollectorType collectorType, int daysToRetrieve)
        //{
        //    var data = Database.Instance.GetDeviceData(deviceID, collectorType, daysToRetrieve);
        //    return data;
        //}

        //private string ToJson(string header, object data)
        //{
        //    string s = JsonConvert.SerializeObject(data);
        //    Trace.WriteLine(string.Format("{0} ({1}): {2}", header, s.Length, s));
        //    return s;
        //}

        //#if LOG_PERFORMANCE
        //        private SimpleFileLog m_performance_log = new SimpleFileLog("C:\\LogFiles", "commonweb_performance.log", true);
        //#endif

        static common()
        {
            Settings settings = Settings.Default;

            gov.sandia.sld.common.logging.EventLog.GlobalSource = "COMMONWeb";

            LogManager.InitializeLogFile(settings.LogPath, settings.LogFilename);
            LogManager.Options.level = (LogManager.LogLevel)settings.LogLevel;

            gov.sandia.sld.common.db.Context.SpecifyFilename(settings.DBLocation);
        }
    }
}
