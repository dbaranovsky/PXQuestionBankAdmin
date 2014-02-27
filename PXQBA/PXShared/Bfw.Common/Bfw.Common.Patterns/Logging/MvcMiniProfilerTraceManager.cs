using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Bfw.Common.Logging;

using StackExchange.Profiling;

namespace Bfw.Common.Patterns.Logging
{
    /// <summary>
    ///  MvcMiniProfiler based implementation of ITraceManager
    /// </summary>
    public class MvcMiniProfilerTraceManager : ITraceManager
    {
        #region ITraceManager Members
        /// <summary>
        /// Starts tracing
        /// </summary>
        public void StartTracing()
        {
            MiniProfiler.Start();
        }

        /// <summary>
        /// Stops tracing
        /// </summary>
        public void EndTracing()
        {
            MiniProfiler.Stop();
        }

        /// <summary>
        /// Starts tracing with a custom message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ITraceHandle StartTrace(string message)
        {
            return new MvcMiniProfilerTraceHandle(MiniProfiler.Current.Step(message));
        }

        #endregion
    }
}
