using System;
using COMMONConfig.Frontend.Models;
using COMMONConfig.Frontend.ViewModel.AbstractClasses;
using COMMONConfig.Frontend.Views.UserContols;
using gov.sandia.sld.common.configuration;

namespace COMMONConfig.Frontend.ViewModel
{
    public class GenericDevicesViewModel : ADeviceControlViewModel
    {

        public override AbstractUserControl MakeControl(EDeviceType type)
        {
            // Don't worry about type--always create a generic device control
            return new GenericDeviceUserControl();
        }

        protected override EDeviceType GetSelectedDeviceType()
        {
            return EDeviceType.Generic;
        }

        protected override EDeviceType GetNewDeviceType()
        {
            return EDeviceType.Generic;
        }

        private int CountDevices()
        {
            var i = 0;
            var e = DevicesView.SourceCollection.GetEnumerator();
            while (e.MoveNext())
            {
                var dm = e.Current as DeviceModel;
                if (dm != null &&
                    (dm.Type == EDeviceType.Camera || dm.Type == EDeviceType.System || dm.Type == EDeviceType.Generic))
                    i++;
            }
            return i;
        }

        protected override string GetDeviceName()
        {
            return @"NetworkDevice-" + CountDevices();
        }

        protected override Predicate<object> GetCollectionFilter()
        {
            return NonWmiDeviceFilter;
        }

        public static readonly Predicate<object> NonWmiDeviceFilter =
            m =>
                !WmiDevicesViewModel.WmiDeviceFilter.Invoke(m) &&
                //!LaneDevicesViewModel.LaneDeviceFilter.Invoke(m) &&
                !SystemDevicesViewModel.SystemDeviceFilter.Invoke(m);
    }
}