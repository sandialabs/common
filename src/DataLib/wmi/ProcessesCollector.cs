using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace gov.sandia.sld.common.data.wmi
{
    public class Process
    {
        public const ulong MB = 1024 * 1024;

        [JsonIgnore]
        public string Name { get; set; }
        public string CPU { get { return CPUNum.ToString(); } set { ulong u; if (ulong.TryParse(value, out u)) CPUNum = u; } }
        public string Memory { get { return MemoryNum.ToString(); } set { ulong u; if (ulong.TryParse(value, out u)) MemoryNum = u; } }
        public string MemoryInMB { get { return string.Format("{0:0.0}", MemoryInMBNum); } }

        public ulong CPUNum { get; set; }

        public ulong WorkingSetNum { get; set; }
        public ulong WorkingSetPrivateNum { get; set; }
        public ulong PoolNonpagedBytesNum { get; set; }
        public ulong PoolPagedBytesNum { get; set; }
        public ulong PrivateBytesNum { get; set; }

        /// <summary>
        /// Returns the largest of the 5 memory numbers
        /// </summary>
        [JsonIgnore]
        public ulong MemoryNum
        {
            get
            {
                return Math.Max(WorkingSetNum, Math.Max(WorkingSetPrivateNum, Math.Max(PoolNonpagedBytesNum, Math.Max(PoolPagedBytesNum, PrivateBytesNum))));
            }
            set
            {
                WorkingSetNum = WorkingSetPrivateNum = PoolNonpagedBytesNum = PoolPagedBytesNum = PrivateBytesNum = value;
            }
        }

        public float MemoryInMBNum { get { return (float)MemoryNum / (float)MB; } }
        public float WorkingSetInMBNum { get { return (float)WorkingSetNum / (float)MB; } }
        public float WorkingSetPrivateInMBNum { get { return (float)WorkingSetPrivateNum / (float)MB; } }
        public float PoolNonpagedBytesInMBNum { get { return (float)PoolNonpagedBytesNum / (float)MB; } }
        public float PoolPagedBytesInMBNum { get { return (float)PoolPagedBytesNum / (float)MB; } }
        public float PrivateBytesInMBNum { get { return (float)PrivateBytesNum / (float)MB; } }

        public ulong NumProcesses { get; set; }

        public Process(string name, ulong cpu, ulong working_set, ulong working_set_private, ulong pool_nonpaged, ulong pool_paged, ulong priv)
        {
            Name = name;
            CPUNum = cpu;
            WorkingSetNum = working_set;
            WorkingSetPrivateNum = working_set_private;
            PoolNonpagedBytesNum = pool_nonpaged;
            PoolPagedBytesNum = pool_paged;
            PrivateBytesNum = priv;
            NumProcesses = 1;
        }

        public void Add(ulong cpu, ulong working_set, ulong working_set_private, ulong pool_nonpaged, ulong pool_paged, ulong priv)
        {
            CPUNum += cpu;
            WorkingSetNum += working_set;
            WorkingSetPrivateNum += working_set_private;
            PoolNonpagedBytesNum += pool_nonpaged;
            PoolPagedBytesNum += pool_paged;
            PrivateBytesNum += priv;
            ++NumProcesses;
        }
    }

    public class ProcessesCollector : WMIDataCollector
    {
        public ProcessesCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_PerfFormattedData_PerfProc_Process", "Name,PercentProcessorTime,WorkingSet,WorkingSetPrivate,PoolNonpagedBytes,PoolPagedBytes,PrivateBytes", remote_info),
                  new DataCollectorContext(id, ECollectorType.Processes))
        {
            m_process_suffix = new Regex(@"(.*)#\d+$");
            m_cores = 0;
        }

        public override CollectedData OnAcquire()
        {
            // The number of cores will be different for each of the devices that we monitor, so we have to get this
            // inside each RunningProcessesData object, not globally. But at least it won't change so we can just get it once.
            if(m_cores == 0)
            {
                WMIRetriever retriever = new WMIRetriever(new WMIContext("Win32_Processor", "NumberOfCores", WmiContext.RemoteInfo), new WMIRetrieverOptions());
                WMIRetriever.RetrievalContext retrieval_context = retriever.Retrieve(null);

                if(retrieval_context != null && retrieval_context.RetrievedData.Count > 0)
                {
                    Dictionary<string, object> r = retrieval_context.RetrievedData[0];
                    uint.TryParse(r["NumberOfCores"].ToString(), out m_cores);
                }
            }

            if (m_cores == 0)
                throw new Exception("Unable to determine the # of cores on the machine");

            Dictionary<string, Process> collected_data = new Dictionary<string, Process>();
            // We do this twice...I found that sometimes the first time we'd try and get the data we'd get nothing,
            // but the second attempt would work. Go figure.
            OnAcquireDelegate(dict => { });
            OnAcquireDelegate(
                dict =>
                {
                    string name = dict["Name"].ToString();
                    string percent_processor_time = dict["PercentProcessorTime"].ToString();
                    string working_set = dict["WorkingSet"].ToString();
                    string working_set_private = dict["WorkingSetPrivate"].ToString();
                    string pool_nonpaged_bytes = dict["PoolNonpagedBytes"].ToString();
                    string pool_paged_bytes = dict["PoolPagedBytes"].ToString();
                    string private_bytes = dict["PrivateBytes"].ToString();

                    if (!ulong.TryParse(percent_processor_time, out ulong cpu) ||
                        !ulong.TryParse(working_set, out ulong set) ||
                        !ulong.TryParse(working_set_private, out ulong set_private) ||
                        !ulong.TryParse(pool_nonpaged_bytes, out ulong pool_nonpaged) ||
                        !ulong.TryParse(pool_paged_bytes, out ulong pool_paged) ||
                        !ulong.TryParse(private_bytes, out ulong priv) ||
                        string.Compare(name, "_Total", true) == 0 ||
                        string.Compare(name, "Idle", true) == 0)
                        return;

                    // Sometimes an app will have multiple parts to it. For example,
                    // Chrome will often report chrome, chrome#1, chrome#2, chrome#3, etc.
                    // This is to combine all of those together into a single 'chrome' report.
                    // We sum up the percentages, which I think is OK.
                    Match m = m_process_suffix.Match(name);
                    if (m.Success)
                    {
                        name = m.Groups[1].Value;

                        if (collected_data.TryGetValue(name, out Process p))
                            p.Add(cpu, set, set_private, pool_nonpaged, pool_paged, priv);
                        else
                            collected_data[name] = new Process(name, cpu, set, set_private, pool_nonpaged, pool_paged, priv);
                    }
                    else
                        collected_data[name] = new Process(name, cpu, set, set_private, pool_nonpaged, pool_paged, priv);
                }
            );

            // Package the process information into a single Data object that contains a double-dictionary. The dictionary will contain
            // a mapping from process name to process information, and the process information is itself a dictionary mapping a string
            // ("Memory", "CPU") to the number representing that.
            GenericDictionaryData<Process> d = new GenericDictionaryData<Process>(Context);

            foreach(KeyValuePair<string, Process> process in collected_data)
            {
                Process p = process.Value;

                // The percentage is based on the capacity of a single core, so we have to divide by the # of cores
                // to get the overall capacity.
                ulong percent = (ulong)((double)p.CPUNum / (double)m_cores + 0.5f);
                p.CPUNum = percent;

                d.Data[p.Name] = p;
            }

            return new CollectedData(Context, d.Data.Count > 0, d);
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            GenericDictionaryData<Process> d = new GenericDictionaryData<Process>(context);
            ProcessInfoBuilder builder = new ProcessInfoBuilder();
            builder.Build(value);
            if (builder.Processes != null)
                d.Data = builder.Processes;
            return d;
        }

        private Regex m_process_suffix;
        private uint m_cores;
    }
}
