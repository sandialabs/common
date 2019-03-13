using System;
using System.IO;
using gov.sandia.sld.common.db.models;
using Newtonsoft.Json;

namespace COMMONConfig.Utils
{
    public class SystemConfigurationJSONSerializer : IConfigSerializer
    {
        public void Serialize(SystemConfiguration config, string path)
        {
            string output = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(path, output);
        }

        public SystemConfiguration Deserialize(string path)
        {
            SystemConfiguration config = null;
            try
            {
                string text = File.ReadAllText(path);
                config = JsonConvert.DeserializeObject<SystemConfiguration>(text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error Reading JSON file", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return config;
        }
    }
}