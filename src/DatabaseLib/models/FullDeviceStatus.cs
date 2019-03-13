using gov.sandia.sld.common.configuration;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    /// <summary>
    /// Used to send alerts and status to the user
    /// </summary>
    public class FullDeviceStatus
    {
        /// <summary>
        /// Maps the device to the set of statuses, including alarms
        /// </summary>
        public Dictionary<long, List<DeviceStatus>> fullStatus { get; set; }

        public FullDeviceStatus()
        {
            fullStatus = new Dictionary<long, List<DeviceStatus>>();
        }

        public void AddStatus(string name, long device_id, string description, EAlertLevel alert_level, string message)
        {
            AddDescription(fullStatus, device_id, description, alert_level, message);
        }

        private static void AddDescription(Dictionary<long, List<DeviceStatus>> dict, long device_id, string description, EAlertLevel alert_level, string message)
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
}
