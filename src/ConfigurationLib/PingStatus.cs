using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.configuration
{
    public class PingStatus
    {
        public static List<EStatusType> OnlineOrOffline { get => new List<EStatusType>(new EStatusType[] { EStatusType.Online, EStatusType.Offline }); }
        public static List<EStatusType> GoodOrSlow { get => new List<EStatusType>(new EStatusType[] { EStatusType.GoodPing, EStatusType.SlowPing }); }

        public static Tuple<EStatusType, EStatusType?> GetStatus(bool is_pingable, long average_response_time)
        {
            EStatusType a = is_pingable ? EStatusType.Online : EStatusType.Offline;

            if (is_pingable)
            {
                // 100 ms
                bool good_ping = average_response_time < 100;
                return Tuple.Create<EStatusType, EStatusType?>(a, good_ping ? EStatusType.GoodPing : EStatusType.SlowPing);
            }
            else
                return Tuple.Create<EStatusType, EStatusType?>(a, null);
        }
    }
}
