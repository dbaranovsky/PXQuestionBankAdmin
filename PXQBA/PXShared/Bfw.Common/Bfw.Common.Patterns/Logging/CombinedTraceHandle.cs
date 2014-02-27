using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Logging;

namespace Bfw.Common.Patterns.Logging
{
    /// <summary>
    /// Combines MvcMiniTrace and LogTrace (ITraceHandles) in one object
    /// </summary>
    public class CombinedTraceHandle : ITraceHandle
    {
        #region Properties

        private ITraceHandle MvcMiniTrace { get; set; }

        private ITraceHandle LogTrace { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mvcMiniTrace">an object that implements ITraceHandle</param>
        /// <param name="logTrace">an object that implements ITraceHandle</param>
        public CombinedTraceHandle(ITraceHandle mvcMiniTrace, ITraceHandle logTrace)
        {
            MvcMiniTrace = mvcMiniTrace;
            LogTrace = logTrace;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes any resources occupied by the object
        /// </summary>
        public void Dispose()
        {
            if (MvcMiniTrace != null)
            {
                MvcMiniTrace.Dispose();
            }

            if (LogTrace != null)
            {
                LogTrace.Dispose();
            }
        }

        #endregion
    }
}
