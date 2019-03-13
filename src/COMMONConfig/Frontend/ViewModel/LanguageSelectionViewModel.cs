using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using COMMONConfig.Utils;
using GalaSoft.MvvmLight;
using WPFLocalizeExtension.Engine;

namespace COMMONConfig.Frontend.ViewModel
{
    public class LanguageSelectionViewModel : ViewModelBase
    {
        private ICommand confirmNewCulture;
        private ObservableCollection<CultureInfo> cultures;
        private CultureInfo selectedCultureInfo;

        public LanguageSelectionViewModel()
        {
            Cultures = LocalizeDictionary.Instance.MergedAvailableCultures;
            Cultures.RemoveAt(0);
        }

        public ICommand ConfirmNewCulture
        {
            get
            {
                return confirmNewCulture ?? (confirmNewCulture = new RelayCommand(p =>
                {
                    LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
                    LocalizeDictionary.Instance.Culture = SelectedCultureInfo;
                }, null));
            }
        }

        public CultureInfo SelectedCultureInfo
        {
            get { return selectedCultureInfo; }
            set
            {
                selectedCultureInfo = value;
                RaisePropertyChanged(nameof(SelectedCultureInfo));
                ConfirmNewCulture.Execute(null);
            }
        }

        public ObservableCollection<CultureInfo> Cultures
        {
            get { return cultures; }
            set
            {
                cultures = value;
                RaisePropertyChanged(nameof(Cultures));
            }
        }
    }
}