namespace gov.sandia.sld.common.data.wmi
{
    /// <summary>
    /// Put the values in a string list
    /// </summary>
    public class ListWMIDataCollector : WMIDataCollector
    {
        public ListWMIDataCollector(WMIContext wmi_context, DataCollectorContext dc_context, ListStringData.Options options)
            : base(wmi_context, dc_context)
        {
            m_options = options;
        }

        public override CollectedData OnAcquire()
        {
            bool success = false;
            ListStringData d = new ListStringData(Context, m_options);
            OnAcquireDelegate(
                dict =>
                {
                    // Assume there's a single thing in Properties. If there are multiple items you'll have
                    // to define your own OnAcquire.

                    object o = dict[WmiContext.Properties];
                    if (o != null)
                    {
                        string s = o.ToString();
                        d.Add(s);
                        success = true;
                    }
                });
            return new CollectedData(Context, success, d);
        }

        private ListStringData.Options m_options;
    }
}
