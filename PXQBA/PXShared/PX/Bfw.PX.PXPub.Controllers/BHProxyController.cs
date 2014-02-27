using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Configuration;

using Bfw.Common.Mvc;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;

using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    /// <summary>
    /// Acts as a proxy for BrainHoney components
    /// </summary>    
    [PerfTraceFilter]
    public class BHProxyController : Controller
    {
        /// <summary>
        /// Header to add to the proxied request.
        /// </summary>
        protected const string APP_ROOT_HEADER = "X-BH-AppRoot";

        /// <summary>
        /// Context of the application.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BHProxyController"/> class.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        public BHProxyController(IBusinessContext ctx)
        {
            Context = ctx;
        }

        /// <summary>
        /// This action method proxies the current request to the brainhoney server and proxies the result back to
        /// the client.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="__px__routename">The __px__routename.</param>
        /// <returns></returns>
        public ActionResult Index(string path, string __px__routename)
        {
            ActionResult result = null;
            var routeMap = new Dictionary<string, string>();

            routeMap.Add("BHProxy", Context.EnvironmentUrl);
            routeMap.Add("PxHTSEditor", ConfigurationManager.AppSettings["DevUrl"]);

            string baseUrl = routeMap[__px__routename];
            var targetUrl = string.Format("{0}/{1}{2}", baseUrl, path, string.IsNullOrEmpty(ControllerContext.HttpContext.Request.Url.Query) ? "" : ControllerContext.HttpContext.Request.Url.Query);

            Context.Logger.Log(String.Format("Proxying to url '{0}'", targetUrl), Bfw.Common.Logging.LogSeverity.Debug);

            Uri target = new Uri(targetUrl);
            Uri proxy = new Uri(Context.ProxyUrl);

            var headers = new WebHeaderCollection();
            headers[APP_ROOT_HEADER] = Context.ProxyUrl;
            result = this.ProxyResult(target, proxy, headers);

            return result;
        }
        [OutputCache(Duration = 15552000, VaryByCustom = "debug", VaryByParam = "*")]
        public ActionResult DirectProxy(string path, string __px__routename)
        {
            ActionResult result = null;
            var routeMap = new Dictionary<string, string>();
            string baseUrl = Context.EnvironmentUrl;
            var targetUrl = string.Format("{0}/{1}{2}", baseUrl, path, string.IsNullOrEmpty(ControllerContext.HttpContext.Request.Url.Query) ? "" : ControllerContext.HttpContext.Request.Url.Query);

            Context.Logger.Log(String.Format("DirectProxying to url '{0}'", targetUrl), Bfw.Common.Logging.LogSeverity.Debug);

            Uri target = new Uri(targetUrl);
            Uri proxy = new Uri(Context.ProxyUrl);

            var headers = new WebHeaderCollection();
            headers[APP_ROOT_HEADER] = Context.ProxyUrl;
            result = this.ProxyResult(target, proxy, headers);

            return result;
        }

        /// <summary>
        /// Renders the component.
        /// </summary>
        /// <param name="componentUrl">The component URL.</param>
        /// <returns></returns>
        public ActionResult RenderComponent(string componentUrl)
        {
            ActionResult result = null;

            string targetUrl = string.Format("{0}/{1}", Context.ProxyUrl, componentUrl);
            Uri target = new Uri(targetUrl, UriKind.RelativeOrAbsolute);

            if (!target.IsAbsoluteUri)
            {
                targetUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Host, targetUrl);
                target = new Uri(targetUrl);
            }

            var webRequest = (HttpWebRequest)WebRequest.Create(target);
            var inputRequest = System.Web.HttpContext.Current.Request;

            if (webRequest.CookieContainer == null)
            {
                webRequest.CookieContainer = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
            }

            for (var i = 0; i < inputRequest.Cookies.Count; i++)
            {
                webRequest.CookieContainer.Add(target, new Cookie
                {
                    Name = inputRequest.Cookies[i].Name,
                    Value = inputRequest.Cookies[i].Value
                });
            }

            try
            {
                webRequest.CookieContainer.Add(target, new Cookie() { Name = ".ASPXAUTH", Value = Context.BhAuthCookieValue });
            }
            catch
            {
                //swallow
            }

            try
            {
                var response = webRequest.GetResponse();

                using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    result = Content(sr.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Context.Logger.Log(ex);
                result = new EmptyResult();
            }

            return result;
        }
    }
}
