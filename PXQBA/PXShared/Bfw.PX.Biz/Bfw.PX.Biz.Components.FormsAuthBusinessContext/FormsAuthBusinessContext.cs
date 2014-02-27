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

namespace Bfw.PX.Biz.Components.FormsAuthBusinessContext
{
    public class FormsAuthBusinessContext : BusinessContextBase
    {
        /// <summary>
        /// Stores the raw level of access value as-is from RA
        /// </summary>
        private int RaLevelOfAccess { get; set; }

        protected string CurrentURL { get; set; }

        public FormsAuthBusinessContext(ISessionManager sm, ILogger logger, ITraceManager tracer, ICacheProvider cacheProvider, IRAServices raServices)
        {
            SessionManager = sm;
            Logger = logger;
            Tracer = tracer;
            CacheProvider = cacheProvider;
            RAServices = raServices;
            CurrentURL = GetCurrentURL();

        }

        /// <summary>
        /// returns the Product Course ID of the URL passed in
        /// </summary>
        /// <param name="url">the url of the course ex. -standard-/dashboard/eportfolio/</param>
        /// <returns>Product Master Course Id for URL</returns>
        public override string GetProductCourseId(String course, string url)
        {
            using (Tracer.DoTrace("FormsAuthBusinessContext.GetProductCourseId"))
            {
                var siteInfo = GetSiteInfo(url);

                return siteInfo.AgilixSiteID;
            }
        }

        protected override void InitializeFromRequest()
        {
            using (Tracer.DoTrace("FormsAuthBusinessContext.InitializeFromRequest"))
            {
                var principal = System.Web.HttpContext.Current.User;

                if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
                {
                    var identity = principal.Identity;

                    SSOData = new SSOData()
                    {
                        User = new SSOUser()
                        {
                            Email = string.Empty
                        },
                        UserId = identity.Name
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

                PopulateSiteInfo(CurrentURL);
            }
        }

        public UserInfo GetExistingUser(string domainId, string referenceId)
        {
            return ExistingUser(AdminConnection(AdminAccessType.RootAdmin), domainId, referenceId);
        }

        protected override void InitializeUser()
        {
            using (Tracer.StartTrace("FormsAuthBusinessContext InitializeUser"))
            {
                IsAnonymous = (SSOData == null || String.IsNullOrEmpty(SSOData.UserId));

                if (IsAnonymous)
                {
                    using (Tracer.StartTrace("FormsAuthBusinessContext InitializeUser Anonymous Session"))
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
                    if (CourseIsProductCourse)
                    {
                        string domainReferenceId = string.Format("{0}/{1}", Domain.Id, SSOData.UserId);
                        string userId = CacheUserId(domainReferenceId);

                        using (Tracer.StartTrace(String.Format("FormsAuthBusinessContext StartSession for logged in user '{0}'", domainReferenceId)))
                        {

                            CurrentUser = GetExistingUser(Course.Domain.Id, SSOData.UserId);

                            if (CurrentUser != null && !CurrentUser.Id.IsNullOrEmpty())
                            {
                                string userName = string.Format("{0}/{1}", Domain.Userspace, CurrentUser.Username);

                                var session = SessionManager.ResumeSession(userName, CurrentUser.Id, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                                if (session == null)
                                {
                                    Logger.Log("Could not resume session, starting new session", LogSeverity.Debug);
                                    session = SessionManager.StartNewSession(userName, BrainhoneyDefaultPassword, true, CurrentUser.Id, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                                }

                                SessionManager.CurrentSession = session;

                                Logger.Log(String.Format("Course domain ID is {0}", Course.Domain.Id), LogSeverity.Debug);

                                if (CurrentUser != null && !CurrentUser.Id.IsNullOrEmpty())
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
                            else
                            {
                                SessionManager.CurrentSession = SessionManager.StartAnnonymousSessionWithOwner(Course.CourseOwner);
                            }
                        }
                    }
                    else
                    {
                        Logger.Log("FormsAuthBusinessContext Have RA User", LogSeverity.Debug);
                        string domainReferenceId = string.Format("{0}/{1}", Domain.Id, SSOData.UserId);
                        string userId = CacheUserId(domainReferenceId);

                        using (Tracer.StartTrace(String.Format("FormsAuthBusinessContext StartSession for logged in user '{0}'", domainReferenceId)))
                        {

                            CurrentUser = GetExistingUser(Course.Domain.Id, SSOData.UserId);

                            if (CurrentUser != null && !CurrentUser.Id.IsNullOrEmpty())
                            {
                                string userName = string.Format("{0}/{1}", Domain.Userspace, CurrentUser.Username);

                                var session = SessionManager.ResumeSession(userName, CurrentUser.Id, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                                if (session == null)
                                {
                                    Logger.Log("Could not resume session, starting new session", LogSeverity.Debug);
                                    session = SessionManager.StartNewSession(userName, BrainhoneyDefaultPassword, true, CurrentUser.Id, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                                }

                                SessionManager.CurrentSession = session;

                                Logger.Log(String.Format("Course domain ID is {0}", Course.Domain.Id), LogSeverity.Debug);

                                if (CurrentUser != null && !CurrentUser.Id.IsNullOrEmpty())
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
                            else
                            {
                                SessionManager.CurrentSession = SessionManager.StartAnnonymousSessionWithOwner(Course.CourseOwner);
                            }
                        }
                    }

                // If we've not found a user, create one from the RA info.        
                if ((CurrentUser == null || CurrentUser.Id.IsNullOrEmpty()) && SSOData != null && SSOData.User != null)
                {
                    CurrentUser = GetNewUserData();
                }
            }
        }

        protected override void InitializePermissions()
        {
            using (Tracer.StartTrace("BusinessContext InitializePermissions"))
            {
                GetSiteUserData(CurrentURL);

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
            var svc = RAServices;
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

        #region Helper Methods

        /// <summary>
        /// Gets any site info necessary from the external provider, in this case RA.
        /// </summary>
        private void PopulateSiteInfo(string url)
        {
            using (Tracer.DoTrace("FormsAuthBusinessContext.PopulateSiteInfo"))
            {
                var siteInfo = GetSiteInfo(url);

                if (siteInfo == null)
                {
                    throw new Exception(string.Format("siteInfo is null for url: {0}", AdjustForSubdomain(url)));
                }
         
                SiteID = siteInfo.SiteID;
                URL = siteInfo.BaseURL;
                ProductCourseId = siteInfo.AgilixSiteID;
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

                    object cachedObject = null;

                    using (Tracer.DoTrace("GetSiteUserData from RAWS"))
                    {
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
                                    var coreSvc = RAServices;
                                    var siteInfo = coreSvc.GetAccessLevelByBaseUrl(SSOData.UserId, BusinessContextBase.AdjustForSubdomain(url));
                                    RawsSiteUserData = new RAg.Net.RAWS.GetSiteUserData.SiteUserData();
                                    RawsSiteUserData.LevelOfAccess = siteInfo.AccessLevel.LevelOfAccess.ToString();
                                    RawsSiteUserData.Expiration = siteInfo.AccessLevel.ExpirationDate.ToString();
                                    RawsSiteUserData.UserID = SSOData.UserId;                                    
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

                        // we should only cache data for users that have some level of access to the product
                        if (RaLevelOfAccess > 20 && cachedObject == null)
                        {
                            CacheProvider.StoreRASiteUserData(SSOData.UserId, foundCourseId, RawsSiteUserData);

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

        protected string GetCurrentURL()
        {
            if (System.Web.HttpContext.Current == null)
            {
                return string.Empty;
            }

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
