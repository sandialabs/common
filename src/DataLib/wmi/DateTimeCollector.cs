using System;
using System.Management;

namespace gov.sandia.sld.common.data.wmi
{
    public class DateTimeCollector : WMIDataCollector
    {
        public DateTimeCollector(WMIContext wmi_context, DataCollectorContext dc_context)
            : base(wmi_context, dc_context)
        {
        }

        public override CollectedData OnAcquire()
        {
            CollectedData d = base.OnAcquire();
            if (d.DataIsCollected)
            {
                foreach (Data data in d.D)
                {
                    if (string.IsNullOrEmpty(data.Value) == false)
                        data.Value = new DateTimeOffset(ManagementDateTimeConverter.ToDateTime(data.Value)).ToString("o");
                }
            }
            return d;
        }
    }
}
