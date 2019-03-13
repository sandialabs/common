using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gov.sandia.sld.common.db.reports
{
    public class Disk : Report<DiskReport>
    {
        public Disk(Database db)
            : base(db)
        {
        }

        public override DiskReport GetReport(int deviceID, IStartEndTime start_end)
        {
            IEnumerable<DeviceData> data = DB.GetDeviceData(deviceID, ECollectorType.Disk, start_end);
            //var definition = new { Value = new Dictionary<string, Dictionary<string, string>>() };
            Dictionary<string, DiskReport.DiskInfo> disk_info = new Dictionary<string, DiskReport.DiskInfo>(StringComparer.InvariantCultureIgnoreCase);

            foreach (DeviceData d in data)
            {
                try
                {
                    //var value = JsonConvert.DeserializeAnonymousType(d.value, definition);
                    var value = JsonConvert.DeserializeObject<DictToDictValue<string, string, string>>(d.value);
                    List<string> disks = value.Value.Keys.ToList();
                    disks.Sort();

                    foreach (string disk in disks)
                    {
                        Dictionary<string, string> disk_data = value.Value[disk];

                        if (disk_info.TryGetValue(disk, out DiskReport.DiskInfo info) == false)
                        {
                            info = new DiskReport.DiskInfo(disk);
                            disk_info[disk] = info;
                        }

                        if (disk_data.TryGetValue("Used", out string used_str) &&
                            disk_data.TryGetValue("Capacity", out string capacity_str))
                        {
                            if (long.TryParse(used_str, out long used) &&
                                long.TryParse(capacity_str, out long capacity) &&
                                capacity > 0)
                            {
                                int percent_used = (int)((double)used / (double)capacity * 100.0);
                                info.Insert(percent_used, d.timeStamp);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            DiskReport report = null;
            List<string> all_disks = disk_info.Keys.ToList();
            if (all_disks.Count > 0)
            {
                report = new DiskReport();
                all_disks.Sort();
                all_disks.ForEach(d => report.disks.Add(disk_info[d]));
            }

            return report;
        }

        private class DictToDictValue<T, U, V>
        {
            public Dictionary<T, Dictionary<U, V>> Value { get; set; }
        }
    }
}
