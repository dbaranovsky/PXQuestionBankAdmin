using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap.Session
{
    /// <summary>
    /// Implementors enable higher level behavior ontop of DlapConnection in order to maintain 
    /// sessions with DLAP.
    /// </summary>
    public interface ISessionManager
    {
        #region Properties

        /// <summary>
        /// Provides access to the current session for the context
        /// </summary>
        ISession CurrentSession { get; set; }

        /// <summary>
        /// Provides access to logging facilities
        /// </summary>
        Bfw.Common.Logging.ILogger Logger { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Logs the user out from DLAP and clears any session information
        /// </summary>
        /// <param name="session">Session to terminate</param>
        void EndSession(ISession session);

        /// <summary>
        /// Attempts to reestablish a session
        /// </summary>
        ISession ResumeSession(string username, string userId, TimeZoneInfo timeZoneInfo);

        /// <summary>
        /// Starts a session as the annonymous user. Null will be returned if the annonymous
        /// user isn't configured properly
        /// </summary>
        /// <returns>Session with annonymous user loged in</returns>
        ISession StartAnnonymousSession();

        /// <summary>
        /// Starts a session as the annonymous user. Null will be returned if the annonymous
        /// user isn't configured properly
        /// </summary>
        /// <returns>Session with annonymous user loged in</returns>
        ISession StartAnnonymousSessionWithOwner(string publicViewOwnerUserId);


        /// <summary>
        /// Establishes a new session with Dlap and makes sure CurrentSession is set
        /// </summary>
        /// <param name="username">user to log in as</param>
        /// <param name="password">password of the user to log in as</param>
        /// <param name="loginToBrainHoney">True if brainhoney login should be performed as part of the session; false otherwise.</param>
        /// <param name="userId">Id of the Agilix user the session is for, if available.</param>
        /// <param name="timeZoneInfo">Time zone of the current user/course</param>
        ISession StartNewSession(string username, string password, bool loginToBrainHoney, string userId, TimeZoneInfo timeZoneInfo);


        /// <summary>
        /// Establishes a new session with Dlap and makes sure CurrentSession is set
        /// </summary>
        /// <param name="username">user to log in as</param>
        /// <param name="password">password of the user to log in as</param>
        /// <param name="loginToBrainHoney">True if brainhoney login should be performed as part of the session; false otherwise.</param>
        /// <param name="userId">Id of the Agilix user the session is for, if available.</param>
        ISession StartNewSession(string username, string password, bool loginToBrainHoney, string userId);

        #endregion
    }
}
