using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Logging
{
    /// <summary>
    /// Defines capabilities for enterprise logging.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// When set, this information is put into every message and trace, allowing them to be correlated.
        /// </summary>
        string CorrelationId { get; set; }

        /// <summary>
        /// Log a simple message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="severity">Severity level of the message.</param>
        void Log(string message, LogSeverity severity);

        /// <summary>
        /// Log a simple message with a set of categories.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="severity">Severity level of the message.</param>
        /// <param name="categories">List of categories the message belongs to.</param>
        void Log(string message, LogSeverity severity, IList<string> categories);

        /// <summary>
        /// Log an exception, assuming it is an Error. Only the Exception.Message is logged by default.
        /// </summary>
        /// <param name="ex">Exception to log.</param>
        void Log(Exception ex);

        /// <summary>
        /// Log an exception, allowing the severity to be specified.
        /// </summary>
        /// <param name="ex">Exception to log.</param>
        /// <param name="severity">Severity of the exception.</param>
        void Log(Exception ex, LogSeverity severity);

        /// <summary>
        /// Log an exception with the given severity and categories.
        /// </summary>
        /// <param name="ex">Exception to log.</param>
        /// <param name="severity">Severity of the exception.</param>
        /// <param name="categories">Set of categories to apply to the exception.</param>
        void Log(Exception ex, LogSeverity severity, IList<string> categories);

        /// <summary>
        /// Log a complex message.
        /// </summary>
        /// <param name="message">Full message to log.</param>
        void Log(LogMessage message);

        /// <summary>
        /// Determines whether, in the current configuration, a log entry with the given categories should be logged.
        /// </summary>
        /// <param name="categories">The list of categories to check for.</param>
        /// <returns><code>true</code> if a log entry with the given categories should be logged, <code>false</code> otherwise.</returns>
        bool ShouldLog(params string[] categories);
    }
}
