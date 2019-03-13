/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:COMMONConfig"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace COMMONConfig.Frontend.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic) return;

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SiteSettingsViewModel>();
            SimpleIoc.Default.Register<ConfigurationViewModel>();
            SimpleIoc.Default.Register<LanguageSelectionViewModel>();
            SimpleIoc.Default.Register<WmiDevicesViewModel>();
            //SimpleIoc.Default.Register<LaneDevicesViewModel>();
            SimpleIoc.Default.Register<GenericDevicesViewModel>();
            SimpleIoc.Default.Register<SystemDevicesViewModel>();
            SimpleIoc.Default.Register<GroupsViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public SiteSettingsViewModel SiteSettings => ServiceLocator.Current.GetInstance<SiteSettingsViewModel>();
        public ConfigurationViewModel Configuration => ServiceLocator.Current.GetInstance<ConfigurationViewModel>();
        public LanguageSelectionViewModel LanguageSelection
            => ServiceLocator.Current.GetInstance<LanguageSelectionViewModel>();
        public WmiDevicesViewModel WmiDevices => ServiceLocator.Current.GetInstance<WmiDevicesViewModel>();
        //public LaneDevicesViewModel LaneDevices => ServiceLocator.Current.GetInstance<LaneDevicesViewModel>();
        public GenericDevicesViewModel GenericDevices => ServiceLocator.Current.GetInstance<GenericDevicesViewModel>();
        public SystemDevicesViewModel SystemDevices => ServiceLocator.Current.GetInstance<SystemDevicesViewModel>();

        public GroupsViewModel Groups => ServiceLocator.Current.GetInstance<GroupsViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}