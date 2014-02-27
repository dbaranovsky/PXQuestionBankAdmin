using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Logging
{
    /// <summary>
    /// Adds convenience methods to handle typical ILogger use cases.
    /// </summary>
    public static class ILoggerExtensions
    {
        /// <summary>
        /// Writes a message to the logger if it is no null. Message can be formatted and will have
        /// LogSeverity.Debug.
        /// </summary>
        /// <param name="logger">ILogger to log message to.</param>
        /// <param name="formattedMessage">Message to log, can contain string.Format replacements.</param>
        /// <param name="args">Any parameters to be passed for the formatted message.</param>
        public static void Debug(this ILogger logger, string formattedMessage, params object[] args)
        {
            if (logger != null)
            {
                logger.Log(string.Format(formattedMessage, args), LogSeverity.Debug);
            }
        }

        /// <summary>
        /// Logs the exception if logger is not null.
        /// </summary>
        /// <param name="logger">ILogger to log exception to.</param>
        /// <param name="ex">Exception to log.</param>
        public static void Exception(this ILogger logger, Exception ex)
        {
            if (logger != null)
            {
                logger.Log(ex);
            }
        }
    }
}
