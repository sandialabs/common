using System;
using COMMONConfig.Frontend.Models;
using COMMONConfig.Frontend.ViewModel.AbstractClasses;
using COMMONConfig.Frontend.Views.UserContols;
using gov.sandia.sld.common.configuration;

namespace COMMONConfig.Frontend.ViewModel
{
    public class WmiDevicesViewModel : ADeviceControlViewModel
    {
        public override AbstractUserControl MakeControl(EDeviceType type)
        {
            if (type == EDeviceType.Server)
                return new ServerUserControl();
            else
                return new MachineUserControl();
        }

        protected override EDeviceType GetSelectedDeviceType()
        {
            if (SelectedDevice != null)
                return SelectedDevice.Type;
            else
                throw new Exception("Unable to determine selected device type");
        }

        protected override EDeviceType GetNewDeviceType()
        {
            return EDeviceType.Workstation;
        }

        private int CountDevices()
        {
            var i = 0;
            var e = DevicesView.SourceCollection.GetEnumerator();
            while (e.MoveNext())
            {
                var dm = e.Current as DeviceModel;
                if (dm != null && (dm.Type == EDeviceType.Server || dm.Type == EDeviceType.Workstation)) i++;
            }
            return i;
        }

        protected override string GetDeviceName()
        {
            return @"WindowsDevice-" + CountDevices();
        }

        protected override Predicate<object> GetCollectionFilter()
        {
            return WmiDeviceFilter;
        }

        public static readonly Predicate<object> WmiDeviceFilter = m =>
        {
            var t = ((DeviceModel) m).Type;
            //return ((t == DeviceType.Server) || (t == DeviceType.Workstation)) && !LaneDevicesViewModel.LaneDeviceFilter.Invoke(m);
            return t == EDeviceType.Server || t == EDeviceType.Workstation;
        };
    }
}