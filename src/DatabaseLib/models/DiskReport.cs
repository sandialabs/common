using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class DiskReport
    {
        public class DiskInfo : CurrentPeakReportBase
        {
            public string name { get; set; }

            public DiskInfo(string name) : base()
            {
                this.name = name;
            }
        }

        public List<DiskInfo> disks { get; set; }

        public DiskReport()
        {
            disks = new List<DiskInfo>();
        }
    }
}
