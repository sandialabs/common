namespace gov.sandia.sld.common.db.models
{
    /// <summary>
    /// Describes the status, and if the status is an alarm or not
    /// </summary>
    public class DeviceStatus
    {
        public string status { get; set; }
        public int alertLevel { get; set; }
        public string message { get; set; }
    }
}
