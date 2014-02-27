using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;

namespace Bfw.PX.PXPub.Components
{
    /// <summary>
    /// This filter can be applied to a Controller or an Action Method
    /// it will compress the response of the action method. It checks if the browser supports compression
    /// by looking at "Accept-Encoding"  in the request header.
    /// </summary>
    public class CompressActionResultFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Is called on executing ation method and send response as compressed
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            HttpRequestBase request = actionContext.HttpContext.Request;
        
            string acceptEncoding = request.Headers["Accept-Encoding"];

            if (!string.IsNullOrWhiteSpace(acceptEncoding))
            {
                acceptEncoding = acceptEncoding.ToLowerInvariant();
                HttpResponseBase response = actionContext.HttpContext.Response;

                if (acceptEncoding.Contains("gzip"))
                {
                    response.AddHeader("Content-encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
                else if (acceptEncoding.Contains("deflate"))
                {
                    response.AddHeader("Content-encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
            }
        }
    }
}
