using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using System.Configuration;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Routing;
using Bfw.Common.Logging;
using Bfw.Common.Collections;
using Bfw.Common.SSO;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.Account.Abstract;
using Microsoft.Practices.ServiceLocation;
using Bfw.Common.Caching;

namespace Bfw.PX.PXPub.Components
{
    /// <summary>
    /// Implements the IBusinessContext using information from Ravell headers. See <see cref="BusinessContextBase"/>.
    /// </summary>
    public class RavellBusinessContext : BusinessContextBase
    {
        #region IBusinessContext Members

        /// <summary>
        /// A private varible to hold the product type.
        /// </summary>
        private string _productType = "";

        /// <summary>
        /// Gets or sets the type of the product. Set the private member of _productType.
        /// </summary>
        /// <value>
        /// The type of the product.
        /// </value>
        public override string ProductType
        {
            get { return _productType.ToLowerInvariant(); }
            set { _productType = value; }
        }

        /// <summary>
        /// Initializes the context from request information.
        /// </summary>
        protected override void InitializeFromRequest()
        {
            using (Tracer.StartTrace("BusinessContext InitializeFromRequest"))
            {
                RequestContext.Logger = Logger;
                RequestContext.Tracer = Tracer;

                using (Tracer.StartTrace("RequestContext.Init"))
                {
                    RequestContext.Init();
                }

                using (Tracer.StartTrace("RequestContext.InitSiteData"))
                {
                    RequestContext.InitSiteData();
                }

                SSOData = RequestContext.SSOData;

                ProtectedUrl = ConfigurationManager.AppSettings["SecureBaseUrl"];
                UnprotectedUrl = ConfigurationManager.AppSettings["InsecureBaseUrl"];
                RedirectIfNecessary();
                InitializeDataFromCookies();
            }
        }

        /// <summary>
        /// Dumps the cookies to log.
        /// </summary>
        private void DumpCookies()
        {
            var request = HttpContext.Current.Request;
            var sb = new System.Text.StringBuilder();

            sb.AppendFormat("Cookies for: {0}\n", request.Url);
            for (int i = 0; i < request.Cookies.Count; ++i)
            {
                sb.AppendFormat("{{ name: {0}, domain: {1}, path: {2}, value: {3} }}\n", request.Cookies[i].Name, request.Cookies[i].Domain, request.Cookies[i].Path, HttpUtility.UrlDecode(request.Cookies[i].Value));
            }

            Logger.Log(sb.ToString(), LogSeverity.Debug);
        }

        /// <summary>
        /// Dumps the headers to log.
        /// </summary>
        private void DumpHeaders()
        {
            var request = HttpContext.Current.Request;
            var sb = new System.Text.StringBuilder();

            sb.AppendFormat("Headers for: {0}\n", request.Url);
            for (int i = 0; i < request.Headers.Count; ++i)
            {
                sb.AppendFormat("{0} -> {1}\n", request.Headers.GetKey(i), request.Headers[i]);
            }

            Logger.Log(sb.ToString(), LogSeverity.Debug);
        }

        /// <summary>
        /// Redirects the user if necessary based on a number of cases.
        /// </summary>
        private void RedirectIfNecessary()
        {
            var request = HttpContext.Current.Request;
            var path = request.Path;
            var query = request.Url.Query;
            string url = string.Empty;
            string urlType = string.Empty;

            Logger.Log(string.Format("Path {0} and query {1}", path, query), LogSeverity.Information);
            if (RequestContext.SSOData.SwitchToProtected)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(ProtectedUrl))
                    {
                        // No need to do redirect because we are already on the protected resource.
                        url = string.Empty;
                    }
                    else
                    {
                        url = path.Replace(UnprotectedUrl, ProtectedUrl);
                    }
                }
                else
                {
                    url = ProtectedUrl;
                }
                urlType = "protected";
            }
            else if (RequestContext.SSOData.SwitchToUnprotected)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    url = path.Replace(ProtectedUrl, UnprotectedUrl);
                }
                else
                {
                    url = UnprotectedUrl;
                }
                urlType = "unprotected";
            }

            if (!string.IsNullOrEmpty(url))
            {
                var redirect = string.Format("{0}://{1}{2}{3}", request.Url.Scheme, request.Url.Host, url, string.IsNullOrEmpty(query) ? "" : "?" + query);
                Logger.Log(string.Format("Redirecting to {1} URL: {0}", redirect, urlType), LogSeverity.Debug);

                HttpContext.Current.Response.Redirect(redirect, true);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the request context.
        /// </summary>
        /// <value>
        /// The request context.
        /// </value>
        protected IRequestContext RequestContext { get; set; }

        /// <summary>
        /// Gets or sets the agilix user ID.
        /// </summary>
        /// <value>
        /// The agilix user ID.
        /// </value>
        private string AgilixUserId { get; set; }

        /// <summary>
        /// Gets or sets the protected URL.
        /// </summary>
        /// <value>
        /// The protected URL.
        /// </value>
        protected string ProtectedUrl { get; set; }

        /// <summary>
        /// Gets or sets the unprotected URL.
        /// </summary>
        /// <value>
        /// The unprotected URL.
        /// </value>
        protected string UnprotectedUrl { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RavellBusinessContext"/> class.
        /// </summary>
        /// <param name="sm">The <see cref="ISessionManager"/>.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="requestContext">The request context.</param>
        /// <param name="tracer">The tracer.</param>
        public RavellBusinessContext(ISessionManager sm, ILogger logger, IRequestContext requestContext, ITraceManager tracer, ICacheProvider cacheProvider)
        {
            SessionManager = sm;
            AccessLevel = AccessLevel.None;
            Logger = logger;
            RequestContext = requestContext;
            Tracer = tracer;
            CacheProvider = cacheProvider;
        }

        #endregion

        /// <summary>
        /// returns the Product Course ID of the URL passed in
        /// </summary>
        /// <param name="url">the url of the course ex. -standard-/dashboard/eportfolio/</param>
        /// <returns>Product Master Course Id for URL</returns>
        public override string GetProductCourseId(String course, String url)
        {
            using (Tracer.DoTrace("SamlBusinessContext.GetProductCourseId"))
            {
                var siteInfo = GetSiteInfo(url);

                return siteInfo.AgilixSiteID;
            }
        }

        #region Helper Methods

        /// <summary>
        /// Uses cookies to initialize certain properties of the buisness context.
        /// </summary>
        private void InitializeDataFromCookies()
        {
            using (Tracer.StartTrace("InitializeDataFromCookies"))
            {
                if (RequestContext.SiteInfo != null)
                {
                    SiteID = RequestContext.SiteInfo.SiteID.ToString();
                    URL = RequestContext.SiteInfo.URL;
                    ProductCourseId = RequestContext.SiteInfo.AgilixCourseId.ToString();
                    Logger.Log(string.Format("RequestContext.SiteInfo is not null.  Product course ID is {0}", ProductCourseId), LogSeverity.Debug);
                }
                else
                {
                    Logger.Log("RequestContext.SiteInfo is null", LogSeverity.Debug);
                }

                if (HttpContext.Current.Request.Cookies.AllKeys.Contains("PXAUTHUSER"))
                {
                    AgilixUserId = HttpContext.Current.Request.Cookies["PXAUTHUSER"].Value;
                }
            }
        }

        /// <summary>
        /// Based on the authenticated RA user, setup the User property of the context.
        /// </summary>
        protected override void InitializeUser()
        {
            using (Tracer.StartTrace("BusinessContext InitializeUser"))
            {
                IsAnonymous = String.IsNullOrEmpty(RequestContext.SSOData.UserId);

                if (IsAnonymous || CourseIsProductCourse)
                {
                    using (Tracer.StartTrace("BusinessContext InitializeUser Anonymous Session"))
                    {
                        SessionManager.CurrentSession = SessionManager.StartAnnonymousSession();
                    }
                }
                else
                {
                    Logger.Log("BusinessContext Have RA User", LogSeverity.Debug);
                    string domainReferenceId = string.Format("{0}/{1}", Domain.Id, RequestContext.SSOData.UserId);
                    string userId = CacheUserId(domainReferenceId);
                    string userName = string.Format("{0}/{1}", Domain.Userspace, RequestContext.SSOData.User.Email);
                    using (Tracer.StartTrace(String.Format("BusinessContext StartSession for logged in user '{0}'", userName)))
                    {
                        if (string.IsNullOrEmpty(userId))
                        {
                            userId = userName;
                        }

                        var started = false;
                        var session = SessionManager.ResumeSession(userName, userId, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                        if (session == null)
                        {
                            Logger.Log("Could not resume session, starting new session", LogSeverity.Debug);
                            session = CourseIsProductCourse ? SessionManager.StartAnnonymousSession() : SessionManager.StartNewSession(userName, BrainhoneyDefaultPassword, true, userId, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                            started = true;
                        }

                        SessionManager.CurrentSession = session;

                        if (!CourseIsProductCourse)
                        {
                            Logger.Log(String.Format("Course domain ID is {0}", Course.Domain.Id), LogSeverity.Debug);
                            CurrentUser = ExistingUser(AdminConnection(AdminAccessType.RootAdmin), Course.Domain.Id, RequestContext.SSOData.UserId);

                            if (CurrentUser != null)
                            {
                                session.UserId = CurrentUser.Id;
                                CacheUserId(domainReferenceId, session.UserId);
                            }
                        }

                        if (CurrentUser != null && AgilixUserId != CurrentUser.Id && !started)
                        {
                            Logger.Log("User changed, starting new session", LogSeverity.Debug);
                            SessionManager.CurrentSession = SessionManager.StartNewSession(userName, BrainhoneyDefaultPassword, true, userId, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                        }
                    }
                }

                // If we've not found a user, create one from the RA info.        
                if (CurrentUser == null && SSOData != null && SSOData.User != null)
                {
                    CurrentUser = new UserInfo()
                    {
                        FirstName = SSOData.User.FirstName,
                        LastName = SSOData.User.LastName,
                        Username = SSOData.User.Email,
                        ReferenceId = SSOData.UserId
                    };
                }
            }
        }

        /// <summary>
        /// Sets up the access level and logs in the current user. Also setups of the
        /// IsAnonymous field as appropriate.
        /// </summary>
        protected override void InitializePermissions()
        {
            using (Tracer.StartTrace("BusinessContext InitializePermissions"))
            {
                if (RequestContext.SiteUserInfo != null)
                {
                    var access = RequestContext.SiteUserInfo.LevelOfAccess;

                    Logger.Log(string.Format("Level of Access is {0}", access), LogSeverity.Debug);

                    if (access >= 70)
                    {
                        AccessLevel = AccessLevel.Instructor;
                        AccessType = AccessType.Adopter;
                        CanCreateCourse = true && (EntityId == ProductCourseId);
                    }
                    else if (access >= 40)
                    {
                        AccessLevel = AccessLevel.Instructor;
                        AccessType = AccessType.Demo;
                    }
                    else if (access >= 30)
                    {
                        AccessLevel = AccessLevel.Student;
                        AccessType = AccessType.Adopter;
                    }
                    else if (access >= 20)
                    {
                        AccessLevel = AccessLevel.Student;
                        // AccessType = AccessType.Demo; <-- Orginal
                        // Changed this to conform to designated naming convention as well as adding 
                        // the additional distinction between an Adaptor whos is an instructor and a 
                        // premium student maybe we will need this down the road.
                        AccessType = AccessType.Basic;
                    }
                    else
                    {
                        AccessLevel = AccessLevel.None;
                    }
                }
                else
                {
                    Logger.Log("Level of Access is not found", LogSeverity.Debug);
                }
                if (ImpersonateStudent)
                {
                    AccessLevel = AccessLevel.Student;
                }
            }
        }

        #endregion
    }
}