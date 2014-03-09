using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Logging;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business
{
    /// <summary>
    /// Represents implementation of business context
    /// </summary>
    public class Context : IContext
    {
        private readonly ISessionManager sessionManager;
        private readonly ITraceManager tracer;
        private readonly ILogger logger;
        public Context(ISessionManager sessionManager, ILogger logger, ITraceManager tracer)
        {
            this.sessionManager = sessionManager;
            this.tracer = tracer;
            this.logger = logger;
        }

        public ISessionManager SessionManager
        {
            get
            {
                return sessionManager;
            }
        }

        /// <summary>
        /// Initializes session in current context
        /// </summary>
        /// <param name="userName">User name</param>
        public void InitializeSession(string userName)
        {
            var domain = "bfwproducts";
            var userId = "187793";
            string domainUserName = string.Format("{0}/{1}", domain, userName);
            var session = SessionManager.ResumeSession(domainUserName, userId, null);
            if (session == null)
            {
                logger.Log("Could not resume session, starting new session", LogSeverity.Debug);
                session = sessionManager.StartNewSession(domainUserName, ConfigurationHelper.GetBrainhoneyDefaultPassword(), true, userId);
            }
            session.UserId = userId;
            sessionManager.CurrentSession = session;
        }
    }
}
