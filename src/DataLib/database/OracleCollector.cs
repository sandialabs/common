using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data.database
{
    public class OracleCollector : IDatabaseCollector
    {
        // This query was found at:
        // http://stackoverflow.com/a/4301844/706747
        public static string Query
        {
            get =>
"select TABLESPACE_NAME \"Tablspace\", FILE_NAME \"Filename\", BYTES \"Size\", MAXBYTES \"Maximum Size\", AUTOEXTENSIBLE \"Autoextensible\" from SYS.DBA_DATA_FILES";
        }

        public Tuple<Dictionary<string, ulong>, bool> GetData(string connection_string)
        {
            Dictionary<string, ulong> data = new Dictionary<string, ulong>();
            bool success = false;

            using (OracleConnection conn = new OracleConnection(connection_string))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand(Query, conn))
                using(OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        decimal size = reader.GetDecimal(3);
                        ulong sizeInMB = (ulong)(size / 1024 / 1024);

                        data[name] = sizeInMB;

                        success = true;
                    }
                }
            }

            return Tuple.Create(data, success);
        }
    }
}
