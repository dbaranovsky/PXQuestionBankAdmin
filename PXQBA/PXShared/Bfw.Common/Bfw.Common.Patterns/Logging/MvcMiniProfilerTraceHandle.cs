using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Logging;

using StackExchange.Profiling;

namespace Bfw.Common.Patterns.Logging
{
    /// <summary>
    /// MvcMiniProfiler based implementation of ITraceHandle
    /// </summary>
    public class MvcMiniProfilerTraceHandle : ITraceHandle
    {
        private IDisposable Trace { get; set; }

        /// <summary>
        /// Constructor to set IDisposable
        /// </summary>
        /// <param name="trace">an object that implements IDisposable</param>
        public MvcMiniProfilerTraceHandle(IDisposable trace)
        {
            Trace = trace;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes any resources occupied by the object
        /// </summary>
        public void Dispose()
        {
            if(Trace != null)
                Trace.Dispose();
        }

        #endregion
    }
}
