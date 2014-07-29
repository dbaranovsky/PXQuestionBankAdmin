using System;
using NLog;

namespace Macmillan.PXQBA.Common.Logging
{
    /// <summary>
    /// Logger class to log info errors and warnings
    /// </summary>
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
                Log(() => logger.LogException(LogLevel.Error, messageText ?? String.Empty, exceptionToLog), logger.IsErrorEnabled);
                if (exceptionToLog != null)
                    exception = exceptionToLog.InnerException;
            } while (exception != null);
        }

        private static void Error(String messageText)
        {
            Log(() => logger.Error(messageText ?? String.Empty), logger.IsErrorEnabled);
        }


        private static void Info(String message)
        {
            Log(() => logger.Info(message ?? String.Empty), logger.IsInfoEnabled);
        }

        /// <summary>
        /// Logs messages for Info level
        /// </summary>
        /// <param name="messageText">Message</param>
        public static void LogInfo(String messageText)
        {
            Info(messageText);
        }

        /// <summary>
        /// Logs messages for Debug level
        /// </summary>
        /// <param name="messageText">Message</param>
        /// <param name="exception">Exception</param>
        public static void LogDebug(String messageText, Exception exception = null)
        {
            do
            {
                Exception exceptionToLog = exception;
                Log(() => logger.LogException(LogLevel.Debug, messageText ?? String.Empty, exceptionToLog), logger.IsDebugEnabled);
                if (exceptionToLog != null)
                    exception = exceptionToLog.InnerException;
            } while (exception != null);
        }

        /// <summary>
        /// Logs messages for Error level
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Error guid</returns>
        public static Guid LogError(Exception exception)
        {
            var id = Guid.NewGuid();
            Error(String.Format("Error id: {0} ", id.ToString()), exception);
            return id;
        }

        /// <summary>
        /// Logs messages for Error level
        /// </summary>
        /// <param name="messageText">Message</param>
        /// <param name="exception">Exception</param>
        /// <returns>Error guid</returns>
        public static Guid LogError(String messageText, Exception exception)
        {
            var id = Guid.NewGuid();
            Error(String.Format("Error id: {0}\n ", id.ToString()) + messageText, exception);
            return id;
        }

        /// <summary>
        /// Logs messages for Warning level
        /// </summary>
        /// <param name="messageText">Message</param>
        public static void LogWarning(String messageText)
        {
            Log(() => logger.Warn(messageText ?? String.Empty), logger.IsWarnEnabled);
        }

        /// <summary>
        /// Logs messages for Error level
        /// </summary>
        /// <param name="messageText">Message</param>
        /// <returns>Error guid</returns>
        public static Guid LogError(String messageText)
        {
            var id = Guid.NewGuid();
            Error(String.Format("Error id: {0}\n ", id.ToString()) + messageText);
            return id;
        }
    }
}
