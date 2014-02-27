using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;

using Bfw.PX.Account.Models;
using Bfw.PX.Account.Abstract;
using BFW.RAg;

namespace Bfw.PX.Account.Controllers
{
    /// <summary>
    /// All actions that can be taken by non-logged in users
    /// </summary>
    public class RequestController : Controller
    {
        #region Properties

        /// <summary>
        /// Context that contains all request information
        /// </summary>
        protected IRequestContext Context { get; set; }

        #endregion

        public RequestController(IRequestContext ctx)
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
        /// Default action encountered by a non-logged in user
        /// </summary>
        /// <param name="url">URL to redirect user to when login is completed successfully</param>
        /// <param name="status">indicates whether a previous login attempt failed or not</param>
        /// <returns></returns>
        public ActionResult Login(string url, string status)
        {
            return View();
        }

        /// <summary>
        /// Logs out the user and redirects them to the correct url
        /// </summary>
        /// <param name="url">url to redirect the user to</param>
        /// <returns></returns>
        public ActionResult Logout(string url)
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
                    for (int i = 0; i< System.Web.HttpContext.Current.Request.Cookies.Count; i++)
                    {
                        HttpCookie iCookie = System.Web.HttpContext.Current.Request.Cookies[i];
                        System.Web.HttpContext.Current.Response.Cookies[iCookie.Name].Value = "";
                        System.TimeSpan diff1 = new TimeSpan(10, 0, 0, 0);
                        System.Web.HttpContext.Current.Response.Cookies[iCookie.Name].Expires = DateTime.Now.Subtract(diff1);
                    }
                    System.Web.HttpContext.Current.Response.Redirect(TargetPublicUrl.ToString(), true);
                }
            }
            return View();
        }

        /// <summary>
        /// Default action encountered by a non-logged in user when they are attempting to adopt a
        /// course they've been demoing
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Adopt(string url)
        {
            return View();
        }

        /// <summary>
        /// Default action encountered by a non-logged in user when they are attempting to register an
        /// account in the system.
        /// </summary>
        /// <param name="url">url to redirect user to after registration is complete</param>
        /// <returns></returns>
        public ActionResult Register(string url)
        {
            return View();
        }

        #endregion
    }
}
