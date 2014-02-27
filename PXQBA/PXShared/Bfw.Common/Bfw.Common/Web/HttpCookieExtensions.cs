using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace Bfw.Common.Web
{
    /// <summary>
    /// Contains useful extensions for the System.Web.HttpCookie class.
    /// </summary>
    public static class HttpCookieExtensions
    {
        /// <summary>
        /// Rewrites the given cookie so that it looks like it comes from the given domain.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <param name="targetUri">The target URI.</param>
        /// <param name="proxyUri">The proxy URI.</param>
        /// <returns></returns>
        public static Cookie Proxy(this Cookie cookie, Uri targetUri, Uri proxyUri)
        {
            var copy = new Cookie(string.Format("{0}_{1}", targetUri.Host, cookie.Name), cookie.Value, cookie.Path, proxyUri.Host);
            copy.Comment = cookie.Comment;
            copy.CommentUri = cookie.CommentUri;
            copy.Discard = cookie.Discard;
            copy.Expired = cookie.Expired;
            copy.Expires = cookie.Expires;
            copy.HttpOnly = cookie.HttpOnly;            
            copy.Secure = cookie.Secure;
            copy.Version = cookie.Version;

            return copy;
        }

        /// <summary>
        /// Rewrites the given cookie for the site's domain.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <param name="targetUri">The target URI.</param>
        /// <returns></returns>
        public static Cookie Unproxy(this Cookie cookie, Uri targetUri)
        {
            var copy = new Cookie(cookie.Name.Replace(string.Format("{0}_", targetUri.Host), ""), cookie.Value, cookie.Path, targetUri.Host);
            copy.Comment = cookie.Comment;
            copy.CommentUri = cookie.CommentUri;
            copy.Discard = cookie.Discard;
            copy.Expired = cookie.Expired;
            copy.Expires = cookie.Expires;
            copy.HttpOnly = cookie.HttpOnly;
            copy.Secure = cookie.Secure;
            copy.Version = cookie.Version;

            return copy;
        }

        /// <summary>
        /// Converts an HttpCookie to a System.Net.Cookie.
        /// </summary>
        /// <param name="httpCookie">The HTTP cookie.</param>
        /// <returns></returns>
        public static Cookie ToNetCookie(this HttpCookie httpCookie)
        {
            var netCookie = new Cookie();

            netCookie.Name = httpCookie.Name;
            netCookie.Path = httpCookie.Path;
            netCookie.Value = httpCookie.Value;
            netCookie.Domain = httpCookie.Domain;
            netCookie.Expires = httpCookie.Expires;
            netCookie.HttpOnly = httpCookie.HttpOnly;
            netCookie.Secure = httpCookie.Secure;

            return netCookie;
        }

        /// <summary>
        /// Converts this cookie to an HttpCookie object.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public static HttpCookie ToHttpCookie(this Cookie cookie)
        {
            var httpCookie = new HttpCookie(cookie.Name);

            httpCookie.Name = cookie.Name;
            httpCookie.Path = cookie.Path;
            httpCookie.Value = cookie.Value;
            httpCookie.Domain = cookie.Domain;
            httpCookie.Expires = cookie.Expires;
            httpCookie.HttpOnly = cookie.HttpOnly;
            httpCookie.Secure = cookie.Secure;

            return httpCookie;
        }

        /// <summary>
        /// Converts an HttpCookieCollection to a System.Net.CookieCollection
        /// </summary>
        /// <param name="httpCookieCollection">The HTTP cookie collection.</param>
        /// <returns></returns>
        public static CookieCollection ToNetCookieCollection(this HttpCookieCollection httpCookieCollection)
        {
            var netCookieCollection = new CookieCollection();
            foreach (string hc in httpCookieCollection)
            {
                netCookieCollection.Add(httpCookieCollection[hc].ToNetCookie());
            }

            return netCookieCollection;
        }
    }
}
