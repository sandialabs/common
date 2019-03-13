using gov.sandia.sld.common.configuration;

namespace gov.sandia.sld.common.requestresponse
{
    public class MonitoredDrivesRequest : Request
    {
        public string MachineName { get; }
        public MonitoredDriveManager DriveManager { get; set; }

        public MonitoredDrivesRequest(string machine_name)
            : base("MonitoredDrivesRequest: " + machine_name)
        {
            MachineName = machine_name;
        }
    }
}
