using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gov.sandia.sld.common.configuration
{
    /// <summary>
    /// The UPS battery status as reported by WMI.
    /// Status values retrieved from https://msdn.microsoft.com/en-us/library/aa394074%28v=vs.85%29.aspx
    /// </summary>
    public enum EUPSBatteryStatus
    {
        [Description("The battery is discharging")]
        Other = 1,
        [Description("The system has access to AC so no battery is being discharged. However, the battery is not necessarily charging.")]
        Unknown,
        [Description("Fully Charged")]
        FullyCharged,
        [Description("Low")]
        Low,
        [Description("Critical")]
        Critical,
        [Description("Charging")]
        Charging,
        [Description("Charging and High")]
        ChargingAndHigh,
        [Description("Charging and Low")]
        ChargingAndLow,
        [Description("Charging and Critical")]
        ChargingAndCritical,
        [Description("Undefined")]
        Undefined,
        [Description("Partially Charged")]
        PartiallyCharged,
    }
}
