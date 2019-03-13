using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System;
using System.Diagnostics;

namespace gov.sandia.sld.common.dailyfiles
{
    /// <summary>
    /// Cleans old daily files
    /// </summary>
    public class Cleaner
    {
        public int DaysToKeep { get; set; }
        public bool IsTimeToClean { get { return m_timer.IsTime(); } }

        public Cleaner(int days_to_keep)
        {
            DaysToKeep = days_to_keep;

            // Get a DateTime for 1:01 AM this morning, then add 1 day to get
            // to tomorrow morning. When we hit that we'll generate the daily file
            // for today (and all earlier dates, if need be).
            DateTime now = DateTime.Now;
            DateTime current_day = new DateTime(now.Year, now.Month, now.Day, 1, 1, 0);
            TimeSpan one_day = TimeSpan.FromDays(1);
            m_timer = new IntervalTimer(current_day + one_day, one_day);
        }

        public void DoClean()
        {
            if (IsTimeToClean == false)
                return;
            m_timer.Reset();

            string msg = $"Deleting old daily file(s) in {Filename.Directory} older than {DaysToKeep} days";
            Stopwatch watch = Stopwatch.StartNew();

            DirectoryCleaner cleaner = new DirectoryCleaner(Filename.Directory, "*.json");
            cleaner.Clean(fi =>
            {
                int age = (int)((DateTime.Now - fi.CreationTime).TotalDays);
                //log.LogInformation($"{fi.FullName} is {age} days old");
                bool to_delete = age >= DaysToKeep;

                if (to_delete)
                    msg += $"\nDeleting {fi.FullName} -- it is {age} days old";

                return to_delete;
            });

            msg += $"\n\nDone deleting old daily file(s) -- it took {watch.ElapsedMilliseconds} ms";

            logging.EventLog log = new ApplicationEventLog();
            log.LogInformation(msg);
        }

        private IntervalTimer m_timer;
    }
}
