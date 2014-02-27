using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Logging
{
    /// <summary>
    /// Represents a message that can be logged by an ILogger.
    /// </summary>
    public class LogMessage
    {
        #region Properties

        /// <summary>
        /// Indicates how severe the message is.
        /// </summary>
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Message to log.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// List of categories the message should appear in.
        /// </summary>
        public IList<string> Categories { get; set; }

        /// <summary>
        /// Name of the application that created the message.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Name of the application component that created the message.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Any additional data the should be placed into the message. Extra information will be
        /// converted to string form via the ToString method. Null entries will be ignored.
        /// </summary>
        public IDictionary<string, object> ExtraInfo { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        public LogMessage()
        {
            Categories = new List<string>();
            ExtraInfo = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Severity implied by the message being logged.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        /// Equivalent to Information.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Indicates the message is about an error that occured.
        /// </summary>
        Error = 4,
        /// <summary>
        /// Indicates the message is about a warning about a possible error.
        /// </summary>
        Warning = 3,
        /// <summary>
        /// Indicates the message is intended to supply information.
        /// </summary>
        Information = 2,
        /// <summary>
        /// Indicates the message is only debug information.
        /// </summary>
        Debug = 1
    }
}
