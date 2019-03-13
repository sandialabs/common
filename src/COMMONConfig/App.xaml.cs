using System.Globalization;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using WPFLocalizeExtension.Engine;

namespace COMMONConfig
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const string AppName = "COMMONConfig";
        private Mutex _instanceMutex = null;

        static App()
        {
            DispatcherHelper.Initialize();
        }
        public App()
        {
            bool createdNew;

            _instanceMutex = new Mutex(false, AppName, out createdNew);

            if (createdNew)
            {
                LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
                LocalizeDictionary.Instance.Culture = new CultureInfo("en");
            }
            else
            {
                DeactivateApp();
            }
        }

        private static void DeactivateApp()
        {
            MessageBox.Show("COMMON Config is already running...");
            Application.Current.Shutdown();
        }
    }
}
