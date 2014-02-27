using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace Bfw.Common.Web
{
    /// <summary>
    /// Contains useful helper methods for HttpWebRequest.
    /// </summary>
    public static class HttpWebRequestResponseExtensions
    {
        /// <summary>
        /// Copies they given request, but changes to URI to the target. Any cookies on
        /// the original request are rewritten to appear as if they come from the target Uri's domain.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="target">The target.</param>
        /// <param name="headers">The headers.</param>
        /// <returns>The copied, modified request.</returns>
        public static HttpWebRequest Proxy(this HttpRequestBase request, Uri target, WebHeaderCollection headers)
        {
            var copy = HttpWebRequest.Create(target) as HttpWebRequest;
            copy.AllowAutoRedirect = true;
            var reqCookies = new CookieCollection();
            var reqHeaders = NameValueToHeaderCollection(request.Headers);

            if (request.Cookies != null && request.Cookies.Count > 0)
            {
                reqCookies = request.Cookies.ToNetCookieCollection();
            }

            if (reqHeaders["Accept"] != null)
            {
                copy.Accept = reqHeaders["Accept"];
            }

            copy.ContentLength = request.TotalBytes;

            if (reqHeaders["Content-Type"] != null)
            {
                copy.ContentType = reqHeaders["Content-Type"];
            }

            DateTime ims;
            if (reqHeaders["If-Modified-Since"] != null && DateTime.TryParse(reqHeaders["If-Modified-Since"], out ims))
            {
                copy.IfModifiedSince = ims;
            }

            if (reqHeaders["Referer"] != null)
            {
                copy.Referer = reqHeaders["Referer"];
            }

            if (reqHeaders["Transfer-Encoding"] != null)
            {
                copy.TransferEncoding = reqHeaders["Transfer-Encoding"];
            }

            copy.Method = request.HttpMethod;
            copy.UserAgent = request.UserAgent;

            if (request.UrlReferrer != null)
            {                
                copy.Referer = Uri.EscapeUriString( request.UrlReferrer.ToString() );
            }

            copy.CookieContainer = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
            if (reqCookies.Count > 0)
            {
                foreach (Cookie cookie in reqCookies)
                {
                    copy.CookieContainer.Add(cookie.Unproxy(target));
                }
            }

            var merged = MergeHeaderCollection(reqHeaders, headers);
            if (merged != null && merged.Count > 0)
            {
                if (copy.Headers == null)
                {
                    copy.Headers = new WebHeaderCollection();
                }

                foreach (string key in merged.Keys)
                {
                    if (!WebHeaderCollection.IsRestricted(key) && !key.ToLowerInvariant().StartsWith("accept"))
                    {
                        copy.Headers[key] = merged[key];
                    }
                }
            }

            if (request.TotalBytes > 0)
            {
                var cstream = copy.GetRequestStream();                
                request.InputStream.Copy(cstream);
                cstream.Flush();
                request.InputStream.Close();
            }            

            return copy;            
        }

        /// <summary>
        /// Copies a response from a source sysem being accessed via proxy into the response the proxy
        /// will send to the client. All headers and cookies are rewritten to look like they come from
        /// the proxy system.
        /// </summary>
        /// <param name="response">Response from the actual system that handled the request.</param>
        /// <param name="target">Response that is going to come from the proxy system.</param>
        /// <param name="proxy">URI of the proxy system for rewriting cookies.</param>
        public static void Proxy(this HttpResponseBase response, HttpWebResponse target, Uri proxy)
        {
            if (!string.IsNullOrEmpty(target.ContentEncoding))
            {
                response.AddHeader("Content-Encoding", target.ContentEncoding);
            }

            string resUrl = target.ResponseUri.ToString();
            response.ContentType = target.ContentType;
            response.StatusCode = (int)target.StatusCode;
            response.StatusDescription = target.StatusDescription;

            if (target.Cookies != null && target.Cookies.Count > 0)
            {
                foreach (Cookie cookie in target.Cookies)
                {
                    response.Cookies.Add(cookie.Proxy(target.ResponseUri, proxy).ToHttpCookie());
                }
            }            

            if (target.ContentLength > 0)
            {
                var tstream = target.GetResponseStream();
                tstream.Copy(response.OutputStream);
                response.OutputStream.Flush();
                tstream.Close();                
            }
        }

        /// <summary>
        /// Copies a NameValueCollection into a WebHeaderCollection.
        /// </summary>
        /// <param name="headers">A <see cref="System.Collections.Specialized.NameValueCollection"/> to copy.</param>
        /// <returns>The given <see cref="System.Collections.Specialized.NameValueCollection"/> value copied to a <see cref="WebHeaderCollection"/>.</returns>
        private static WebHeaderCollection NameValueToHeaderCollection(System.Collections.Specialized.NameValueCollection headers)
        {
            var copy = new WebHeaderCollection();

            if (headers != null && headers.Count > 0)
            {
                foreach (string header in headers)
                {
                    copy.Add(header, headers[header]);
                }
            }

            return copy;
        }

        /// <summary>
        /// Makes a copy of the WebHeaderCollection.
        /// </summary>
        /// <param name="original">The <see cref="WebHeaderCollection"/> to copy.</param>
        /// <returns>The copied <see cref="WebHeaderCollection"/>.</returns>
        public static WebHeaderCollection CopyCollection(this WebHeaderCollection original)
        {
            WebHeaderCollection copy = null;

            if (original != null && original.Count > 0)
            {
                copy = new WebHeaderCollection();
                foreach (string header in original)
                {
                    copy.Add(header, original[header]);
                }
            }

            return copy;
        }

        /// <summary>
        /// Merges two WebHeaderCollections.
        /// </summary>
        /// <param name="a">First collection.</param>
        /// <param name="b">Second collection.</param>
        /// <returns>
        /// * If a is null and b is not, then result is a copy of b.
        /// * If b is null and a is not, then result is a copy of a.
        /// * If both a and b are non-null, then a and b are combined into one result: a new object is created
        /// with a's items, then all of b's items are written into that copy, potentially overwriting
        /// any items with keys from a.
        /// </returns>
        private static WebHeaderCollection MergeHeaderCollection(WebHeaderCollection a, WebHeaderCollection b)
        {
            WebHeaderCollection merged = null;

            if (b == null && a != null)
            {
                merged = a.CopyCollection();
            }
            else if (a == null && b != null)
            {
                merged = b.CopyCollection();
            }
            else if(a != null && b != null)
            {
                merged = a.CopyCollection();
                foreach (string header in b)
                {
                    merged[header] = b[header];
                }
            }

            return merged;
        }
    }
}
