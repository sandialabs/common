using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.dailyfiles;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.dailyreport;
using gov.sandia.sld.common.db.interpreters;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.db.responders;
using gov.sandia.sld.common.requestresponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace DailyFileImport
{
    class Program
    {
        static void Main(string[] args)
        {
            string config_file = null;
            for(int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-i")
                {
                    if (++i < args.Length)
                        config_file = args[i];
                }
            }

            Database db = new Database();
            Console.WriteLine("Initializing Database");
            Initializer initializer = new Initializer(new Initializer.EOptions[] { Initializer.EOptions.SkipSystemCreation });
            initializer.Initialize(db);

            SystemConfiguration config = null;
            using (SQLiteConnection conn = db.Connection)
            {
                conn.Open();

                if(config_file != null)
                {
                    try
                    {
                        Console.WriteLine("Reading config file: " + config_file);
                        FileInfo fi = new FileInfo(config_file);
                        string text = File.ReadAllText(fi.FullName);
                        SystemConfiguration c = JsonConvert.DeserializeObject<SystemConfiguration>(text);
                        if (c != null)
                        {
                            // There's no timestamp in the file, so about all we can do is use the timestamp
                            // of the file itself.
                            SetSystemConfiguration(c, fi.CreationTime, db);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error reading config file");
                        Console.WriteLine(ex.Message);
                    }
                }

                // Get the current configuration from the database, then when we read in a new
                // configuration from the files we'll update the DB.
                config = GetSystemConfiguration(conn);
            }

            Console.WriteLine("Reading Daily Files");

            FileInformationResponder responder = new FileInformationResponder();
            SystemBus.Instance.Subscribe(responder);

            string here = AppDomain.CurrentDomain.BaseDirectory;
            Reader reader = new Reader(here);
            List<Reader.DailyFileInfo> reports = reader.DoRead();

            // Put the reports in order by day so we process the configuration changes properly.
            reports.Sort((a, b) => a.Day.CompareTo(b.Day));

            // If there's nothing to speak of in the database, such as when we just initialized it,
            // scan through the daily files until we get a SystemConfiguration. That's about the best we can
            // hope for.
            if(config == null ||
                config.devices.Count < 2)
            {
                config = FindConfigInDailyFiles(reports);
            }

            Dictionary<string, DataCollectorContext> collector_id_cache = GetCollectorIDCache(config);

            DataStorage data_storage = new DataStorage();
            BaseInterpreter.AllInterpreters();

            using (SQLiteConnection conn = db.Connection)
            {
                conn.Open();
                foreach (Reader.DailyFileInfo report in reports)
                {
                    Console.WriteLine($"Loading {report.Info.Name}");

                    Stopwatch watch = Stopwatch.StartNew();
                    int count = 0;
                    using (SQLiteTransaction t = conn.BeginTransaction())
                    {
                        foreach (DailyReport.Record record in report.Report.records)
                        {
                            if (record.type == ECollectorType.Configuration)
                            {
                                // Deserialize to a SystemConfiguration object, then store that in the DB, and reload the
                                // config from the DB
                                config = JsonConvert.DeserializeObject<SystemConfiguration>(record.value);
                                SetSystemConfiguration(config, record.timestamp, db);
                                collector_id_cache = GetCollectorIDCache(GetSystemConfiguration(conn));
                            }
                            else
                            {
                                if (collector_id_cache.TryGetValue(record.collector, out DataCollectorContext dc_context))
                                {
                                    DataStorage.DataRecord r = new DataStorage.DataRecord(dc_context, record.value, record.timestamp);
                                    r.D.CollectedAt = record.timestamp;
                                    data_storage.Insert(r, conn);
                                    count++;
                                }
                                else
                                {
                                    Console.WriteLine($"Unable to find {record.collector}");
                                }
                            }
                        }
                        t.Commit();
                    }
                    long ms = watch.ElapsedMilliseconds;
                    long ms_per_record = count == 0 ? ms : ms / count;

                    Console.WriteLine($"Inserting {count} records took {ms} ms, at {ms_per_record} ms per record");
                }
            }
        }

        private static SystemConfiguration FindConfigInDailyFiles(List<Reader.DailyFileInfo> reports)
        {
            SystemConfiguration config = null;
            foreach (Reader.DailyFileInfo report in reports)
            {
                foreach (DailyReport.Record record in report.Report.records)
                {
                    if (record.type == ECollectorType.Configuration)
                    {
                        config = JsonConvert.DeserializeObject<SystemConfiguration>(record.value);
                        break;  // out of the inner foreach
                    }
                }
                if (config != null)
                    break;  // out of the outer foreach
            }

            return config;
        }

        private static SystemConfiguration GetSystemConfiguration(SQLiteConnection conn)
        {
            return SystemConfigurationStore.Get(false, conn);
        }

        private static void SetSystemConfiguration(SystemConfiguration config, DateTimeOffset timestamp, Database db)
        {
            SystemConfigurationStore.Set(config, timestamp, db);
        }

        private static Dictionary<string, DataCollectorContext> GetCollectorIDCache(SystemConfiguration config)
        {
            Dictionary<string, DataCollectorContext> collector_id_cache = new Dictionary<string, DataCollectorContext>();
            config.devices.ForEach(d => { d.collectors.ForEach(c => { collector_id_cache[c.name] = c.DCContext; }); });
            return collector_id_cache;
        }
    }
}
