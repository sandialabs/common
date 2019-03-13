using COMMONConfig.Utils;
using gov.sandia.sld.common.db.models;

namespace COMMONConfig.Frontend.Models
{
    public class ConfigModel : ConfigurationData
    {
        public ConfigModel()
        {
        }

        public ConfigModel(ConfigurationData data)
        {
            configID = data.configID;
            path = data.path;
            value = data.value;
            deleted = data.deleted;
            DisplayName = path.LocalizeDBString();
        }

        public string DisplayName { get; set; }

        public ConfigurationData ToBase()
        {
            return new ConfigurationData
            {
                configID = configID,
                path = path,
                value = value,
                deleted = deleted
            };
        }
    }
}