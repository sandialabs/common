using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class AllApplicationsHistory
    {
        public Dictionary<string, ApplicationHistory> history { get; set; }

        public AllApplicationsHistory()
        {
            history = new Dictionary<string, ApplicationHistory>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void AddHistory(string app_name, string version, DateTimeOffset timestamp)
        {
            ApplicationHistory hist = null;
            if(history.TryGetValue(app_name, out hist) == false)
            {
                hist = new ApplicationHistory(app_name);
                history[app_name] = hist;
            }

            hist.history.Add(new ApplicationHistory.Snapshot { version = version, timestamp = timestamp });
        }
    }
}
