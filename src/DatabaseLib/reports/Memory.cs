using gov.sandia.sld.common.db.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.reports
{
    public class Memory : Report<MemoryReport>
    {
        public Memory(Database db)
            : base(db)
        {
        }

        public override MemoryReport GetReport(int deviceID, IStartEndTime start_end)
        {
            MemoryReport report = null;
            IEnumerable<DeviceData> data = DB.GetMemoryData(deviceID, start_end);

            foreach (DeviceData d in data)
            {
                try
                {
                    var value = JsonConvert.DeserializeObject<DictValue<string, string>>(d.value);
                    if (value != null)
                        ProcessValue(ref report, d, value.Value);
                }
                catch (Exception)
                {
                }
            }

            return report;
        }

        private void ProcessValue(ref MemoryReport report, DeviceData d, Dictionary<string, string> value)
        {
            if (value.TryGetValue("Memory Used", out string used_str) &&
                value.TryGetValue("Memory Capacity", out string capacity_str))
            {
                Int64 used, capacity;
                if (Int64.TryParse(used_str, out used) &&
                    Int64.TryParse(capacity_str, out capacity))
                {
                    int percent_used = (int)((double)used / (double)capacity * 100.0);

                    if(report == null)
                        report = new MemoryReport();
                    report.Insert(percent_used, d.timeStamp);
                }
            }
        }

        private class DictValue<T, U>
        {
            public Dictionary<T, U> Value { get; set; }
        }
    }
}
