using COMMONConfig.Localization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static gov.sandia.sld.common.db.ConfigStrings;

namespace COMMONConfig.Utils
{
    public static class DBLocalizationHelper
    {
        private static readonly Dictionary<string, string> c_translation_map = new Dictionary<string, string>
        {
            {countryCode, Resources.StringCountryCode},
            {siteName, Resources.StringSiteName},
            {cpuUsageAlert, Resources.StringCpuUsageAlert},
            {cpuUsageAlertCounts, Resources.StringCpuUsageAlertCount},
            {diskLowAlert, Resources.StringLowDiskAlert},
            {diskLowCriticalAlert, Resources.StringCriticalLowDiskAlert},
            {memoryLowAlert, Resources.StringLowMemAlert},
            {memoryLowCriticalAlert, Resources.StringCriticalLowMemAlert},
            {rebootRecentAlert, Resources.StringRecentRebootAlert},
            {rebootLongAlert, Resources.StringLongRebootAlert},
            {databaseConnectionString, " " + Resources.StringDBConnection},
            {databaseType, " " + Resources.StringDBType},
            {dailyfileLocation, Resources.StringDailyFileLocation},
            {deleteDays, Resources.StringDeleteDays},
            {unknown, Resources.StringUnknown},
            {alertsEmailSMTP, Resources.StringAlertEmailSMTP},
            {alertsEmailFrom, Resources.StringAlertEmailFrom},
            {alertsEmailTo, Resources.StringAlertEmailTo},
        };

        public static string LocalizeDBString(this string s)
        {
            return IsDbString(s) ? GetLocalized(s) : s;
        }

        private static string GetLocalized(string s)
        {
            foreach (string str in c_translation_map.Keys)
            {
                if (s.EndsWith(str))
                    return s.Replace(str, c_translation_map[str]);
            }

            Trace.WriteLine(string.Format("Didn't find {0} in c_translation_map", s));

            return s;
        }

        private static bool IsDbString(string s)
        {
            return DBStrings.Any(s.EndsWith);
        }
    }
}
