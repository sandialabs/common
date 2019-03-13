using Npgsql;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data.database
{
    public class PostgresCollector : IDatabaseCollector
    {
        // This query was found at:
        // https://wiki.postgresql.org/wiki/Disk_Usage
        public static string Query
        {
            get =>
@"
SELECT d.datname AS Name,  pg_catalog.pg_get_userbyid(d.datdba) AS Owner,
    CASE WHEN pg_catalog.has_database_privilege(d.datname, 'CONNECT')
        THEN pg_catalog.pg_size_pretty(pg_catalog.pg_database_size(d.datname))
        ELSE 'No Access'
    END AS SIZE
FROM pg_catalog.pg_database d
    ORDER BY
    CASE WHEN pg_catalog.has_database_privilege(d.datname, 'CONNECT')
        THEN pg_catalog.pg_database_size(d.datname)
        ELSE NULL
    END DESC -- nulls first
";
        }

        public Tuple<Dictionary<string, ulong>, bool> GetData(string connection_string)
        {
            Dictionary<string, ulong> data = new Dictionary<string, ulong>();
            bool success = false;

            using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(Query, conn))
                using(NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        ulong sizeInMB = (ulong)reader.GetDecimal(2);

                        data[name] = sizeInMB;

                        success = true;
                    }
                }
            }

            return Tuple.Create(data, success);
        }
    }
}
