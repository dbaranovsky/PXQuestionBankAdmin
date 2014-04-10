using System;
using NLog;

namespace Macmillan.PXQBA.Common.Logging
{
    public static class StaticLogger
    {
        private static readonly Logger logger;

        static StaticLogger()
        {
            logger = LogManager.GetLogger("commonLogger");
        }

        private static void Log(Action logAction, bool canExecute)
        {
            if (canExecute)
            {
                logAction();
            }
        }

        private static void Error(String messageText, Exception exception)
        {
            do
            {
                Exception exceptionToLog = exception;
                Log(() => logger.Error(messageText ?? String.Empty, exceptionToLog), logger.IsErrorEnabled);
                if (exceptionToLog != null)
                    exception = exceptionToLog.InnerException;
            } while (exception != null);
        }


        private static void Info(String message)
        {
            Log(() => logger.Info(message ?? String.Empty), logger.IsInfoEnabled);
        }

        public static void LogInfo(String messageText)
        {
            Info(messageText);
        }

        public static void LogDebug(String messageText, Exception exception = null)
        {
            do
            {
                Exception exceptionToLog = exception;
                Log(() => logger.Debug(messageText ?? String.Empty, exceptionToLog), logger.IsDebugEnabled);
                if (exceptionToLog != null)
                    exception = exceptionToLog.InnerException;
            } while (exception != null);
        }

        public static Guid LogError(Exception exception)
        {
            var id = Guid.NewGuid();
            Error(String.Format("Error id: {0} ", id.ToString()), exception);
            return id;
        }

        public static Guid LogError(String messageText, Exception exception)
        {
            var id = Guid.NewGuid();
            Error(String.Format("Error id: {0}\n ", id.ToString()) + messageText, exception);
            return id;
        }

        public static void LogWarning(String messageText)
        {
            Log(() => logger.Warn(messageText ?? String.Empty), logger.IsWarnEnabled);
        }
    }
}
