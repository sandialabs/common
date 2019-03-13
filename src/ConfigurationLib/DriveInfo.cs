using gov.sandia.sld.common.utilities;

namespace gov.sandia.sld.common.configuration
{
    public class DriveInfo
    {
        public string letter { get; set; }
        public string name { get; set; }
        public string typeDescription { get { return type.GetDescription(); } }
        public EDriveType type { get; set; }

        public DriveInfo()
        {
            letter = name = string.Empty;
            type = EDriveType.Unknown;
        }
    }
}

