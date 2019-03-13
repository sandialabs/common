using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using COMMONConfig.Boundaries.Messages;
using COMMONConfig.Frontend.Models;
using COMMONConfig.Frontend.ViewModel.AbstractClasses;
using COMMONConfig.Presenter;
using COMMONConfig.Utils;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Practices.ServiceLocation;
using COMMONConfig.Frontend.Views.UserContols;

namespace COMMONConfig.Frontend.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ConfigPresenter configPresenter;
        private ConfigurationViewModel configuration;
        private ADeviceControlViewModel genericADeviceControl;
        //private ADeviceControlViewModel laneADeviceControl;
        private ADeviceControlViewModel systemADeviceControl;
        private SystemConfiguration model;
        private ICommand saveConfigurationCommand;
        private ICommand loadConfigurationFromJSONFile;
        private ICommand saveConfigurationToJSONFile;
        private ICommand loadConfigurationFromCSVFile;
        private ICommand saveConfigurationToCSVFile;
        private SiteSettingsViewModel siteSettings;
        private GroupsViewModel groups;

        private ADeviceControlViewModel workstationADeviceControl;

        public MainViewModel()
        {
            configPresenter = new ConfigPresenter();
            Initialize();
        }

        public ObservableCollection<DeviceModel> Devices { get; set; }
        public Collection<DeviceModel> DevicesToBeRemoved { get; set; }

        public ICommand SaveConfigurationCommand
        {
            get
            {
                return saveConfigurationCommand ?? (saveConfigurationCommand = new RelayCommand(p =>
                {
                    CommitGui();
                    configPresenter.Config = model;
                    configPresenter.SaveConfigDB();
                    model = configPresenter.LoadConfigDB();
                    UpdateGui();
                }, null));
            }
        }

        public ICommand LoadConfigurationFromJSONFile
        {
            get
            {
                return loadConfigurationFromJSONFile ?? (loadConfigurationFromJSONFile = new RelayCommand(p =>
                {
                    configPresenter.LoadConfigJSON();
                    if (configPresenter.Config == null)
                        return;

                    Trace.WriteLine("Incoming:");
                    Trace.WriteLine(JsonConvert.SerializeObject(configPresenter.Config));

                    model.Merge(configPresenter.Config);

                    Trace.WriteLine("Merged:");
                    Trace.WriteLine(JsonConvert.SerializeObject(model));

                    UpdateGui();
                }, null));
            }
        }

        public ICommand SaveConfigurationToJSONFile
        {
            get
            {
                return saveConfigurationToJSONFile ?? (saveConfigurationToJSONFile = new RelayCommand(p =>
                {
                    CommitGui();
                    configPresenter.Config = model;
                    configPresenter.SaveConfigJSON();
                }, null));
            }
        }

        public ICommand LoadConfigurationFromCSVFile
        {
            get
            {
                return loadConfigurationFromCSVFile ?? (loadConfigurationFromCSVFile = new RelayCommand(p =>
                {
                    configPresenter.LoadConfigCSV();
                    if (configPresenter.Config == null)
                        return;

                    // Remove all of the devices, except the System device, then add in all that were read in
                    List<DeviceInfo> doomed = new List<DeviceInfo>();
                    foreach(DeviceInfo di in model.devices)
                    {
                        if (di.type != EDeviceType.System && di.type != EDeviceType.Server)
                            doomed.Add(di);
                    }
                    doomed.ForEach(di => model.devices.Remove(di));
                    configPresenter.Config.devices.ForEach(di => model.devices.Add(di));

                    UpdateGui();
                }, null));
            }
        }

        public ICommand SaveConfigurationToCSVFile
        {
            get
            {
                return saveConfigurationToCSVFile ?? (saveConfigurationToCSVFile = new RelayCommand(p =>
                {
                    CommitGui();
                    configPresenter.Config = model;
                    configPresenter.SaveConfigCSV();
                }, null));
            }
        }

        public void Initialize()
        {
            Messenger.Default.Register<AddDeviceMessage>(this, AddDeviceAction);
            Messenger.Default.Register<RemoveDeviceMessage>(this, RemoveDeviceAction);
            Messenger.Default.Register<AddGroupMessage>(this, AddGroupAction);

            Devices = new ObservableCollection<DeviceModel>();
            DevicesToBeRemoved = new Collection<DeviceModel>();

            workstationADeviceControl = ServiceLocator.Current.GetInstance<WmiDevicesViewModel>();
            genericADeviceControl = ServiceLocator.Current.GetInstance<GenericDevicesViewModel>();
            //laneADeviceControl = ServiceLocator.Current.GetInstance<LaneDevicesViewModel>();
            siteSettings = ServiceLocator.Current.GetInstance<SiteSettingsViewModel>();
            configuration = ServiceLocator.Current.GetInstance<ConfigurationViewModel>();
            systemADeviceControl = ServiceLocator.Current.GetInstance<SystemDevicesViewModel>();
            groups = ServiceLocator.Current.GetInstance<GroupsViewModel>();

            model = configPresenter.LoadConfigDB();
            UpdateGui();
        }

        private void AddDeviceAction(AddDeviceMessage action)
        {
            Devices.Add(action.Device);
        }

        private void RemoveDeviceAction(RemoveDeviceMessage action)
        {
            // Default value is 0, if this device is in the database then it will have been changed
            if (action.Device.Info.id >= 0)
            {
                action.Device.Info.deleted = true;
                DevicesToBeRemoved.Add(action.Device);
            }
            if (action.Device != null) Devices.Remove(action.Device);
        }

        private void SetupDeviceFilters()
        {
            workstationADeviceControl.SetDeviceView(new CollectionViewSource {Source = Devices}.View);
            genericADeviceControl.SetDeviceView(new CollectionViewSource {Source = Devices}.View);
            systemADeviceControl.SetDeviceView(new CollectionViewSource {Source = Devices}.View);
            //laneADeviceControl.SetDeviceView(new CollectionViewSource {Source = Devices}.View);
        }

        private void AddGroupAction(AddGroupMessage action)
        {
            int new_group_num = model.groups.Count + 1;
            model.groups.Add(new Group() { id = new_group_num, name = string.Format("Group {0}", new_group_num) });
            UpdateGroups();
        }

        public void UpdateGroups()
        {
            List<Group> g = new List<Group>();
            g.Add(new Group() { id = -1, name = string.Empty });
            g.AddRange(model.groups);
            groups.UpdateGroups(g);
        }

        private void UpdateGui()
        {
            model.groups.Sort((a, b) => string.Compare(a.name, b.name, true));
            UpdateGroups();

            AbstractUserControlContext ctx = new AbstractUserControlContext() { Groups = groups.Groups };

            Devices = new ObservableCollection<DeviceModel>(model.devices.Select(t =>
            {
                var dm = new DeviceModel(t);
                ctx.Info = t;

                switch (t.type)
                {
                    case EDeviceType.Server:
                    case EDeviceType.Workstation:
                        dm.Control = workstationADeviceControl.MakeControl(t.type).Initialize(ctx);
                        return dm;
                    case EDeviceType.System:
                        dm.Control = systemADeviceControl.MakeControl(t.type).Initialize(ctx);
                        return dm;
                    case EDeviceType.Unknown:
                        throw new ArgumentOutOfRangeException();

                        // Let's use generic as the default type, so we can add new ones in
                        // the future without causing a problem here.
                    default:
                        dm.Control = genericADeviceControl.MakeControl(t.type).Initialize(ctx);
                        return dm;
                }
            }));
            configuration.UpdateConfigs(model.configuration.Values.ToList());
            siteSettings.UpdateLanguages(model.languages);

            ConfigurationData site_name = null;
            if(model.configuration.TryGetValue(@"site.name", out site_name))
                siteSettings.SiteName = site_name.value;

            SetupDeviceFilters();
        }

        private void CommitGui()
        {
            Parallel.Invoke(UpdateLanguages, UpdateConfigurations);
            UpdateDevices();
            SaveSiteName(siteSettings.SiteName);
            var lc = new ConfigurationData
            {
                path = @"languages",
                value = siteSettings.DbString()
            };
            if (model.configuration.ContainsKey(@"languages"))
            {
                model.configuration[@"languages"] = lc;
            }
            else
            {
                model.configuration.Add(@"languages", lc);
            }
        }

        private void UpdateLanguages()
        {
            model.languages = new List<LanguageConfiguration>(siteSettings.Languages);
        }

        private void UpdateConfigurations()
        {
            model.configuration =
                new Dictionary<string, ConfigurationData>(
                    configuration.Configs.Select(t => t.ToBase()).ToDictionary(t => t.path));
        }

        private void UpdateDevices()
        {
            foreach (var dm in DevicesToBeRemoved)
            {
                Devices.Add(dm);
            }

            foreach (var dm in Devices)
            {
                dm.Control.DoDataBinding();
                var di = model.devices.Find(d => d.Equals(dm.Info));
                if (di != null)
                {
                    di.collectors = dm.Info.collectors;
                    di.ipAddress = dm.Info.ipAddress;
                }
                else
                {
                    model.devices.Add(dm.Info);
                }
            }

            DevicesToBeRemoved.Clear();
        }

        private void SaveSiteName(string siteName)
        {
            var siteNameConfig = new ConfigurationData
            {
                path = @"site.name",
                value = siteName
            };


            if (model.configuration.ContainsKey(@"site.name"))
            {
                model.configuration[@"site.name"] = siteNameConfig;
            }
            else
            {
                model.configuration.Add(@"site.name", siteNameConfig);
            }
        }

    }
}