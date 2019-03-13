using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gov.sandia.sld.common.data.database
{
    public interface IDatabaseCollectorFactory
    {
        IDatabaseCollector Create(EDatabaseType type);
    }

    public class DatabaseCollectorFactory : IDatabaseCollectorFactory
    {
        public IDatabaseCollector Create(EDatabaseType type)
        {
            IDatabaseCollector collector = null;
            try
            {
                switch (type)
                {
                    case EDatabaseType.Oracle:
                        collector = new OracleCollector();
                        break;
                    case EDatabaseType.SqlServer:
                        collector = new SqlServerCollector();
                        break;
                    case EDatabaseType.Postgres:
                        collector = new PostgresCollector();
                        break;
                    case EDatabaseType.Unknown:
                    default:
                        throw new Exception($"Unknown database type {type}");
                }
            }
            catch (Exception ex)
            {
                ApplicationEventLog log = new ApplicationEventLog();
                log.LogError("Error in DatabaseCollectorFactory connection");
                log.Log(ex);
            }

            return collector;
        }
    }
}
