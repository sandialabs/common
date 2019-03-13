using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.utilities;
using GalaSoft.MvvmLight;

namespace COMMONConfig.Frontend.ViewModel
{
    public class SiteSettingsViewModel : ViewModelBase
    {
        public SiteSettingsViewModel()
        {
            InitializeLanguages();
        }

        private void InitializeLanguages()
        {
            var langUtil = new Languages();
            Languages = new ObservableCollection<LanguageConfiguration>();
            foreach (var language in langUtil.All)
            {
                var config = new LanguageConfiguration
                {
                    isEnabled = langUtil.IsEnabled(language),
                    language = language.ToString(),
                    languageCode = language.GetDescription()
                };
                Languages.Add(config);
            }
        }

        public string DbString()
        {
            var langUtil = new Languages();
            foreach (var l in Languages)
            {
                langUtil.Enable(l.language, l.isEnabled);
            }
            return langUtil.EnabledAsDelimitedString();
        }

        public void UpdateLanguages(List<LanguageConfiguration> langs)
        {
            Languages = new ObservableCollection<LanguageConfiguration>(langs.Union(Languages, new LanguageComparable()));
        }

        private class LanguageComparable : EqualityComparer<LanguageConfiguration>
        {
            public override bool Equals(LanguageConfiguration x, LanguageConfiguration y)
            {
                return x.Equals(y);
            }

            public override int GetHashCode(LanguageConfiguration obj)
            {
                return 0;
            }
        }

        #region Bindables

        private ObservableCollection<LanguageConfiguration> languages;

        public ObservableCollection<LanguageConfiguration> Languages
        {
            get { return languages; }
            set
            {
                languages = value;
                RaisePropertyChanged(nameof(Languages));
            }
        }

        private string siteName;

        public string SiteName
        {
            get { return siteName; }
            set
            {
                siteName = value;
                RaisePropertyChanged(nameof(SiteName));
            }
        }

        #endregion
    }
}