using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Security;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Macmillan.PXQBA.Web.Constants;

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
    }
}