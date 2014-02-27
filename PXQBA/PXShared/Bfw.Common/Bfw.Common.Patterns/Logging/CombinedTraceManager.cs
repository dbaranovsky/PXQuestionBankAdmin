using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Logging;

using StackExchange.Profiling;


namespace Bfw.Common.Patterns.Logging
{
    /// <summary>
    /// Manages start and stop of both LogTracer and MvcMiniProfiler
    /// </summary>
    public class CombinedTraceManager : ITraceManager
    {
        private LogTraceManager LogTracer { get; set; }

        /// <summary>
        /// Constructor to set ILogger
        /// </summary>
        /// <param name="logger">an object that implements ILogger</param>
        public CombinedTraceManager(ILogger logger)
        {
            LogTracer = new LogTraceManager(logger);
        }

        #region ITraceManager Members

        /// <summary>
        /// starts tracing of LogTracer and MvcMiniProfiler
        /// </summary>
        public void StartTracing()
        {
            LogTracer.StartTracing();
            MiniProfiler.Start();
        }

        /// <summary>
        /// Stops Tracing of LogTracer and MvcMiniProfiler
        /// </summary>
        public void EndTracing()
        {
            LogTracer.EndTracing();
            MiniProfiler.Stop();
        }

        /// <summary>
        /// Returns combined object for LogTracer and MvcMiniProfiler
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ITraceHandle StartTrace(string message)
        {
            var miniTrace = new MvcMiniProfilerTraceHandle(MiniProfiler.Current.Step(message));
            var logTrace = LogTracer.StartTrace(message);

            return new CombinedTraceHandle(miniTrace, logTrace);
        }

        #endregion
    }
}
