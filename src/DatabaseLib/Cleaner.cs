using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System;
using System.Data.SQLite;
using System.Diagnostics;

namespace gov.sandia.sld.common.db
{
    /// <summary>
    /// Cleans "old" data out of the database
    /// </summary>
    public class Cleaner
    {
        public void CleanOldData(int days_to_keep, SQLiteConnection conn)
        {
            if (GlobalIsRunning.IsRunning == false)
                return;

            try
            {
                DateTimeOffset dt = DateTimeOffset.Now - TimeSpan.FromDays(days_to_keep);
                string dt_as_8601 = dt.DayBeginAs8601();

                //log.LogInformation("Deleting old data before " + dt_as_8601));

                Stopwatch watch = Stopwatch.StartNew();
                Deleter deleter = new Deleter("NetworkStatus", $"DatePingAttempted < '{dt_as_8601}'", conn);
                deleter.Execute();

                // To keep a single delete from taking too long, let's just do 100 at a time.
                int count = 100;
                string clause = $"DataID IN (SELECT DataID FROM Data WHERE TimeStamp < '{dt_as_8601}' ORDER BY TimeStamp ASC LIMIT {count})";

                if (GlobalIsRunning.IsRunning == true)
                {
                    deleter = new Deleter("MostRecentDataPerCollector", clause, conn);
                    deleter.Execute();
                }

                if (GlobalIsRunning.IsRunning == true)
                {
                    deleter = new Deleter("Data", clause, conn);
                    deleter.Execute();
                }

                if (GlobalIsRunning.IsRunning == true)
                {
                    // Don't delete from the DeviceStatus table if the status is still valid. Only ones
                    // that have been superceded should be deleted.
                    clause = $"Date < '{dt_as_8601}' AND IsValid = 0";
                    deleter = new Deleter("DeviceStatus", clause, conn);
                    deleter.Execute();
                }

                watch.Stop();
                long elapsed = watch.ElapsedMilliseconds;
                if (elapsed > 1000)
                {
                    logging.EventLog elog = new ApplicationEventLog();
                    elog.LogInformation($"Finished deleting old data prior to {dt_as_8601}. It took {elapsed} ms");
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("CleanOldData:");
                log.Error(ex);
            }
        }
    }
}
