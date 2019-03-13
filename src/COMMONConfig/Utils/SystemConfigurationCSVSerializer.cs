using gov.sandia.sld.common.db.models;

namespace COMMONConfig.Utils
{
    public class SystemConfigurationCSVSerializer : IConfigSerializer
    {
        public void Serialize(SystemConfiguration config, string path)
        {
            DevicesCSV csv = new DevicesCSV(config.devices, path);
            csv.Save();
        }

        public SystemConfiguration Deserialize(string path)
        {
            SystemConfiguration config = new SystemConfiguration();
            DevicesCSV csv = new DevicesCSV(path);
            csv.Load();

            if(csv.Errors.Count > 0)
            {
                string error = string.Empty;
                csv.Errors.ForEach(e => error += e + "\n");
                System.Windows.MessageBox.Show(error, "Error Reading CSV file", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return null;
            }

            csv.Entries.ForEach(d => config.devices.Add(d));

            return config;
        }
    }
}