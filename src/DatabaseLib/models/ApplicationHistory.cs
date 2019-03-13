using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class ApplicationHistory
    {
        public string name { get; set; }
        public List<Snapshot> history { get; set; }

        public ApplicationHistory(string name)
        {
            this.name = name;
            history = new List<Snapshot>();
        }

        public class Snapshot
        {
            public string version { get; set; }
            public DateTimeOffset timestamp { get; set; }
        }
    }
}
