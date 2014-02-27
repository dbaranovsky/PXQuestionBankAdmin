using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Logging
{
    /// <summary>
    /// Abstracts the process of managing performance tracing in the application.
    /// </summary>
    public interface ITraceManager
    {
        /// <summary>
        /// Perform necessary initialization to start tracing.
        /// </summary>
        void StartTracing();

        /// <summary>
        /// Performs any necessary cleanup to turn off tracing.
        /// </summary>
        void EndTracing();

        /// <summary>
        /// Starts a new performance trace which should immediately start accruing time.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        ITraceHandle StartTrace(string message);
    }
}
