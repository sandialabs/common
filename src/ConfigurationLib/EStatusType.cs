using System.Collections.Generic;
using System.ComponentModel;
using gov.sandia.sld.common.utilities;

namespace gov.sandia.sld.common.configuration
{
    public enum EStatusType : int
    {
        [Description("Is offline")]
        [AlertLevel(EAlertLevel.Alarm)]
        Offline,
        [Description("Is online")]
        [AlertLevel(EAlertLevel.Normal)]
        Online,
        
        [Description("Is low on free disk space")]
        [AlertLevel(EAlertLevel.Alarm)]
        LowOnDiskSpace,
        [Description("Is critically low on free disk space")]
        [AlertLevel(EAlertLevel.Alarm)]
        CriticallyLowOnDiskSpace,
        [Description("Has adequate free disk space")]
        [AlertLevel(EAlertLevel.Normal)]
        AdequateDiskSpace,

        [Description("Is low on free memory")]
        [AlertLevel(EAlertLevel.Alarm)]
        LowFreeMemory,
        [Description("Is critically low on free memory")]
        [AlertLevel(EAlertLevel.Alarm)]
        CriticallyLowFreeMemory,
        [Description("Has adequate free memory")]
        [AlertLevel(EAlertLevel.Normal)]
        AdequateFreeMemory,

        [Description("Has been recently rebooted")]
        [AlertLevel(EAlertLevel.Information)]
        RecentlyRebooted,
        [Description("Has not been rebooted recently")]
        [AlertLevel(EAlertLevel.Information)]
        ExcessiveUptime,

        [Description("Has excessive CPU usage")]
        [AlertLevel(EAlertLevel.Alarm)]
        ExcessiveCPU,
        [Description("Has normal CPU usage")]
        [AlertLevel(EAlertLevel.Normal)]
        NormalCPU,

        [Description("Good Ping response (less than 100ms)")]
        [AlertLevel(EAlertLevel.Normal)]
        GoodPing,
        [Description("Slow Ping response (greater than 100ms)")]
        [AlertLevel(EAlertLevel.Information)]
        SlowPing,

        [Description("UPS is providing power")]
        [AlertLevel(EAlertLevel.Information)]
        UPSProvidingPower,
        [Description("UPS status is normal")]
        [AlertLevel(EAlertLevel.Normal)]
        UPSNotProvidingPower,

        [Description("SMART is predicting disk failure")]
        [AlertLevel(EAlertLevel.Alarm)]
        DiskFailurePredicted,
        [Description("SMART indicates disk is healthy")]
        [AlertLevel(EAlertLevel.Normal)]
        DiskFailureNotPredicted,
    }

    public static class EStatusTypeExtensions
    {
        public static string Join(this List<EStatusType> types)
        {
            return types.ConvertAll<int>(t => (int)t).JoinStrings(",");
        }

        /// <summary>
        /// Make sure we only drive the status "up". Critically low should be >
        /// low, and low should be > adequate
        /// </summary>
        /// <param name="a">The first status to compare</param>
        /// <param name="b">The second status to compare</param>
        /// <returns>CriticallyLowOnDiskSpace, LowOnDiskSpace, or AdequateDiskSpace depending upon which is greater: a or b</returns>
        public static EStatusType DiskSpaceCompare(this EStatusType a, EStatusType b)
        {
            if (a == EStatusType.CriticallyLowOnDiskSpace || b == EStatusType.CriticallyLowOnDiskSpace)
                return EStatusType.CriticallyLowOnDiskSpace;

            if (a == EStatusType.LowOnDiskSpace || b == EStatusType.LowOnDiskSpace)
                return EStatusType.LowOnDiskSpace;

            return EStatusType.AdequateDiskSpace;
        }
    }
}
