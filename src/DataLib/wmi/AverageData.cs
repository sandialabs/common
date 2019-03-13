using System;

namespace gov.sandia.sld.common.data.wmi
{
    /// <summary>
    /// The values we're getting are all unsigned numbers, and we want to average them all
    /// </summary>
    public class AverageData : WMIDataCollector
    {
        public AverageData(WMIContext wmi_context, DataCollectorContext dc_context)
            : base(wmi_context, dc_context)
        {
        }

        public override CollectedData OnAcquire()
        {
            UInt64 sum = 0;
            UInt64 count = 0;
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
                        {
                            sum += u;
                            ++count;
                        }
                    }
                });

            Data d = new Data(Context) { Value = "0" };
            if (count > 0)
            {
                UInt64 average = (UInt64)Math.Round((double)sum / (double)count);
                d.Value = average.ToString();
            }
            return new CollectedData(Context, count > 0, d);
        }
    }
}
