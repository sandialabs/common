using gov.sandia.sld.common.configuration;

namespace gov.sandia.sld.common.requestresponse
{
    public class DatabaseTypeRequest : Request
    {
        public string DeviceName { get; private set; }
        public string ConnectionString { get; set; }
        public EDatabaseType DatabaseType { get; set; }

        public DatabaseTypeRequest(string name, string device_name)
            : base(name)
        {
            DeviceName = device_name;
        }
    }
}
