using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Dlap.Components.Session
{
    /// <summary>
    /// Provides a web app specific implementation of ISession.
    /// </summary>
    internal class WebSession : SessionBase
    {
        /// <summary>
        /// This seems to be unused.
        /// </summary>
        [Obsolete("This constant does not seem to be used anywhere.")]
        private const string CONTEXT_USER_ID_KEY = "WebSession_User_Id";

        #region Constructors

        /// <summary>
        /// Retains the given DlapConnection as the Connection property.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="tracer">The tracer.</param>
        public WebSession(DlapConnection connection, Bfw.Common.Logging.ILogger logger, Bfw.Common.Logging.ITraceManager tracer)
        {
            Connection = connection;
            base.UserId = string.Empty;
            Logger = logger;
            Tracer = tracer;
        }

        #endregion
    }
}
