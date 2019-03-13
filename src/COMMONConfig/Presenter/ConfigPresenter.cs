using COMMONConfig.Boundaries.Configuration;
using COMMONConfig.UseCase;
using gov.sandia.sld.common.db.models;

namespace COMMONConfig.Presenter
{
    internal class ConfigPresenter : ILoadResponse
    {
        private readonly LoadConfigurationDB loadConfigurationDb;
        private readonly SaveConfigurationDB saveConfigurationDb;
        private readonly LoadConfigurationJSON loadConfigurationJSON;
        private readonly SaveConfigurationJSON saveConfigurationJSON;
        private readonly LoadConfigurationCSV loadConfigurationCSV;
        private readonly SaveConfigurationCSV saveConfigurationCSV;

        public ConfigPresenter()
        {
            loadConfigurationDb = new LoadConfigurationDB(this);
            saveConfigurationDb = new SaveConfigurationDB();

            loadConfigurationJSON = new LoadConfigurationJSON(this);
            saveConfigurationJSON = new SaveConfigurationJSON();

            loadConfigurationCSV = new LoadConfigurationCSV(this);
            saveConfigurationCSV = new SaveConfigurationCSV();
        }


        public SystemConfiguration Config { get; set; }

        public void ConfigResponse(SystemConfiguration response)
        {
            Config = response;
        }

        public void SaveConfigDB()
        {
            saveConfigurationDb.SaveRequest(Config);
        }

        public SystemConfiguration LoadConfigDB()
        {
            loadConfigurationDb.LoadRequest();
            return Config;
        }

        public void SaveConfigJSON()
        {
            saveConfigurationJSON.SaveRequest(Config);
        }

        public SystemConfiguration LoadConfigJSON()
        {
            loadConfigurationJSON.LoadRequest();
            return Config;
        }

        public void SaveConfigCSV()
        {
            saveConfigurationCSV.SaveRequest(Config);
        }

        public SystemConfiguration LoadConfigCSV()
        {
            loadConfigurationCSV.LoadRequest();
            return Config;
        }
    }
}
