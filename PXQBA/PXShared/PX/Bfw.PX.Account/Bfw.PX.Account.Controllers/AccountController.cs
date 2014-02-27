using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

using Bfw.PX.Account.Abstract;
using System.Web;

namespace Bfw.PX.Account.Controllers
{
    /// <summary>
    /// All actions that can be taken by a logged in user
    /// </summary>
    public class AccountController : Controller
    {
        #region Properties

        /// <summary>
        /// Context that contains all request information
        /// </summary>
        protected IRequestContext Context { get; set; }

        #endregion

        public AccountController(IRequestContext ctx)
        {
            Context = ctx;
            ViewData["BfwAuthSession"] = null;
            ViewData["SwitchToProtected"] = null;
            ViewData["AgilixCourseId"] = null;
            ViewData["RaBaseUrl"] = null;
            ViewData["Urls"] = "";
            ViewData["Context"] = "{";
            StringBuilder ContextJson = new StringBuilder("{");
            Uri TargetSecureUrl = System.Web.HttpContext.Current.Request.Url;
            Uri TargetPublicUrl = System.Web.HttpContext.Current.Request.Url;
            ViewData["IsProtected"] = null;
            ViewData["SecureUrl"] = "";
            ViewData["PublicUrl"] = "";
            ViewData["TargetIsProtected"] = null;
            ViewData["TargetSecureUrl"] = "";
            ViewData["TargetPublicUrl"] = "";
            if (Context != null)
            {
                if (Context.SSOData != null)
                {
                    // Determine Secure/Public URLs for current request
                    System.Configuration.Configuration rootWebConfig1 =
                        System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
                    ViewData["Urls"] += "<br/><br/><b>This request</b>";
                    ViewData["Urls"] += "<br/><br/>";
                    ViewData["Urls"] += "<br/><br/>Public URL: " + Context.PublicUrl.ToString();
                    ViewData["Urls"] += "<br/><br/>Secure URL: " + Context.SecureUrl.ToString();
                    ViewData["Urls"] += "<br/><br/>";
                    ViewData["SecureUrl"] = Context.SecureUrl.ToString();
                    ViewData["PublicUrl"] = Context.PublicUrl.ToString();
                    if (Context.TargetUrl != null)
                    {
                        // Determine Secure/Public URLs for target request
                        ViewData["Urls"] += "<br/><br/><b>Target request</b>";
                        ViewData["Urls"] += "<br/><br/>";
                        System.Configuration.KeyValueConfigurationElement NovellSSO_PXTarget_SecurePathElement =
                            rootWebConfig1.AppSettings.Settings["NovellSSO_PXTarget_SecurePathElement"];
                        System.Configuration.KeyValueConfigurationElement NovellSSO_PXTarget_PublicPathElement =
                            rootWebConfig1.AppSettings.Settings["NovellSSO_PXTarget_PublicPathElement"];
                        System.Configuration.KeyValueConfigurationElement NovellSSO_PXTarget_SecurePathElementIndex =
                            rootWebConfig1.AppSettings.Settings["NovellSSO_PXTarget_SecurePathElementIndex"];
                        int TargetSecurePathElementIndex = -1;
                        if (NovellSSO_PXTarget_SecurePathElementIndex != null)
                        {
                            if (Int32.TryParse(NovellSSO_PXTarget_SecurePathElementIndex.Value, out TargetSecurePathElementIndex))
                            {
                                if (NovellSSO_PXTarget_SecurePathElement != null && NovellSSO_PXTarget_PublicPathElement != null)
                                {
                                    StringBuilder SecureUrlString = new StringBuilder();
                                    StringBuilder PublicUrlString = new StringBuilder();
                                    bool TargetIsSecure = false;
                                    for (int i = 0; i < Context.TargetUrl.Segments.Length; i++)
                                    {
                                        if (i == TargetSecurePathElementIndex)
                                        {
                                            if (NovellSSO_PXTarget_PublicPathElement.Value == "" && Context.TargetUrl.Segments[i] != NovellSSO_PXTarget_SecurePathElement.Value)
                                            {
                                                SecureUrlString.Append(NovellSSO_PXTarget_SecurePathElement.Value);
                                                PublicUrlString.Append(NovellSSO_PXTarget_PublicPathElement.Value);
                                                SecureUrlString.Append(Context.TargetUrl.Segments[i]);
                                                PublicUrlString.Append(Context.TargetUrl.Segments[i]);
                                            }
                                            else
                                            {
                                                if (Context.TargetUrl.Segments[i] == NovellSSO_PXTarget_SecurePathElement.Value)
                                                {
                                                    TargetIsSecure = true;
                                                }
                                                SecureUrlString.Append(NovellSSO_PXTarget_SecurePathElement.Value);
                                                PublicUrlString.Append(NovellSSO_PXTarget_PublicPathElement.Value);
                                            }
                                        }
                                        else
                                        {
                                            SecureUrlString.Append(Context.TargetUrl.Segments[i]);
                                            PublicUrlString.Append(Context.TargetUrl.Segments[i]);
                                        }
                                    }
                                    ViewData["Urls"] += "<br/><br/>secure?: " + Convert.ToString(TargetIsSecure);
                                    if (TargetIsSecure)
                                    {
                                        string holdPubUrl = PublicUrlString.ToString();
                                        PublicUrlString = new StringBuilder(Context.TargetUrl.OriginalString.Replace(SecureUrlString.ToString(), PublicUrlString.ToString()));
                                        SecureUrlString = new StringBuilder(Context.TargetUrl.OriginalString.Replace(holdPubUrl, SecureUrlString.ToString()));
                                    }
                                    else
                                    {
                                        string holdPubUrl = PublicUrlString.ToString();
                                        PublicUrlString = new StringBuilder(Context.TargetUrl.OriginalString.Replace(SecureUrlString.ToString(), PublicUrlString.ToString()));
                                        SecureUrlString = new StringBuilder(Context.TargetUrl.OriginalString.Replace(holdPubUrl, SecureUrlString.ToString()));
                                    }
                                    TargetPublicUrl = new Uri(PublicUrlString.ToString());
                                    TargetSecureUrl = new Uri(SecureUrlString.ToString());
                                    ViewData["Urls"] += "<br/><br/>Public URL: " + TargetPublicUrl.ToString();
                                    ViewData["Urls"] += "<br/><br/>Secure URL: " + TargetSecureUrl.ToString();
                                    ViewData["Urls"] += "<br/><br/>";
                                    ViewData["TargetSecureUrl"] = TargetSecureUrl.ToString();
                                    ViewData["TargetPublicUrl"] = TargetPublicUrl.ToString();
                                }
                            }
                        }
                    }
                    Context.InitSiteData(TargetPublicUrl);

                    if (!string.IsNullOrEmpty(Context.SSOData.UserId))
                    {
                        //Context.RedirectToTarget();
                        ContextJson.Append("\"UserId\": \"" + Context.SSOData.UserId + "\"");
                    }
                    ViewData["BfwAuthSession"] = Context.SSOData.AuthSession;
                    ViewData["IsProtected"] = Context.SSOData.IsProtected.ToString();
                    ViewData["SwitchToProtected"] = Context.SSOData.SwitchToProtected.ToString();
                    //this.Url();
                    if (Context.SiteInfo != null)
                    {
                        ViewData["AgilixCourseId"] = Context.SiteInfo.AgilixCourseId;
                        ViewData["RaBaseUrl"] = Context.SiteInfo.BaseURL;
                    }
                }
            }
            ContextJson.Append("}");
            ViewData["Context"] = ContextJson.ToString();
        }

        #region Actions

        /// <summary>
        /// Logged in view
        /// </summary>
        /// <param name="url">url to redirect the user to</param>
        /// <returns></returns>
        public ActionResult Login(string url)
        {
            if (Context != null)
            {
                if (Context.SSOData != null)
                {
                    string TargetPublicUrlString = ViewData["TargetPublicUrl"].ToString();
                    string TargetSecureUrlString = ViewData["TargetSecureUrl"].ToString();
                    Uri TargetPublicUrl = new Uri(TargetPublicUrlString);
                    Uri TargetSecureUrl = new Uri(TargetSecureUrlString);
                    System.Web.HttpCookie TargetCookieIn = System.Web.HttpContext.Current.Request.Cookies.Get("Target");
                    if (TargetCookieIn != null)
                    {
                        if (TargetCookieIn.Value != Context.TargetUrl.ToString())
                        {
                            Context.ReInitSiteData(TargetPublicUrl);
                            System.Web.HttpCookie TargetCookieOut = new System.Web.HttpCookie("Target");
                            TargetCookieOut.Value = Context.TargetUrl.ToString();
                            TargetCookieOut.Path = "/";
                            System.Web.HttpContext.Current.Response.Cookies.Add(TargetCookieOut);
                        }
                    }
                    else
                    {
                        System.Web.HttpCookie TargetCookieOut = new System.Web.HttpCookie("Target");
                        TargetCookieOut.Value = Context.TargetUrl.ToString();
                        TargetCookieOut.Path = "/";
                        System.Web.HttpContext.Current.Response.Cookies.Add(TargetCookieOut);
                    }

                    ViewData["Urls"] += "<br/><br/>IsProtected? " + Convert.ToString(Context.SSOData.IsProtected) + ", UserId? " + Convert.ToString(string.IsNullOrEmpty(Context.SSOData.UserId));
                    ViewData["Urls"] += "<br/><br/>";
                    if (!Context.SSOData.IsProtected && !string.IsNullOrEmpty(Context.SSOData.UserId))
                    {
                        System.Web.HttpContext.Current.Response.Redirect(Context.SecureUrl.ToString());
                    }
                    else if (Context.SSOData.IsProtected && string.IsNullOrEmpty(Context.SSOData.UserId))
                    {
                        System.Web.HttpContext.Current.Response.Redirect(Context.PublicUrl.ToString());
                    }
                    else if (TargetPublicUrl != null && Context.SSOData.IsProtected && !string.IsNullOrEmpty(Context.SSOData.UserId))
                    {
                        System.Web.HttpContext.Current.Response.Redirect(TargetPublicUrl.ToString());
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// Logs out the user and redirects them to the public URL
        /// Attempts to expire and empty user cookies
        /// </summary>
        /// <param name="url">url to redirect the user to</param>
        /// <returns></returns>
        public ActionResult Logout(string url)
        {
            //un protected resource http://dev.whfreeman.com/beta/universe9e/lms/37956
            string TargetPublicUrlString = ViewData["TargetPublicUrl"].ToString();

            //protected resource http://dev.whfreeman.com/beta/secure/universe9e/lms/37956
            string TargetSecureUrlString = ViewData["TargetSecureUrl"].ToString();

            if (Context != null)
            {
                if (Context.SSOData != null)
                {    
                    Uri TargetPublicUrl = new Uri(TargetPublicUrlString);
                    Uri TargetSecureUrl = new Uri(TargetSecureUrlString);
                    System.Web.HttpCookie TargetCookieIn = System.Web.HttpContext.Current.Request.Cookies.Get("Target");
                    if (TargetCookieIn != null)
                    {
                        if (TargetCookieIn.Value != Context.TargetUrl.ToString())
                        {
                            Context.ReInitSiteData(TargetPublicUrl);
                            System.Web.HttpCookie TargetCookieOut = new System.Web.HttpCookie("Target");
                            TargetCookieOut.Value = TargetPublicUrlString;
                            TargetCookieOut.Path = "/";
                            System.Web.HttpContext.Current.Response.Cookies.Add(TargetCookieOut);
                        }
                    }
                    else
                    {
                        System.Web.HttpCookie TargetCookieOut = new System.Web.HttpCookie("Target");
                        TargetCookieOut.Value = TargetPublicUrlString;
                        TargetCookieOut.Path = "/";
                        System.Web.HttpContext.Current.Response.Cookies.Add(TargetCookieOut);
                    }

                    ViewData["Urls"] += "<br/><br/>IsProtected? " + Convert.ToString(Context.SSOData.IsProtected) + ", UserId? " + Convert.ToString(string.IsNullOrEmpty(Context.SSOData.UserId));
                    ViewData["Urls"] += "<br/><br/>";
                    if (!Context.SSOData.IsProtected && !string.IsNullOrEmpty(Context.SSOData.UserId))
                    {
                        System.Web.HttpContext.Current.Response.Redirect(Context.SecureUrl.ToString());
                    }
                    else if (Context.SSOData.IsProtected && string.IsNullOrEmpty(Context.SSOData.UserId))
                    {
                        System.Web.HttpContext.Current.Response.Redirect(Context.PublicUrl.ToString());
                    }
                }
            }

            //get Novell Logout URL from web.config
            System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
            System.Configuration.KeyValueConfigurationElement NovellSSO_LogoutURL = rootWebConfig1.AppSettings.Settings["NovellSSO_LogoutURL"];

            if (NovellSSO_LogoutURL != null)
            {
                //Loop through cookies and try to expire and empty value
                //loop makes it easier to deal with the path madness
                foreach (string key in System.Web.HttpContext.Current.Request.Cookies.AllKeys)
                {
                    HttpCookie c = System.Web.HttpContext.Current.Request.Cookies[key];

                    if (c.Name == "SiteUserData") //siteUserData is really the only cookie we want expire and clear
                    {
                        c.Value = string.Empty;
                        c.Expires = DateTime.Now.AddDays(-1);
                        c.Path = c.Path;
                        c.HttpOnly = true;
                        System.Web.HttpContext.Current.Response.Cookies.Set(c);
                    }
                }

                //send to Novell logout URL with a public URL as Target, once logout is done Novell will send(302) the client there
                //System.Web.HttpContext.Current.Response.Redirect(NovellSSO_LogoutURL.Value + "?target=" + TargetPublicUrlString);

                //Push this client side, so we can blow away cookies with JS because of Path madness
                ViewData["GoToTarget"] = NovellSSO_LogoutURL.Value + "?target=" + TargetPublicUrlString;
            }
            return View();
        }

        /// <summary>
        /// Encountered by logged in user that is adoptinga  course
        /// </summary>
        /// <param name="url">url to redirect the user to once adoption is complete</param>
        /// <returns></returns>
        public ActionResult Adopt(string url)
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View("~/Views/Account/TermsAndConditions.ascx");
        }

        #endregion
    }
}
