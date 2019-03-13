using System;

namespace gov.sandia.sld.common.logging
{
    /// <summary>
    /// Helper for making event log entries. For this one you can specify your own log name.
    /// </summary>
    public class EventLog
    {
        /// <summary>
        /// The name of the application making the log entry
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// The name of the log. Typically it will be Application or System, but you can make your own.
        /// </summary>
        public string LogName { get; private set; }

        public bool IsLogging { get; private set; }
        public String ErrorMessage { get; private set; }

        public static string GlobalSource { get; set; }

        protected EventLog(string source, string log_name)
        {
            Setup(source, log_name);
        }

        protected EventLog(string log_name)
        {
            if (string.IsNullOrEmpty(GlobalSource))
                IsLogging = false;
            else
                Setup(GlobalSource, log_name);
        }

        private void Setup(string source, string log_name)
        {
            Source = source;
            LogName = log_name;
            IsLogging = true;

            try
            {
                //if (System.Diagnostics.EventLog.Exists(Source) == false)
                //{
                //    try
                //    {
                //        string logname = System.Diagnostics.EventLog.LogNameFromSourceName(Source, ".");
                //        if(logname != log_name)
                //        {
                //            try
                //            {
                //                System.Diagnostics.EventLog.Delete(logname);
                //            }
                //            catch (Exception)
                //            {
                //            }
                //        }
                //        System.Diagnostics.EventLog.DeleteEventSource(Source);
                //        System.Diagnostics.EventLog.CreateEventSource(Source, LogName);
                //    }
                //    catch (Exception)
                //    {
                //    }

                //    //System.Diagnostics.EventLog.WriteEntry(Source, "Created log", System.Diagnostics.EventLogEntryType.Information);
                //}
            }
            catch(Exception e)
            {
                IsLogging = false;
                ErrorMessage = e.Message;
            }
        }

        public void LogInformation(string message)
        {
            if (IsLogging)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Information);
        }

        public void LogError(string message)
        {
            if(IsLogging)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Error);
        }

        public void Log(Exception e)
        {
            if(IsLogging)
            {
                string message = e.StackTrace;
                while(e != null)
                {
                    message += "\n\n" + e.Message;
                    e = e.InnerException;
                }
                LogError(message);
            }
        }

        public void LogWarning(string message)
        {
            if(IsLogging)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Warning);
        }

        static EventLog()
        {
            GlobalSource = string.Empty;
        }
    }

    /// <summary>
    /// Make an event log helper that goes in the Application log
    /// </summary>
    public class ApplicationEventLog : EventLog
    {
        public ApplicationEventLog(string source)
            : base(source, "Application")
        {
        }

        public ApplicationEventLog()
            : base("Application")
        {
        }
    }

    /// <summary>
    /// Make an event log helper that goes in the System log
    /// </summary>
    public class SystemEventLog : EventLog
    {
        public SystemEventLog(string source)
            : base(source, "System")
        {
        }

        public SystemEventLog()
            : base("System")
        {
        }
    }
}
