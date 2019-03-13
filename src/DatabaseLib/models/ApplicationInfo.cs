namespace gov.sandia.sld.common.db.models
{
    /// <summary>
    /// The application data collected has this format, but we typically want to send it to the
    /// web site in the DeviceApplications.ApplicationInfo format, so there's an easy
    /// conversion application provided.
    /// </summary>
    public class ApplicationInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public DeviceApplications.ApplicationInfo AsDeviceApplicationsApplicationInfo()
        {
            return new DeviceApplications.ApplicationInfo()
            {
                name = Name,
                version = Version,
            };
        }
    }
}
