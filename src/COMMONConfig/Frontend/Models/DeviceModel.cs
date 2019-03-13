using COMMONConfig.Frontend.Views.UserContols;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.models;

namespace COMMONConfig.Frontend.Models
{
    public class DeviceModel
    {
        public DeviceModel()
        {
        }

        public DeviceModel(DeviceInfo info)
        {
            Info = info;
            Type = info.type;
            Name = info.name;
        }

        public EDeviceType Type { get; set; }
        public AbstractUserControl Control { get; set; }
        public string Name { get; set; }
        public DeviceInfo Info { get; set; }
    }
}