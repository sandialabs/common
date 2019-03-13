using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class DatabaseHistory
    {
        public int deviceID { get; set; }
        public string databaseName { get; set; }
        public List<DatabaseDetail> details { get; set; }

        public DatabaseHistory()
        {
            deviceID = -1;
            databaseName = string.Empty;
            details = new List<DatabaseDetail>();
        }
    }
}
