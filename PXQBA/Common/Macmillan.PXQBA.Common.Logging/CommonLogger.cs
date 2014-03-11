﻿using Bfw.Common.Logging;
using NLog;

namespace Macmillan.PXQBA.Common.Logging
{
    public class CommonLogger : LoggerBase
    {
        private readonly Logger logger;

        public CommonLogger()
        {
            logger = LogManager.GetLogger("commonLogger");
        }

        /// <summary>
        /// Logs the messages into configured destination
        /// </summary>
        /// <param name="message"></param>
        public override void Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Information:
                    logger.Info(message.Message);
                    break;
                case LogSeverity.Debug:
                    logger.Debug(message.Message);
                    break;
                case LogSeverity.Warning:
                    logger.Warn(message.Message);
                    break;
                case LogSeverity.Error:
                    logger.Error(message.Message);
                    break;
                default:
                    logger.Info(message.Message);
                    break;
            }
        }

        public override bool ShouldLog(params string[] categories)
        {
            return true;
        }
    }
}
