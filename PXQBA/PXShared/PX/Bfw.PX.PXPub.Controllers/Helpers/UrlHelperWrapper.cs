using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public class UrlHelperWrapper : IUrlHelperWrapper
    {
        private UrlHelper urlHelper;

        public UrlHelperWrapper()
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            var routeData = RouteTable.Routes.GetRouteData(context);
            urlHelper = new UrlHelper(new RequestContext(context, routeData));
        }

        public string RouteUrl(object routeValues)
        {
            return urlHelper.RouteUrl(routeValues);
        }

        public string RouteUrl(RouteValueDictionary routeValues)
        {
            return urlHelper.RouteUrl(routeValues);
        }

        public string RouteUrl(string routeName)
        {
            return urlHelper.RouteUrl(routeName);
        }

        public string RouteUrl(string routeName, object routeValues)
        {
            return urlHelper.RouteUrl(routeName, routeValues);
        }

        public string RouteUrl(string routeName, RouteValueDictionary routeValues)
        {
            return urlHelper.RouteUrl(routeName, routeValues);
        }

        public string RouteUrl(string routeName, object routeValues, string protocol)
        {
            return urlHelper.RouteUrl(routeName, routeValues, protocol);
        }

        public string RouteUrl(string routeName, RouteValueDictionary routeValues, string protocol, string hostName)
        {
            return urlHelper.RouteUrl(routeName, routeValues, protocol, hostName);
        }

        public string Action(string actionName, string controllerName, object routeValues)
        {
            return urlHelper.Action(actionName,controllerName, routeValues);
        }
    }
}
