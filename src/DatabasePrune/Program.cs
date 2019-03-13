using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace DatabasePrune
{
    class Program
    {
        static void Main(string[] args)
        {
            string db_filename = string.Empty;

            if (args.Length >= 1)
                db_filename = args[0];

            if(string.IsNullOrEmpty(db_filename))
            {
                ShowUsage();
                return;
            }
            else
            {
                Context.SpecifyFilename(db_filename);
                Database db = new Database();
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    SystemConfiguration config = SystemConfigurationStore.Get(false, conn);
                    foreach (DeviceInfo device in config.devices)
                        PruneDevice(device, conn);

                    Vaccuum(conn);
                }
            }
        }

        private static void Vaccuum(SQLiteConnection conn)
        {
            Console.WriteLine("Compacting empty space");

            Stopwatch watch = Stopwatch.StartNew();
            conn.ExecuteNonQuery("VACUUM");
            Console.WriteLine($"Compacting took {watch.ElapsedMilliseconds} ms");

            Console.WriteLine("");
            Console.WriteLine("Complete");
        }

        private static void PruneDevice(DeviceInfo device, SQLiteConnection conn)
        {
            Console.WriteLine($"Pruning {device.name}");
            foreach (CollectorInfo collector in device.collectors)
                PruneCollector(collector, conn);
        }

        private static void PruneCollector(CollectorInfo collector, SQLiteConnection conn)
        {
            Console.WriteLine($">>> Starting prune for {collector.name}");
            ECollectorType type = collector.collectorType;
            int default_frequency = type.GetFrequencyInMinutes();
            TimeSpan span = TimeSpan.FromMinutes(default_frequency);

            Stopwatch watch = Stopwatch.StartNew();

            List<long> doomed = GatherDoomedData(collector, span, out long saved_count, conn);

            Console.WriteLine($"Found {doomed.Count} records to delete, {saved_count} will be retained");
            Console.WriteLine($"Search took {watch.ElapsedMilliseconds} ms");

            watch.Restart();

            MeetTheirDemise(doomed, conn);

            Console.WriteLine($"<<< Completed prune for {collector.name}, it took {watch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Gets the list of DataIDs that should go away
        /// </summary>
        /// <param name="collector">Which collector is being pruned</param>
        /// <param name="span">The desired span between data that is retained</param>
        /// <param name="saved_count">How many data records will be saved</param>
        /// <param name="conn">The DB connection</param>
        /// <returns>The list of DataIDs</returns>
        private static List<long> GatherDoomedData(CollectorInfo collector, TimeSpan span, out long saved_count, SQLiteConnection conn)
        {
            List<long> doomed = new List<long>();

            string sql = $"SELECT DataID, TimeStamp FROM Data WHERE CollectorID = {collector.id} ORDER BY TimeStamp DESC;";
            saved_count = 0;
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                DateTimeOffset? next_time = null;

                while (reader.Read())
                {
                    DateTimeOffset timestamp = DateTimeOffset.Parse(reader.GetString(1));
                    if (next_time.HasValue == false)
                    {
                        next_time = timestamp - span;
                        ++saved_count;
                    }
                    else if (timestamp < next_time.Value)
                    {
                        next_time -= span;
                        ++saved_count;
                    }
                    else
                    {
                        long data_id = reader.GetInt64(0);
                        doomed.Add(data_id);
                    }
                }
            }

            return doomed;
        }

        /// <summary>
        /// Performs the deletions of the specified DataIDs
        /// </summary>
        /// <param name="doomed">The list of DataIDs that should be deleted</param>
        /// <param name="conn">The DB connection</param>
        private static void MeetTheirDemise(List<long> doomed, SQLiteConnection conn)
        {
            List<List<long>> chunks = doomed.ChunkBy(100);
            int ten_percent = chunks.Count / 10;
            for (int i = 0; i < chunks.Count; ++i)
            {
                List<long> chunk = chunks[i];
                string in_clause = chunk.JoinStrings(",");

                if (i > 0 && i % ten_percent == 0)
                    Console.Write(".");

                string sql = $"DELETE FROM MostRecentDataPerCollector WHERE DataID IN ({in_clause});";
                conn.ExecuteNonQuery(sql);

                sql = $"DELETE FROM Data WHERE DataID IN ({in_clause});";
                conn.ExecuteNonQuery(sql);
            }
            Console.WriteLine("");
        }

        private static void ShowUsage()
        {
            Console.WriteLine("DatabasePrune <path to database>");
            Console.WriteLine(" Deletes data from the database, keeping the default collection");
            Console.WriteLine(" rates for each collector.");
            Console.WriteLine("Do not use on a real database--only a backup. You know why.");
        }
    }
}
