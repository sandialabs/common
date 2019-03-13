using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.reports
{
    public class CPU : Report<CPUReport>
    {
        public CPU(Database db)
            : base(db)
        {
        }

        public override CPUReport GetReport(int deviceID, IStartEndTime start_end)
        {
            CPUReport report = null;
            IEnumerable<DeviceData> data = DB.GetDeviceData(deviceID, ECollectorType.CPUUsage, start_end);
            //var definition = new { Value = string.Empty };

            foreach (DeviceData d in data)
            {
                try
                {
                    //var value = JsonConvert.DeserializeAnonymousType(d.value, definition);
                    var value = JsonConvert.DeserializeObject<ValueClass<string>>(d.value);
                    string percent_str = value.Value;
                    if (long.TryParse(percent_str, out long percent))
                    {
                        if (report == null)
                            report = new CPUReport();
                        report.Insert((int)percent, d.timeStamp);
                    }
                }
                catch (Exception)
                {
                }
            }

            return report;
        }

        private class ValueClass<T>
        {
            public T Value { get; set; }
        }

    }
}
