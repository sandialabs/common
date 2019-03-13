using COMMONConfig.Boundaries.Configuration;
using COMMONConfig.Utils;
using gov.sandia.sld.common.db.models;
using Microsoft.Win32;

namespace COMMONConfig.UseCase
{
    public class LoadConfigurationCSV : ILoadRequest
    {
        private readonly ILoadResponse loadBoundaryOutput;

        public LoadConfigurationCSV(ILoadResponse loadBoundaryOutput)
        {
            this.loadBoundaryOutput = loadBoundaryOutput;
        }

        public void LoadRequest()
        {
            loadBoundaryOutput.ConfigResponse(LoadModelFromCSV());
        }

        private static SystemConfiguration LoadModelFromCSV()
        {
            //var serializer = new SystemConfigurationJSONSerializer();
            var serializer = new SystemConfigurationCSVSerializer();
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            bool? result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value == true)
                return serializer.Deserialize(openFileDialog.FileName);
            return null;
        }
    }
}