using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Security;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Macmillan.PXQBA.Web.Constants;
using Bfw.Agilix.Dlap.Configuration;
namespace Macmillan.PXQBA.Web.Filters
{
    public class AuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
     
        /// <summary>
        /// Authenticates user using MARS system
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if(!filterContext.HttpContext.Request.IsAuthenticated)
            {
                var token = filterContext.HttpContext.Request.Form[WebKeys.AuthenticationToken];
                if (string.IsNullOrEmpty(token))
                {
                    var redirectUrl = string.Format(ConfigurationHelper.GetMarsLoginUrl(), filterContext.HttpContext.Request.Url);
                    filterContext.Result = new RedirectResult(redirectUrl);
                    return;
                }
                UpdateCookie(token);
                // SetAuthCookie only sets cookie to the response. In current request IsAuthenticated is still false
                // That's why need redirection
                var configManager = ConfigurationManager.GetSection("agilixSessionManager") as SessionManagerSection;
                StartBrainHoneySession(configManager, TimeZoneInfo.Local);
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Url.ToString());
            }
           
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {

        }

        private void UpdateCookie(string cookieValue)
        {
            var ticket = FormsAuthentication.Decrypt(cookieValue);
            if (ticket != null)
            {
                FormsAuthentication.SetAuthCookie(ticket.Name, false);
                var newCookie = HttpContext.Current.Response.Cookies[WebKeys.AuthenticationCookie];
                HttpContext.Current.Request.Cookies.Set(newCookie);
            }
        }

        private void StartBrainHoneySession(SessionManagerSection config, TimeZoneInfo timeZoneInfo)
        {

                    var username = config.AdminUser.Username;
                    var password = config.AdminUser.Password;
                    var brainHoneyAuthUrl = config.BrainHoneyConnection.Url;
                    var userDomain = config.BrainHoneyConnection.UserDomain;
                    var domain = config.BrainHoneyConnection.CookieDomain;
                    var cookieJar = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
                    var cookieName = config.BrainHoneyConnection.CookieName;
                    var browserCheckCookie = "BHBrowserCheck";
                    var domainCookieName= config.BrainHoneyConnection.ActiveDomainCookieName;
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
                            if (c.Name.ToLowerInvariant() == cookieName.ToLowerInvariant() ||
                                c.Name.ToLowerInvariant() == browserCheckCookie.ToLowerInvariant())
                            {
                                var cookie = new HttpCookie(c.Name)
                                {
                                    Path = c.Path,
                                    Value = c.Value,
                                    HttpOnly = c.HttpOnly,
                                    Secure = c.Secure,
                                    Expires = c.Expires
                                };

                                var activeDomainCookie = new HttpCookie(domainCookieName)
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
   }
