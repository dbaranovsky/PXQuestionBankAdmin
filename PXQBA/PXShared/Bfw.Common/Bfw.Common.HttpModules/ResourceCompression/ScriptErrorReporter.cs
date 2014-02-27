using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.HttpModules.ResourceCompression
{
    /// <summary>
    /// Logs details about any error or warnings encountered while processing a script.
    /// </summary>
    public class ScriptErrorReporter : EcmaScript.NET.ErrorReporter
    {
        /// <summary>
        /// List of error encountered.
        /// </summary>
        public IList<ScriptError> Errors { get; protected set; }

        /// <summary>
        /// Initializes the object state.
        /// </summary>
        public ScriptErrorReporter()
        {
            Errors = new List<ScriptError>();
        }

        /// <summary>
        /// Logs an Error.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="sourceName">Name of the source file the error is from.</param>
        /// <param name="line">Line number the error occured on.</param>
        /// <param name="lineSource">Source code of the line with the error.</param>
        /// <param name="lineOffset">Character position of the error on the line.</param>
        public void Error(string message, string sourceName, int line, string lineSource, int lineOffset)
        {
            Errors.Add(new ScriptError()
            {
                Message = message,
                Line = line,
                Character = lineOffset,
                Source = lineSource,
                Type = ErrorType.Error
            });
        }

        /// <summary>
        /// Logs a runtime error.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="sourceName">Name of the source file the error is from.</param>
        /// <param name="line">Line number the error occured on.</param>
        /// <param name="lineSource">Source code of the line with the error.</param>
        /// <param name="lineOffset">Character position of the error on the line.</param>
        /// <returns>Exception object that can be thrown.</returns>
        public EcmaScript.NET.EcmaScriptRuntimeException RuntimeError(string message, string sourceName, int line, string lineSource, int lineOffset)
        {
            Errors.Add(new ScriptError()
            {
                Message = message,
                Line = line,
                Character = lineOffset,
                Source = lineSource,
                Type = ErrorType.Error
            });

            return new EcmaScript.NET.EcmaScriptRuntimeException(message, sourceName, line, lineSource, lineOffset);
        }

        /// <summary>
        /// Logs a warning against the script.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="sourceName">Name of the source file the error is from.</param>
        /// <param name="line">Line number the error occured on.</param>
        /// <param name="lineSource">Source code of the line with the error.</param>
        /// <param name="lineOffset">Character position of the error on the line.</param>
        public void Warning(string message, string sourceName, int line, string lineSource, int lineOffset)
        {
            Errors.Add(new ScriptError()
            {
                Message = message,
                Line = line,
                Character = lineOffset,
                Source = lineSource,
                Type = ErrorType.Warning
            });
        }
    }
}
