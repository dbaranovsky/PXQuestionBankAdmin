using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Net;

using Bfw.Common;
using Bfw.Common.Web;

namespace Bfw.Common.Mvc
{
    /// <summary>
    /// Proxies the current request to the specified Target. Adds all headers in Headers to 
    /// the proxied request.  Cookies are rewritten in both directions. The final response stream
    /// is the response from the target system.
    /// </summary>
    public class ProxyResult : ActionResult
    {
        /// <summary>
        /// Target URI to proxy the call to.
        /// </summary>
        protected Uri Target { get; set; }

        /// <summary>
        /// URI of the proxy system.
        /// </summary>
        protected Uri Proxy { get; set; }

        /// <summary>
        /// Additional headers to add to the request when requesting the proxied data.
        /// </summary>
        protected WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyResult"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="proxy">The proxy URL.</param>
        /// <param name="headers">The headers.</param>
        public ProxyResult(Uri target, Uri proxy, WebHeaderCollection headers)
        {
            Target = target;
            Proxy = proxy;
            Headers = headers;
        }

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult"/> class.
        /// </summary>
        /// <param name="context">The context in which the result is executed. The context information includes the controller, HTTP content, request context, and route data.</param>
        public override void ExecuteResult(ControllerContext context)
        {    
            var client = new System.Net.WebClient();

            bool isBinary = context.HttpContext.Request.AcceptTypes.All(s => !s.ToLowerInvariant().Contains("text"));
            bool isApplication = context.HttpContext.Request.AcceptTypes.Any(s => s.ToLowerInvariant().Contains("application")); 
            //context.HttpContext.Request.AcceptTypes.All(s => s.ToLowerInvariant().Contains("application"));

            bool isHtml = context.HttpContext.Request.AcceptTypes.Any(s => s.ToLowerInvariant().Contains("html"));
            StringBuilder sb = new StringBuilder();
            byte[] binaryContent;
            string textContent;


            if (isHtml)
            {
                sb = BuildArgaWrapper("22094", "ANGEL_astroportal__universe9e__master_296CEF1F3569A87D7774BFCC24890008", "http://pxmigration.dev.brainhoney.bfwpub.com/brainhoney");                
            }

            if (isBinary || isApplication)
            {
                try
                {
                    client.Headers = Headers;
                    binaryContent = client.DownloadData(Target);
                    context.HttpContext.Response.BinaryWrite(binaryContent);
                }
                catch (Exception ex)
                {
                    context.HttpContext.Response.Write(ex.Message);
                    context.HttpContext.Response.ContentType = "text/plain";
                    context.HttpContext.Response.StatusCode = 500;
                    return;
                }
            }
            else
            {
                textContent = client.DownloadString(Target);

                if (sb.Length > 0)
                {
                    sb.Append("</head>");
                    textContent = Regex.Replace(textContent, "</head>", sb.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                }

                context.HttpContext.Response.Write(textContent);
            }
            
            var contentType = client.ResponseHeaders["Content-Type"];

            context.HttpContext.Response.ContentType = contentType;
        }

        private static StringBuilder BuildArgaWrapper(string enrollmentId, string itemId, string brainHoneyUrl)
        {
            var sb = new StringBuilder("<script type=\"text/javascript\" language=\"javascript\">");

            sb.AppendFormat(@"
                var DEJSInput = {{ 
                    EnrollmentId: null, 
                    ItemId: null, 
                    AppRoot: null
                }};
                DEJSInput.EnrollmentId = {0}; 
                DEJSInput.ItemId = '{1}'; 
                DEJSInput.AppRoot = '{2}'; 
            ", enrollmentId, itemId, brainHoneyUrl);

            sb.Append("</script>");

            return sb;
        }
    }
}
