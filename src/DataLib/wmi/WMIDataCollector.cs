using System.Collections.Generic;

namespace gov.sandia.sld.common.data.wmi
{
    public class WMIDataCollector : DataCollector
    {
        protected WMIContext WmiContext { get; private set; }
        protected WMIRetrieverOptions RetrieverOptions { get; set; }

        public WMIDataCollector(WMIContext wmi_context, DataCollectorContext dc_context)
            : base(dc_context)
        {
            WmiContext = wmi_context;
            RetrieverOptions = new WMIRetrieverOptions();
        }

        public override CollectedData OnAcquire()
        {
            bool success = false;
            Data d = null;

            // Default acquire: just make a comma-delimited string of all the values.
            OnAcquireDelegate(
                dict =>
                {
                    // Assume there's a single thing in Properties. If there are multiple items you'll have
                    // to define your own OnAcquire.

                    object o = dict[WmiContext.Properties];
                    if (o != null)
                    {
                        if(d == null)
                            d = new Data(Context);

                        if (string.IsNullOrEmpty(d.Value) == false)
                            d.Value += ",";
                        d.Value += o.ToString();
                        success = true;
                    }
                });

            return new CollectedData(Context, success, d);
        }

        protected delegate void OnProperty(Dictionary<string, object> dict);
        protected virtual void OnAcquireDelegate(OnProperty on_property)
        {
            WMIRetriever retriever = new WMIRetriever(WmiContext, RetrieverOptions);
            WMIRetriever.RetrievalContext retrieval_context = retriever.Retrieve(m_log);

            if(retrieval_context != null)
                retrieval_context.RetrievedData.ForEach(r => on_property(r));
        }
    }
}
