using COMMONConfig.Frontend.ViewModel;
using gov.sandia.sld.common.logging;

namespace COMMONConfig.Frontend.Views
{
    /// <summary>
    ///     Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            EventLog.GlobalSource = "COMMONConfig";

            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}