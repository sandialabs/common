using System;
using COMMONConfig.Boundaries.Messages;
using COMMONConfig.Frontend.Models;
using COMMONConfig.Frontend.ViewModel.AbstractClasses;
using COMMONConfig.Frontend.Views.UserContols;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db;
using GalaSoft.MvvmLight.Messaging;

namespace COMMONConfig.Frontend.ViewModel
{
    //public class LaneDevicesViewModel : ADeviceControlViewModel
    //{
    //    public override AbstractUserControl MakeControl()
    //    {
    //        return new GenericDeviceUserControl();
    //    }

    //    protected override DeviceType GetDeviceType()
    //    {
    //        return DeviceType.RPM;
    //    }

    //    private int CountNetworkDevices()
    //    {
    //        var i = 0;
    //        var e = DevicesView.SourceCollection.GetEnumerator();
    //        while (e.MoveNext())
    //        {
    //            var dm = e.Current as DeviceModel;
    //            if (dm != null &&
    //                (dm.Type == DeviceType.Camera || dm.Type == DeviceType.System || dm.Type == DeviceType.Generic))
    //                i++;
    //        }
    //        return i;
    //    }

    //    private int CountDevices()
    //    {
    //        var i = 0;
    //        var e = DevicesView.SourceCollection.GetEnumerator();
    //        while (e.MoveNext())
    //        {
    //            var dm = e.Current as DeviceModel;
    //            if (dm != null && (dm.Type == DeviceType.RPM)) i++;
    //        }
    //        return i;
    //    }

    //    protected override string GetDeviceName()
    //    {
    //        return @"Lane" + CountDevices() + @" RPM";
    //    }

    //    protected override Predicate<object> GetCollectionFilter()
    //    {
    //        return LaneDeviceFilter;
    //    }

    //    protected override void AddDeviceMessage()
    //    {
    //        var lane = MakeDeviceModel();
    //        string name = lane.Name.Replace(@" RPM", "");
    //        Messenger.Default.Send(new AddDeviceMessage {Device = lane});

    //        Messenger.Default.Send(new AddDeviceMessage {Device = AttachCamera(lane, name, 1)});
    //        Messenger.Default.Send(new AddDeviceMessage {Device = AttachCamera(lane, name, 2)});
    //    }


    //    private DeviceModel AttachCamera(DeviceModel model, string name, int count)
    //    {
    //        var di = new DeviceInfo(DeviceType.Camera)
    //        {
    //            name = name + @" NetworkDevice-" + count
    //        };
    //        var dm = new DeviceModel
    //        {
    //            Type = di.type,
    //            Control = MakeControl().Initialize(di),
    //            Info = di,
    //            Name = di.name
    //        };
    //        return dm;
    //    }

    //    public static readonly Predicate<object> LaneDeviceFilter =
    //        m => ((DeviceModel) m).Name.ToLower().Contains(@"lane");
    //}
}