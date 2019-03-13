using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// The languages we support. To add a new language, just do two things.
    /// 1) Add the new language here along with its two-digit language code.
    /// 2) Add the language translation file in CommonWeb under languages.
    ///     The language filename needs to match the two-digit language code
    ///     specified here.
    ///     
    /// For example, if we added Italian, add an 'Italian' enum
    /// value with the Description of "IT".
    /// We would also then add an it.json file with the appropriate translations
    /// in the languages folder in CommonWeb.
    /// </summary>
    public enum Language
    {
        [Description("EN")]
        English,
        [Description("ES")]
        Spanish,
        [Description("MN")]
        Mongolian,
        [Description("FR")]
        French,
        [Description("RO")]
        Romanian,
    }

    /// <summary>
    /// Used to manage the languages, and which languages are enabled/disabled.
    /// </summary>
    public class Languages
    {
        public List<Language> All { get { return EnumExtensions.GetValues<Language>().ToList(); } }

        public Languages()
        {
            m_is_enabled = new Dictionary<Language, bool>();

            All.ForEach(l => m_is_enabled[l] = l == Language.English);
        }

        public bool IsEnabled(Language lang)
        {
            return m_is_enabled[lang];
        }

        /// <summary>
        /// See if the language is enabled/disabled from
        /// the two-digit language code
        /// </summary>
        /// <param name="lang">The two-digit language code</param>
        /// <returns>true if that language is enabled; false if disabled</returns>
        public bool IsEnabled(string lang)
        {
            return IsEnabled(FromString(lang));
        }

        /// <summary>
        /// Disable all languages except for English--it can never be disabled.
        /// </summary>
        public void DisableAll()
        {
            All.ForEach(l => m_is_enabled[l] = l == Language.English);
        }

        /// <summary>
        /// Enable/disable the specified language. English can never be disabled.
        /// </summary>
        /// <param name="lang">The language to enable/disable</param>
        /// <param name="enable">true to enable; false to disable</param>
        public void Enable(Language lang, bool enable)
        {
            // Always keep English enabled
            if (lang == Language.English)
                return;

            m_is_enabled[lang] = enable;
        }

        /// <summary>
        /// Enable/disable the language specified by its two-digit code.
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="enable"></param>
        public void Enable(string lang, bool enable)
        {
            Enable(FromString(lang), enable);
        }

        /// <summary>
        /// Enables the languages specified in the string. It is
        /// comma-delimited with the two-digit codes for the languages
        /// to enable.
        /// </summary>
        /// <param name="langs">The comma-delimited string with country codes</param>
        public void EnableFromDelimitedString(string langs)
        {
            List<Language> enabled = FromDelimitedString(langs);
            DisableAll();
            enabled.ForEach(l => Enable(l, true));
        }

        /// <summary>
        /// Gets a list of languages specified in the comma-delimited string
        /// containing language codes.
        /// </summary>
        /// <param name="langs"></param>
        /// <returns></returns>
        public List<Language> FromDelimitedString(string langs)
        {
            List<Language> languages = new List<Language>();

            // English should always in there, so always add it
            languages.Add(Language.English);

            string[] l = langs.Split(',');
            foreach(string l2 in l)
            {
                Language l3 = FromString(l2);
                if (languages.Contains(l3) == false)
                    languages.Add(l3);
            }
            return languages;
        }

        /// <summary>
        /// Gets a comma-delimited string with all the
        /// country codes that have been enabled.
        /// </summary>
        /// <returns></returns>
        public string EnabledAsDelimitedString()
        {
            string str = string.Empty;

            foreach (Language l in All)
            {
                if (IsEnabled(l))
                {
                    if (str != string.Empty)
                        str += ",";
                    str += l.GetDescription();
                }
            }

            return str;
        }

        /// <summary>
        /// Determines the Language enum from a language code
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private Language FromString(string lang)
        {
            lang = lang.Trim();

            foreach (Language l in All)
            {
                if (string.Compare(lang, l.GetDescription(), true) == 0)
                    return l;
                if (string.Compare(lang, l.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                    return l;
            }

            return Language.English;
        }

        private Dictionary<Language, bool> m_is_enabled;
    }
}
