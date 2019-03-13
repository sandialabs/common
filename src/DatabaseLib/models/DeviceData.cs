using System;

namespace gov.sandia.sld.common.db.models
{
    public class DeviceData
    {
        public long dataID { get; set; }
        public long collectorID { get; set; }
        public string value { get; set; }
        public DateTimeOffset timeStamp { get; set; }

        public DeviceData Clone()
        {
            return new DeviceData() { dataID = dataID, collectorID = collectorID, value = value, timeStamp = timeStamp };
        }
    }
}
