using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.HttpModules.ResourceCompression
{
    /// <summary>
    /// An error or warning encountered while processing a script.
    /// </summary>
    public class ScriptError
    {
        /// <summary>
        /// Error/Warning message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Line the Error/Warning occured on.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Character position the Error/Warning starts.
        /// </summary>
        public int Character { get; set; }

        /// <summary>
        /// Source code to add context to the message.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Whether the message is for an error or warning.
        /// </summary>
        public ErrorType Type { get; set; }
    }

    /// <summary>
    /// Type of error encountered while processing a script.
    /// </summary>
    public enum ErrorType
    {
        Error,
        Warning
    }
}
