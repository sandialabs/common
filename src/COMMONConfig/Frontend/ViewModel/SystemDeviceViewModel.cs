using COMMONConfig.Frontend.Models;
using COMMONConfig.Frontend.ViewModel.AbstractClasses;
using COMMONConfig.Frontend.Views.UserContols;
using gov.sandia.sld.common.configuration;

namespace COMMONConfig.Frontend.ViewModel
{
    public class SystemDeviceViewModel : ADeviceControlViewModel
    {
        public override AbstractUserControl MakeControl()
        {
            return new GenericDeviceUserControl();
        }

        protected override DeviceType GetDeviceType()
        {
            return DeviceType.System;
        }

        private int CountDevices()
        {
            return 1;
        }

        protected override string GetDeviceName()
        {
            return "System";
        }
    }
}