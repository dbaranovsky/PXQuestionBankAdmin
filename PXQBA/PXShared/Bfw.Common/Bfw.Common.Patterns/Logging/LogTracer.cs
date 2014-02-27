using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Bfw.Common.Logging;

namespace Bfw.Common.Patterns.Logging
{
    /// <summary>
    /// Starts a timer when instantiated and stops it when disposed
    /// </summary>
    public class LogTraceHandle : ITraceHandle
    {
        #region Properties

        private bool _disposed2 = false;

        /// <summary>
        /// Used to time operation
        /// </summary>
        protected Stopwatch Timer { get; set; }

        /// <summary>
        /// Message included in output to identify the trace
        /// </summary>
        protected string Message { get; set; }

        /// <summary>
        /// trace manager that created the trace
        /// </summary>
        protected LogTraceManager TraceManager { get; set; }

        /// <summary>
        /// Property to access the order of the tracer
        /// </summary>
        public int Order { get; protected set; }

        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="traceManager"></param>
        /// <param name="message"></param>
        /// <param name="order"></param>
        public LogTraceHandle(LogTraceManager traceManager, string message, int order)
        {
            Message = message;
            TraceManager = traceManager;
            Order = order;

            Timer = Stopwatch.StartNew();
        }

        #region IDisposable Members

        /// <summary>
        /// Alert the trace manager that we are done
        /// </summary>
        public void Dispose()
        {
            if (!_disposed2)
            {
                Timer.Stop();
                TraceManager.TraceCompleted();
                _disposed2 = true;
            }
        }

        #endregion

        /// <summary>
        /// Output the message and the elapsed time
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} => {1}h:{2}m:{3}s:{4}ms", Message, Timer.Elapsed.Hours, Timer.Elapsed.Minutes, Timer.Elapsed.Seconds, Timer.Elapsed.Milliseconds);
        }
    }

    /// <summary>
    /// Manages creating and outputing traces
    /// </summary>
    public class LogTraceManager : ITraceManager
    {
        #region Properties

        /// <summary>
        /// Logger to write trace messages to
        /// </summary>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// Set of traces created so far
        /// </summary>
        protected Stack<LogTraceHandle> Traces { get; set; }

        /// <summary>
        /// List of completed traces
        /// </summary>
        protected IList<Timing> CompletedTraces { get; set; }

        private string _indent = "    ";

        /// <summary>
        /// Property to access the order of the tracer
        /// </summary>
        protected int Order { get; set; }

        /// <summary>
        /// True if StartTracing has been called.
        /// </summary>
        private bool _tracing = false;

        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public LogTraceManager(ILogger logger)
        {
            Logger = logger;
            Order = 0;
            Traces = new Stack<LogTraceHandle>();
            CompletedTraces = new List<Timing>();
        }

        #region Methods

        /// <summary>
        /// Stops the trace and adds it to the list of completed traces
        /// </summary>
        public void TraceCompleted()
        {
            if (Traces.Count > 0)
            {
                var trace = Traces.Pop();
                CompletedTraces.Add(new Timing()
                {
                    Message = string.Format("{0}{1}", CurrentIndent(), trace.ToString()),
                    Order = trace.Order
                });
            }
        }

        /// <summary>
        /// Returns the correct indentation string depending on stack depth
        /// </summary>
        /// <returns></returns>
        private string CurrentIndent()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < Traces.Count; ++i)
            {
                sb.Append(_indent);
            }

            return sb.ToString();
        }

        #endregion

        #region ITraceManager Members

        /// <summary>
        /// Start tracing
        /// </summary>
        public void StartTracing()
        {
            _tracing = true;
        }

        /// <summary>
        /// Stop tracing
        /// </summary>
        public void EndTracing()
        {
            if (Traces != null)
            {
                while (Traces.Count > 0)
                {
                    var trace = Traces.Peek();
                    trace.Dispose();
                }
            }

            if (CompletedTraces.Count > 0 && _tracing)
            {
                var completed = CompletedTraces.OrderBy(c => c.Order);
                var sb = new StringBuilder();
                sb.AppendLine("PerfTrace");
                foreach (var c in completed)
                    sb.AppendLine(c.Message);

                Logger.Log(sb.ToString(), LogSeverity.Debug);
            }

            _tracing = false;
        }

        /// <summary>
        /// Start trace with the given message
        /// </summary>
        /// <param name="message">string that should be used when strating a trace</param>
        /// <returns></returns>
        public ITraceHandle StartTrace(string message)
        {
            var trace = new LogTraceHandle(this, message, Order++);

            if (_tracing)
            {
                Traces.Push(trace);
            }

            return trace;
        }

        #endregion
    }

    /// <summary>
    /// Class represeting Message and Order of trace
    /// </summary>
    public class Timing
    {
        /// <summary>
        /// Message of trace
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Order of trace
        /// </summary>
        public int Order { get; set; }
    }
}
