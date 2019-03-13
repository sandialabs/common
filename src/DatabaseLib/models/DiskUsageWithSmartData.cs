using gov.sandia.sld.common.configuration;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class DiskUsageWithSmartData
    {
        public string driveLetter { get; set; }
        public List<DiskUsageData> diskUsage { get; set; }
        public HardDisk smartData { get; set; }
    }
}
