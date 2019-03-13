using System;
using System.Collections.Generic;
using gov.sandia.sld.common.data.wmi;
using Newtonsoft.Json;

namespace gov.sandia.sld.common.data
{
    /// <summary>
    /// Used in handling our "old" format data structure that kept the process information, and
    /// our "new" format. See the Build() method for an explanation.
    /// </summary>
    public class ProcessInfoBuilder
    {
        public Dictionary<string, Process> Processes { get; set; }

        public ProcessInfoBuilder()
        {
            Processes = new Dictionary<string, Process>();
            m_use_original_format = true;
        }

        public void Build(string value)
        {
            Processes = new Dictionary<string, Process>();

            // We changed the way the process data was stored. Originally, it was a map from the process
            // name to an integer with the CPU usage. Now it's a map from the process name to a dictionary
            // mapping "CPU" to the CPU usage of the process, and mapping "Memory" to the number of bytes used by the process.

            // We'll start out using the 'old' format, and when an exception is thrown, we'll switch to the new.

            if (m_use_original_format)
            {
                BuildOriginal(value);
            }

            if (!m_use_original_format)
            {
                BuildNew(value);

                if(m_use_original_format)
                {
                    // Try one last time with the original
                    BuildOriginal(value);
                }
            }
        }

        private void BuildOriginal(string value)
        {
            try
            {
                var v = JsonConvert.DeserializeObject<OriginalFormat>(value);
                foreach (KeyValuePair<string, string> process in v.Value)
                {
                    ulong.TryParse(process.Value, out ulong cpu);
                    Processes[process.Key] = new Process(process.Key, cpu, 0, 0, 0, 0, 0);
                }
            }
            catch (JsonException)
            {
                m_use_original_format = false;
            }
        }

        private void BuildNew(string value)
        {
            try
            {
                var v = JsonConvert.DeserializeObject<NewFormat>(value);
                Processes = v.Value;
            }
            catch (Exception)
            {
                m_use_original_format = true;
            }
        }

        private bool m_use_original_format;

        private class OriginalFormat
        {
            public Dictionary<string, string> Value { get; set; }
        }

        private class NewFormat
        {
            public Dictionary<string, Process> Value { get; set; }
        }
    }
}
