using System;

namespace gov.sandia.sld.common.logging
{
    /// <summary>
    /// Object to use for logging to a file. Logs to the LogManager file log singleton.
    /// </summary>
    public class FileLogger : BaseLogger
    {
        public override bool IsErrorEnabled { get { return LogManager.Level >= LogManager.LogLevel.Error; } }
        public override bool IsWarnEnabled { get { return LogManager.Level >= LogManager.LogLevel.Warn; } }
        public override bool IsInfoEnabled { get { return LogManager.Level >= LogManager.LogLevel.Info; } }
        public override bool IsDebugEnabled { get { return LogManager.Level == LogManager.LogLevel.Debug; } }

        public FileLogger(Type type, int thread_id)
            : base(type, LogManager.LogLevel.Error)
        {
            m_thread_id = thread_id.ToString().PadLeft(3);
        }

        protected override void Output(string message, string level)
        {
            if (LogManager.LogFile == null)
                return;

            string output = string.Format("[{0}] {1} {2}: {3}", m_thread_id, level, TypeString, message);
            DoOutput(output);
        }

        protected override void DoOutput(string output)
        {
            LogManager.LogFile.Append(output);
        }

        private string m_thread_id;
    }
}
