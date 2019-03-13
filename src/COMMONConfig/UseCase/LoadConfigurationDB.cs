using COMMONConfig.Boundaries.Configuration;
using gov.sandia.sld.common.db;
using System.Data.SQLite;

namespace COMMONConfig.UseCase
{
    public class LoadConfigurationDB : ILoadRequest
    {
        private readonly ILoadResponse loadBoundaryOutput;

        public LoadConfigurationDB(ILoadResponse loadBoundaryOutput)
        {
            this.loadBoundaryOutput = loadBoundaryOutput;
        }

        public void LoadRequest()
        {
            Database db = new Database();
            new Initializer(null).Initialize(db);
            using (SQLiteConnection conn = db.Connection)
            {
                conn.Open();
                loadBoundaryOutput.ConfigResponse(SystemConfigurationStore.Get(false, conn));
            }
        }
    }
}