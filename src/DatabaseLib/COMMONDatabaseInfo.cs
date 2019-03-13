using System;
using System.IO;

namespace gov.sandia.sld.common.db
{
    public class COMMONDatabaseInfo
    {
        public Database DB { get; }

        public COMMONDatabaseInfo(Database db)
        {
            DB = db;
        }

        public Tuple<string, uint> GetDatabaseInformation()
        {
            Tuple<string, uint> result = null;

            try
            {
                FileInfo f = new FileInfo(DB.Filename);
                if (f.Exists)
                    result = Tuple.Create(DB.Filename, (uint)(f.Length / (1024 * 1024)));
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}
