using System;

namespace gov.sandia.sld.common.data.wmi
{
    /// <summary>
    /// The values we're getting are all unsigned numbers, and we want to sum them all up
    /// </summary>
    public class SummedData : WMIDataCollector
    {
        public SummedData(WMIContext wmi_context, DataCollectorContext dc_context)
            : base(wmi_context, dc_context)
        {
        }

        public override CollectedData OnAcquire()
        {
            bool success = false;
            UInt64 sum = 0;
            OnAcquireDelegate(
                dict =>
                {
                    // Assume there's a single thing in Properties. If there are multiple items you'll have
                    // to define your own OnAcquire.

                    object o = dict[WmiContext.Properties];
                    if (o != null)
                    {
                        UInt64 u = 0;
                        string s = o.ToString();
                        if (string.IsNullOrEmpty(s) == false && UInt64.TryParse(s, out u))
                            sum += u;
                        success = true;
                    }
                });

            Data d = new Data(Context) { Value = sum.ToString() };
            return new CollectedData(Context, success, d);
        }
    }
}
