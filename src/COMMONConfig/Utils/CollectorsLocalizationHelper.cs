using System;
using System.Globalization;
using System.Windows.Data;
using COMMONConfig.Localization;
using gov.sandia.sld.common.configuration;

namespace COMMONConfig.Utils
{
    public static class CollectorsLocalizationHelper
    {
        public static string CollectorName(this ECollectorType c)
        {
            switch (c)
            {
                case ECollectorType.Memory:
                    return Resources.StringMemory;
                case ECollectorType.Disk:
                    return Resources.StringDisk;
                case ECollectorType.CPUUsage:
                    return Resources.StringCpuUsage;
                case ECollectorType.NICUsage:
                    return Resources.StringNicUsage;
                case ECollectorType.Uptime:
                    return Resources.StringUptime;
                case ECollectorType.LastBootTime:
                    return Resources.StringLastBootTime;
                case ECollectorType.Processes:
                    return Resources.StringProcesses;
                case ECollectorType.Ping:
                    return Resources.StringPing;
                case ECollectorType.InstalledApplications:
                    return Resources.StringInstalledApplications;
                case ECollectorType.Services:
                    return Resources.StringServices;
                case ECollectorType.SystemErrors:
                    return Resources.StringSystemErrors;
                case ECollectorType.ApplicationErrors:
                    return Resources.StringApplicationErrors;
                case ECollectorType.DatabaseSize:
                    return Resources.StringDatabaseSize;
                case ECollectorType.UPS:
                    return Resources.StringUPS;
                case ECollectorType.DiskSpeed:
                    return Resources.StringDiskSpeed;
                case ECollectorType.Unknown:
                    return Resources.StringUnknown;
                default:
                    // If we can't get a translated version, use the best thing we have instead of choking
                    return c.ToString();
                    //throw new ArgumentOutOfRangeException(nameof(c), c, null);
            }
        }
    }

    public sealed class CollectorsNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ECollectorType)
            {
                return ((ECollectorType) value).CollectorName();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}