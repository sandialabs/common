namespace gov.sandia.sld.common.db.models
{
    //public class DiskData
    //{
    //    public Dictionary<string, List<DeviceData>> DiskToDeviceData { get; set; }
    //    public bool 
    //}

    /// <summary>
    /// Holds an individual key/value pair from the Configuration table.
    /// 
    /// The deleted attribute is set to 1 when the user deletes a configuration
    /// value. When it's retrieved from the database and sent out it will be 0.
    /// </summary>
    public class ConfigurationData
    {
        /// <summary>
        /// The database-assigned ConfigurationID
        /// </summary>
        public int configID { get; set; }

        /// <summary>
        /// The specified path (i.e. 'site.name')
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// The configuration value (i.e. 'Freeport')
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// true if the configuration value should be deleted; defaults to false
        /// </summary>
        public bool deleted { get; set; }

        public ConfigurationData()
        {
            configID = -1;
            path = value = string.Empty;
            deleted = false;
        }
    }
}
