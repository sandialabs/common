namespace gov.sandia.sld.common.configuration
{
    public class MonitoredDrive : DriveInfo
    {
        public bool isMonitored { get; set; }

        public MonitoredDrive()
        {
            isMonitored = true;
        }
    }
}
