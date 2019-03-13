using System;
using COMMONConfig.Boundaries.Configuration;
using COMMONConfig.Utils;
using gov.sandia.sld.common.db.models;
using Microsoft.Win32;

namespace COMMONConfig.UseCase
{
    public class SaveConfigurationJSON : ISaveRequest
    {
        public void SaveRequest(SystemConfiguration configuration)
        {
            SaveModelToXml(configuration);
        }

        private static void SaveModelToXml(SystemConfiguration configuration)
        {
            var serializer = new SystemConfigurationJSONSerializer();
            var sitename = configuration.configuration[@"site.name"].value.ToLower().Replace(' ', '_');
            var date = DateTime.Now.ToString(@"yyyyMMdd_HHmmss");
            var fname = sitename + @"_config_" + date + @".json";
            var saveFileDialog = new SaveFileDialog
            {
                FileName = fname,
                DefaultExt = @".json",
                Filter = @"JSON files (*.json)|*.json|All files (*.*)|*.*"
            };

            bool? result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
                serializer.Serialize(configuration, saveFileDialog.FileName);
        }
    }
}