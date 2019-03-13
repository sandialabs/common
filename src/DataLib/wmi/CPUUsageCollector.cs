using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;

namespace gov.sandia.sld.common.data.wmi
{
    public class CPUUsageCollector : MultiPropertyWMIDataCollector
    {
        public CPUUsageCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_PerfFormattedData_PerfOS_Processor", "Name,PercentProcessorTime", remote_info),
                  new DataCollectorContext(id, ECollectorType.CPUUsage))
        {
        }

        public override CollectedData OnAcquire()
        {
            // The total, average, CPU usage is the last one, but we can't guarantee the last
            // one returned is the actual one named "_Total", so we have to check for that.
            // http://www.allenconway.net/2013/07/get-cpu-usage-across-all-cores-in-c.html

            bool success = false;
            Data d = new Data(Context);
            OnAcquireDelegate(
                dict =>
                {
                    string name = dict["Name"].ToString();
                    object p = dict["PercentProcessorTime"];
                    if (name == "_Total" && p != null)
                    {
                        d.Value = p.ToString();
                        success = true;
                    }
                });

            return new CollectedData(Context, success, d);
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            Data d = null;
            var definition = new { Value = string.Empty };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d = new Data(context) { Value = data.Value.ToString() };
            return d;
        }
    }
}
