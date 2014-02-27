using System;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Routing;
using System.Web.Security;

using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Direct.Services;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class AccountController : Controller
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IUserActions implementation.
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }

        protected Bfw.Common.Caching.ICacheProvider CacheProvider { get; set; }

        /// <summary>
        /// Constructs a default AccountWidgetController. Depends on a business context
        /// and user actions implementation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userActions">The user actions.</param>
        public AccountController(BizSC.IBusinessContext context, BizSC.IUserActions userActions, Bfw.Common.Caching.ICacheProvider cacheProvider)
        {
            Context = context;
            UserActions = userActions;
            CacheProvider = cacheProvider;
        }

        /// <summary>
        /// Displays a login for to the user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ActionResult result = null;

            var loginMethod = ConfigurationManager.AppSettings["LoginMethod"];

            string sBaseUrl;
            var targetUrl = GetReturnURL(out sBaseUrl);

            if (!Context.IsAnonymous)
            {
                // users already logged in should be redirected to the home page
                var url = Url.RouteUrl("CourseSectionHome");
                result = Redirect(url);
            }
            else if (loginMethod.ToLowerInvariant() == "mars")
            {
                //use the MARS application for login
                string target = string.Format("{0}/Account/Auth", targetUrl);
                string login = string.Format(ConfigurationManager.AppSettings["MarsPathLogin"], target, HttpUtility.UrlEncode(HttpUtility.UrlEncode(returnUrl)));

                result = Redirect(login);
            }
            else
            {
                //case of loginMethod == local
                result = View();
            }

            return result;
        }


        public ActionResult TermsAndConditions()
        {
            return View("TermsAndConditions");
        }

        /// <summary>
        /// Logs in the specified account and redirects the user to the Login action.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public ActionResult Login(Account account)
        {
            ActionResult result = null;
            bool error = false;

            try
            {
                if (ModelState.IsValid)
                {
                    var svc = new Bfw.PX.Biz.Direct.Services.RAServices();
                    var response = svc.AuthenticateUser(account.Username, account.Password);

                    if (response.Error.Code == "0")
                    {
                        FormsAuthentication.SetAuthCookie(response.UserInfo.UserId, false);

                        var url = Url.RouteUrl("CourseSectionHome");
                        result = Redirect(url);
                    }
                    else
                    {
                        error = true;
                    }
                }
                else
                {
                    error = true;
                }
            }
            catch
            {
                error = true;
            }

            if (error)
            {
                account.Password = string.Empty;
                ViewData.Model = account;
                ModelState.AddModelError("", "Username or Password is invalid");

                result = View();
            }

            return result;
        }

        /// <summary>
        /// Accepts security token and return url to land on the MARS login page
        /// </summary>
        /// <param name="token"></param>
        /// <param name="BaseUrl"></param>
        /// <returns></returns>
        public ActionResult Auth(string token, string BaseUrl)
        {
            ActionResult result = null;

            var returnUrl = BaseUrl; //.Replace("__HASH__", "#");

            if (string.IsNullOrEmpty(returnUrl))
            {
                string sBaseUrl;
                returnUrl = GetReturnURL(out sBaseUrl);
            }

            try
            {
                var ticket = FormsAuthentication.Decrypt(token);
                FormsAuthentication.SetAuthCookie(ticket.Name, false);

                //var userRefId = ticket.Name;
                //var productCourseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
                
                //Context.CacheProvider.InvalidateUserEnrollmentList(userRefId, productCourseId);
                //Context.CacheProvider.InvalidateUsersByReference(userRefId); //***LMS - is this still user ref id?

                result = Redirect(returnUrl);                
            }
            catch
            {
                result = Redirect(string.Format("{0}?auth=failed", returnUrl));
            }

            return result;
        }

        /// <summary>
        /// Logs out the user and redirects them to the default page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            ActionResult result = null;

            var loginMethod = ConfigurationManager.AppSettings["LoginMethod"].ToLowerInvariant();

            //Expire start page cookie from clien
            if (Request.Cookies["StartPageViewed"] != null)
            {
                var cookie = new HttpCookie("StartPageViewed");
                cookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cookie);
            }

            string sBaseUrl;
            var includeCourseID = false; // We dont need to include course id as part of the URL so set to false
            var returnUrl = GetReturnURL(out sBaseUrl, includeCourseID);

            if (!Context.IsAnonymous)
            {
                CacheHelper.InvalidateUser(CacheProvider, Context.CurrentUser);

                //clear all cookies. This is a hack. I feel bad about myself.
                var cookies = System.Web.HttpContext.Current.Request.Cookies.AllKeys;

                foreach (var cookie in cookies)
                {
                    System.Web.HttpContext.Current.Request.Cookies[cookie].Expires = System.DateTime.Now.AddYears(-1);
                    System.Web.HttpContext.Current.Request.Cookies[cookie].Value = string.Empty;
                    System.Web.HttpContext.Current.Response.Cookies[cookie].Expires = System.DateTime.Now.AddYears(-1);
                    System.Web.HttpContext.Current.Response.Cookies[cookie].Value = string.Empty;
                }

                if (loginMethod == "mars")
                {
                    string login = string.Format(ConfigurationManager.AppSettings["MarsPathLogout"], returnUrl, sBaseUrl);

                    result = Redirect(login);
                }
                else
                {
                    result = Redirect(returnUrl);
                }
            }
            else
            {
                result = Redirect(returnUrl);
            }

            return result;
        }

        /// <summary>
        /// Upgrades this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Upgrade()
        {
            return View();
        }

        /// <summary>
        /// Populates a list of all scripts required for Authentication process
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthenticationScripts()
        {
            string RABaseUrl = ConfigurationManager.AppSettings["RABaseUrl"];
            string[] scripts = new string[] { "/BFWglobal/js/json2.js", "/BFWglobal/js/jquery/jquery.cookie.js", "/BFWglobal/js/global.js", "/BFWglobal/js/BFW_LogError.js", "/BFWglobal/js/BFW_Error.js", "/BFWglobal/RAg/v2.0/RA.js", "/BFWglobal/RAg/v2.0/RAWS.js", "/BFWglobal/RAg/v2.0/RAif.js" };
            string raCss = ConfigurationManager.AppSettings["RABaseUrl"] + "/BFWglobal/RAg/v2.0/css/RAif.css";

            for (int i = 0; i < scripts.Length; i++)
            {
                scripts[i] = RABaseUrl + scripts[i];
            }

            ViewData["rascripts"] = scripts;
            ViewData["racss"] = raCss;

            return View();
        }

        /// <summary>
        /// Useful to certain views or AJAX calls to determine if user is authenticated.
        /// </summary>
        /// <returns>
        /// Returns true if the user is authenticated, false otherwise.
        /// </returns>
        public ActionResult UserAuthenticated()
        {
            if (Context.IsAnonymous) return Content("false");
            return Content("true");
        }

        /// <summary>
        /// Currents the user id.
        /// </summary>
        /// <returns></returns>
        public ActionResult CurrentUserId()
        {
            var id = "0";

            if (Context.CurrentUser != null)
            {
                id = Context.CurrentUser.Id;
            }

            return Content(id);
        }

        /// <summary>
        /// Determines whether this instance is instructor.
        /// </summary>
        /// <returns></returns>
        public ActionResult IsInstructor()
        {
            var isInstructor = "false";

            if (Context.AccessLevel == AccessLevel.Instructor)
            {
                isInstructor = "true";
            }

            return Content(isInstructor);
        }

        public ActionResult KeepAlive()
        {
            return new EmptyResult();
        }

        public ActionResult SessionExtended()
        {
            return View();
        }

        private string GetReturnURL(out string bUrl, bool includeCourseId = true)
        {
            var baseUrl = ConfigurationManager.AppSettings["InsecureBaseUrl"];
            var excludeCourseSection = ConfigurationManager.AppSettings["ExcludeCourseSection"];

            var course = RouteData.Values["course"];
            var section = RouteData.Values["section"];
            var courseIdObj = RouteData.Values["courseid"];
            string returnUrl;

            // Required by mars authentication service
            bUrl = string.Format("{0}{1}/{2}/{3}", HttpContext.Request.Url.Host, baseUrl, section, course);

            if (excludeCourseSection != null && excludeCourseSection == "true" && includeCourseId)
                returnUrl = string.Format("http://{0}{1}/{2}/{3}", HttpContext.Request.Url.Host, baseUrl, course, courseIdObj);
            else if (excludeCourseSection != null && excludeCourseSection == "true" && !includeCourseId)
                returnUrl = string.Format("http://{0}{1}/{2}", HttpContext.Request.Url.Host, baseUrl, course);
            else if (includeCourseId)
                returnUrl = string.Format("http://{0}{1}/{2}/{3}/{4}", HttpContext.Request.Url.Host, baseUrl, section, course, courseIdObj);
            else
                returnUrl = string.Format("http://{0}{1}/{2}/{3}", HttpContext.Request.Url.Host, baseUrl, section, course);

            return returnUrl;
        }

    }
}
