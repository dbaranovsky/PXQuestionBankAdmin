using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Bfw.Common.Patterns.Logging
{
    /// <summary>
    /// EnterpriseLibrary based implementation of ITraceManager
    /// </summary>
    public class EnterpriseLibraryTraceManager : ITraceManager
    {
        #region Properties

        /// <summary>
        /// TraceManager used to provide tracing functionality
        /// </summary>
        protected TraceManager TraceMgr { get; set; }

        #endregion

        /// <summary>
        /// Constructor to set TraceManager
        /// </summary>
        /// <param name="mgr">TraceManager to be used</param>
        public EnterpriseLibraryTraceManager(TraceManager mgr)
        {
            TraceMgr = mgr;
        }

        #region ITraceManager Members

        /// <summary>
        /// Perform necessary initialization to start tracing.
        /// </summary>
        public void StartTracing()
        {
        }


        /// <summary>
        /// Performs any necessary cleanup to turn off tracing.
        /// </summary>
        public void EndTracing()
        {
        }

        /// <summary>
        /// Starts a new performance trace which should immediately start accruing time.
        /// </summary>
        /// <param name="message">message to be used with trace</param>
        /// <returns></returns>
        public ITraceHandle StartTrace(string message)
        {
            var handle = new EnterpriseLibraryTracer(TraceMgr.StartTrace(message));

            return handle;
        }

        #endregion
    }
}
