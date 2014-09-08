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
    /// <summary>
    /// Authentication filter that is used before every request to check authentication and authenticate the user
    /// </summary>
    public class AuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        ///     Authenticates user using MARS system
        /// </summary>
        /// <param name="filterContext">Authentication filter context</param>
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

         
        }

        /// <summary>
        /// Executes authentication challenge if necessary
        /// </summary>
        /// <param name="filterContext">Authentication filter context</param>
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
    }
}