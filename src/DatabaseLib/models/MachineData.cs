using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class MachineData
    {

        public int id { get; private set; }
        public IStartEndTime startEnd { get; private set; }

        public List<DeviceData> cpu { get; set; }
        public List<DeviceData> nic { get; set; }
        public DeviceProcessInfo processes { get; set; }
        public List<DeviceData> memory { get; set; }
        public Dictionary<string, DiskUsageWithSmartData> diskUsage { get; set; }
        public Dictionary<string, List<DeviceData>> diskPerformance { get; set; }
        public DeviceErrors systemErrors { get; set; }
        public DeviceErrors applicationErrors { get; set; }
        public ValueInfo database { get; set; }
        public DeviceApplications applications { get; set; }
        public Services services { get; set; }
        public List<DeviceData> ups { get; set; }

        public MachineData(int deviceID, IStartEndTime start_end)
        {
            id = deviceID;
            startEnd = start_end;
        }
    }
}
