using NLog;
using System;
using System.ComponentModel.Composition;

namespace Logger
{
    public interface ILogger
    {
        void WARN(string message);
        void DEBUG(string message);
        void DEBUG(string message,Exception ex);
        void ERROR(string message);
        void ERROR(string message, Exception ex);
        void FATAL(string message);
        void INFO(string message);
        void TRACE(string message);
    }

    [Export(typeof(ILogger))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Logger : ILogger
    {
        private static  NLog.Logger NLogger = LogManager.GetCurrentClassLogger();

        public void TRACE(string message)
        {
            NLogger.Trace(message);
        }

        public void DEBUG(string message)
        {
            NLogger.Debug(message);
        }

        public void WARN(string message)
        {
            NLogger.Warn(message);
        }

        public void DEBUG(string message, Exception ex)
        {
            NLogger.Debug(ex, message);
        }

        public void ERROR(string message)
        {
            NLogger.Error(message);
        }

        public void ERROR(string message, Exception ex)
        {
            NLogger.Error(ex, message);
        }

        public void FATAL(string message)
        {
            NLogger.Fatal(message);
        }

        public void INFO(string message)
        {
            NLogger.Info(message);
        }
    }
}
