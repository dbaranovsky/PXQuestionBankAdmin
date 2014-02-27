using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Linq;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.Common;
using Bfw.Common.Logging;
using Bfw.Common.Patterns.Logging;
using Bfw.Common.Patterns.Unity;

namespace Bfw.PX.SSO
{
    public class SsoHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        static SsoHandler()
        {
            ConfigureServiceLocator();
        }

        /// <summary>
        /// Header to add to the proxied request
        /// </summary>
        protected const string APP_ROOT_HEADER = "X-BH-AppRoot";

        public void ProcessRequest(HttpContext context)
        {
            var result = "";
            var inputRequest = context.Request;
            var pxFilePath = inputRequest.Path;
            var queryString = (inputRequest.QueryString != null && inputRequest.QueryString.Count > 0) ? "?" + inputRequest.QueryString : "";
            var pxBaseUrl = ConfigurationManager.AppSettings.Get("PxWebBaseUrl");
            var bhBaseUrl = ConfigurationManager.AppSettings.Get("BhBaseUrl");
            var bfwGlobalBaseUrl = ConfigurationManager.AppSettings.Get("BfwGlobalBaseUrl");
            var isBh = inputRequest.Url.Segments.Length > 1 && inputRequest.Url.Segments[1].ToLower() == "brainhoney/";
            var isBfwGlobal = inputRequest.Url.Segments.Length > 1 && inputRequest.Url.Segments[1].ToLower() == "bfwglobal/";

            var baseUrl = pxBaseUrl;
            if (isBh)
            {
                baseUrl = bhBaseUrl;
            }
            else if (isBfwGlobal)
            {
                baseUrl = bfwGlobalBaseUrl;
            }

            var pxUri = new Uri(baseUrl + pxFilePath + queryString);
            var correlationId = System.Guid.NewGuid().ToString("N");
            var logger = ServiceLocator.Current.GetInstance(typeof(ILogger)) as ILogger;
            logger.CorrelationId = correlationId;

            if (!isBh)
            {
                CookieManager.SetCookies(context, correlationId);
            }

            var webRequest = (HttpWebRequest)WebRequest.Create(pxUri);

            webRequest.AllowAutoRedirect = false;
            webRequest.Method = inputRequest.HttpMethod;
            webRequest.Timeout = System.Threading.Timeout.Infinite;

            SetHeaders(inputRequest, webRequest, isBh, ConfigurationManager.AppSettings["ProxyUrl"]);

            if (webRequest.CookieContainer == null)
            {
                webRequest.CookieContainer = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
            }

            for (var i = 0; i < inputRequest.Cookies.Count; i++)
            {
                Cookie cookie = null;
                if (isBh)
                {
                    cookie = new Cookie
                    {
                        Name = inputRequest.Cookies[i].Name,
                        Value = inputRequest.Cookies[i].Value,
                        Domain = ConfigurationManager.AppSettings["TargetCookieDomain"],
                        Path = "/"
                    };

                    webRequest.CookieContainer.Add(cookie);
                }
                else
                {
                    try
                    {
                        string name = inputRequest.Cookies[i].Name;
                        //Video player's cookies are ignored.
                        if (!string.IsNullOrEmpty(name) && (name == "clientLastPTimes" || name == "clientLastHTimes" || name == "AkamaiAnalyticsDO_bitRateBucketsCsv"))
                            continue;

                        webRequest.CookieContainer.Add(pxUri, new Cookie
                        {
                            Name = inputRequest.Cookies[i].Name,
                            Value = inputRequest.Cookies[i].Value
                        });
                    }
                    catch
                    {
                        logger.Log("Sending Request to PX", LogSeverity.Debug);
                    }
                }
            }

            try
            {
                if (!isBh && logger != null)
                {
                    logger.Log("Sending Request to PX", LogSeverity.Debug);
                }

                if (inputRequest.TotalBytes > 0)
                {
                    var webStream = webRequest.GetRequestStream();
                    inputRequest.InputStream.Copy(webStream);
                }

                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                if (!isBh && logger != null)
                {
                    logger.Log("Got Response from PX", LogSeverity.Debug);
                }

                var pxResponse = webResponse.GetResponseStream();
                context.Response.ContentType = webResponse.ContentType;

                if (webResponse.ContentEncoding.ToLower().Contains("gzip"))
                {
                    context.Response.AppendHeader("Content-Encoding", "gzip");
                }
                else if (webResponse.ContentEncoding.ToLower().Contains("deflate"))
                {
                    context.Response.AppendHeader("Content-Encoding", "deflate");
                }

                if (webResponse.Headers.AllKeys.Contains("Content-Disposition"))
                {
                    context.Response.AppendHeader("Content-Disposition", webResponse.Headers["Content-Disposition"]);
                }

                if (webResponse.Headers.AllKeys.Contains("Pragma"))
                {
                    context.Response.AppendHeader("Pragma", "no-cache");
                }

                if (webResponse.Headers.AllKeys.Contains("X-MiniProfiler-Ids"))
                {
                    context.Response.AppendHeader("X-MiniProfiler-Ids", webResponse.Headers["X-MiniProfiler-Ids"]);
                }

                if (webResponse.Headers.AllKeys.Contains("Expires"))
                {
                    DateTime expireTime;

                    if (DateTime.TryParse(webResponse.Headers["Expires"], out expireTime))
                    {
                        context.Response.Cache.SetExpires(expireTime);
                    }
                }

                for (var respCookieCounter = 0; respCookieCounter < webResponse.Cookies.Count; respCookieCounter++)
                {
                    var c = webResponse.Cookies[respCookieCounter];
                    var cookie = new HttpCookie(c.Name)
                    {
                        Path = c.Path,
                        Value = c.Value,
                        Expires = c.Expires,
                        HttpOnly = c.HttpOnly,
                        Secure = c.Secure,
                        Domain = ConfigurationManager.AppSettings["ProxyCookieDomain"]
                    };
                    context.Response.Cookies.Add(cookie);
                }

                string location = string.Empty;
                if (webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Moved || webResponse.StatusCode == HttpStatusCode.MovedPermanently || webResponse.StatusCode == HttpStatusCode.Redirect || webResponse.StatusCode == HttpStatusCode.RedirectMethod)
                {
                    location = webResponse.Headers["Location"];
                    context.Response.AddHeader("Location", location);
                }

                // Remove caching on request if user on IE browser. 
                // Required in order to prevent caching of ajax GET requests
                // More permanent fix required long term
                if (context.Request.Browser.Browser == "IE")
                {
                    context.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                    context.Response.Cache.SetValidUntilExpires(false);
                    context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    context.Response.Cache.SetNoStore();
                }

                context.Response.StatusCode = (int)webResponse.StatusCode;

                pxResponse.Copy(context.Response.OutputStream);
                webResponse.Close();
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                if (e is WebException && ((WebException)e).Status == WebExceptionStatus.ProtocolError)
                {
                    var errResp = ((WebException)e).Response;
                    var stream = errResp.GetResponseStream();
                    var buff = new byte[stream.Length];

                    stream.Read(buff, 0, (int)stream.Length);
                    context.Response.OutputStream.Write(buff, 0, buff.Length);
                }
                else
                {
                    var errorHtml = new StringBuilder();
                    errorHtml.Append("<div style='color:red;font-size:20px;font-family:verdana'>");
                    errorHtml.Append("Application is currently unavailable.<br /> Error from SSO </div>");
                    errorHtml.Append("<div style='color:black;font-size:14px;font-family:verdana;margin-top:40px'>");
                    if (!string.IsNullOrEmpty(result))
                    {
                        errorHtml.Append("<div> Web Exception:" + result + "</div>");
                    }

                    errorHtml.Append("<div> Error Message:" + e.Message + "</div>");

                    if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.ToString()))
                        errorHtml.Append("<div> Inner Exception:" + e.InnerException + "</div>");
                    errorHtml.Append("</div>");

                    context.Response.Write(errorHtml.ToString());

                }
            }
        }

        private static void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private static void SetHeaders(HttpRequest inputRequest, HttpWebRequest newRequest, bool isBhRequest, string proxyUrl)
        {
            var basicHeaders = new List<string> {
                                                    "Accept",
                                                    "Accept-Encoding",
                                                    "Connection",
                                                    "Content-Length",
                                                    "Content-Type",
                                                    "Expect",
                                                    "Date",
                                                    "Host",
                                                    "If-Modified-Since",
                                                    "Range",
                                                    "Referer",
                                                    "Transfer-Encoding",
                                                    "User-Agent",
                                                    "SOAPAction",
                                                    "X-Requested-With",
                                                    "Pragma",
                                                    "Content-Disposition"
                                                };

            newRequest.ContentLength = inputRequest.ContentLength;
            if (!string.IsNullOrEmpty(inputRequest.ContentType))
                newRequest.ContentType = inputRequest.ContentType;
            if (!string.IsNullOrEmpty(inputRequest.UserAgent))
                newRequest.UserAgent = inputRequest.UserAgent;

            for (var i = 0; i < inputRequest.Headers.Count; i++)
            {
                var key = inputRequest.Headers.GetKey(i);
                if (!basicHeaders.Contains(inputRequest.Headers.GetKey(i)) || string.IsNullOrEmpty(inputRequest.Headers[i]))
                {
                    continue;
                }

                if (!WebHeaderCollection.IsRestricted(key) && !key.ToLowerInvariant().StartsWith("accept"))
                {
                    if (newRequest.Headers[inputRequest.Headers.GetKey(i)] == null)
                        newRequest.Headers.Add(inputRequest.Headers.GetKey(i), inputRequest.Headers[i]);
                    else
                        newRequest.Headers.Set(inputRequest.Headers.GetKey(i), inputRequest.Headers[i]);
                }
                else if (key.ToLowerInvariant() == "accept")
                {
                    newRequest.Accept = inputRequest.Headers["Accept"];
                }

                if (key.ToLower() == "accept-encoding")
                {
                    if (newRequest.Headers[key] == null)
                        newRequest.Headers.Add(key, inputRequest.Headers[key]);
                    else
                        newRequest.Headers.Set(key, inputRequest.Headers[key]);
                }
            }

            if (isBhRequest)
            {
                newRequest.Headers.Add(APP_ROOT_HEADER, proxyUrl);
            }
        }
    }
}