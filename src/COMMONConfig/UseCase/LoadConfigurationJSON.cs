using COMMONConfig.Boundaries.Configuration;
using COMMONConfig.Utils;
using gov.sandia.sld.common.db.models;
using Microsoft.Win32;

namespace COMMONConfig.UseCase
{
    public class LoadConfigurationJSON : ILoadRequest
    {
        private readonly ILoadResponse loadBoundaryOutput;

        public LoadConfigurationJSON(ILoadResponse loadBoundaryOutput)
        {
            this.loadBoundaryOutput = loadBoundaryOutput;
        }

        public void LoadRequest()
        {
            loadBoundaryOutput.ConfigResponse(LoadModelFromJSON());
        }

        private static SystemConfiguration LoadModelFromJSON()
        {
            var serializer = new SystemConfigurationJSONSerializer();
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"JSON files (*.json)|*.json|All files (*.*)|*.*"
            };

            bool? result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value == true)
                return serializer.Deserialize(openFileDialog.FileName);
            return null;
        }
    }
}