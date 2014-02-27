using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Dlap.Components.Session
{
    /// <summary>
    /// Provides a thread-specific implementation of ISession.
    /// </summary>
    internal class ThreadSession : SessionBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSession"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="tracer">The tracer.</param>
        public ThreadSession(DlapConnection connection, Bfw.Common.Logging.ILogger logger, Bfw.Common.Logging.ITraceManager tracer)
        {
            Connection = connection;
            Logger = logger;
            Tracer = tracer;
        }

        #endregion
    }
}
