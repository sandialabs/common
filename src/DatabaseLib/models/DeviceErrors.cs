using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class DeviceErrors
    {
        public int deviceID { get; set; }
        public List<ErrorInfo> errors { get; set; }

        public DeviceErrors()
        {
            deviceID = -1;
            errors = new List<ErrorInfo>();
        }
    }
}
