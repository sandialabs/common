using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data.wmi
{
    public class ApplicationInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class ApplicationsCollector : WMIDataCollector
    {
        public ApplicationsCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_Product", "Name,Version", remote_info),
                  new DataCollectorContext(id, ECollectorType.InstalledApplications))
        {
            RetrieverOptions = new WMIRetrieverOptions() { Timeout = TimeSpan.FromMinutes(3) };
        }

        public override CollectedData OnAcquire()
        {
            ListData<ApplicationInfo> data = new ListData<ApplicationInfo>(Context);

            OnAcquireDelegate(
                dict =>
                {
                    if(dict.ContainsKey("Name") &&
                        dict.ContainsKey("Version"))
                    {
                        object name = dict["Name"];
                        object version = dict["Version"];

                        if(name != null && version != null)
                            data.Data.Add(new ApplicationInfo() { Name = name.ToString(), Version = version.ToString() });
                    }
                    else
                    {
                        m_log.Debug("ApplicationsCollector: Missing Name and/or Version");
                    }
                });

            return new CollectedData(Context, data.Data.Count > 0, data);
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            ListData<ApplicationInfo> d = new ListData<ApplicationInfo>(context);
            var definition = new { Value = new List<ApplicationInfo>() };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d.Data.AddRange(data.Value);
            return d;
        }

    }
}
