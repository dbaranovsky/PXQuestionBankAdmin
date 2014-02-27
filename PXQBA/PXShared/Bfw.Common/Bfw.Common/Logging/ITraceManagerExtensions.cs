using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Logging
{
    /// <summary>
    /// Extension methods to handle various cases that the ITraceManager interface can't on it's own.
    /// </summary>
    public static class ITraceManagerExtensions
    {
        /// <summary>
        /// Calls ITraceManager.StartTrace if tracer is not null. If tracer is null, then an
        /// instance of NoOpTraceHandle is returned instead. This allows for easier coding 
        /// in situations where the ambient ITraceManager may be null.
        /// </summary>
        /// <param name="tracer">ITraceManager instance to call StartTrace on.</param>
        /// <param name="message">Message to pass to StartTrace. Message follows string.Format syntax</param>
        /// <param name="args">Extra Arguments</param>
        /// <returns>ITraceHandle from StartTrace, or a NoOpTraceHandle</returns>
        public static ITraceHandle DoTrace(this ITraceManager tracer, string message, params object[] args)
        {
            ITraceHandle handle = null;

            try
            {
                if (tracer != null)
                {
                    handle = tracer.StartTrace(string.Format(message, args));
                }
                else
                {
                    handle = new NoOpTraceHandle();
                }
            }
            catch
            {
                if (handle != null)
                {
                    handle.Dispose();
                }

                throw;
            }

            return handle;
        }
    }
}
