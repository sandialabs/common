using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System;

namespace gov.sandia.sld.common.data.wmi
{
    public class UptimeCollector : WMIDataCollector
    {
        public UptimeCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_PerfFormattedData_PerfOS_System", "SystemUpTime", remote_info),
                  new DataCollectorContext(id, ECollectorType.Uptime))
        {
        }

        public override CollectedData OnAcquire()
        {
            CollectedData data = base.OnAcquire();

            if (data.DataIsCollected)
            {
                foreach (Data d in data.D)
                {
                    // The value is in seconds. I think we probably want that in days hours:minutes:seconds format
                    UInt64? seconds = d.ValueAsUInt64;
                    if (seconds != null)
                    {
                        UInt64 days = seconds.Value / (60 * 60 * 24);
                        UInt64 hours = (seconds.Value / (60 * 60)) % 24;
                        UInt64 minutes = (seconds.Value / 60) % 60;
                        UInt64 seconds2 = seconds.Value % 60;

                        d.Value = string.Format("{0} {1:D2}:{2:D2}:{3:D2}", days, hours, minutes, seconds2);
                    }
                }
            }

            return data;
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            Data d = null;
            var definition = new { Value = string.Empty };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d = new Data(context) { Value = data.Value };
            return d;
        }
    }
}
