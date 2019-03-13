using System.Collections.Generic;

namespace gov.sandia.sld.common.data.wmi
{
    public class MultiPropertyWMIDataCollector : WMIDataCollector
    {
        public MultiPropertyWMIDataCollector(WMIContext wmi_context, DataCollectorContext dc_context)
            : base(wmi_context, dc_context)
        {
        }
        public override CollectedData OnAcquire()
        {
            bool success = false;
            List<Data> dataList = new List<Data>();
            // Default acquire: just make a comma-delimited string of all the values.
            OnAcquireDelegate(
                dict =>
                {
                    DictionaryData d = new DictionaryData(Context);

                    // Assume there are multiple thing in PropertiesList.
                    foreach (var prop in WmiContext.PropertiesList)
                    {
                        if (dict.ContainsKey(prop))
                        {
                            object o = dict[prop];
                            if (o != null)
                            {
                                d.Data[prop] = o.ToString();
                                success = true;
                            }
                        }
                    }
                    dataList.Add(d);
                });

            return new CollectedData(Context, success, dataList);
        }
    }
}
