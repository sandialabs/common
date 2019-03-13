using System;

namespace gov.sandia.sld.common.logging
{
    /// <summary>
    /// Interface that mirrors log4net's interface. When logging you'll get an
    /// actual object with this interface allowing you to log at different levels.
    /// </summary>
    public interface ILog
    {
        bool IsErrorEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsDebugEnabled { get; }

        void Fatal(string message);
        void Error(string message);
        void Warn(string message);
        void Info(string message);
        void Debug(string message);
        void Error(Exception ex);

        void FatalFormat(string fmt, params object[] args);
        void ErrorFormat(string fmt, params object[] args);
        void WarnFormat(string fmt, params object[] args);
        void InfoFormat(string fmt, params object[] args);
        void DebugFormat(string fmt, params object[] args);
    }
}
