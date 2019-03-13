using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace gov.sandia.sld.common.db
{
    public class CollectionTime
    {
        public SQLiteConnection Conn { get; private set; }

        public CollectionTime(SQLiteConnection conn)
        {
            Conn = conn;
        }

        /// <summary>
        /// Make the return list a priority list. The IDs in the list will be returned
        /// sorted by the time they should be collected.
        /// </summary>
        /// <returns></returns>
        public List<long> GetCollectorIDs()
        {
            Stopwatch watch = Stopwatch.StartNew();

            List<long> devices_to_collect = new List<long>();
            List<Tuple<long, DateTimeOffset>> to_collect = new List<Tuple<long, DateTimeOffset>>();
            string sql = "SELECT C.CollectorID, C.NextCollectionTime, C.CollectorType FROM Collectors C INNER JOIN Devices D ON C.DeviceID = D.DeviceID WHERE D.DateDisabled IS NULL AND C.IsEnabled = 1;";
            //ILog log = LogManager.GetLogger(typeof(Database));
            //logging.EventLog log = new ApplicationEventLog();

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, Conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    DateTimeOffset now = DateTimeOffset.Now;
                    while (reader.Read())
                    {
                        try
                        {
                            long id = reader.GetInt64(0);
                            int type = reader.GetInt32(2);
                            if (type == (int)ECollectorType.Configuration)
                                continue;

                            // If it's never been collected, collect. No need to 
                            // record the collection time--just add it straight to devices_to_collect
                            if (reader.IsDBNull(1))
                                devices_to_collect.Add(id);
                            else
                            {
                                DateTimeOffset next_collection_time = DateTimeOffset.Parse(reader.GetString(1));
                                if (next_collection_time < now)
                                    to_collect.Add(Tuple.Create(id, next_collection_time));
                            }
                        }
                        catch (Exception ex)
                        {
                            logging.EventLog log = new ApplicationEventLog();
                            log.LogError("GetDevicesToCollect");
                            log.Log(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logging.EventLog log = new ApplicationEventLog();
                log.LogError("GetDevicesToCollect: " + sql);
                log.Log(ex);
            }

            to_collect.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            to_collect.ForEach(c => devices_to_collect.Add(c.Item1));

            //log.LogInformation($"GetDevicesToCollect took {watch.ElapsedMilliseconds} ms for {devices_to_collect.Count} devices");

            return devices_to_collect;
        }

        public void UpdateNextCollectionTime(long collector_id)
        {
            //logging.EventLog log = new ApplicationEventLog();

            try
            {
                string sql = $"SELECT FrequencyInMinutes, NextCollectionTime FROM Collectors WHERE CollectorID = {collector_id};";
                DateTimeOffset next = DateTimeOffset.Now;
                TimeSpan? span = null;

                using (SQLiteCommand command = new SQLiteCommand(sql, Conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int frequency = reader.GetInt32(0);
                        if (frequency > 0)
                            span = TimeSpan.FromMinutes(frequency);

                        if (reader.IsDBNull(1) == false)
                            next = DateTimeOffset.Parse(reader.GetString(1));
                    }
                }

                if (span.HasValue)
                {
                    // It's possible that the next collection time is way in the past, such as would be the case
                    // when we start up after having been down for a long time. We don't just want to increment
                    // the next collection time and leave it still in the past, so loop here until the next
                    // time is in the future.
                    while (next <= DateTimeOffset.Now)
                        next += span.Value;

                    Updater updater = new Updater("Collectors", $"CollectorID = {collector_id}", Conn);
                    updater.Set("NextCollectionTime", next);
                    //log.LogInformation("UpdateNextCollectionTime: " + updater.Statement);
                    updater.Execute();
                }
            }
            catch (Exception e)
            {
                logging.EventLog log = new ApplicationEventLog();
                log.LogError($"Error in UpdateNextCollectionTime({collector_id})");
                log.Log(e);
            }
        }

        public void UpdateCollectionAttemptTime(long collector_id, DateTimeOffset time)
        {

            try
            {
                Updater updater = new Updater("Collectors", $"CollectorID = {collector_id}", Conn);
                updater.Set("LastCollectionAttempt", time);
                updater.Execute();
            }
            catch (Exception e)
            {
                logging.EventLog log = new ApplicationEventLog();
                log.LogError($"Error in UpdateCollectionAttemptTime({collector_id}, {time})");
                log.Log(e);
            }
        }

        public CollectorInfo CollectNow(long collector_id)
        {
            CollectorInfo collector_info = null;

            try
            {
                Updater updater = new Updater("Collectors", $"CollectorID = {collector_id}", Conn);

                // Set the time to null so it will rise to the top of the to-do list.
                updater.SetNull("NextCollectionTime");
                updater.Execute();

                collector_info = GetCollectorInfo(collector_id);
            }
            catch (Exception e)
            {
                logging.EventLog log = new ApplicationEventLog();
                log.LogInformation($"Error in CollectNow({collector_id})");
                log.Log(e);
            }

            return collector_info;
        }

        public void BeingCollected(long collector_id, bool is_being_collected)
        {
            try
            {
                Updater updater = new Updater("Collectors", $"CollectorID = {collector_id}", Conn);
                updater.Set("CurrentlyBeingCollected", is_being_collected ? 1 : 0);
                updater.Execute();
            }
            catch (Exception e)
            {
                logging.EventLog log = new ApplicationEventLog();
                log.LogInformation($"Error in BeingCollected({collector_id}, {is_being_collected})");
                log.Log(e);
            }
        }

        private CollectorInfo GetCollectorInfo(long collector_id)
        {
            CollectorInfo collector_info = null;
            string sql = $"SELECT Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes, LastCollectionAttempt, LastCollectedAt, NextCollectionTime, SuccessfullyCollected, CurrentlyBeingCollected FROM Collectors WHERE CollectorID = {collector_id};";
            using (SQLiteCommand command = new SQLiteCommand(sql, Conn))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    CollectorInfo c = new CollectorInfo((ECollectorType)reader.GetInt32(2));
                    c.CID = new data.CollectorID(collector_id, reader.GetString(0));
                    c.isEnabled = reader.GetInt32(3) != 0;
                    c.frequencyInMinutes = reader.GetInt32(4);

                    if (reader.IsDBNull(5) == false)
                        c.lastCollectionAttempt = DateTimeOffset.Parse(reader.GetString(5));
                    if (reader.IsDBNull(6) == false)
                        c.lastCollectedAt = DateTimeOffset.Parse(reader.GetString(6));
                    if (reader.IsDBNull(7) == false)
                        c.nextCollectionTime = DateTimeOffset.Parse(reader.GetString(7));
                    c.successfullyCollected = reader.GetInt32(8) != 0;
                    c.isBeingCollected = reader.GetInt32(9) == 1;

                    collector_info = c;
                }
            }

            return collector_info;
        }
    }
}
