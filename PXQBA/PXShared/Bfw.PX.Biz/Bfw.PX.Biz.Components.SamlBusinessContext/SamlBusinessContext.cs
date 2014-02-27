using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Caching;
using Bfw.Common.Logging;
using Bfw.Common.SSO;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;

using Bfw.PX.PXPub.Components;

namespace Bfw.PX.Biz.Components.SamlBusinessContext
{
    public class SamlBusinessContext : BusinessContextBase
    {
        #region Constants

        /// <summary>
        /// ClaimType for the RA User ID
        /// </summary>
        private const string RaUserIdClaim = @"/UserAttribute[@ldap:targetAttribute=""cn""]";

        /// <summary>
        /// ClaimType for the RA Email address (aka username)
        /// </summary>
        private const string RaUserEmailClaim = @"/UserAttribute[@ldap:targetAttribute=""mail""]";

        #endregion

        /// <summary>
        /// Stores the raw level of access value as-is from RA
        /// </summary>
        private int RaLevelOfAccess { get; set; }

        #region Constructors

        public SamlBusinessContext(ISessionManager sm, ILogger logger, ITraceManager tracer, ICacheProvider cacheProvider)
        {
            SessionManager = sm;
            Logger = logger;
            Tracer = tracer;
            CacheProvider = cacheProvider;
        }

        #endregion

        #region BusinessContextBase

        /// <summary>
        /// returns the Product Course ID of the URL passed in
        /// </summary>
        /// <param name="url">the url of the course ex. -standard-/dashboard/eportfolio/</param>
        /// <returns>Product Master Course Id for URL</returns>
        public override string GetProductCourseId(String course, string url)
        {            
            using (Tracer.DoTrace("SamlBusinessContext.GetProductCourseId"))
            {
                var siteInfo = GetSiteInfo(url);

                return siteInfo.AgilixSiteID;
            }
        }

        protected override void InitializeFromRequest()
        {
            using (Tracer.DoTrace("SamlBusinessContext.InitializeFromRequest"))
            {
                var principal = System.Web.HttpContext.Current.User;

                if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
                {
                    var identity = principal.Identity as Microsoft.IdentityModel.Claims.IClaimsIdentity;

                    SSOData = new SSOData()
                    {
                        User = new SSOUser()
                        {
                            Email = identity.Claims.First(c => c.ClaimType == RaUserEmailClaim).Value
                        },
                        UserId = identity.Claims.First(c => c.ClaimType == RaUserIdClaim).Value
                    };
                }
                else
                {
                    var message = "user is unauthenticated because: ";
                    if (principal == null)
                    {
                        message += "principal is null";
                    }
                    else if (principal.Identity.IsAuthenticated == false)
                    {
                        message += "principal.Identity.IsAuthenticated is false";
                    }
                    else if (principal.Identity == null)
                    {
                        message += "principal.Identity is null";
                    }
                    else
                    {
                        message += "unknown reason";
                    }

                    Logger.Log(message, LogSeverity.Debug);
                }

                var url = GetCurrentURL();
                PopulateSiteInfo(url);
            }
        }

        protected override void InitializeUser()
        {
            using (Tracer.StartTrace("SamlBusinessContext InitializeUser"))
            {
                IsAnonymous = (SSOData == null || String.IsNullOrEmpty(SSOData.UserId));

                if (IsAnonymous || CourseIsProductCourse)
                {
                    using (Tracer.StartTrace("SamlBusinessContext InitializeUser Anonymous Session"))
                    {
                        if (Course.CourseOwner.IsNullOrEmpty())
                        {
                            SessionManager.CurrentSession = SessionManager.StartAnnonymousSession();
                        }
                        else
                        {
                            SessionManager.CurrentSession = SessionManager.StartAnnonymousSessionWithOwner(Course.CourseOwner);
                        }

                        //set the IsPublicView property for a Presentation Course
                        var currentUserId = CurrentUser == null ? string.Empty : CurrentUser.Id;
                        SetPublicView(currentUserId);
                    }
                }
                else
                {
                    Logger.Log("SamlBusinessContext Have RA User", LogSeverity.Debug);
                    string domainReferenceId = string.Format("{0}/{1}", Domain.Id, SSOData.UserId);
                    string userId = CacheUserId(domainReferenceId);
                    string userName = string.Format("{0}/{1}", Domain.Userspace, SSOData.User.Email);
                    using (Tracer.StartTrace(String.Format("SamlBusinessContext StartSession for logged in user '{0}'", userName)))
                    {
                        if (string.IsNullOrEmpty(userId))
                        {
                            userId = userName;
                        }

                        // related to the code below marked at potentially irrelevant
                        //var started = false;
                        var session = SessionManager.ResumeSession(userName, userId, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                        if (session == null)
                        {
                            Logger.Log("Could not resume session, starting new session", LogSeverity.Debug);
                            session = CourseIsProductCourse ? SessionManager.StartAnnonymousSession() : SessionManager.StartNewSession(userName, BrainhoneyDefaultPassword, true, userId, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                            //started = true;
                        }

                        SessionManager.CurrentSession = session;

                        if (!CourseIsProductCourse)
                        {
                            Logger.Log(String.Format("Course domain ID is {0}", Course.Domain.Id), LogSeverity.Debug);
                            CurrentUser = ExistingUser(AdminConnection(AdminAccessType.RootAdmin), Course.Domain.Id, SSOData.UserId);

                            if (CurrentUser != null)
                            {
                                session.UserId = CurrentUser.Id;
                                CacheUserId(domainReferenceId, session.UserId);
                                SetPublicView(CurrentUser.Id);
                                if (IsPublicView)
                                {
                                    SessionManager.CurrentSession = SessionManager.StartAnnonymousSessionWithOwner(Course.CourseOwner);
                                }
                            }
                        }


                    }
                }

                // If we've not found a user, create one from the RA info.        
                if (CurrentUser == null && SSOData != null && SSOData.User != null)
                {
                    CurrentUser = GetNewUserData();
                }
            }
        }

        protected override void InitializePermissions()
        {
            using (Tracer.StartTrace("BusinessContext InitializePermissions"))
            {
                var url = GetCurrentURL();
                GetSiteUserData(url);

                Logger.Log(string.Format("Level of Access is {0}", RaLevelOfAccess), LogSeverity.Debug);

                if (RaLevelOfAccess >= 70)
                {
                    AccessLevel = AccessLevel.Instructor;
                    AccessType = AccessType.Adopter;
                    CanCreateCourse = true && (EntityId == ProductCourseId);
                }
                else if (RaLevelOfAccess >= 40)
                {
                    AccessLevel = AccessLevel.Instructor;
                    AccessType = AccessType.Demo;
                }
                else if (RaLevelOfAccess >= 30)
                {
                    AccessLevel = AccessLevel.Student;
                    AccessType = AccessType.Adopter;
                }
                else if (RaLevelOfAccess >= 20)
                {
                    AccessLevel = AccessLevel.Student;
                    AccessType = AccessType.Basic;
                }
                else
                {
                    AccessLevel = AccessLevel.None;
                }

                if (ImpersonateStudent)
                {
                    AccessLevel = AccessLevel.Student;
                    AccessType = AccessType.Adopter;
                }
            }
        }

        /// <summary>
        /// In this context we call the RA user profile service to get the accurate data.
        /// </summary>
        /// <returns>UserInfo based on RA user profile</returns>
        public override UserInfo GetNewUserData()
        {
            var svc = new Biz.Direct.Services.RAServices();
            var profile = svc.GetUserProfile(new string[] { SSOData.UserId });
            var fname = profile.UserProfile.First().FirstName;
            var lname = profile.UserProfile.First().LastName;
            var username = profile.UserProfile.First().Username;
            var email = profile.UserProfile.First().Email;
            UserInfo user = null;

            user = new UserInfo()
            {
                FirstName = fname,
                LastName = lname,
                Password = BrainhoneyDefaultPassword,
                PasswordQuestion = "more",
                PasswordAnswer = "please",
                Username = SSOData.UserId,
                Email = email
            };

            return user;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets any site info necessary from the external provider, in this case RA.
        /// </summary>
        private void PopulateSiteInfo(string url)
        {
            using (Tracer.DoTrace("SamlBusinessContext.PopulateSiteInfo"))
            {
                RAg.Net.RAWS.GetCourseSiteID.SiteInfo RawsSiteInfo = null;
                var raws = new RAg.Net.RAWS.GetCourseSiteID.RAGetAgilixCourseIDSoapClient();                

                using (Tracer.DoTrace("GetAgilixCourseID from RAWS"))
                {
                    try
                    {
                        url = BusinessContextBase.AdjustForSubdomain(url);
                        RawsSiteInfo = raws.GetAgilixCourseID(url);
                        SiteID = RawsSiteInfo.SiteID;
                        URL = RawsSiteInfo.BaseURL;
                        ProductCourseId = RawsSiteInfo.AgilixSiteID;
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Log(string.Format("GetSiteData failed: {0}", ex.Message), LogSeverity.Error);
                        throw new Exception("Can not initialize businesscontext because GetSiteData failed", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the correct user's entitlement based on the URL they are accessing
        /// </summary>
        /// <param name="url">URL the user is accessing</param>
        private void GetSiteUserData(string url)
        {
            using (Tracer.DoTrace("GetSiteUserData"))
            {
                if (SSOData != null && !string.IsNullOrEmpty(SSOData.UserId))
                {                    
                    RAg.Net.RAWS.GetSiteUserData.SiteUserData RawsSiteUserData = null;

                    var exp = "/(?<courseid>[0-9]+)/?";
                    var match = System.Text.RegularExpressions.Regex.Match(url, exp);
                    var foundCourseId = string.Empty;
                    if (match.Success)
                    {
                        foundCourseId = match.Groups["courseid"].Value;
                    }
                    else
                    {
                        match = System.Text.RegularExpressions.Regex.Match(System.Web.HttpContext.Current.Request.Url.ToString(), exp);
                        foundCourseId = string.Empty;

                        if (match.Success)
                        {
                            foundCourseId = match.Groups["courseid"].Value;
                        }
                    }
                    
                    if (string.IsNullOrEmpty(foundCourseId) && !string.IsNullOrEmpty(ProductCourseId))
                    {
                        foundCourseId = ProductCourseId;
                    }

                    //var cacheKey = string.Format("{0}:{1}", SSOData.UserId, foundCourseId);

                    using (Tracer.DoTrace("GetSiteUserData from RAWS"))
                    {
                        object cachedObject = null;

                        if (!string.IsNullOrEmpty(foundCourseId))
                        {
                            cachedObject = CacheProvider.FetchRASiteUserData(SSOData.UserId, foundCourseId);
                        }

                        if (cachedObject == null)
                        {
                            using (Tracer.DoTrace("From Service {0}:{1}", SSOData.UserId, foundCourseId))
                            {
                                try
                                {
                                    int siteId = 0;
                                    int.TryParse(SiteID, out siteId);
                                    var rawsSiteUserDataService = new RAg.Net.RAWS.GetSiteUserData.RAGetSiteUserDataSoapClient();
                                    RawsSiteUserData = rawsSiteUserDataService.GetSiteUserData(siteId, Convert.ToInt32(SSOData.UserId), System.Web.HttpContext.Current.Request.UserHostAddress);
                                    CacheProvider.StoreRASiteUserData(SSOData.UserId, foundCourseId, RawsSiteUserData);
                                }
                                catch (System.Exception ex)
                                {
                                    Logger.Log(string.Format("GetSiteUserData failed: {0} with URL", ex.Message, url), LogSeverity.Error);
                                }
                            }
                        }
                        else
                        {
                            using (Tracer.DoTrace("From Cache {0}:{1}", SSOData.UserId, foundCourseId))
                            {
                                RawsSiteUserData = cachedObject as RAg.Net.RAWS.GetSiteUserData.SiteUserData;
                            }
                        }
                    }

                    if (RawsSiteUserData != null)
                    {
                        int intVal = 0;
                        RaLevelOfAccess = 10;
                        if (Int32.TryParse(RawsSiteUserData.LevelOfAccess, out intVal))
                        {
                            RaLevelOfAccess = intVal;
                        }
                    }
                    else
                    {
                        Logger.Log("RawsSiteUserData is null", LogSeverity.Debug);
                    }
                }
                else
                {
                    Logger.Log("SSOData.UserId is null", LogSeverity.Debug);
                }
            }
        }

        private string GetCurrentURL()
        {
            var currentHttpContext = System.Web.HttpContext.Current;
            var routeData = System.Web.Routing.RouteTable.Routes.GetRouteData(new System.Web.HttpContextWrapper(currentHttpContext));
            var course = routeData.Values["course"];
            var section = routeData.Values["section"];
            var courseIdObj = routeData.Values["courseid"];
            var urlHelper = new System.Web.Mvc.UrlHelper(currentHttpContext.Request.RequestContext);
            var uri = currentHttpContext.Request.Url;
            string route = string.Empty;

            if (course != null && section != null)
            {
                route = urlHelper.RouteUrl("ProductHome", new { course = course.ToString(), section = section.ToString() });
            }

            var url = string.Format("http://{0}{1}", uri.Host, route);

            return url;
        }

        #endregion
    }
}
