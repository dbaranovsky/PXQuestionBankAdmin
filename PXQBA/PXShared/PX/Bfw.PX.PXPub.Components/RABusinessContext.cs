using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.XPath;
using System.Configuration;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Routing;
using Bfw.Common.Caching;
using Bfw.Common.Logging;
using Bfw.Common.Collections;
using Bfw.Common.SSO;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Account.Abstract;
using Bfw.PX.Biz.Services.Mappers;
using Microsoft.Practices.ServiceLocation;
using Bfw.Agilix.DataContracts;
using PxWebUser;

namespace Bfw.PX.PXPub.Components
{
    /// <summary>
    /// Implements the IBusinessContext using information from RA. See <see cref="BusinessContextBase"/>.
    /// </summary>
    public class RABusinessContext : BusinessContextBase
    {
        #region IBusinessContext Members

        /// <summary>
        /// Gets or sets the agilix user ID.
        /// </summary>
        /// <value>
        /// The agilix user ID.
        /// </value>
        private string AgilixUserId { get; set; }

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

        /// <summary>
        /// Uses cookies to initialize certain properties of the buisness context.
        /// </summary>
        protected override void InitializeFromRequest()
        {
            using (Tracer.StartTrace("BusinessContext InitializeFromRequest"))
            {
                DumpCookies();
                RASession = new BFW.RAg.Session();

                var rasCookie = HttpContext.Current.Request.Cookies.Get("RAS");
                if (rasCookie != null)
                {
                    var jsSerializer = new JavaScriptSerializer();
                    var siteInfo = jsSerializer.Deserialize<SiteInfo>(HttpUtility.UrlDecode(rasCookie.Value));
                    SiteID = siteInfo.ID;
                    URL = siteInfo.URL;
                }

                var raAgilixCookie = HttpContext.Current.Request.Cookies.Get("RAAGILIX");
                if (null != raAgilixCookie)
                {
                    ProductCourseId = raAgilixCookie.Values["AGILIXID"];
                }

                //if (HttpContext.Current.Request.Cookies.AllKeys.Contains(BrainHoneyAuthCookie))
                //{
                //    BhAuthCookieValue = HttpContext.Current.Request.Cookies[BrainHoneyAuthCookie].Value;
                //}

                if (HttpContext.Current.Request.Cookies.AllKeys.Contains("PXAUTHUSER"))
                {
                    AgilixUserId = HttpContext.Current.Request.Cookies["PXAUTHUSER"].Value;
                }

                if (RASession.Check())
                {
                    SSOData = new SSOData()
                    {
                        User = new SSOUser()
                        {
                            Email = RASession.CurrentUser.Email,
                            FirstName = RASession.CurrentUser.FirstName,
                            LastName = RASession.CurrentUser.LastName
                        },
                        UserId = RASession.CurrentUser.UID.ToString()
                    };
                }
            }
        }

        public override UserInfo GetNewUserData()
        {
            var svc = new Biz.Direct.Services.RAServices();
            var profile = svc.GetUserProfile(new string[] { SSOData.UserId });
            var fname = profile.UserProfile.First().FirstName;
            var lname = profile.UserProfile.First().LastName;
            var username = profile.UserProfile.First().Username;
            var email = profile.UserProfile.First().Email;

            var user = new UserInfo()
            {
                FirstName = fname,
                LastName = lname,
                Password = BrainhoneyDefaultPassword,
                PasswordQuestion = "more",
                PasswordAnswer = "please",
                Username = username,
                Email = email,
                ReferenceId = SSOData.UserId
            };

            return user;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RABusinessContext"/> class.
        /// </summary>
        /// <param name="sm">The <see cref="ISessionManager"/>.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="tracer">The tracer.</param>
        public RABusinessContext(ISessionManager sm, ILogger logger, ITraceManager tracer, ICacheProvider cacheProvider)
        {
            SessionManager = sm;
            AccessLevel = AccessLevel.None;
            Logger = logger;
            Tracer = tracer;
            CacheProvider = cacheProvider;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Dumps the cookies.
        /// </summary>
        private void DumpCookies()
        {
            var request = HttpContext.Current.Request;
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < request.Cookies.Count; ++i)
            {
                sb.AppendFormat("{{ name: {0}, domain: {1}, path: {2}, value: {3} }}\n", request.Cookies[i].Name, request.Cookies[i].Domain, request.Cookies[i].Path, HttpUtility.UrlDecode(request.Cookies[i].Value));
            }

            Logger.Log(sb.ToString(), LogSeverity.Debug);
        }

        /// <summary>
        /// Based on the authenticated RA user, setup the User property of the
        /// context.
        /// </summary>
        protected override void InitializeUser()
        {
            using (Tracer.StartTrace("BusinessContext InitializeUser"))
            {
                var racheck = false;

                using (Tracer.StartTrace("BusinessContext RASession.Check"))
                {
                    racheck = RASession.Check();
                }

                if (racheck && RASession.CurrentUser != null && Course != null)
                {
                    Logger.Log("BusinessContext Have RA User", LogSeverity.Debug);

                    string domainReferenceId = string.Format("{0}/{1}", Domain.Id, RASession.CurrentUser.UID);
                    string userId = CacheUserId(domainReferenceId);
                    string userName = string.Format("{0}/{1}", Domain.Userspace, RASession.CurrentUser.Email);
                    using (Tracer.StartTrace("BusinessContext StartSession for logged in user"))
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

                        var refId = RASession.CurrentUser.UID.ToString();
                        // Get the agilix users and enrollments for this RA user.
                        var agilixUsers = CacheProvider.FetchUsersByReference(refId);
                        if (agilixUsers != null)
                        {
                            Logger.Log(string.Format("Agilix users loaded from cache for user: {0}", refId),LogSeverity.Debug);
                        }
                        else
                        {
                            agilixUsers = GetAgxUsersForRaUser(refId);
                        }



                        if (!CourseIsProductCourse && !agilixUsers.IsNullOrEmpty())
                        {
							if (agilixUsers != null)
							{
								if (agilixUsers.Any(u1 => u1.Domain.Id == Course.Domain.Id))
								{
                            CurrentUser = agilixUsers.Filter(u => u.Domain.Id == Course.Domain.Id).First().ToUserInfo();
                            session.UserId = CurrentUser.Id;
                            CacheUserId(domainReferenceId, session.UserId);
                        }
							}
                        }

                        SSOData = new SSOData()
                        {
                            User = new SSOUser()
                            {
                                AgilixUsers = agilixUsers.Map(u => new AgilixAccount() { ID = u.Id, DomainID = u.Domain.Id, Userspace = u.Domain.Userspace }),
                                FirstName = RASession.CurrentUser.FirstName,
                                LastName = RASession.CurrentUser.LastName,
                                CustomerID = RASession.CurrentUser.UID.ToString(),
                                Email = RASession.CurrentUser.Email
                            },
                            UserId = RASession.CurrentUser.UID.ToString()
                        };

                        if (CurrentUser == null)
                        {
                            CurrentUser = new UserInfo()
                            {
                                FirstName = SSOData.User.FirstName,
                                LastName = SSOData.User.LastName,
                                Username = SSOData.User.Email,
                                Email = SSOData.User.Email,
                                ReferenceId = RASession.CurrentUser.UID.ToString(),

                            };
                        }

                        if (CurrentUser != null && AgilixUserId != CurrentUser.Id && !started)
                        {
                            Logger.Log("User changed, starting new session", LogSeverity.Debug);
                            SessionManager.CurrentSession = CourseIsProductCourse ? SessionManager.StartAnnonymousSession() : SessionManager.StartNewSession(userName, BrainhoneyDefaultPassword, true, userId, TimeZoneInfo.FindSystemTimeZoneById(Course.CourseTimeZone));
                        }

                    }

                    IsAnonymous = false;

                    //set the IsPublicView property for a Presentation Course
                    SetPublicView(CurrentUser.Id);
                    if (IsPublicView)
                    {                        
                        SessionManager.CurrentSession = SessionManager.StartAnnonymousSessionWithOwner(Course.CourseOwner);
                    }

                    if (CurrentUser != null)
                    {
                        RASession.SetCurrentUserAgilixID(Convert.ToInt32(CurrentUser.Id));
                    }
                }
                else
                {
                    using (Tracer.StartTrace("BusinessContext InitializeUser Anonymous Session"))
                    {
                        if (Course.CourseOwner.IsNullOrEmpty())
                        {
                            SessionManager.CurrentSession = SessionManager.StartAnnonymousSession();
                        }
                        else
                        {
                            SessionManager.CurrentSession = SessionManager.StartAnnonymousSessionWithOwner(Course.CourseOwner);
                        }
                        IsAnonymous = true;

                        //set the IsPublicView property for a Presentation Course
                        var currentUserId = CurrentUser == null ? string.Empty : CurrentUser.Id;
                        SetPublicView(currentUserId);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Agilix users for RA user.
        /// </summary>
        /// <param name="userExternalId">The user external ID.</param>
        /// <returns></returns>
        public IEnumerable<AgilixUser> GetAgxUsersForRaUser(string userExternalId)
        {
            var userCmd = new GetUsers()
            {
                SearchParameters = new Bfw.Agilix.DataContracts.UserSearch()
                {
                    ExternalId = userExternalId
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(userCmd);

            var agilixUsers = new List<AgilixUser>();
            foreach (var agilixUser in userCmd.Users)
            {
                if (!agilixUsers.Exists(u => u.Id == agilixUser.Id && u.Domain.Id == agilixUser.Domain.Id))
                {
                    agilixUsers.Add(agilixUser);
                }
            }

            return agilixUsers.ToList();
        }

        /// <summary>
        /// Sets up the access level and logs in the current user. Also sets up the 
        /// IsAnonymous field as appropriate.
        /// </summary>
        protected override void InitializePermissions()
        {
            using (Tracer.StartTrace("BusinessContext InitializePermissions"))
            {
                if (RASession.Check() && RASession.CurrentUser != null)
                {
                    if (RASession.CurrentUser.CurrentSiteLogin.LevelOfAccess >= 70)
                    {
                        AccessLevel = AccessLevel.Instructor;
                        AccessType = AccessType.Adopter;
                        CanCreateCourse = true && (EntityId == ProductCourseId);
                    }
                    else if (RASession.CurrentUser.CurrentSiteLogin.LevelOfAccess >= 40)
                    {
                        AccessLevel = AccessLevel.Instructor;
                        AccessType = AccessType.Demo;
                    }
                    else if (RASession.CurrentUser.CurrentSiteLogin.LevelOfAccess >= 30)
                    {
                        AccessLevel = AccessLevel.Student;
                        AccessType = AccessType.Adopter;
                    }
                    else if (RASession.CurrentUser.CurrentSiteLogin.LevelOfAccess >= 20)
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
                if (ImpersonateStudent)
                {
                    AccessLevel = AccessLevel.Student;
                }
            }
        }

        #endregion
    }
}