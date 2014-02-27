using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using Bfw.PX.Biz.ServiceContracts;

using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.PXPub.Components
{
    /// <summary>
    /// Attribute that makes an action not cache, used for IE hack. This is potentially not used any more.
    /// </summary>
    public class NoCacheActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the MVC framework before the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }
}