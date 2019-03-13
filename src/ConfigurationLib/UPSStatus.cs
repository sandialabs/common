using System.Collections.Generic;

namespace gov.sandia.sld.common.configuration
{
    public class UPSStatus
    {
        public static List<EStatusType> Types
        {
            get =>
                new List<EStatusType>(new EStatusType[] {
                    EStatusType.UPSProvidingPower,
                    EStatusType.UPSNotProvidingPower
                });
        }

        public EStatusType GetStatus(EUPSBatteryStatus battery_status)
        {
            return battery_status == EUPSBatteryStatus.Other ? EStatusType.UPSProvidingPower : EStatusType.UPSNotProvidingPower;
        }
    }
}
