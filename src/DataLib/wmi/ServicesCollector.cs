using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data.wmi
{
    public class ServicesCollector : ListWMIDataCollector
    {
        public ServicesCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_Service", "DisplayName", remote_info),
                new DataCollectorContext(id, ECollectorType.Services),
                ListStringData.Options.IgnoreCase | ListStringData.Options.KeepSorted | ListStringData.Options.NoDuplicates)
        {
            // Just find the services that are actually running
            WmiContext.Where = "State='Running'";
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            ListStringData d = new ListStringData(context, ListStringData.Options.IgnoreCase | ListStringData.Options.KeepSorted | ListStringData.Options.NoDuplicates);
            var definition = new { Value = new List<string>() };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d.Data.AddRange(data.Value);
            return d;
        }

    }
}
