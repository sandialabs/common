using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class ProcessHistory
    {
        public int deviceID { get; set; }
        public string processName { get; set; }

        // For performance reasons, this will be a list of [timestamp, CPU %, Memory]
        public List<object[]> details { get; set; }

        public ProcessHistory()
        {
            deviceID = -1;
            processName = string.Empty;
            details = new List<object[]>();
        }

        public void AddData(DateTimeOffset timestamp, ulong cpu, ulong memory)
        {
            object[] o = new object[] { timestamp, cpu, memory };
            details.Add(o);
        }
    }
}
