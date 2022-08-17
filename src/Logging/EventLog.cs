using System;

namespace gov.sandia.sld.common.logging
{
    /// <summary>
    /// Helper for making event log entries. For this one you can specify your own log name.
    /// </summary>
    public class EventLog : ILog
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

        public LogManager.LogLevel Level { get; set; }

        public static string GlobalSource { get; set; }

        public virtual bool IsDebugEnabled { get { return IsLogging && Level >= LogManager.LogLevel.Debug; } }
        public virtual bool IsErrorEnabled { get { return IsLogging && Level >= LogManager.LogLevel.Error; } }
        public virtual bool IsInfoEnabled { get { return IsLogging && Level >= LogManager.LogLevel.Info; } }
        public virtual bool IsWarnEnabled { get { return IsLogging && Level >= LogManager.LogLevel.Warn; } }

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

            // Default to error level
            Level = LogManager.LogLevel.Debug;

            try
            {
                CreateLog(source, log_name);
            }
            catch(Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);

                IsLogging = false;
                ErrorMessage = e.Message;
            }
        }

        public static void CreateLog(string source, string log_name)
        {
            if (System.Diagnostics.EventLog.SourceExists(source) == false)
            {
                System.Diagnostics.EventLog.CreateEventSource(source, log_name);
                System.Diagnostics.EventLog.WriteEntry(source, $"Created log '{log_name}'", System.Diagnostics.EventLogEntryType.Information);
            }
        }

        public void LogInformation(string message)
        {
            if (IsInfoEnabled)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Information);
        }

        public void LogError(string message)
        {
            if(IsErrorEnabled)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Error);
        }

        public void Log(Exception e)
        {
            if(IsErrorEnabled)
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

        public void Fatal(string message)
        {
            if (IsErrorEnabled)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Error);
        }

        public void Error(string message)
        {
            if (IsErrorEnabled)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Error);
        }

        public void Warn(string message)
        {
            if (IsWarnEnabled)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Warning);
        }

        public void Info(string message)
        {
            if (IsInfoEnabled)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Information);
        }

        public void Debug(string message)
        {
            if (IsDebugEnabled)
                System.Diagnostics.EventLog.WriteEntry(Source, message, System.Diagnostics.EventLogEntryType.Information);
        }

        public void Error(Exception ex)
        {
            Log(ex);
        }

        public void FatalFormat(string fmt, params object[] args)
        {
            if (IsErrorEnabled)
                Fatal(string.Format(fmt, args));
        }

        public void ErrorFormat(string fmt, params object[] args)
        {
            if (IsErrorEnabled)
                Error(string.Format(fmt, args));
        }

        public void WarnFormat(string fmt, params object[] args)
        {
            if (IsWarnEnabled)
                Warn(string.Format(fmt, args));
        }

        public void InfoFormat(string fmt, params object[] args)
        {
            if (IsInfoEnabled)
                Info(string.Format(fmt, args));
        }

        public void DebugFormat(string fmt, params object[] args)
        {
            if (IsDebugEnabled)
                Debug(string.Format(fmt, args));
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
