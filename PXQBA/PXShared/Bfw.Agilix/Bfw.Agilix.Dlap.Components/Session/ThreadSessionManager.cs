using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Dlap.Components.Session
{
    /// <summary>
    /// Provides a thread-specific implementation of Dlap session management.
    /// </summary>
    public class ThreadSessionManager : SessionManagerBase
    {
        #region Data Members

        /// <summary>
        /// Stores the current session.
        /// </summary>
        [ThreadStatic]
        private ISession _currentSession = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSessionManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="tracer">The tracer.</param>
        public ThreadSessionManager(Bfw.Common.Logging.ILogger logger, Bfw.Common.Logging.ITraceManager tracer)
        {
            Logger = logger;
            Tracer = tracer;
        }

        #endregion

        #region ISessionManager Members

        /// <summary>
        /// The current session is stored in thread local storage.
        /// </summary>
        public override ISession CurrentSession
        {
            get
            {                
                ISession session = null;

                if (null != _currentSession)
                {
                    session = _currentSession;
                }

                return session;
            }
            set
            {
                if (null != value)
                {
                    _currentSession = value;
                }
            }
        }

        /// <summary>
        /// Creates a new session and returns it.
        /// </summary>
        /// <param name="username">User name to authenticate as.</param>
        /// <param name="password">Password for the user.</param>
        /// <param name="loginToBrainHoney">Login to BrainHoney</param>
        /// <param name="userId">User Id</param>
        /// <returns>
        /// New session if connection to DLAP is successful.
        /// </returns>
        /// <exception cref="DlapException">On any error establishing a connection to DLAP.</exception>
        public override ISession StartNewSession(string username, string password, bool loginToBrainHoney, string userId)
        {
            ThreadSession session = null;
            var conn = ConfigureConnection();

            if (!string.IsNullOrEmpty(userId))
            {
                string user = username;

                if (!string.IsNullOrEmpty(userId))
                {
                    user = userId;
                }

                conn.TrustHeaderUsername = user;
            }
            else
            {
                var response = Login(conn, username, password);
                //SetSessionUserId(this, response);
                if (DlapResponseCode.OK != response.Code)
                {
                    var msg = string.Format("Could not establish new session with the DLAP server: {0}", conn.Url);
                    var ex = new DlapException(msg);

                    Logger.Log(msg, Common.Logging.LogSeverity.Error);
                    throw ex;
                }
            }

            session = new ThreadSession(conn, Logger, Tracer);

            return session;
        }

        /// <summary>
        /// Starts a new session as the anonymous user.
        /// </summary>
        /// <returns>
        /// Session initialized with the anonymous user logged in.
        /// </returns>
        public override ISession StartAnnonymousSession()
        {
            var conn = ConfigureConnection();
            var response = LoginAnnonymous(conn);
            ThreadSession session = null;

            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("Could not establish new session with the DLAP server: {0}", conn.Url));
            }

            session = new ThreadSession(conn, Logger, Tracer);

            return session;
        }

        /// <summary>
        /// Passes the current request's cookies to DLAP to see if a connection can be
        /// reestablished.
        /// </summary>
        /// <returns>A session is the connection was resumed, null otherwise.</returns>
        public override ISession ResumeSession(string username, string userId, TimeZoneInfo timeZoneInfo)
        {
            ISession session = CurrentSession;

            return session;
        }

        #endregion
    }
}
