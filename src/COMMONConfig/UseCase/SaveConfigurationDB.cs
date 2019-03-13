using COMMONConfig.Boundaries.Configuration;
using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.models;
using System;
using System.Data.SQLite;

namespace COMMONConfig.UseCase
{
    public class SaveConfigurationDB : ISaveRequest
    {
        public void SaveRequest(SystemConfiguration configuration)
        {
            Database db = new Database();
            SystemConfigurationStore.Set(configuration, DateTimeOffset.Now, db);
        }
    }
}
