using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gov.sandia.sld.common.logging
{
    public abstract class BaseLogger : ILog
    {
        public string TypeString { get; private set; }
        public LogManager.LogLevel Level { get; set; }

        public BaseLogger(Type type, LogManager.LogLevel level = LogManager.LogLevel.Error)
        {
            TypeString = type.ToString();
            Level = level;
        }

        public virtual bool IsDebugEnabled { get { return Level >= LogManager.LogLevel.Debug; } }
        public virtual bool IsErrorEnabled { get { return Level >= LogManager.LogLevel.Error; } }
        public virtual bool IsInfoEnabled { get { return Level >= LogManager.LogLevel.Info; } }
        public virtual bool IsWarnEnabled { get { return Level >= LogManager.LogLevel.Warn; } }

        public void Debug(string message)
        {
            if (IsDebugEnabled)
                Output(message, "DEBUG");
        }

        public void DebugFormat(string fmt, params object[] args)
        {
            if (IsDebugEnabled)
                Debug(string.Format(fmt, args));
        }

        public void Error(Exception ex)
        {
            if (IsErrorEnabled)
            {
                Error(ex.StackTrace);
                while (ex != null)
                {
                    Error(ex.Message);
                    ex = ex.InnerException;
                }
            }
        }

        public void Error(string message)
        {
            if (IsErrorEnabled)
                Output(message, "ERROR");
        }

        public void ErrorFormat(string fmt, params object[] args)
        {
            if (IsErrorEnabled)
                Error(string.Format(fmt, args));
        }

        public void Fatal(string message)
        {
            Output(message, "FATAL");
        }

        public void FatalFormat(string fmt, params object[] args)
        {
            Fatal(string.Format(fmt, args));
        }

        public void Info(string message)
        {
            if (IsInfoEnabled)
                Output(message, "INFO");
        }

        public void InfoFormat(string fmt, params object[] args)
        {
            if (IsInfoEnabled)
                Info(string.Format(fmt, args));
        }

        public void Warn(string message)
        {
            if (IsWarnEnabled)
                Output(message, "WARN");
        }

        public void WarnFormat(string fmt, params object[] args)
        {
            if (IsWarnEnabled)
                Warn(string.Format(fmt, args));
        }

        protected virtual void Output(string message, string level_message)
        {
            string output = string.Format("[{0}] {1} {2}: {3}", DateTimeOffset.Now.ToString("o"), level_message, TypeString, message);
            DoOutput(output);
        }

        protected abstract void DoOutput(string output);
    }
}
