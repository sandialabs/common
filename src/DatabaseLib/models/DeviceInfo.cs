using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    /// <summary>
    /// Used to pass info about devices to the user
    /// </summary>
    public class DeviceInfo
    {
        private DeviceID m_device_id { get; set; }

        [JsonIgnore]
        public DeviceID DID { get { return m_device_id; } set { if (value == null) return; UpdateCollectors(value); m_device_id = value; } }

        public long id { get { return m_device_id.ID; } set { m_device_id.ID = value; } }

        /// <summary>
        /// The name of the device (i.e. "Server" or "Lane 1 Workstation")
        /// </summary>
        public string name { get { return m_device_id.Name; } set { m_device_id.Name = value; } }

        /// <summary>
        /// The type of device (see the DeviceType enum)
        /// </summary>
        public EDeviceType type { get; set; }

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
            type = EDeviceType.Unknown;
            ipAddress = "0.0.0.0";
            username = password = string.Empty;
            deleted = false;
            collectors = new List<CollectorInfo>();
            driveNames = new Dictionary<string, string>();
            monitoredDrives = new MonitoredDriveManager();
            groupID = -1;
        }

        public DeviceInfo(EDeviceType type)
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
}
