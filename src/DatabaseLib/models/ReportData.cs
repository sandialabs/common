namespace gov.sandia.sld.common.db.models
{
    public class ReportData
    {
        public int id { get; private set; }
        public IStartEndTime startEnd { get; private set; }

        public MemoryReport memory { get; set; }
        public DiskReport disk { get; set; }
        public CPUReport cpu { get; set; }
        public NICReport nic { get; set; }

        public ReportData(int deviceID, IStartEndTime start_end)
        {
            id = deviceID;
            startEnd = start_end;
        }
    }
}
