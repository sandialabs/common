using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using COMMONConfig.Boundaries.Messages;
using COMMONConfig.Frontend.Models;
using COMMONConfig.Frontend.ViewModel.Interfaces;
using COMMONConfig.Frontend.Views.UserContols;
using gov.sandia.sld.common.configuration;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db.models;

namespace COMMONConfig.Frontend.ViewModel.AbstractClasses
{
    /// <summary>
    /// This class contains the shared functions by the view models who display devices.
    /// </summary>
    public abstract class ADeviceControlViewModel : ViewModelBase, IDeviceCollection
    {
        // ICommands are triggered events (usually by button click, or something else)
        private ICommand addDevice;
        private ICommand removeDevice;
        
        // This is the collection of devices that this ViewModel contains
        private ICollectionView devicesView;

        // This is the currently selected DeviceModel
        private DeviceModel selectedDevice;


        // Anything that is bound to from the View needs to have this type of boiler plat.
        // We extend the setter by sending an event to paired classes with the name of the
        // data structure that was modified. These events are caught by the paired classes
        // and they update accordingly. Other than this boiler plate this mostly happens
        // automagically.
        public DeviceModel SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                RaisePropertyChanged(nameof(SelectedDevice));
            }
        }

        // We use something known as a RelayCommand, which is basically a closure. We also
        // make use of lazy initializing. At program startup this command is NULL. When
        // first queried it initializes itself. Future querries don't require this initialization.
        public ICommand RemoveDevice => removeDevice ?? (removeDevice = new RelayCommand(RemoveDeviceMessage));

        public ICommand AddDevice => addDevice ?? (addDevice = new RelayCommand(AddDeviceMessage));



        public ICollectionView DevicesView
        {
            get { return devicesView; }
            set
            {
                devicesView = value;
                RaisePropertyChanged(nameof(DevicesView));
            }
        }

        /// <summary>
        /// This guys defines what might be most important to people extending this class.
        /// Basically we are an overglorified list, but we additionally maintain our own
        /// filter. Unfortunately this isn't so easily expressed in WPF, which is why this
        /// all looks so terrible.
        /// </summary>
        /// <param name="collection">The collectino of devices that we filter (all devices from the backend).</param>
        public void SetDeviceView(ICollectionView collection)
        {
            DevicesView = collection;
            DevicesView.Filter = GetCollectionFilter();
        }

        /// <summary>
        /// DevicesModels are reified DeviceInfos, that is to say that devices first come into the application
        /// as pure data. It is our responsibility to inflate them into the correct DeviceModel.
        /// </summary>
        /// <returns>DeviceModel representation of a DeviceInfo</returns>
        public DeviceModel MakeDeviceModel()
        {
            EDeviceType type = GetNewDeviceType();
            var di = new DeviceInfo(type) { DID = new DeviceID(-1, GetDeviceName()) };
            AbstractUserControlContext ctx = new AbstractUserControlContext() { Info = di };
            var dm = new DeviceModel
            {
                Type = type,
                Control = MakeControl(type).Initialize(ctx),
                Info = di,
                Name = di.name
            };
            return dm;
        }

        /// <summary>
        /// The control for each device is different, so the implementing class must specify what the control contains.
        /// </summary>
        /// <returns></returns>
        public abstract AbstractUserControl MakeControl(EDeviceType type);
        /// <summary>
        /// The DeviceType for each device is different, so the implementing class must specify what the control contains.
        /// </summary>
        /// <returns></returns>
        protected abstract EDeviceType GetSelectedDeviceType();
        /// <summary>
        /// The type of device when creating a new one
        /// </summary>
        /// <returns></returns>
        protected abstract EDeviceType GetNewDeviceType();

        /// <summary>
        /// The DeviceName for each device is different, so the implementing class must specify what the control contains.
        /// The implementer is responsible for creating unique names.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetDeviceName();

        /// <summary>
        /// We leave it up to the inheriting class to define the collection filter.
        /// </summary>
        /// <returns></returns>
        protected abstract Predicate<object> GetCollectionFilter();

        protected virtual void AddDeviceMessage()
        {
            Messenger.Default.Send(new AddDeviceMessage {Device = MakeDeviceModel()});
        }


        protected void RemoveDeviceMessage()
        {
            if (selectedDevice == null) return;

            var metroWindow = (Application.Current.MainWindow as MetroWindow);

            if (selectedDevice.Type == EDeviceType.Server)
                metroWindow.ShowMessageAsync(@"Delete",
                    @"Servers cannot be deleted",
                    MessageDialogStyle.Affirmative);
            else
                metroWindow.ShowMessageAsync(@"Delete Device",
                    @"Are you sure you would like to delete " + selectedDevice.Name + " ?",
                    MessageDialogStyle.AffirmativeAndNegative).ContinueWith(t =>
                    {
                        if (t.Result == MessageDialogResult.Affirmative)
                            DispatcherHelper.UIDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => { Messenger.Default.Send(new RemoveDeviceMessage { Device = selectedDevice }); }));

                    });
        }
    }
}