using gov.sandia.sld.common.data;
using gov.sandia.sld.common.data.wmi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    /// <summary>
    /// Basically this is a mashup of DiskUsage and DeviceData
    /// </summary>
    public class DiskUsageData
    {
        public long dataID { get; set; }
        public long collectorID { get; set; }
        public DateTimeOffset timeStamp { get; set; }
        public UInt64 capacity { get; set; }
        public UInt64 free { get; set; }
        public UInt64 used { get { return capacity - free; } }

        public DiskUsageData()
        {
            dataID = collectorID = -1;
            timeStamp = DateTimeOffset.MinValue;
            capacity = free = 0;
        }

        public DiskUsageData(DiskUsage du)
        {
            dataID = collectorID = -1;
            timeStamp = DateTimeOffset.MinValue;
            capacity = du.CapacityNum;
            free = du.FreeNum;
        }

        public static List<Tuple<string, DiskUsageData>> FromJSON(string json)
        {
            List<Tuple<string, DiskUsageData>> list = new List<Tuple<string, DiskUsageData>>();

            if (json.Contains("Disk Name"))
            {
                // Old-style, convert it to the new-style
                // Old: Value: {"Disk Capacity":"123","Disk Free":"456","Disk Name":"C:","Disk Used":"789"}
                // New: Value: {"Capacity":"123","Free":"456","Used":"789"}
                DictionaryData d = JsonConvert.DeserializeObject<DictionaryData>(json);
                string disk_name = d.Data["Disk Name"];
                string capacity = d.Data["Disk Capacity"];
                string free = d.Data["Disk Free"];
                ulong c, f;
                if (string.IsNullOrEmpty(disk_name) == false &&
                    ulong.TryParse(capacity, out c) &&
                    ulong.TryParse(free, out f))
                {
                    DiskUsageData du = new DiskUsageData()
                    {
                        capacity = c,
                        free = f,
                    };
                    list.Add(Tuple.Create(disk_name, du));
                }
            }
            else
            {
                var definition = new { Value = new Dictionary<string, DiskUsage>() };
                var value = JsonConvert.DeserializeAnonymousType(json, definition);
                foreach (KeyValuePair<string, DiskUsage> kvp in value.Value)
                {
                    DiskUsageData du = new DiskUsageData()
                    {
                        capacity = kvp.Value.CapacityNum,
                        free = kvp.Value.FreeNum,
                    };
                    list.Add(Tuple.Create(kvp.Key, du));
                }
            }

            return list;
        }
    }
}
