using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using COMMONConfig.Frontend.Models;
using gov.sandia.sld.common.db.models;
using GalaSoft.MvvmLight;

namespace COMMONConfig.Frontend.ViewModel
{
    public class ConfigurationViewModel : ViewModelBase
    {
        private ICollectionView alerts;
        private ObservableCollection<ConfigModel> configs;
        private ICollectionView settings;

        public ConfigurationViewModel()
        {
            Configs = new ObservableCollection<ConfigModel>();
        }

        public ObservableCollection<ConfigModel> Configs
        {
            get { return configs; }
            set
            {
                configs = value;
                RaisePropertyChanged(nameof(Configs));

                SetupViews();
            }
        }

        public ICollectionView Settings
        {
            get { return settings; }
            set
            {
                settings = value;
                RaisePropertyChanged(nameof(Settings));
            }
        }

        public ICollectionView Alerts
        {
            get { return alerts; }
            set
            {
                alerts = value;
                RaisePropertyChanged(nameof(Alerts));
            }
        }

        private void SetupViews()
        {
            Settings = new CollectionViewSource {Source = Configs}.View;
            Settings.Filter = SettingsFilter;
            Alerts = new CollectionViewSource {Source = Configs}.View;
            Alerts.Filter = AlertFilter;
        }

        private static bool SettingsFilter(object item)
        {
            var config = item as ConfigModel;
            return config != null && !AlertFilter(item) && config.path != @"site.name";
        }

        private static bool AlertFilter(object item)
        {
            var config = item as ConfigModel;
            return config != null && config.path.Contains(@"alert");
        }

        public void UpdateConfigs(List<ConfigurationData> configDatas)
        {
            Configs = new ObservableCollection<ConfigModel>(configDatas.Select(t => new ConfigModel(t)));
        }
    }
}