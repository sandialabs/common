using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data.wmi
{
    public class NICUsageCollector : WMIDataCollector
    {
        public NICUsageCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_PerfFormattedData_Tcpip_NetworkInterface", "BytesTotalPerSec,CurrentBandwidth", remote_info),
                  new DataCollectorContext(id, ECollectorType.NICUsage))
        {
        }

        public override CollectedData OnAcquire()
        {
            bool collected = false;
            ulong sum_bps = 0;
            ulong sum_bandwidth = 0;

            OnAcquireDelegate(
                dict =>
                {
                    ulong bps = (ulong)dict["BytesTotalPerSec"];
                    ulong bandwidth = (ulong)dict["CurrentBandwidth"];

                    sum_bps += bps;
                    sum_bandwidth += bandwidth;

                    collected = true;
                }
            );

            DictionaryData d = new DictionaryData(Context);
            if (collected)
            {
                // CurrentBandwidth is in bits-per-second, so convert it to bytes
                sum_bandwidth /= 8;
                double avg = ((double)sum_bps / (double)sum_bandwidth) * 100.0f;

                d.Data["BPS"] = sum_bps.ToString();
                d.Data["Capacity"] = sum_bandwidth.ToString();
                d.Data["Avg"] = avg.ToString("0.0");
            }

            return new CollectedData(Context, collected, d);
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            Data d = null;
            var definition = new { Value = new Dictionary<string, string>() };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d = new DictionaryData(context) { Data = data.Value };
            return d;
        }
    }
}
