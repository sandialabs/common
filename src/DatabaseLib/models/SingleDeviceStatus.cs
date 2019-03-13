using gov.sandia.sld.common.configuration;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
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

            if (alert_level == EAlertLevel.Alarm)
                alarms.Add(ds);
            status.Add(ds);
        }
    }
}
