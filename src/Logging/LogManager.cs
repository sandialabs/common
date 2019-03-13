using System;
using System.Threading;
using System.IO;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;

namespace gov.sandia.sld.common.logging
{
    /// <summary>
    /// Static class for managing logging.
    /// Do GetLogger(typeof(ThisClass)) to get an ILog, which you can
    /// then use to log stuff.
    /// Use InitializeConfigurator(filename) to set up a thread that
    /// gets the logging configuration from the specified filename,
    /// and will monitor that file for changes every 10 seconds. If
    /// the file changes, the logging configuration will be updated.
    /// </summary>
    public static class LogManager
    {
        public enum LogLevel { Fatal, Error, Warn, Info, Debug };

        public class ConfigOptions
        {
            public string path { get; set; }
            public LogManager.LogLevel level { get; set; }
            public bool isEnabled { get; set; }
            public int keepDays { get; set; }

            public ConfigOptions()
            {
                path = @"C:\COMMON\LogFiles\logfile.txt";
                level = LogLevel.Error;
                isEnabled = true;
                keepDays = 90;
            }
        }

        internal static SimpleFileLog LogFile { get; private set; }

        public static ConfigOptions Options
        {
            get { return c_options; }
            internal set
            {
                lock(c_lock)
                    c_options = value;
                SetOptions();
            }
        }

        public static LogLevel Level
        {
            get { return c_options.level; }
            set
            {
                if(Enum.IsDefined(typeof(LogLevel), value))
                    c_options.level = value;
            }
        }

        public static ILog GetLogger(Type type)
        {
            return new FileLogger(type, Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// Create a log file in the specified directory with the specified name
        /// </summary>
        /// <param name="directory">The directory to write the file in</param>
        /// <param name="name">The base name of the file. Date and sequence
        /// will be added to the actual filename.</param>
        public static void InitializeLogFile(string directory, string name)
        {
            lock (c_lock)
                LogFile = new SimpleFileLog(directory, name, true);
        }

        /// <summary>
        /// Read the specified filename to get log file configuration settings.
        /// Should be a JSON file with this basic format:
        /// {
        ///   "path": "c:\\common\\logfiles\\logfile.txt",
        ///   "level": "error",
        ///   "isEnabled": "false",
        ///   "keepDays": "90"
        ///}
        /// </summary>
        /// <param name="filename">The JSON file to get the configuration options from</param>
        public static void InitializeConfigurator(string filename)
        {
            lock (c_lock)
                c_config_checker = new ConfigChecker(filename);
        }

        public static void CleanOldData()
        {
            bool is_time = c_clean_logs_timer.IsTime();

            if (is_time)
            {
                c_clean_logs_timer.Reset();

                string directory = string.Empty;
                lock (c_lock)
                {
                    if (LogFile != null)
                        directory = LogFile.Directory;
                }

                if (directory != string.Empty)
                {
                    DirectoryCleaner cleaner = new DirectoryCleaner(directory);
                    cleaner.Clean(fi =>
                    {
                        return (DateTime.Now - fi.LastAccessTime).TotalDays >= Options.keepDays;
                    });
                }
            }
        }

        /// <summary>
        /// Call this periodically to see if the configuration file may have changed
        /// </summary>
        public static void CheckConfiguration()
        {
            lock (c_lock)
                if (c_config_checker != null)
                    c_config_checker.Update();
        }

        private static void SetOptions()
        {
            EventLog el = new ApplicationEventLog();

            try
            {
                if (c_options.isEnabled)
                {
                    FileInfo info = new FileInfo(c_options.path);
                    lock (LogManager.c_lock)
                        LogManager.LogFile = new SimpleFileLog(info.Directory.FullName, info.Name, true);

                    el.LogInformation(string.Format("Logging to '{0}' with level '{1}'", c_options.path, c_options.level));
                }
                else
                {
                    // If we disable it, it should stop logging
                    lock (LogManager.c_lock)
                        LogManager.LogFile = null;
                    el.LogInformation(string.Format("No longer logging to '{0}'", c_options.path));
                }

                el.LogInformation(string.Format("Keeping {0} days of log files", c_options.keepDays));
            }
            catch (Exception ex)
            {
                el.LogError("Error: disabling logging\n\n" + ex.Message);
                lock (LogManager.c_lock)
                    LogManager.LogFile = null;
            }
        }

        static LogManager()
        {
            c_lock = new object();
            c_config_checker = null;
            c_clean_logs_timer = new IntervalTimer(DateTime.Now, TimeSpan.FromHours(6));

            LogFile = null;
            Options = new ConfigOptions();
        }

        private static object c_lock;
        private static ConfigOptions c_options;
        private static ConfigChecker c_config_checker;
        private static IntervalTimer c_clean_logs_timer;

        /// <summary>
        /// Class for peridocially checking the log configuration file for changes.
        /// 
        /// The XML in the file looks something like this:
        // <log>
        //   <file enabled="true" fullpath="c:\logs\logfile.txt" level="debug"/>
        // </log>
        /// 
        /// </summary>
        private class ConfigChecker
        {
            private FileInfo m_file;
            private DateTime? m_last_file_write_time;
            private DateTime m_next_check_time;
            private TimeSpan m_interval;

            public ConfigChecker(string filename)
            {
                string base_dir = AppDomain.CurrentDomain.BaseDirectory + filename;

                new ApplicationEventLog().LogInformation("Reading logfile configuration from: " + base_dir);
        
                m_file = new FileInfo(base_dir);

                m_next_check_time = DateTime.MinValue;
                m_interval = TimeSpan.FromSeconds(10);

                Update();
            }

            public void Update()
            {
                if (DateTime.Now > m_next_check_time)
                {
                    m_next_check_time = DateTime.Now + m_interval;

                    m_file.Refresh();
                    if (m_file.Exists &&
                        (m_last_file_write_time == null || m_file.LastWriteTime > m_last_file_write_time.Value))
                    {
                        try
                        {
                            string text = File.ReadAllText(m_file.FullName);
                            ConfigOptions options = JsonConvert.DeserializeObject<ConfigOptions>(text);
                            if(options != null)
                                LogManager.Options = options;
                        }
                        catch (Exception ex)
                        {
                            EventLog el = new ApplicationEventLog();
                            el.LogError("Error: disabling logging\n\n" + ex.Message);
                            lock (LogManager.c_lock)
                                LogManager.LogFile = null;
                        }

                        m_last_file_write_time = m_file.LastWriteTime;
                    }
                }
            }
        }
    }
}
