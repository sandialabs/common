using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data.wmi
{
    public class MemoryUsage
    {
        [JsonProperty(PropertyName="Memory Capacity")]
        public string Capacity { get { return CapacityNum.ToString(); } set { if (ulong.TryParse(value, out ulong c)) CapacityNum = c; } }
        [JsonProperty(PropertyName = "Free Memory")]
        public string Free { get { return FreeNum.ToString(); } set { if (ulong.TryParse(value, out ulong f)) FreeNum = f; } }
        [JsonProperty(PropertyName = "Memory Used")]
        public string Used { get { return UsedNum.ToString(); } }

        [JsonIgnore]
        public ulong CapacityNum { get; set; }
        [JsonIgnore]
        public ulong FreeNum { get; set; }
        [JsonIgnore]
        public ulong UsedNum { get { return CapacityNum - FreeNum; } }
    }

    public class MemoryUsageCollector : DataCollector
    {
        public MemoryUsageCollector(CollectorID id, Remote remote_info)
            : base(new DataCollectorContext(id, ECollectorType.Memory))
        {
            m_capacity = new MemoryCapacity(new CollectorID(-1, Context.Name + ".MemoryCapacity"), remote_info);
            m_free = new MemoryFree(new CollectorID(-1, Context.Name + ".FreeMemory"), remote_info);
        }

        public override CollectedData OnAcquire()
        {
            CollectedData capacity_data = m_capacity.OnAcquire();
            CollectedData free_data = m_free.OnAcquire();
            Data data = null;

            if (capacity_data.DataIsCollected && free_data.DataIsCollected)
            {
                if (capacity_data.D.Count > 0 && free_data.D.Count > 0)
                {
                    Data capacity = capacity_data.D[0];
                    Data free = free_data.D[0];

                    ulong? c = capacity.ValueAsUInt64;
                    ulong? f = free.ValueAsUInt64;

                    if (c.HasValue && f.HasValue)
                    {
                        MemoryUsage usage = new MemoryUsage() { CapacityNum = c.Value, FreeNum = f.Value };
                        data = new GenericData<MemoryUsage>(Context, usage);

                        //UInt64 used = c.Value - f.Value;

                        //d.Data["Memory Capacity"] = capacity.Value.ToString();
                        //d.Data["Free Memory"] = free.Value.ToString();
                        //d.Data["Memory Used"] = used.ToString();
                    }
                }
            }

            return new CollectedData(Context, data != null, data);
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

        private MemoryCapacity m_capacity;
        private MemoryFree m_free;
    }

    public class MemoryCapacity : WMIDataCollector
    {
        public MemoryCapacity(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_OperatingSystem", "TotalVisibleMemorySize", remote_info),
                  new DataCollectorContext(id, ECollectorType.Memory))
        {
        }

        public override CollectedData OnAcquire()
        {
            // For memory, WMI gives you the values in KB. Let's convert KB to B for consistency. Just multiply it by 1024.
            CollectedData capacity_data = base.OnAcquire();
            if (capacity_data.DataIsCollected)
            {
                foreach (Data d in capacity_data.D)
                    d.Value = (d.ValueAsUInt64 * 1024).ToString();
            }
            return capacity_data;
        }
    }

    public class MemoryFree : WMIDataCollector
    {
        public MemoryFree(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_OperatingSystem", "FreePhysicalMemory", remote_info),
                new DataCollectorContext(id, ECollectorType.Memory))
        {
        }

        public override CollectedData OnAcquire()
        {
            // For memory, WMI gives you the values in KB. Let's convert KB to B for consistency. Just multiply it by 1024.
            CollectedData free_data = base.OnAcquire();
            if (free_data.DataIsCollected)
            {
                foreach (Data d in free_data.D)
                    d.Value = (d.ValueAsUInt64 * 1024).ToString();
            }
            return free_data;
        }
    }
}
