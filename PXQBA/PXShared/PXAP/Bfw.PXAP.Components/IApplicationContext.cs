using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Logging;

namespace Bfw.PXAP.Components
{
    /// <summary>
    /// Interface that provides access to all cross-cutting concerns, e.g. logging.
    /// </summary>
    public interface IApplicationContext
    {
        /// <summary>
        /// Keeps track of the current Environment
        /// </summary>
        string Environment { get; set; }

        /// <summary>
        /// Access to the application logging system.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Access to the application tracing system.
        /// </summary>
        ITraceManager Tracer { get; }


    }
}
