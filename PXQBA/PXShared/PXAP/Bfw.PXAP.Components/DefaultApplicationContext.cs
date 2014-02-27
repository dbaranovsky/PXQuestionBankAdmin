using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Logging;

namespace Bfw.PXAP.Components
{
    /// <summary>
    /// Default implmentation requires ILogger and ITraceManager to be injected.
    /// </summary>
    public class DefaultApplicationContext : IApplicationContext
    {
        #region Constructor

        /// <summary>
        /// Initializes object's state.
        /// </summary>
        /// <param name="logger">ILogger implementation to use.</param>
        /// <param name="tracer">ITraceManager implementation to use.</param>
        public DefaultApplicationContext(ILogger logger, ITraceManager tracer)
        {
            Logger = logger;
            Tracer = tracer;
        }

        #endregion

        #region IApplicationContext Members

        /// <summary>
        /// Keeps track of the current Environment
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// ILogger implementation.
        /// </summary>
        public Common.Logging.ILogger Logger { get; protected set; }

        /// <summary>
        /// ITraceManager implementation.
        /// </summary>
        public Common.Logging.ITraceManager Tracer { get; protected set; }

        #endregion
    }
}
