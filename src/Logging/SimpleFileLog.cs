using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace gov.sandia.sld.common.logging
{
    /// <summary>
    /// Just a quick class for logging string entries to a file. The file can optionally
    /// have the date/time the log entry was made in its filename.
    /// Each entry in the log file will have the time/date of the log entry prepended to it.
    /// The maximum size the file can be can also be set. Once the file reaches that size,
    /// a new file will be created. The file names have a -XXXX inserted into the filename,
    /// and each X is a hexadecimal character, allowing for up to 65k files per day.
    /// </summary>
    public class SimpleFileLog
    {
        public static String DefaultLogDirectory { get { return s_default_directory; } set { s_default_directory = value; } }
        public static bool LogUTC { get; set; }

        public String Directory { get; private set; }
        public String Filename { get; private set; }   // this is the filename format string
        public String CurrentFilename { get; private set; }
        public bool AppendDateToFilename { get; private set; }
        public uint MaxFileSizeInMB
        {
            get { return m_max_file_size / (1024 * 1024); }
            set { m_max_file_size = value * (1024 * 1024); }
        }

        /// <summary>
        /// Create an object to write simple string entries to a file.
        /// </summary>
        /// <param name="directory">The directory the file will go in</param>
        /// <param name="filename">The name the file should have</param>
        /// <param name="append_date_to_filename">true to add the time/date to the filename</param>
        public SimpleFileLog(String directory, String filename, bool append_date_to_filename)
        {
            Directory = directory;
            Filename = filename;
            AppendDateToFilename = append_date_to_filename;
            m_last_datetime = DateTime.Now;
            m_counter = 0;
            m_lock = new object();

            lock (c_lock)
                m_log_num = c_next_log_num++;

            // Default is 10 MB
            m_max_file_size = 1024 * 1024 * 10;

            if (!Directory.EndsWith("\\"))
                Directory += "\\";

            System.IO.Directory.CreateDirectory(Directory);

            Filename.Replace("\\", String.Empty);

            // Fix up Filename now so we only have to do it once
            if (append_date_to_filename)
                Filename = Filename.Replace(".", "-{0:D4}-{1:D2}-{2:D2}-{3:X4}.");
            else
                Filename = Filename.Replace(".", "-{0:X4}.");
        }

        public static SimpleFileLog Instance(String name)
        {
            SimpleFileLog result = null;
            lock (c_static_loggers)
            {
                if (c_static_loggers.ContainsKey(name)) result = c_static_loggers[name];
                else
                {
                    String directory = DefaultLogDirectory;
                    result = new SimpleFileLog(directory, name + "_log.txt", true);
                    c_static_loggers[name] = result;
                }
            }
            return result;
        }

        /// <summary>
        /// Add a string to the log file.
        /// </summary>
        /// <param name="str">The string to log</param>
        /// <returns>true if successful; false if an error occurred</returns>
        public bool Append(String str)
        {
            return Append((sw, prefix) => sw.WriteLine(prefix + str));
        }

        public bool Append(List<String> messages)
        {
            return Append((sw, prefix) => messages.ForEach(m => sw.WriteLine(prefix + m)));
        }

        private delegate void DoAppend(StreamWriter writer, string prefix);
        private bool Append(DoAppend callback)
        {
            lock (m_lock)
            {
                bool success = true;
                DateTime now = DateTime.Now;

                if (now.Day != m_last_datetime.Day)
                    m_counter = 0;

                String filename = GenerateFilename();
                FileInfo info = new FileInfo(filename);

                // Do a while loop here so restarting will get to the correct counter value
                while (info.Exists && info.Length > m_max_file_size)
                {
                    ++m_counter;
                    filename = GenerateFilename();
                    info = new FileInfo(filename);
                }

                CurrentFilename = filename;

                try
                {
                    string prefix = GeneratePrefix(now);
                    using (StreamWriter writer = new StreamWriter(filename, true))
                    {
                        callback(writer, prefix);
                    }
                }
                catch (Exception)
                {
                    success = false;
                }

                m_last_datetime = now;

                return success;
            }
        }

        private static String GetDefaultDirectory()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            String directory = System.IO.Path.GetDirectoryName(assembly.Location) + "\\LogFiles";
            return directory;
        }

        private string GenerateFilename()
        {
            DateTime now = DateTime.Now;
            if(AppendDateToFilename)
                return Directory + String.Format(Filename, now.Year, now.Month, now.Day, m_counter);
            else
                return Directory + String.Format(Filename, m_counter);
        }

        private string GeneratePrefix(DateTime now)
        {
            if (LogUTC)
                now = now.ToUniversalTime();
            return now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ");
        }

        static SimpleFileLog()
        {
            LogUTC = true;
        }

        private uint m_max_file_size;
        private DateTime m_last_datetime;
        private ushort m_counter;
        private uint m_log_num;
        private object m_lock;

        private static Dictionary<String, SimpleFileLog> c_static_loggers = new Dictionary<string, SimpleFileLog>();
        private static object c_lock = new object();
        private static uint c_next_log_num = 1;
        private static String s_default_directory = SimpleFileLog.GetDefaultDirectory();
    }
}
