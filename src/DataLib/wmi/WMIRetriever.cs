using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace gov.sandia.sld.common.data.wmi
{
    public class WMIRetrieverOptions
    {
        public TimeSpan Timeout { get; set; }

        public WMIRetrieverOptions()
        {
            Timeout = TimeSpan.FromSeconds(30);
        }
    }

    public class WMIRetriever
    {
        private WMIContext Context { get; set; }
        private WMIRetrieverOptions Options { get; set; }

        public WMIRetriever(WMIContext context, WMIRetrieverOptions options)
        {
            Context = context;
            Options = options;

            m_retrieval_context = new RetrievalContext(Options.Timeout);
        }

        /// <summary>
        /// Gets a list of Dictionaries mapping the property name to the object retrieved
        /// for that property.
        /// A list is returned because some WMI call return multiple things
        /// 
        /// Was recently updated to handle the WMI calls in an asynchronous way, so the
        /// data collection can be stopped during a long-running WMI call.
        /// 
        /// https://msdn.microsoft.com/en-us/library/cc143292.aspx
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public RetrievalContext Retrieve(ILog log)
        {
            string query_str = string.Format("SELECT {0} FROM {1}", Context.Properties, Context.Class);
            if (string.IsNullOrEmpty(Context.Where) == false)
                query_str += " WHERE " + Context.Where;
            string path = string.Empty;

            try
            {
                SelectQuery query = new SelectQuery(query_str);
                ManagementScope scope = Context.GetManagementScope();
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    path = scope.Path.Path;
                    if (log != null)
                        log.Debug("Executing: " + path + " -- " + query_str);

                    ManagementOperationObserver results = new ManagementOperationObserver();
                    results.ObjectReady += new ObjectReadyEventHandler(this.OnNewObject);
                    results.Completed += new CompletedEventHandler(this.OnComplete);

                    // Start a new Retrieving object so everything is up-to-date right now
                    m_retrieval_context = new RetrievalContext(Options.Timeout);

                    searcher.Get(results);

                    while (m_retrieval_context.IsCompleted == false)
                    {
                        if (m_retrieval_context.HasTimedOut)
                        {
                            results.Cancel();
                            throw new Exception($"Timeout executing '{query_str}', path is '{path}'");
                        }

                        if (GlobalIsRunning.IsRunning == false)
                        {
                            m_retrieval_context.IsCompleted = true;
                            results.Cancel();
                        }
                        else
                            Thread.Sleep(250);
                    }
                }
            }
            catch (Exception e)
            {
                if (log != null)
                {
                    log.Error(e);
                    log.Error(query_str);
                    if (string.IsNullOrEmpty(path) == false)
                        log.Error(path);
                }
            }
            finally
            {
                m_retrieval_context.IsCompleted = true;
            }

            return m_retrieval_context;
        }

        /// <summary>
        /// Callback when part of the WMI data has been retrieved. There may be multiple
        /// parts to the WMI response, so this may be called repeatedly before OnComplete is
        /// called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNewObject(object sender, ObjectReadyEventArgs args)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            try
            {
                foreach (string property in Context.PropertiesList)
                {
                    object o = args.NewObject[property];
                    if (o != null)
                    {
                        dict[property] = o;
                        //Console.WriteLine($"Property {property} == {o.ToString()}");
                    }
                }
            }
            catch (Exception)
            {
            }

            m_retrieval_context.Add(dict);

            // We got something, so let's restart the timer so we don't timeout on a long
            // response such as when we're getting lots of system errors.
            m_retrieval_context.OnNewObject();
        }

        /// <summary>
        /// Callback for when the WMI data has been completely retrieved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnComplete(object sender, CompletedEventArgs args)
        {
            m_retrieval_context.IsCompleted = true;
        }

        public class RetrievalContext
        {
            public bool IsCompleted { get; set; }
            public bool HasTimedOut { get { return Watch.ElapsedMilliseconds > Timeout; } }
            public List<Dictionary<string, object>> RetrievedData { get; private set; }
            public long Timeout { get; private set; }
            private Stopwatch Watch { get; set; }
            public uint RetrievedCount { get { lock (RetrievedData) return (uint)RetrievedData.Count; } }

            public RetrievalContext(TimeSpan timeout)
            {
                IsCompleted = false;
                Timeout = (long)timeout.TotalMilliseconds;
                RetrievedData = new List<Dictionary<string, object>>();
                Watch = Stopwatch.StartNew();
            }

            public void OnNewObject()
            {
                Watch.Restart();
            }

            public void Add(Dictionary<string, object> dict)
            {
                if (dict == null || dict.Count == 0)
                    return;

                lock (RetrievedData)
                    RetrievedData.Add(dict);
            }
        }

        private RetrievalContext m_retrieval_context;
    }
}
