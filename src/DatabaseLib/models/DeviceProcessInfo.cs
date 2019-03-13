using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class DeviceProcessInfo
    {
        public int deviceID { get; set; }
        public Dictionary<ulong, List<String>> cpuToProcesses { get; set; }
        public DateTimeOffset timestamp { get; set; }

        public DeviceProcessInfo()
        {
            deviceID = -1;
            cpuToProcesses = new Dictionary<ulong, List<string>>();
            timestamp = DateTimeOffset.MinValue;
        }

        public void Add(string process, ulong cpu)
        {
            List<string> processes;
            cpuToProcesses.TryGetValue(cpu, out processes);
            if(processes == null)
            {
                processes = new List<string>();
                cpuToProcesses[cpu] = processes;
            }
            processes.Add(process);
        }

        public void Sort()
        {
            foreach (List<string> processes in cpuToProcesses.Values)
                processes.Sort((a, b) => string.Compare(a, b, true));
        }
    }
}
