using System;

namespace gov.sandia.sld.common.db.models
{
    public class LanguageConfiguration : IEquatable<LanguageConfiguration>
    {
        public string languageCode { get; set; }
        public string language { get; set; }
        public bool isEnabled { get; set; }
        public bool Equals(LanguageConfiguration other)
        {

            var b = language.Equals(other.language, StringComparison.InvariantCultureIgnoreCase)
                   || languageCode.Equals(other.languageCode, StringComparison.InvariantCultureIgnoreCase);
            return b;
        }
    }
}
