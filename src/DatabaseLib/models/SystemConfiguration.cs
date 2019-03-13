using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gov.sandia.sld.common.db.models
{
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
        public DateTimeOffset mostRecentData { get; set; }

        public SystemConfiguration()
        {
            configuration = new Dictionary<string, ConfigurationData>(StringComparer.OrdinalIgnoreCase);
            devices = new List<DeviceInfo>();
            groups = new List<Group>();
            languages = new List<LanguageConfiguration>();
            softwareVersion = string.Empty;
            mostRecentData = DateTimeOffset.Now;
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
            DeviceInfo system = devices.FirstOrDefault(d => d.type == EDeviceType.System);
            doomed.Remove("System");
            DeviceInfo server = devices.FirstOrDefault(d => d.type == EDeviceType.Server);
            if (server != null)
                doomed.Remove(server.name);

            foreach (DeviceInfo new_device in config.devices)
            {
                // Don't allow the System to be imported
                if (new_device.type == EDeviceType.System)
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
                    case EDeviceType.Server:
                    case EDeviceType.System:
                        insert_device.ipAddress = "0.0.0.0";
                        break;
                    case EDeviceType.Workstation:
                        insert_device.ipAddress = new_device.ipAddress;
                        insert_device.username = new_device.username;
                        insert_device.password = new_device.password;
                        break;
                    case EDeviceType.Camera:
                    case EDeviceType.RPM:
                    case EDeviceType.Generic:
                    case EDeviceType.Unknown:
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

            mostRecentData = config.mostRecentData;
        }
    }
}
