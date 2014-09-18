using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Collections.Generic;

using Bfw.Common;

using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap.Configuration;

namespace Bfw.Agilix.Dlap.Components.Session
{
    /// <summary>
    /// Provides a Web specific implementation of Dlap session management.
    /// </summary>
    public class WebSessionManager : SessionManagerBase
    {
        /// <summary>
        /// Key for session context where DLAP session ID is kept.
        /// </summary>
        private const string CONTEXT_SESSION_KEY = "WebSessionManager_Agilix_Dlap_Session";

        /// <summary>
        /// Key for session context where DLAP session ID is kept.
        /// </summary>
        private const string AUTH_COOKIE_NAME = "WebSessionManager_Agilix_Auth";

        /// <summary>
        /// The name of the BrainHoney cookie.
        /// </summary>
        private readonly string BRAIN_HONEY_COOKIE_NAME;

        /// <summary>
        /// The name of the DLAP cookie.
        /// </summary>
        private readonly string DLAP_COOKIE_NAME;

        /// <summary>
        /// Name of the cookie that contains the active brainhoney domain.
        /// </summary>
        private readonly string BRAIN_HONEY_ACTIVE_DOMAIN_COOKIE_NAME;

        /// <summary>
        /// Name of the cookie that tells brainhoney components whether the browser has been verified
        /// </summary>
        private readonly string BRAIN_HONEY_BROWSER_CHECK_COOKIE = "BHBrowserCheck";

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSessionManager"/> class.  Sets the BrainHoney and DLAP cookie names.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="tracer">The tracer.</param>
        public WebSessionManager(Bfw.Common.Logging.ILogger logger, Bfw.Common.Logging.ITraceManager tracer)
        {
            Logger = logger;
            Tracer = tracer;
            BRAIN_HONEY_COOKIE_NAME = Configuration.BrainHoneyConnection.CookieName;
            BRAIN_HONEY_ACTIVE_DOMAIN_COOKIE_NAME = Configuration.BrainHoneyConnection.ActiveDomainCookieName;
            DLAP_COOKIE_NAME = Configuration.Connection.CookieName;
        }

        #endregion

        #region ISessionManager Members

        /// <summary>
        /// The current session is stored in the current HttpContext's Items collection.
        /// </summary>
        public override ISession CurrentSession
        {
            get
            {
                var context = HttpContext.Current;
                ISession session = null;

                if (null != context && context.Items.Contains(CONTEXT_SESSION_KEY))
                {
                    session = context.Items[CONTEXT_SESSION_KEY] as ISession;
                }

                return session;
            }
            set
            {
                if (null != value)
                {
                    var context = HttpContext.Current;
                    if (null != context)
                    {
                        context.Items[CONTEXT_SESSION_KEY] = value;
                    }
                }
            }
        }


        /// <summary>
        /// Creates a new WebSession and returns it.
        /// </summary>
        /// <param name="username">User name to authenticate as.</param>
        /// <param name="password">Password for the user.</param>
        /// <param name="loginToBrainHoney">Login to BrainHoney</param>
        /// <param name="userId">User Id</param>
        /// <param name="timeZoneInfo">Time Zone info of the course or user</param>
        /// <returns>
        /// New WebSession if connection to DLAP is successful.
        /// </returns>
        /// <exception cref="DlapException">On any error establishing a connection to DLAP.</exception>
        public override ISession StartNewSession(string username, string password, bool loginToBrainHoney, string userId)
        {
            return StartNewSession(username, password, loginToBrainHoney, userId, null);
        }

        /// <summary>
        /// Creates a new WebSession and returns it.
        /// </summary>
        /// <param name="username">User name to authenticate as.</param>
        /// <param name="password">Password for the user.</param>
        /// <param name="loginToBrainHoney">Login to BrainHoney</param>
        /// <param name="userId">User Id</param>
        /// <param name="timeZoneInfo">Time Zone info of the course or user</param>
        /// <returns>
        /// New WebSession if connection to DLAP is successful.
        /// </returns>
        /// <exception cref="DlapException">On any error establishing a connection to DLAP.</exception>
        public override ISession StartNewSession(string username, string password, bool loginToBrainHoney, string userId, TimeZoneInfo timeZoneInfo)
        {
            var conn = ConfigureConnection();

            WebSession session = null;

            using (Tracer.StartTrace(String.Format("SessionManager DLAP login with username {0}", username)))
            {
                string user = username;

                if (!string.IsNullOrEmpty(userId))
                {
                    user = userId;
                }

                conn.TrustHeaderUsername = user;
            }

            var context = HttpContext.Current;

            if (null != context)
            {
                if (loginToBrainHoney && context.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                {
                    StartBrainHoneySession(username, password, timeZoneInfo);
                }
                TransferCookies(conn, context.Response);
            }


            session = new WebSession(conn, Logger, Tracer);
            session.AllowAsync = Configuration.Connection.AllowAsync;

            return session;
        }

        /// <summary>
        /// Transfers cookie values from an <see cref="HttpRequest"/> to a <see cref="DlapConnection"/>.
        /// </summary>
        /// <param name="from">The request to copy cookie values from.</param>
        /// <param name="to">The DLAP connection object the values are to be copied into.</param>
        private void TransferCookies(HttpRequest from, DlapConnection to)
        {
            if (from != null)
            {
                if (to.CookieContainer == null)
                {
                    to.CookieContainer = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
                }

                var uri = new Uri(to.Url);
                if (from.Cookies.AllKeys.Contains(DLAP_COOKIE_NAME))
                {
                    to.CookieContainer.Add(uri, new Cookie(DLAP_COOKIE_NAME, from.Cookies[DLAP_COOKIE_NAME].Value));
                }
            }
        }

        /// <summary>
        /// Transfers values from a <see cref="DlapConnection"/> to the cookie container of the HtmlResponse/>.
        /// </summary>
        /// <param name="from">The DLAP connection fro copy values from.</param>
        /// <param name="to">The response, into whose cookie container the values will be copied.</param>
        private void TransferCookies(DlapConnection from, HttpResponse to)
        {
            if (to != null && from.CookieContainer != null)
            {
                var uri = new Uri(from.Url);
                var cookies = from.CookieContainer.GetCookies(uri);
                if (cookies[DLAP_COOKIE_NAME] != null)
                {
                    to.Cookies.Add(new HttpCookie(DLAP_COOKIE_NAME, cookies[DLAP_COOKIE_NAME].Value));
                }
            }
        }

        /// <summary>
        /// Starts a new session as the anonymous user.
        /// </summary>
        /// <returns>
        /// Session initialized with the anonymous user logged in.
        /// </returns>
        public override ISession StartAnnonymousSession()
        {
            var context = HttpContext.Current;
            var config = ConfigurationManager.GetSection("agilixSessionManager") as SessionManagerSection;

            var conn = ConfigureConnection();
            TransferCookies(HttpContext.Current.Request, conn);
            conn.TrustHeaderUsername = Configuration.AnnonymousUser.Id;

            WebSession session = new WebSession(conn, Logger, Tracer);
            session.AllowAsync = Configuration.Connection.AllowAsync;
            //if (null != context)
            //{
            //    if (context.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            //    {
            //        StartBrainHoneySession(Configuration.AnnonymousUser.Username, Configuration.AnnonymousUser.Password);
            //    }
            //}

            return session;
        }

        /// <summary>
        /// Starts a new session as the anonymous user.
        /// </summary>
        /// <returns>
        /// Session initialized with the anonymous user logged in.
        /// </returns>
        public override ISession StartAnnonymousSessionWithOwner(string publicViewOwnerUserId)
        {
            var context = HttpContext.Current;
            var config = ConfigurationManager.GetSection("agilixSessionManager") as SessionManagerSection;

            var conn = ConfigureConnection();
            TransferCookies(HttpContext.Current.Request, conn);
            conn.TrustHeaderUsername = Configuration.AnnonymousUser.Id;
            if (!string.IsNullOrEmpty(publicViewOwnerUserId))
            {
                conn.TrustHeaderUsername = publicViewOwnerUserId;
            }

            WebSession session = new WebSession(conn, Logger, Tracer);
            session.AllowAsync = Configuration.Connection.AllowAsync;
            //if (null != context)
            //{
            //    if (context.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            //    {
            //        StartBrainHoneySession(Configuration.AnnonymousUser.Username, Configuration.AnnonymousUser.Password);
            //    }
            //}

            return session;
        }


        /// <summary>
        /// Passes the current request's cookies to DLAP to see if a connection can be
        /// reestablished
        /// </summary>
        /// <returns>A session is the connection was resumed, null otherwise</returns>
        public override ISession ResumeSession(string username, string userId, TimeZoneInfo timeZoneInfo)
        {
            WebSession session = null;

            if (null != CurrentSession)
            {
                return CurrentSession;
            }

            if (Configuration.BrainHoneyConnection.AllowResumeSession)
            {
                var context = HttpContext.Current;
                var cookie = context.Request.Cookies[BRAIN_HONEY_COOKIE_NAME];
                var domainCookie = context.Request.Cookies[BRAIN_HONEY_ACTIVE_DOMAIN_COOKIE_NAME];
                var usernameParts = username.Split('/');

                if (domainCookie != null && usernameParts.Length > 1)
                {
                    var domainName = usernameParts[0].ToLowerInvariant();
                    var activeDomainName = domainCookie.Value.ToLowerInvariant();

                    if (domainName != activeDomainName)
                    {
                        //even if we have an auth cookie, at this point we know that the auth cookie must 
                        //be invalid since the user has switched between domains.
                        return null;
                    }
                }

                if (null != cookie && null != domainCookie)
                {
                    var conn = ConfigureConnection();
                    conn.TrustHeaderUsername = userId;

                    TransferCookies(conn, context.Response);
                    session = new WebSession(conn, Logger, Tracer);
                    session.AllowAsync = Configuration.Connection.AllowAsync;

                    System.Web.Security.FormsAuthenticationTicket ticket = null;

                    try
                    {
                        ticket = System.Web.Security.FormsAuthentication.Decrypt(cookie.Value);
                    }
                    catch
                    {
                        // not much we can do here, so just reauth the user...
                    }

                    //If ticket has expired, sometimes BH will renew the session with userid 13. In this case ,we renew the session for the user
                    if (ticket == null || ticket.Expired || ticket.Name != userId)
                    {
                        StartBrainHoneySession(username, ConfigurationManager.AppSettings["BrainhoneyDefaultPassword"], timeZoneInfo);
                    }

                    Logger.Log("Agilix Session Resumed", Bfw.Common.Logging.LogSeverity.Debug);
                }
            }

            return session;
        }

        /// <summary>
        /// Ends the session and clears the necessary cookie values so that the connection
        /// can not be restarted.
        /// </summary>
        /// <param name="session"></param>
        public override void EndSession(ISession session)
        {
            var context = HttpContext.Current;

            if (context.Response.Cookies.AllKeys.Contains(AUTH_COOKIE_NAME))
            {
                context.Response.Cookies[AUTH_COOKIE_NAME].Expires = DateTime.Now.AddDays(-1d);
                context.Response.Cookies[AUTH_COOKIE_NAME].Value = string.Empty;
            }

            if (context.Response.Cookies.AllKeys.Contains(BRAIN_HONEY_COOKIE_NAME))
            {
                context.Response.Cookies[BRAIN_HONEY_COOKIE_NAME].Expires = DateTime.Now.AddDays(-1d);
                context.Response.Cookies[BRAIN_HONEY_COOKIE_NAME].Value = string.Empty;
            }
        }

        /// <summary>
        /// Determines whether the current request has the BrainHoney cookie set.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the current request has the BrainHoney cookie set, otherwise, <c>false</c>.
        /// </returns>
        private bool HasBHCookie()
        {
            using (Tracer.StartTrace("Checking for BrainHoney Cookie"))
            {
                var cookie = HttpContext.Current.Request.Cookies.Get(BRAIN_HONEY_COOKIE_NAME);

                var hasCookie = (cookie != null && !string.IsNullOrEmpty(cookie.Value));
                if (hasCookie)
                {
                    Logger.Log("Already have BrainHoney cookie", Bfw.Common.Logging.LogSeverity.Debug);
                }
                else
                {
                    Logger.Log("No BrainHoney cookie found", Bfw.Common.Logging.LogSeverity.Debug);
                }
                return hasCookie;
            }
        }

        /// <summary>
        /// Create a new BrainHoney session cookie.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public void StartBrainHoneySession(string username, string password, TimeZoneInfo timeZoneInfo)
        {
            using (Tracer.StartTrace("StartBrainHoneySession"))
            {
                try
                {
                    var config = ConfigurationManager.GetSection("agilixSessionManager") as Configuration.SessionManagerSection;
                    var brainHoneyAuthUrl = config.BrainHoneyConnection.Url;
                    var userDomain = config.BrainHoneyConnection.UserDomain;
                    var domain = config.BrainHoneyConnection.CookieDomain;
                    var cookieJar = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
                   
                    using (Tracer.StartTrace("Performing BH Connection"))
                    {
                        if (!username.Contains("/"))
                        {
                            username = userDomain + "/" + username;
                        }
                        else
                        {
                            var parts = username.Split('/');
                            userDomain = parts[0];
                        }

                        var uri = brainHoneyAuthUrl.Replace("{1}", userDomain);

                        Logger.Log("Using BrainHoney URL: " + uri, Bfw.Common.Logging.LogSeverity.Debug);
                        var bhUri = new Uri(uri);
                        var server = HttpContext.Current.Server;
                        

                        var requestData = "action=login&username=" + server.UrlEncode(username) + "&password=" + server.UrlEncode(password);
                        if (timeZoneInfo != null && timeZoneInfo.GetAdjustment(DateTime.Now.Year) != null)
                        {
                            var adjustment = timeZoneInfo.GetAdjustment(DateTime.Now.Year);

                            requestData += "&standardOffset=" + -1 * timeZoneInfo.BaseUtcOffset.TotalMinutes +
                                           "&daylightOffset=" +
                                           -1 * (adjustment.DaylightDelta.TotalMinutes +
                                            timeZoneInfo.BaseUtcOffset.TotalMinutes) +
                                           "&standardStartTime=" +
                                           server.UrlEncode(adjustment.DaylightTransitionEnd
                                                     .GetTransitionInfo(DateTime.Now.Year)
                                                     .ToUniversalTime()
                                                     .ToString("s") + "Z") +
                                           "&daylightStartTime=" +
                                           server.UrlEncode(adjustment.DaylightTransitionStart
                                                     .GetTransitionInfo(DateTime.Now.Year)
                                                     .ToUniversalTime()
                                                     .ToString("s") + "Z");

                        }
                        else
                        {
                            timeZoneInfo = TimeZoneInfo.Local;
                            requestData += "&standardOffset=" + -1 * timeZoneInfo.BaseUtcOffset.TotalMinutes +
                                           "&daylightOffset=" + timeZoneInfo.BaseUtcOffset.TotalMinutes;
                        }
                        
                        cookieJar.Add(new Cookie("BHBrowserCheck", "1", "/", bhUri.Host));

                        var webRequest = (HttpWebRequest)WebRequest.Create(bhUri);

                        webRequest.CookieContainer = cookieJar;
                        webRequest.Method = "POST";
                        webRequest.Accept = "*/*";
                        webRequest.ContentLength = requestData.Length;
                        webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

                        var byteData = Encoding.UTF8.GetBytes(requestData);
                        using (var postStream = webRequest.GetRequestStream())
                        {
                            postStream.Write(byteData, 0, requestData.Length);
                        }

                        var response = webRequest.GetResponse();
                        foreach (Cookie c in cookieJar.GetCookies(webRequest.RequestUri))
                        {
                            if (c.Name.ToLowerInvariant() == BRAIN_HONEY_COOKIE_NAME.ToLowerInvariant() ||
                                c.Name.ToLowerInvariant() == BRAIN_HONEY_BROWSER_CHECK_COOKIE.ToLowerInvariant())
                            {
                                var cookie = new HttpCookie(c.Name)
                                {
                                    Path = c.Path,
                                    Value = c.Value,
                                    HttpOnly = c.HttpOnly,
                                    Secure = c.Secure,
                                    Expires = c.Expires
                                };

                                var activeDomainCookie = new HttpCookie(BRAIN_HONEY_ACTIVE_DOMAIN_COOKIE_NAME)
                                {
                                    Path = c.Path,
                                    Value = userDomain,
                                    HttpOnly = c.HttpOnly,
                                    Secure = c.Secure,
                                    Expires = c.Expires
                                };

                                if (!string.IsNullOrEmpty(domain))
                                {
                                    cookie.Domain = domain;
                                    activeDomainCookie.Domain = domain;
                                }

                                HttpContext.Current.Response.Cookies.Add(cookie);
                                HttpContext.Current.Response.Cookies.Add(activeDomainCookie);
                            }
                        }

                        response.Close();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, Bfw.Common.Logging.LogSeverity.Warning);
                }
            }
        }

        /// <summary>
        /// Pings BrainHoney, keeping the BrainHoney cookie alive.
        /// </summary>
        /// <returns>The BrainHoney cookie.</returns>
        private string PingBrainHoney()
        {
            var cookieValue = string.Empty;

            using (Tracer.StartTrace("PingBrainHoney"))
            {
                var currentCookieValue = string.Empty;
                var pingUri = new Uri(string.Format("{0}/ping", Configuration.BrainHoneyConnection.BaseUrl));

                if (HttpContext.Current.Request.Cookies.AllKeys.Contains(Configuration.BrainHoneyConnection.CookieName))
                {
                    currentCookieValue = HttpContext.Current.Request.Cookies[Configuration.BrainHoneyConnection.CookieName].Value;
                }

                var webRequest = (HttpWebRequest)WebRequest.Create(pingUri);
                webRequest.CookieContainer = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
                webRequest.Method = "GET";

                if (!string.IsNullOrEmpty(currentCookieValue))
                {
                    webRequest.CookieContainer.Add(pingUri, new Cookie(Configuration.BrainHoneyConnection.CookieName, currentCookieValue));
                }

                try
                {
                    var response = webRequest.GetResponse() as HttpWebResponse;

                    using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        var json = sr.ReadToEnd();
                        if (json.Contains("true"))
                        {
                            var cookie = response.Cookies[Configuration.BrainHoneyConnection.CookieName];
                            cookieValue = cookie.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, Bfw.Common.Logging.LogSeverity.Error);
                }
            }

            return cookieValue;
        }

        #endregion
    }
}