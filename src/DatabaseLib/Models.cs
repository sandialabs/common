using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gov.sandia.sld.common.db
{
    public class CollectorInfo
    {
        [JsonIgnore]
        public CollectorID CID { get; set; }

        public int id { get { return CID.ID; } }

        [JsonIgnore]
        /// <summary>
        /// The ID of the device this collector belongs to
        /// </summary>
        public DeviceID DID { get; set; }

        public int deviceID { get { return DID.ID; } }

        /// <summary>
        /// The name of the collector (i.e. "System.Ping", or "Server.Disk")
        /// </summary>
        public string name { get { return CID.Name; } }

        /// <summary>
        /// The type of collector (ping, memory, CPU usage, etc.)
        /// See the CollectorType enum.
        /// </summary>
        public CollectorType collectorType { get; set; }

        /// <summary>
        /// Used to enable/disable the collector
        /// </summary>
        public bool isEnabled { get; set; }

        /// <summary>
        /// How frequently, in minutes, the data should be collected
        /// </summary>
        public int frequencyInMinutes { get; set; }

        [JsonIgnore]
        public TimeSpan Frequency { get { return TimeSpan.FromMinutes(frequencyInMinutes); } }

        /// <summary>
        /// Certain collectors aren't configurable, so we can skip them
        /// </summary>
        /// <returns>true if the collector can be skipped; false if it should be displayed</returns>
        [JsonIgnore]
        public bool SkipConfiguration { get; private set; }

        public DateTimeOffset? lastCollectionAttempt { get; set; }
        public DateTimeOffset? lastCollectedAt { get; set; }
        public DateTimeOffset? nextCollectionTime { get; set; }
        public bool successfullyCollected { get; set; }
        public bool isBeingCollected { get; set; }

        private CollectorInfo()
        {
            CID = new CollectorID(-1, string.Empty);
            DID = new DeviceID(-1, string.Empty);
            collectorType = CollectorType.Unknown;
            isEnabled = true;
            frequencyInMinutes = collectorType.GetFrequencyInMinutes();
            SkipConfiguration = collectorType.GetSkipConfiguration();
            lastCollectionAttempt = lastCollectedAt = nextCollectionTime = null;
            successfullyCollected = true;
            isBeingCollected = false;
        }

        public CollectorInfo(CollectorType type)
        {
            CID = new CollectorID(-1, string.Empty);
            DID = new DeviceID(-1, string.Empty);
            collectorType = type;
            isEnabled = true;
            frequencyInMinutes = collectorType.GetFrequencyInMinutes();
            SkipConfiguration = collectorType.GetSkipConfiguration();
            lastCollectionAttempt = lastCollectedAt = nextCollectionTime = null;
            successfullyCollected = true;
            isBeingCollected = false;
        }

        public static List<CollectorInfo> FromCollectorTypes(List<CollectorType> types)
        {
            List<CollectorInfo> infos = new List<CollectorInfo>();
            types.ForEach(t => infos.Add(new CollectorInfo(t)));
            return infos;
        }

        public override string ToString()
        {
            return string.Format("CollectorInfo: id {0}, name {1}, collectorType {2}, isEnabled {3}, frequencyInMinutes {4}",
                id, name, collectorType, isEnabled, frequencyInMinutes);
        }

        public void Merge(CollectorInfo other)
        {
            frequencyInMinutes = other.frequencyInMinutes;
            isEnabled = other.isEnabled;
        }
    }

    /// <summary>
    /// Used to pass info about devices to the user
    /// </summary>
    public class DeviceInfo
    {
        private DeviceID m_device_id { get; set; }

        [JsonIgnore]
        public DeviceID DID { get { return m_device_id; } set { if (value == null) return; UpdateCollectors(value); m_device_id = value; } }

        public int id { get { return DID.ID; } }

        /// <summary>
        /// The name of the device (i.e. "Server" or "Lane 1 Workstation")
        /// </summary>
        public string name { get { return m_device_id.Name; } set { DeviceID did = new DeviceID(m_device_id.ID, value); DID = did; } }

        /// <summary>
        /// The type of device (see the DeviceType enum)
        /// </summary>
        public DeviceType type { get; set; }

        /// <summary>
        /// The IP address of the device, if that's relevant (it wouldn't be for the server, or the
        /// "system" device).
        /// </summary>
        public string ipAddress { get; set; }

        /// <summary>
        /// The username needed to connect to a remote device to collect its data
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// The password needed to connect to a remote device to collect its data
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// Set to true to indicate the device should be deleted; defaults to false
        /// </summary>
        public bool deleted { get; set; }

        /// <summary>
        /// The set of data that will be collected for this device
        /// </summary>
        public List<CollectorInfo> collectors { get; set; }

        public Dictionary<string, string> driveNames { get; set; }

        public MonitoredDriveManager monitoredDrives { get; set; }

        public int groupID { get; set; }

        public DeviceInfo()
        {
            m_device_id = new DeviceID(-1, string.Empty);
            type = DeviceType.Unknown;
            ipAddress = "0.0.0.0";
            username = password = string.Empty;
            deleted = false;
            collectors = new List<CollectorInfo>();
            driveNames = new Dictionary<string, string>();
            monitoredDrives = new MonitoredDriveManager();
            groupID = -1;
        }

        public DeviceInfo(DeviceType type)
        {
            m_device_id = new DeviceID(-1, string.Empty);
            this.type = type;
            ipAddress = "0.0.0.0";
            username = password = string.Empty;
            deleted = false;
            collectors = CollectorInfo.FromCollectorTypes(type.GetCollectors());
            driveNames = new Dictionary<string, string>();
            monitoredDrives = new MonitoredDriveManager();
            groupID = -1;
        }

        public override string ToString()
        {
            string str = string.Format("DeviceInfo: id {0}, name {1}, type {2}, ip_address {3}, username {4}, password {5}, deleted {6}",
                id, name, type, ipAddress, username, password, deleted);
            foreach(CollectorInfo c in collectors)
                str += "\n" + c.ToString();

            return str;
        }

        private void UpdateCollectors(DeviceID device_id)
        {
            // UpdateCollectors is called prior to setting m_name so we
            // can see if the new name == the existing name. If it does
            // we don't have to do anything.
            if (device_id == null || device_id.Name == m_device_id.Name)
                return;

            foreach (CollectorInfo c in collectors)
            {
                string name = c.name;
                int index = name.IndexOf('.');
                c.CID.Name = (index >= 0) ? device_id.Name + name.Substring(index) : device_id.Name + "." + c.collectorType;
            }
        }
    }

    /// <summary>
    /// Describes the status, and if the status is an alarm or not
    /// </summary>
    public class DeviceStatus
    {
        public string status { get; set; }
        public int alertLevel { get; set; }
        public string message { get; set; }
    }

    /// <summary>
    /// Used to send alerts and status to the user
    /// </summary>
    public class FullDeviceStatus
    {
        /// <summary>
        /// Maps the device to the set of alarms for that device
        /// </summary>
        //public Dictionary<int, List<DeviceStatus>> alarms { get; set; }

        /// <summary>
        /// Maps the device to the set of statuses, including alarms
        /// </summary>
        public Dictionary<int, List<DeviceStatus>> fullStatus { get; set; }

        //public Dictionary<int, string> deviceIDToName { get; set; }

        public FullDeviceStatus()
        {
            //alarms = new Dictionary<int, List<DeviceStatus>>();
            fullStatus = new Dictionary<int, List<DeviceStatus>>();
            //deviceIDToName = new Dictionary<int, string>();
        }

        public void AddStatus(string name, int device_id, string description, EAlertLevel alert_level, string message)
        {
            //if (deviceIDToName.ContainsKey(device_id) == false)
            //    deviceIDToName[device_id] = name;

            //if (is_alarm)
            //    AddDescription(alarms, device_id, description, is_alarm);
            AddDescription(fullStatus, device_id, description, alert_level, message);
        }

        private static void AddDescription(Dictionary<int, List<DeviceStatus>> dict, int device_id, string description, EAlertLevel alert_level, string message)
        {
            List<DeviceStatus> descriptions = null;
            dict.TryGetValue(device_id, out descriptions);
            if(descriptions == null)
            {
                descriptions = new List<DeviceStatus>();
                dict[device_id] = descriptions;
            }
            descriptions.Add(new DeviceStatus() { status = description, alertLevel = (int)alert_level, message = message });
        }
    }

    public class SingleDeviceStatus
    {
        public int deviceID { get; set; }
        public List<DeviceStatus> alarms { get; set; }
        public List<DeviceStatus> status { get; set; }

        public SingleDeviceStatus()
        {
            alarms = new List<DeviceStatus>();
            status = new List<DeviceStatus>();
        }

        public void AddStatus(string description, EAlertLevel alert_level)
        {
            DeviceStatus ds = new DeviceStatus()
            {
                alertLevel = (int)alert_level,
                status = description,
            };

            if (alert_level.IsAlarm())
                alarms.Add(ds);
            status.Add(ds);
        }
    }

    public class PingAttempt
    {
        public bool successful { get; set; }
        public DateTimeOffset timestamp { get; set; }

        public PingAttempt()
        {
            successful = false;
            timestamp = DateTimeOffset.MinValue;
        }
    }

    /// <summary>
    /// Information about the ping status of specific IP address. If we know the name of a given device, name will be set to that.
    /// successfulPing is set to 1 if the last ping attempt was successful.
    /// </summary>
    public class NetworkStatus
    {
        /// <summary>
        /// The name of the device pinged, if we know it. Otherwise, it's the IP address.
        /// </summary>
        public string name { get; set; }

        public int deviceID { get; set; }

        /// <summary>
        /// Set to true if the last ping attempt was successful; false otherwise
        /// </summary>
        public bool successfulPing { get; set; }

        /// <summary>
        /// The time/date the last successful ping occurred
        /// </summary>
        public DateTimeOffset? dateSuccessfulPingOccurred { get; set; }

        /// <summary>
        /// The time/date the last ping attempt was made
        /// </summary>
        public DateTimeOffset datePingAttempted { get; set; }

        /// <summary>
        /// The IP address of the ping attempt. May be the same as name if we don't know the name of what was pinged.
        /// </summary>
        public string ipAddress { get; set; }

        /// <summary>
        /// True if it has ever been pinged; false if it has never responded to a ping
        /// </summary>
        public bool hasBeenPinged { get; set; }

        public List<PingAttempt> attempts { get; set; }

        public NetworkStatus()
        {
            deviceID = -1;
            hasBeenPinged = false;
            attempts = new List<PingAttempt>();
        }
    }

    public class DeviceData
    {
        public int dataID { get; set; }
        public int collectorID { get; set; }
        public string value { get; set; }
        public DateTimeOffset timeStamp { get; set; }

        public DeviceData Clone()
        {
            return new DeviceData() { dataID = dataID, collectorID = collectorID, value = value, timeStamp = timeStamp };
        }
    }

    //public class DiskData
    //{
    //    public Dictionary<string, List<DeviceData>> DiskToDeviceData { get; set; }
    //    public bool 
    //}

    /// <summary>
    /// Holds an individual key/value pair from the Configuration table.
    /// 
    /// The deleted attribute is set to 1 when the user deletes a configuration
    /// value. When it's retrieved from the database and sent out it will be 0.
    /// </summary>
    public class ConfigurationData
    {
        /// <summary>
        /// The database-assigned ConfigurationID
        /// </summary>
        public int configID { get; set; }

        /// <summary>
        /// The specified path (i.e. 'site.name')
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// The configuration value (i.e. 'Freeport')
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// true if the configuration value should be deleted; defaults to false
        /// </summary>
        public bool deleted { get; set; }

        public ConfigurationData()
        {
            configID = -1;
            path = value = string.Empty;
            deleted = false;
        }
    }

    public class LanguageConfiguration : IEquatable<LanguageConfiguration>
    {
        public string languageCode { get; set; }
        public string language { get; set; }
        public bool isEnabled { get; set; }
        public bool Equals(LanguageConfiguration other)
        {

            var b = language.Equals(other.language, StringComparison.InvariantCultureIgnoreCase)
                   || languageCode.Equals(other.languageCode, StringComparison.InvariantCultureIgnoreCase);
            return b;
        }
    }

    public class Group
    {
        public int id { get; set; }
        public string name { get; set; }

        public Group()
        {
            id = -1;
            name = string.Empty;
        }
    }

    /// <summary>
    /// Contains all of the system configuration.
    /// 
    /// The configuration maps the key to the ConfigurationData, but the key
    /// is also kept in ConfigurationData for ease of access.
    /// </summary>
    public class SystemConfiguration
    {
        /// <summary>
        /// The generic configuration path/value pairs
        /// </summary>
        public Dictionary<string, ConfigurationData> configuration { get; set; }

        /// <summary>
        /// Details about each individual device
        /// </summary>
        public List<DeviceInfo> devices { get; set; }

        public List<Group> groups { get; set; }

        public List<LanguageConfiguration> languages { get; set; }

        public string softwareVersion { get; set; }

        public SystemConfiguration()
        {
            configuration = new Dictionary<string, ConfigurationData>(StringComparer.OrdinalIgnoreCase);
            devices = new List<DeviceInfo>();
            groups = new List<Group>();
            languages = new List<LanguageConfiguration>();
            softwareVersion = string.Empty;
        }

        public void Merge(SystemConfiguration config)
        {
            HashSet<string> doomed = new HashSet<string>(configuration.Keys, StringComparer.OrdinalIgnoreCase);
            foreach(KeyValuePair<string, ConfigurationData> configpair in config.configuration)
            {
                doomed.Remove(configpair.Key);
                configuration[configpair.Key] = configpair.Value;
            }
            doomed.ToList().ForEach(c => configuration[c].deleted = true);

            doomed.Clear();
            doomed.Concat(devices.ConvertAll<string>(d => d.name));

            // Never remove the System device, or the device that is a Server.
            DeviceInfo system = devices.FirstOrDefault(d => d.type == DeviceType.System);
            doomed.Remove("System");
            DeviceInfo server = devices.FirstOrDefault(d => d.type == DeviceType.Server);
            if (server != null)
                doomed.Remove(server.name);

            foreach (DeviceInfo new_device in config.devices)
            {
                // Don't allow the System to be imported
                if (new_device.type == DeviceType.System)
                    continue;

                doomed.Remove(new_device.name);
                DeviceInfo old_device = devices.FirstOrDefault(d => string.Compare(d.name, new_device.name, true) == 0);
                DeviceInfo insert_device = new DeviceInfo(new_device.type) { DID = new DeviceID(-1, new_device.name) };
                if (old_device != null)
                {
                    devices.Remove(old_device);
                    insert_device.DID = new DeviceID(old_device.DID.ID, new_device.name);
                }
                
                switch (new_device.type)
                {
                    case DeviceType.Server:
                    case DeviceType.System:
                        insert_device.ipAddress = "0.0.0.0";
                        break;
                    case DeviceType.Workstation:
                        insert_device.ipAddress = new_device.ipAddress;
                        insert_device.username = new_device.username;
                        insert_device.password = new_device.password;
                        break;
                    case DeviceType.Camera:
                    case DeviceType.RPM:
                    case DeviceType.Generic:
                    case DeviceType.Unknown:
                    default:
                        insert_device.ipAddress = new_device.ipAddress;
                        insert_device.username = insert_device.password = string.Empty;
                        break;
                }

                foreach(CollectorInfo insert_ci in insert_device.collectors)
                {
                    CollectorInfo new_ci = new_device.collectors.FirstOrDefault(nc => nc.collectorType == insert_ci.collectorType);
                    if(new_ci != null)
                        insert_ci.Merge(new_ci);
                }

                devices.Add(insert_device);
            }

            foreach(string name in doomed)
            {
                DeviceInfo doomed_device = devices.FirstOrDefault(d => d.name == name);
                if (doomed_device != null)
                    doomed_device.deleted = true;
            }
        }
    }

    /// <summary>
    /// Used to send information about a specific device to the user
    /// </summary>
    public class DeviceDetails
    {
        /// <summary>
        /// The database-assigned device ID
        /// </summary>
        public int deviceID { get; set; }

        /// <summary>
        /// How long the device has been running
        /// </summary>
        public string uptime { get; set; }

        /// <summary>
        /// The last time the device was booted
        /// </summary>
        public DateTimeOffset? lastBootTime { get; set; }

        public DeviceDetails()
        {
            deviceID = -1;
            uptime = string.Empty;
            lastBootTime = null;
        }
    }

    public class ErrorInfo
    {
        public string message { get; set; }
        public DateTimeOffset timestamp { get; set; }

        public int count { get; set; }
        public DateTimeOffset? firstTimestamp { get; set; }
        public DateTimeOffset? lastTimestamp { get; set; }

        public ErrorInfo()
        {
            message = string.Empty;
            timestamp = DateTimeOffset.MinValue;

            count = 1;
            firstTimestamp = lastTimestamp = null;
        }

        public void IncrementCount(DateTimeOffset timestamp)
        {
            ++count;
            if (firstTimestamp.HasValue == false)
                firstTimestamp = timestamp;
            if (lastTimestamp.HasValue == false)
                lastTimestamp = timestamp;

            if (timestamp.CompareTo(firstTimestamp.Value) < 0)
                firstTimestamp = timestamp;
            if (timestamp.CompareTo(lastTimestamp.Value) > 0)
                lastTimestamp = timestamp;
        }
    }

    public class DeviceErrors
    {
        public int deviceID { get; set; }
        public List<ErrorInfo> errors { get; set; }

        public DeviceErrors()
        {
            deviceID = -1;
            errors = new List<ErrorInfo>();
        }
    }

    /// <summary>
    /// Used to send a snapshot of the applications at the specified timestamp to the web server
    /// </summary>
    public class DeviceApplications
    {
        public int deviceID { get; set; }
        public List<ApplicationInfo> applications { get; set; }
        public DateTimeOffset timestamp { get; set; }

        public DeviceApplications()
        {
            deviceID = -1;
            applications = new List<ApplicationInfo>();
            timestamp = DateTimeOffset.MinValue;
        }

        public class ApplicationInfo
        {
            public string name { get; set; }
            public string version { get; set; }
        }
    }

    public class ValueInfo
    {
        public int deviceID { get; set; }
        public CollectorType collectorType { get; set; }
        public string value { get; set; }
        public DateTimeOffset timestamp { get; set; }
        [JsonIgnore]
        public bool IsValid { get { return string.IsNullOrEmpty(value) == false && timestamp != DateTimeOffset.MinValue; } }

        public ValueInfo()
        {
            deviceID = -1;
            collectorType = CollectorType.Unknown;
            value = string.Empty;
            timestamp = DateTimeOffset.MinValue;
        }
    }

    public class UPSStatus
    {
        public int deviceID { get; set; }
        public DateTimeOffset timestamp { get; set; }
        public string name { get; set; }
        public string upsStatus { get; set; }
        public string batteryStatus { get; set; }
        public int estimatedRunTimeInMinutes { get; set; }
        public int estimatedChargeRemainingPercentage { get; set; }

        public UPSStatus()
        {
            deviceID = -1;
            timestamp = DateTimeOffset.MinValue;
            name = upsStatus = batteryStatus = string.Empty;
            estimatedRunTimeInMinutes = estimatedChargeRemainingPercentage = 0;
        }
    }

    public class ProcessHistory
    {
        public int deviceID { get; set; }
        public string processName { get; set; }

        // For performance reasons, this will be a list of [timestamp, CPU %, Memory]
        public List<object[]> details { get; set; }

        public ProcessHistory()
        {
            deviceID = -1;
            processName = string.Empty;
            details = new List<object[]>();
        }

        public void AddData(DateTimeOffset timestamp, ulong cpu, ulong memory)
        {
            object[] o = new object[] { timestamp, cpu, memory };
            details.Add(o);
        }
    }

    public class DeviceProcessInfo
    {
        public int deviceID { get; set; }
        public Dictionary<ulong, List<String>> cpuToProcesses { get; set; }
        public DateTimeOffset timestamp { get; set; }

        public DeviceProcessInfo()
        {
            deviceID = -1;
            cpuToProcesses = new Dictionary<ulong, List<string>>();
            timestamp = DateTimeOffset.MinValue;
        }

        public void Add(string process, ulong cpu)
        {
            List<string> processes;
            cpuToProcesses.TryGetValue(cpu, out processes);
            if(processes == null)
            {
                processes = new List<string>();
                cpuToProcesses[cpu] = processes;
            }
            processes.Add(process);
        }

        public void Sort()
        {
            foreach (List<string> processes in cpuToProcesses.Values)
                processes.Sort((a, b) => string.Compare(a, b, true));
        }
    }

    public class DatabaseDetail
    {
        public int sizeInMB { get; set; }
        public DateTimeOffset timestamp { get; set; }
    }

    public class DatabaseHistory
    {
        public int deviceID { get; set; }
        public string databaseName { get; set; }
        public List<DatabaseDetail> details { get; set; }

        public DatabaseHistory()
        {
            deviceID = -1;
            databaseName = string.Empty;
            details = new List<DatabaseDetail>();
        }
    }

    /// <summary>
    /// The application data collected has this format, but we typically want to send it to the
    /// web site in the DeviceApplications.ApplicationInfo format, so there's an easy
    /// conversion application provided.
    /// </summary>
    public class ApplicationInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public DeviceApplications.ApplicationInfo AsDeviceApplicationsApplicationInfo()
        {
            return new DeviceApplications.ApplicationInfo()
            {
                name = Name,
                version = Version,
            };
        }
    }

    /// <summary>
    /// Used when building the app changes part of the report. It returns the name of the app, the version before/after
    /// the change, and the time the change occurred.
    /// </summary>
    //public class ApplicationChanges
    //{
    //    public string name { get; set; }
    //    public string before { get; set; }
    //    public string after { get; set; }
    //    public DateTimeOffset timeChanged { get; set; }
    //}

    public class ApplicationHistory
    {
        public string name { get; set; }
        public List<Snapshot> history { get; set; }

        public ApplicationHistory(string name)
        {
            this.name = name;
            history = new List<Snapshot>();
        }

        public class Snapshot
        {
            public string version { get; set; }
            public DateTimeOffset timestamp { get; set; }
        }
    }

    public class AllApplicationsHistory
    {
        public Dictionary<string, ApplicationHistory> history { get; set; }

        public AllApplicationsHistory()
        {
            history = new Dictionary<string, ApplicationHistory>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void AddHistory(string app_name, string version, DateTimeOffset timestamp)
        {
            ApplicationHistory hist = null;
            if(history.TryGetValue(app_name, out hist) == false)
            {
                hist = new ApplicationHistory(app_name);
                history[app_name] = hist;
            }

            hist.history.Add(new ApplicationHistory.Snapshot { version = version, timestamp = timestamp });
        }
    }

    public abstract class CurrentPeakReportBase
    {
        public int currentPercentUsed { get; set; }
        public int peakPercentUsed { get; set; }
        public DateTimeOffset? peakTimestamp { get; set; }

        public CurrentPeakReportBase()
        {
            currentPercentUsed = peakPercentUsed = -1;
            peakTimestamp = null;
        }

        public void Insert(int percent_used, DateTimeOffset timestamp)
        {
            // The data is coming back in time order, so the most recent will be the last one.
            // Just update current and the last one will be the most recent.
            currentPercentUsed = percent_used;

            // Do >= here so we will show the most recent peak if there are multiples of the same value
            if (percent_used >= peakPercentUsed)
            {
                peakPercentUsed = percent_used;
                peakTimestamp = timestamp;
            }
        }
    }

    public class MemoryReport : CurrentPeakReportBase
    {
        public MemoryReport() : base()
        {
        }
    }

    public class DiskReport
    {
        public class DiskInfo : CurrentPeakReportBase
        {
            public string name { get; set; }

            public DiskInfo(string name) : base()
            {
                this.name = name;
            }
        }

        public List<DiskInfo> disks { get; set; }

        public DiskReport()
        {
            disks = new List<DiskInfo>();
        }
    }

    public class CPUReport : CurrentPeakReportBase
    {
        public CPUReport() : base()
        {
        }
    }

    public class NICReport : CurrentPeakReportBase
    {
        public int bps { get; set; }
        public int peakBps { get; set; }

        public NICReport() : base()
        {
            bps = peakBps = -1;
        }

        public void Insert(int percent_used, DateTimeOffset timestamp, int bps)
        {
            // The data is coming back in time order, so the most recent will be the last one.
            // Just update current and the last one will be the most recent.
            currentPercentUsed = percent_used;
            this.bps = bps;

            // Do >= here so we will show the most recent peak if there are multiples of the same value
            if (bps >= peakBps)
            {
                peakPercentUsed = percent_used;
                peakBps = bps;
                peakTimestamp = timestamp;
            }
        }
    }

    public class Services
    {
        public List<string> services { get; set; }
        public DateTimeOffset? timestamp { get; set; }

        public Services()
        {
            services = new List<string>();
            timestamp = null;
        }

        public Services(List<string> services)
        {
            this.services = services;
            timestamp = null;
        }
    }
}
