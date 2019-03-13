using System;

namespace gov.sandia.sld.common.db.models
{
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
}
