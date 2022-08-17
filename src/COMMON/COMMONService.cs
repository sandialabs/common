using COMMONWeb;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.dailyfiles;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.interpreters;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.db.responders;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.requestresponse;
using gov.sandia.sld.common.utilities;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace gov.sandia.sld.common
{
    internal partial class COMMONService : ServiceBase
    {
        public COMMONService()
        {
            InitializeComponent();

#if DEBUG
            Console.CancelKeyPress += delegate {
                Shutdown();
                Environment.Exit(0);
            };
#endif

            logging.EventLog.GlobalSource = "COMMONService";

            try
            {
                LogManager.InitializeConfigurator("common_log_config.json");
            }
            catch (Exception)
            {
            }

            m_daily_file_writer = new Writer();
            m_shutdown = new ManualResetEvent(false);
            m_startup = new ManualResetEvent(false);

            m_last_configuration_update = DateTimeOffset.MinValue;
            m_system_device = null;
            m_days_to_keep = 180;
            m_daily_file_cleaner = new dailyfiles.Cleaner(m_days_to_keep);
            m_db_cleaner = new db.Cleaner();

            m_responders = new List<IResponder>(new IResponder[] {
                new EventLogResponder(),
                new DatabaseInfoResponder(),
                new AttributeResponder(),
                new IPAddressResponder(),
                new COMMONDBSizeResponder(),
                new MonitoredDrivesResponder(),
                //new SMARTFailureResponder(),
            });

            m_host_configuration = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            m_host = new NancyHost(new Uri("http://localhost:8080"), new COMMONDatabaseBootstrapper(), m_host_configuration);
        }

        protected override void OnStart(string[] args)
        {
            Startup();
        }

        protected override void OnStop()
        {
            Shutdown();
        }

        public void Startup()
        {
            if (m_thread != null)
                throw new Exception("Startup: Starting while already running");

            Stopwatch totalWatch = Stopwatch.StartNew();

            logging.EventLog elog = new ApplicationEventLog();

            elog.LogInformation("Starting");
            GlobalIsRunning.Start();
            m_shutdown.Reset();

            elog.LogInformation("Initializing database");
            Stopwatch watch = Stopwatch.StartNew();
            Database db = new Database();
            new Initializer(null).Initialize(db);
            elog.LogInformation($"Database initialization took {watch.ElapsedMilliseconds} ms");

            db.SetAttribute("service.startup_time", DateTimeOffset.Now.ToString("o"));
            string assembly_ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            db.SetAttribute("software.version", assembly_ver);

            using (SQLiteConnection conn = db.Connection)
            {
                conn.Open();

                AlertsEmailSMTP emailSMTP = new AlertsEmailSMTP();
                AlertsEmailFrom emailFrom = new AlertsEmailFrom();
                AlertsEmailTo emailTo = new AlertsEmailTo();

                m_alertReceiver = new AlertReceiver(emailSMTP.GetValue(conn), emailFrom.GetValue(conn), emailTo.GetValue(conn), elog);
                SystemBus.Instance.Subscribe(m_alertReceiver);
            }

            m_interpreters = BaseInterpreter.AllInterpreters();

            elog.LogInformation("Setting up responders");
            m_responders.ForEach(r => SystemBus.Instance.Subscribe(r));

            elog.LogInformation("Starting web server");
            watch.Restart();
            m_host.Start();
            elog.LogInformation($"Web server startup took {watch.ElapsedMilliseconds} ms");

            elog.LogInformation("Starting work thread");
            watch.Restart();
            m_thread = new Thread(new ThreadStart(ThreadFunc));

            m_startup.Reset();
            m_thread.Start();

            // Wait for the thread to start
            m_startup.WaitOne();

            elog.LogInformation($"Work thread startup took {watch.ElapsedMilliseconds} ms");
            elog.LogInformation($"Completed startup in {totalWatch.ElapsedMilliseconds} ms");
        }

        public void Shutdown()
        {
            Stopwatch totalWatch = Stopwatch.StartNew();

            logging.EventLog elog = new ApplicationEventLog();

            elog.LogInformation("Stopping");

            Stopwatch watch = Stopwatch.StartNew();
            GlobalIsRunning.Stop();
            m_shutdown.Set();

            m_thread.Join();

            elog.LogInformation($"Stopping worker thread took {watch.ElapsedMilliseconds} ms");

            Database db = new Database();
            db.SetAttribute("service.stop_time", DateTimeOffset.Now.ToString("o"));

            elog.LogInformation("Stopping web server");
            m_host.Stop();
            m_host.Dispose();

            elog.LogInformation("Clearing responders");
            m_responders.ForEach(r => SystemBus.Instance.Unsubscribe(r));

            SystemBus.Instance.Unsubscribe(m_alertReceiver);

            m_thread = null;
            elog.LogInformation($"Completed stopping in {totalWatch.ElapsedMilliseconds} ms");
        }

        protected void ThreadFunc()
        {
            try
            {
                //int max_collections_per_pass = 10;
                logging.EventLog elog = new ApplicationEventLog();
                Database db = new Database();
                DataStorage storage = new DataStorage();
                m_interpreters.ForEach(i => storage.AddInterpreter(i));

                m_startup.Set();

                while (GlobalIsRunning.IsRunning)
                {
                    //int collector_count = 0;

                    using (SQLiteConnection conn = db.Connection)
                    {
                        conn.Open();

                        CheckForConfigurationChanges(storage, conn);

                        // Used to hold which collector was doing its thing if/when an exception occurs.
                        // It's used in the exception handler.
                        string collector_name = string.Empty;

                        try
                        {
                            DBCollectionTimeRetriever retriever = new DBCollectionTimeRetriever(conn);

                            // Gets the list of things that need to be collected right now. They'll
                            // be in the order they should be collected.
                            List<DataCollector> collectors = m_system_device.GetCollectors(retriever);

                            // Keep track of how many collecters there are to collect from
                            //collector_count = collectors.Count;

                            //// Limit this to the top 10 or so
                            //while (collectors.Count > max_collections_per_pass)
                            //    collectors.RemoveAt(collectors.Count - 1);

                            foreach (DataCollector collector in collectors)
                            {
                                if (GlobalIsRunning.IsRunning == false)
                                    break;  // out of the foreach loop

                                collector_name = collector.Context.Name;
                                //elog.LogInformation($"Collecting {collector_name}");

                                Stopwatch watch = Stopwatch.StartNew();

                                // Records that the collector is being collected, updates the next collection
                                // time, and records when the collection attempt was started. When it's destroyed
                                // when exiting the using, it records that it is no longer being collected.
                                //
                                // This was done so even if an exception occurs within Acquire(), the flag
                                // that the collector is being collected will be cleared.
                                using (BeingCollected bc = new BeingCollected(collector.Context.ID.ID, conn))
                                {
                                    collector.Acquire();
                                }

                                //long elapsed_ms = watch.ElapsedMilliseconds;
                                //if(elapsed_ms > 500)
                                //    elog.LogInformation($"Collecting {collector_name} took {elapsed_ms} ms");
                            }
                        }
                        catch (Exception e)
                        {
                            elog.LogError($"Exception from within {collector_name}");
                            elog.Log(e);
                        }

                        // Will write the daily file when it's the right time to do so; otherwise, it does nothing.
                        if (GlobalIsRunning.IsRunning)
                            m_daily_file_writer.DoWrite(conn);

                        if (GlobalIsRunning.IsRunning)
                            m_db_cleaner.CleanOldData(m_days_to_keep, conn);
                    }

                    // Deletes any old daily files
                    if (GlobalIsRunning.IsRunning)
                        m_daily_file_cleaner.DoClean();

                    // Delete any old log files too, if it's time to do so.
                    if (GlobalIsRunning.IsRunning)
                        LogManager.CleanOldData();

                    // And make sure we update our logging if it changed
                    if (GlobalIsRunning.IsRunning)
                        LogManager.CheckConfiguration();

                    // m_shutdown will be reset when the thread starts, and set when it's time to
                    // stop the thread. So this will wait if this event hasn't been
                    // set, but will return immediately if it has been set. That way we can shut
                    // down immediately if we're in the middle of sleeping.
                    //
                    // We wait up to 10 seconds so we're using minimal resources. Better than busy looping
                    // to find there's nothing to collect.
                    m_shutdown.WaitOne(TimeSpan.FromSeconds(10));
                }

                m_interpreters.ForEach(i => storage.RemoveInterpreter(i));
            }
            catch (Exception e)
            {
                logging.EventLog elog = new ApplicationEventLog();
                elog.Log(e);

                m_shutdown.Reset();
                m_thread.Abort();
                m_thread = null;
            }
        }

        private void CheckForConfigurationChanges(DataStorage storage, SQLiteConnection conn)
        {
            Database db = new Database();

            DateTimeOffset? configuration_update = db.GetLastConfigurationUpdateAttribute(conn);

            if (m_last_configuration_update == DateTimeOffset.MinValue || (configuration_update.HasValue && configuration_update.Value != m_last_configuration_update))
            {
                if (configuration_update.HasValue)
                    m_last_configuration_update = configuration_update.Value;

                logging.EventLog elog = new ApplicationEventLog();
                elog.LogInformation("Loading configuration from database");
                //db.Initialize();

                SystemConfiguration config = SystemConfigurationStore.Get(false, conn);
                m_system_device = new SystemDevice(config, storage);

                DeleteDays delete_days = new DeleteDays();
                int? days = delete_days.GetValueAsInt(conn);
                m_days_to_keep = days ?? 180;

                m_daily_file_cleaner.DaysToKeep = m_days_to_keep;
            }
        }

        private Writer m_daily_file_writer;
        private dailyfiles.Cleaner m_daily_file_cleaner;
        private db.Cleaner m_db_cleaner;
        private Thread m_thread;
        private ManualResetEvent m_shutdown;
        private ManualResetEvent m_startup;

        private SystemDevice m_system_device;
        private DateTimeOffset m_last_configuration_update;
        private int m_days_to_keep;

        private HostConfiguration m_host_configuration;
        private NancyHost m_host;

        private List<IResponder> m_responders;
        private List<IDataInterpreter> m_interpreters;

        private AlertReceiver m_alertReceiver;
    }
}
