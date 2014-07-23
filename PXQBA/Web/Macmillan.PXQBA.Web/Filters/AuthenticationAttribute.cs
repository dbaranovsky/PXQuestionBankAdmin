using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Security;
using Bfw.Agilix.Dlap.Configuration;
using Bfw.Common;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.Constants;

namespace Macmillan.PXQBA.Web.Filters
{
    public class AuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        ///     Authenticates user using MARS system
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                string token = filterContext.HttpContext.Request.Form[WebKeys.AuthenticationToken];
                if (string.IsNullOrEmpty(token))
                {
                    string redirectUrl = string.Format(ConfigurationHelper.GetMarsLoginUrl(),
                        filterContext.HttpContext.Request.Url);
                    filterContext.Result = new RedirectResult(redirectUrl);
                    return;
                }
                UpdateCookie(token);
                // SetAuthCookie only sets cookie to the response. In current request IsAuthenticated is still false
                // That's why need redirection
               
                
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Url.ToString());
            }

            var configManager = ConfigurationManager.GetSection("agilixSessionManager") as SessionManagerSection;
            if (IsBrainHoneySessionExpired(configManager))
            {
                StartBrainHoneySession(configManager, TimeZoneInfo.Local);
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
        }

        private void UpdateCookie(string cookieValue)
        {
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookieValue);
            if (ticket != null)
            {
                FormsAuthentication.SetAuthCookie(ticket.Name, false);
                HttpCookie newCookie = HttpContext.Current.Response.Cookies[WebKeys.AuthenticationCookie];
                HttpContext.Current.Request.Cookies.Set(newCookie);
            }
        
        }

        private bool IsBrainHoneySessionExpired(SessionManagerSection configManager)
        {
            var context = HttpContext.Current;
            var cookie = context.Request.Cookies[configManager.BrainHoneyConnection.CookieName];
            var domainCookie = context.Request.Cookies[configManager.BrainHoneyConnection.ActiveDomainCookieName];

            if (null == cookie || null == domainCookie)
            {
                return true;
            }

            FormsAuthenticationTicket ticket = null;

            try
            {
                ticket = FormsAuthentication.Decrypt(cookie.Value);
            }
            catch
            {
                // not much we can do here, so just reauth the user...
            }

            return ticket == null || ticket.Expired || ticket.IssueDate.AddMinutes(15) < DateTime.Now;
        }

        private void StartBrainHoneySession(SessionManagerSection config, TimeZoneInfo timeZoneInfo)
        {
            string username = config.AdminUser.Username;
            string password = config.AdminUser.Password;
            string brainHoneyAuthUrl = config.BrainHoneyConnection.Url;
            string userDomain = config.BrainHoneyConnection.UserDomain;
            string domain = config.BrainHoneyConnection.CookieDomain;
           
            string cookieName = config.BrainHoneyConnection.CookieName;
            string browserCheckCookie = "BHBrowserCheck";
            string domainCookieName = config.BrainHoneyConnection.ActiveDomainCookieName;
            if (!username.Contains("/"))
            {
                username = userDomain + "/" + username;
            }
            else
            {
                string[] parts = username.Split('/');
                userDomain = parts[0];
            }

            string uri = brainHoneyAuthUrl.Replace("{1}", userDomain);
            var requestData = GenerateRequestData(username, password, timeZoneInfo);

            ProccessBrainHoneyResponse(uri, requestData, cookieName, browserCheckCookie, domainCookieName, userDomain, domain);
           
        }

        private string GenerateRequestData(string username, string password, TimeZoneInfo timeZoneInfo)
        {
            HttpServerUtility server = HttpContext.Current.Server;


            string requestData = "action=login&username=" + server.UrlEncode(username) + "&password=" +
                                 server.UrlEncode(password);
            if (timeZoneInfo != null && timeZoneInfo.GetAdjustment(DateTime.Now.Year) != null)
            {
                TimeZoneInfo.AdjustmentRule adjustment = timeZoneInfo.GetAdjustment(DateTime.Now.Year);

                requestData += "&standardOffset=" + -1*timeZoneInfo.BaseUtcOffset.TotalMinutes +
                               "&daylightOffset=" +
                               -1*(adjustment.DaylightDelta.TotalMinutes +
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
                requestData += "&standardOffset=" + -1*timeZoneInfo.BaseUtcOffset.TotalMinutes +
                               "&daylightOffset=" + timeZoneInfo.BaseUtcOffset.TotalMinutes;
            }
            return requestData;
        }

        private void ProccessBrainHoneyResponse(string uri, string requestData, string cookieName, string browserCheckCookie, string domainCookieName, string userDomain, string domain)
        {
            var bhUri = new Uri(uri);
            var webRequest = CreateRequest(bhUri);
            var cookieJar = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
            cookieJar.Add(new Cookie("BHBrowserCheck", "1", "/", bhUri.Host));
            webRequest.CookieContainer = cookieJar;
            webRequest.ContentLength = requestData.Length;

            var byteData = Encoding.UTF8.GetBytes(requestData);
            using (Stream postStream = webRequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, requestData.Length);
            }

            WebResponse response = webRequest.GetResponse();
            ProcessCookies(cookieJar, webRequest, cookieName, browserCheckCookie, domainCookieName, userDomain, domain);

            response.Close();
        }

        private HttpWebRequest CreateRequest(Uri bhUri)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(bhUri);
            webRequest.Method = "POST";
            webRequest.Accept = "*/*";
            webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            return webRequest;
        }

        private void ProcessCookies(CookieContainer cookieJar, HttpWebRequest webRequest, string cookieName, string browserCheckCookie, string domainCookieName, string userDomain, string domain)
        {
            foreach (Cookie c in cookieJar.GetCookies(webRequest.RequestUri))
            {
                if (c.Name.ToLowerInvariant() != cookieName.ToLowerInvariant() &&
                    c.Name.ToLowerInvariant() != browserCheckCookie.ToLowerInvariant()) continue;
                var cookie = NewCookieFromResponse(c, c.Name);
                   
                var activeDomainCookie = NewCookieFromResponse(c, domainCookieName);
                activeDomainCookie.Value = userDomain;

                if (!string.IsNullOrEmpty(domain))
                {
                    cookie.Domain = domain;
                    activeDomainCookie.Domain = domain;
                }

                SetCookie(cookie);
                SetCookie(activeDomainCookie);
            }
            
        }

        private void SetCookie(HttpCookie cookie)
        {
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private HttpCookie NewCookieFromResponse(Cookie cookie, string name)
        {
            return new HttpCookie(name)
            {
                Path = cookie.Path,
                Value = cookie.Value,
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
                Expires = cookie.Expires
            };
        }
    }
}