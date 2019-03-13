using System;
using COMMONConfig.Boundaries.Configuration;
using COMMONConfig.Utils;
using gov.sandia.sld.common.db.models;
using Microsoft.Win32;

namespace COMMONConfig.UseCase
{
    public class SaveConfigurationCSV : ISaveRequest
    {
        public void SaveRequest(SystemConfiguration configuration)
        {
            SaveModelToCSV(configuration);
        }

        private static void SaveModelToCSV(SystemConfiguration configuration)
        {
            //var serializer = new SystemConfigurationJSONSerializer();
            var serializer = new SystemConfigurationCSVSerializer();
            var sitename = configuration.configuration[@"site.name"].value.ToLower().Replace(' ', '_');
            var date = DateTime.Now.ToString(@"yyyyMMdd_HHmmss");
            var fname = sitename + @"_config_" + date + @".csv";
            var saveFileDialog = new SaveFileDialog
            {
                FileName = fname,
                DefaultExt = @".csv",
                Filter = @"CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            bool? result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
                serializer.Serialize(configuration, saveFileDialog.FileName);
        }
    }
}