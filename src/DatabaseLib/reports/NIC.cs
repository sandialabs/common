using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.reports
{
    public class NIC : Report<NICReport>
    {
        public NIC(Database db)
            : base(db)
        {
        }

        public override NICReport GetReport(int deviceID, IStartEndTime start_end)
        {
            NICReport report = null;
            IEnumerable<DeviceData> data = DB.GetDeviceData(deviceID, ECollectorType.NICUsage, start_end);
            //var definition = new { Value = new Dictionary<string, string>() };

            foreach (DeviceData d in data)
            {
                try
                {
                    //var value = JsonConvert.DeserializeAnonymousType(d.value, definition);
                    var value = JsonConvert.DeserializeObject<DictValue<string, string>>(d.value);
                    if (value.Value.TryGetValue("Avg", out string avg_str) &&
                        value.Value.TryGetValue("BPS", out string bps_str))
                    {
                        double avg;
                        Int64 bps;
                        if (double.TryParse(avg_str, out avg) &&
                            Int64.TryParse(bps_str, out bps))
                        {
                            if (report == null)
                                report = new NICReport();
                            report.Insert((int)(avg + 0.5f), d.timeStamp, (int)bps);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return report;
        }

        private class DictValue<T, U>
        {
            public Dictionary<T, U> Value { get; set; }
        }
    }
}
