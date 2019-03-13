using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gov.sandia.sld.common.configuration
{
    /// <summary>
    /// Used to generate memory status values given the low and critically
    /// low thresholds. The thresholds are given in percentage, so will
    /// range from 0 to 100.
    /// 
    /// Values between 0 and Low will be EStatusType.AdequateDiskSpace
    /// Values >= Low and < CriticallyLow will be EStatusType.LowOnDiskSpace.
    /// Values >= CriticallyLow will be EStatusType.CriticallyLowOnDiskSpace
    /// </summary>
    public class DiskSpaceStatus
    {
        public static List<EStatusType> Types
        {
            get => 
                new List<EStatusType>(new EStatusType[] {
                    EStatusType.CriticallyLowOnDiskSpace,
                    EStatusType.LowOnDiskSpace,
                    EStatusType.AdequateDiskSpace });
        }

        public PercentThreshold2 Threshold { get; private set; }

        /// <summary>
        /// Specify the thresholds that will be used.
        /// </summary>
        /// <param name="low">The first transiton point between Adequate and Low</param>
        /// <param name="critically_low">The second transtion point between Low and CriticallyLow</param>
        public DiskSpaceStatus(double low, double critically_low)
        {
            Threshold = new PercentThreshold2(low, critically_low);
        }

        public Tuple<EStatusType, double> GetStatus(ulong used, ulong capacity)
        {
            EStatusType status = EStatusType.AdequateDiskSpace;

            used = Math.Min(used, capacity);
            ulong l_threshold = (ulong)((double)capacity * (Threshold.Transition / 100.0f));
            ulong cl_threshold = (ulong)((double)capacity * (Threshold.Transition2 / 100.0f));

            if (used >= cl_threshold)
                status = EStatusType.CriticallyLowOnDiskSpace;
            else if (used >= l_threshold)
                status = EStatusType.LowOnDiskSpace;

            double used_percent = 0;
            if(capacity > 0)
                used_percent = (double)used / (double)capacity * 100.0;

            return Tuple.Create(status, used_percent);
        }
    }
}
