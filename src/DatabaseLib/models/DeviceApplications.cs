using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
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
}
