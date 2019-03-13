using System;
using COMMONConfig.Frontend.Models;
using COMMONConfig.Frontend.ViewModel.AbstractClasses;
using COMMONConfig.Frontend.Views.UserContols;
using gov.sandia.sld.common.configuration;

namespace COMMONConfig.Frontend.ViewModel
{
    public class SystemDevicesViewModel : ADeviceControlViewModel
    {
        public override AbstractUserControl MakeControl(EDeviceType type)
        {
            // Don't worry about the type--always make a System control
            return new SystemUserControl();
        }

        protected override EDeviceType GetSelectedDeviceType()
        {
            return EDeviceType.System;
        }

        protected override EDeviceType GetNewDeviceType()
        {
            throw new Exception("Should not be creating new System devices");
        }

        /// <summary>
        /// This method counts the number of systems in the list, which we can append to
        /// the name of a device to get unique names.
        /// </summary>
        /// <returns></returns>
        private int CountDevices()
        {
            var i = 0;
            var e = DevicesView.SourceCollection.GetEnumerator();
            while (e.MoveNext())
            {
                var dm = e.Current as DeviceModel;
                if (dm != null && dm.Type == EDeviceType.System)
                    i++;
            }
            return i;
        }

        /// <summary>
        /// Overriding to make specific names for this device
        /// </summary>
        /// <returns></returns>
        protected override string GetDeviceName()
        {
            return @"SystemDevice-" + CountDevices();
        }

        protected override Predicate<object> GetCollectionFilter()
        {
            return SystemDeviceFilter;
        }

        /// <summary>
        /// Our predicate matches on device types of Systems.
        /// 
        /// It's probably a bad idea to make these guys public, these should either be
        /// moved to the IoC container, or just made private.
        /// </summary>
        public static Predicate<object> SystemDeviceFilter =
            m => ((DeviceModel)m).Type.Equals(EDeviceType.System);
    }
}