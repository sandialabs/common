using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System;

namespace gov.sandia.sld.common.db.models
{
    public class ValueInfo
    {
        public int deviceID { get; set; }
        public ECollectorType collectorType { get; set; }
        public string value { get; set; }
        public DateTimeOffset timestamp { get; set; }
        [JsonIgnore]
        public bool IsValid { get { return string.IsNullOrEmpty(value) == false && timestamp != DateTimeOffset.MinValue; } }

        public ValueInfo()
        {
            deviceID = -1;
            collectorType = ECollectorType.Unknown;
            value = string.Empty;
            timestamp = DateTimeOffset.MinValue;
        }
    }
}
