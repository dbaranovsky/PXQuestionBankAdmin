using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Agilix.Dlap.Components.Session
{
    public class EnvironmentSessionManager : ThreadSessionManager
    {
        public string Environment { get; protected set; }

        /// <summary>
        /// Configuration section that stores all of the DLAP connection information.
        /// </summary>
        private Dlap.Configuration.SessionManagerSection configuration = null;

        /// <summary>
        /// Configuration section that stores all of the DLAP connection information.
        /// </summary>
        /// <value>Set <see cref="configuration"/></value>
        protected Dlap.Configuration.SessionManagerSection Configuration
        {
            get
            {
                if (this.configuration == null)
                {
                    var configName = string.Format("agilixSessionManager{0}", Environment.ToLowerInvariant());
                    this.configuration = ConfigurationManager.GetSection(configName) as Dlap.Configuration.SessionManagerSection;
                }

                return this.configuration;
            }
        }
        
        public EnvironmentSessionManager(string environment, Bfw.Common.Logging.ILogger logger, Bfw.Common.Logging.ITraceManager tracer)
            : base(logger, tracer)
        {
            Environment = environment;
        }

        public override Dlap.Session.ISession StartNewSession(string username, string password, bool loginToBrainHoney, string userId)
        {
            EnvironmentSession session = null;
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

            session = new EnvironmentSession(Environment, conn, Logger, Tracer);

            return session;
        }

        /// <summary>
        /// Starts a new session as the anonymous user.
        /// </summary>
        /// <returns>
        /// Session initialized with the anonymous user logged in.
        /// </returns>
        public override Dlap.Session.ISession StartAnnonymousSession()
        {
            var conn = ConfigureConnection();
            var response = LoginAnnonymous(conn);
            EnvironmentSession session = null;

            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("Could not establish new session with the DLAP server: {0}", conn.Url));
            }

            session = new EnvironmentSession(Environment, conn, Logger, Tracer);

            return session;
        }

        /// <summary>
        /// Initializes DlapConnection configured based on the information contained in
        /// the agilixSessionManager configSection.
        /// </summary>
        /// <returns>Initialized DlapConnection.</returns>
        protected DlapConnection ConfigureConnection()
        {
            DlapConnection conn = null;

            Logger.Log("Configuring connection", Common.Logging.LogSeverity.Error);
            if (null != Configuration)
            {
                conn = ConnectionFactory.GetDlapConnection(Configuration.Connection.Url);
                conn.Timeout = Configuration.Connection.Timeout;
                conn.UseCompression = Configuration.Connection.Compress;
                conn.UserAgent = Configuration.Connection.Agent;
                conn.TrustHeaderKey = Configuration.Connection.SecretKey;
                conn.Logger = Logger;
                conn.Tracer = Tracer;
            }

            return conn;
        }
    }
}
