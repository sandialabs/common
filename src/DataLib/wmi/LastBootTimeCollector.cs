using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System;

namespace gov.sandia.sld.common.data.wmi
{
    public class LastBootTimeCollector : DateTimeCollector
    {
        public LastBootTimeCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_OperatingSystem", "LastBootUpTime", remote_info),
                  new DataCollectorContext(id, ECollectorType.LastBootTime))
        {
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            Data d = null;
            var definition = new { Value = string.Empty };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d = new Data(context) { Value =data.Value };
            return d;
        }
    }
}
