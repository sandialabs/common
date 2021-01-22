using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

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
            RetrievalContext retrieval_context = new RetrievalContext();

            string query_str = $"SELECT {Context.Properties} FROM {Context.Class}";
            if (string.IsNullOrEmpty(Context.Where) == false)
                query_str += $" WHERE {Context.Where}";
            string path = string.Empty;

            try
            {
                SelectQuery query = new SelectQuery(query_str);
                ManagementScope scope = Context.GetManagementScope();
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    path = scope.Path.Path;
                    if (log != null)
                        log.Debug($"Executing: {path} -- {query_str}");

                    Task task = new Task(() =>
                    {
                        foreach (ManagementObject queryObj in searcher.Get())
                        {
                            Dictionary<string, object> dict = new Dictionary<string, object>();
                            foreach (string property in Context.PropertiesList)
                            {
                                object o = queryObj[property];
                                if (o != null)
                                    dict[property] = o;
                            }
                            retrieval_context.Add(dict);
                        }
                    });

                    task.Start();

                    // Returns true if the task completed before the timeout; false if it timed out.
                    // If the cancellation token is fired, an exception will be thrown.
                    bool completed = task.Wait((int)Options.Timeout.TotalMilliseconds, GlobalIsRunning.Source.Token);
                    retrieval_context.IsCompleted = completed;
                    if (completed == false)
                        throw new Exception($"Timeout executing '{query_str}', path is '{path}'");
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

            return retrieval_context;
        }

        public class RetrievalContext
        {
            public bool IsCompleted { get; set; }
            public List<Dictionary<string, object>> RetrievedData { get; private set; }

            public RetrievalContext()
            {
                IsCompleted = true;
                RetrievedData = new List<Dictionary<string, object>>();
            }

            public void Add(Dictionary<string, object> dict)
            {
                if (dict == null || dict.Count == 0)
                    return;

                lock (RetrievedData)
                    RetrievedData.Add(dict);
            }
        }
    }
}
