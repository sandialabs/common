namespace gov.sandia.sld.common.db.reports
{
    public abstract class Report<T>
    {
        public Database DB { get; private set; }

        public Report(Database db)
        {
            DB = db;
        }

        public abstract T GetReport(int deviceID, IStartEndTime start_end);
    }
}
