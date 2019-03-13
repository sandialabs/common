using System;

namespace gov.sandia.sld.common.db.models
{
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
}
