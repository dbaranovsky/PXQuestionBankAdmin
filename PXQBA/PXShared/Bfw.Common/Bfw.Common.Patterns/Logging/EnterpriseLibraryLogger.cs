using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Logging;

using Bfw.Common.Logging;

namespace Bfw.Common.Patterns.Logging
{
    /// <summary>
    /// Implements an ILogger, via LoggerBase, that writes to the Enterprise Library
    /// logging framework.
    /// </summary>
    public class EnterpriseLibraryLogger : LoggerBase
    {

        /// <summary>
        /// LogWriter to use for logging to the Logging Application Block
        /// </summary>
        protected LogWriter Writer { get; set; }

        /// <summary>
        /// Requires a LogWriter instance
        /// </summary>
        /// <param name="logWriter">LogWriter to use</param>
        public EnterpriseLibraryLogger(LogWriter logWriter)
        {
            Writer = logWriter;
        }

        /// <summary>
        /// Converts the LogMessage to a LogEntry and writes it using the LogWriter 
        /// instance this object was created with
        /// </summary>
        /// <param name="message"></param>
        public override void Log(LogMessage message)
        {
            IList<string> cats = new List<string>();

            if (message.Categories != null)
            {
                cats = message.Categories;
            }

            var entry = new LogEntry()
            {
                Message = message.Message,
                Categories = cats,
                Priority = (int)message.Severity,
                ExtendedProperties = message.ExtraInfo
            };

            try
            {
                entry.ExtendedProperties["IPAddress"] = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            catch
            {
                entry.ExtendedProperties["IPAddress"] = "Unknown";
            }

            if (!string.IsNullOrEmpty(CorrelationId))
            {
                if (entry.ExtendedProperties == null)
                    entry.ExtendedProperties = new Dictionary<string, object>();

                entry.ExtendedProperties["CorrelationId"] = CorrelationId;
            }

            switch (message.Severity)
            {
                case LogSeverity.Default:
                    entry.Severity = System.Diagnostics.TraceEventType.Information;
                    if (!cats.Contains("Information")) { cats.Add("Information"); }
                    break;

                case LogSeverity.Information:
                    entry.Severity = System.Diagnostics.TraceEventType.Information;
                    if (!cats.Contains("Information")) { cats.Add("Information"); }
                    break;

                case LogSeverity.Warning:
                    entry.Severity = System.Diagnostics.TraceEventType.Warning;
                    if (!cats.Contains("Warning")) { cats.Add("Warning"); }
                    break;

                case LogSeverity.Debug:
                    entry.Severity = System.Diagnostics.TraceEventType.Information;
                    if (!cats.Contains("Debug")) { cats.Add("Debug"); }
                    break;

                case LogSeverity.Error:
                    entry.Severity = System.Diagnostics.TraceEventType.Error;
                    if (!cats.Contains("Error")) { cats.Add("Error"); }
                    break;
            }

            if(Writer.ShouldLog(entry))
                Writer.Write(entry);
        }

        /// <summary>
        /// Determines whether, in the current configuration, a log entry with the given categories should be logged.
        /// </summary>
        /// <param name="categories">The list of categories to check for.</param>
        /// <returns><code>true</code> if a log entry with the given categories should be logged, <code>false</code> otherwise.</returns>
        public override bool ShouldLog(params string[] categories)
        {
            var testEntry = new LogEntry();
            testEntry.Categories = categories.ToArray();
            return Writer.ShouldLog(testEntry);
        }
    }
}
