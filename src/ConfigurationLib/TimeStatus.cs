using gov.sandia.sld.common.utilities;
using System.Collections.Generic;

namespace gov.sandia.sld.common.configuration
{
    public class TimeStatus
    {
        public static List<EStatusType> Types
        {
            get =>
                new List<EStatusType>(new EStatusType[]
                {
                    EStatusType.RecentlyRebooted,
                    EStatusType.ExcessiveUptime
                });
        }

        public Threshold2 T { get; private set; }

        public TimeStatus(double short_uptime, double long_uptime)
        {
            T = new Threshold2(short_uptime, long_uptime);
        }

        public EStatusType? GetStatus(ulong days_up)
        {
            if (days_up >= T.Transition && days_up < T.Transition2)
                return null;
            else if (days_up < T.Transition)
                return EStatusType.RecentlyRebooted;
            else
                return EStatusType.ExcessiveUptime;
        }
    }
}
