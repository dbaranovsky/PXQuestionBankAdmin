using System.Collections.Generic;
using System.Web;
using System.Configuration;
using Bfw.PX.SSO.RAGetCourseSiteID;

namespace Bfw.PX.SSO
{
    public class CookieManager
    {
        public static void SetCookies(HttpContext context, string correlationId)
        {
            var ignoreFileTypes = new List<string> { ".css", ".js", ".jpg", ".gif", ".ico" };
            foreach (var type in ignoreFileTypes)
            {
                if (context.Request.FilePath.Contains(type)) return;
            }
            var webService = new RAGetAgilixCourseID();
            var requestUrl = context.Request.Url.ToString();

            var rhost = ConfigurationManager.AppSettings["RequestUrl"];
            if (!string.IsNullOrEmpty(rhost))
            {
                var uri = new System.Uri(requestUrl);
                requestUrl = string.Format("{0}://{1}{2}", uri.Scheme, rhost, uri.PathAndQuery);
            }

            var raAgilix = context.Request.Cookies.Get("RAAGILIX");
            // Call service to get RA Site data, with Agilix course ID
            var agilixCourse = webService.GetAgilixCourseID(requestUrl);

            if (agilixCourse == null) return;
            raAgilix = new HttpCookie("RAAGILIX");
            raAgilix.Values.Add("AGILIXID", agilixCourse.AgilixSiteID);
            raAgilix.Path = "/";
            context.Response.Cookies.Add(raAgilix);
            context.Request.Cookies.Add(raAgilix);

            context.Request.Cookies.Add(new HttpCookie("PXREQ", correlationId));
        }
    }
}