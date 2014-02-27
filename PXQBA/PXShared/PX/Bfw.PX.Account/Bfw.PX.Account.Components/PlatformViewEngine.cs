using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bfw.PX.Account.Components
{
    /// <summary>
    /// Determines the path of a view based on an optional routing parameter that identifies the platform
    /// being used
    /// </summary>
    public class PlatformViewEngine : VirtualPathProviderViewEngine
    {
        /// <summary>
        /// The name of the route parameter that tells us the platform name
        /// </summary>
        private const string RouteParameterName = "platform";

        /// <summary>
        /// 
        /// </summary>
        public PlatformViewEngine()
        {
            /* {0} = view name or master page name
             * {1} = controller name
             */

            // create our master page location
            MasterLocationFormats = new[] {
                "~/Views/Shared/{0}.master"
            };

            // create our views and common shared locations
            ViewLocationFormats = new[] {
                "~/Views/{1}/{0}.aspx",
                "~/Views/Shared/{0}.aspx",
            };

            // create our partial views and common shared locations
            PartialViewLocationFormats = new[] {
                "~/Views/{1}/{0}.ascx",
                "~/Views/Shared/{0}.ascx"
            };
        }

        /// <summary>
        /// If there is a platform parameter in the routing information then it is used to check
        /// if a more specific view exists for than the default.
        /// For example, a default view path would be: ~/Views/Home/Index.aspx
        /// If the platform route parameter exists with value "px" then we check to see if the 
        /// following file exists: ~/px/Views/Home/Index.aspx
        /// If that path exists, then it is returned. Otherwise, the original path is returned
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private string PlatformViewPath(ControllerContext controllerContext, string path)
        {
            var platPath = path;

            if (!string.IsNullOrEmpty(platPath) && 
                (controllerContext.HttpContext.Request.QueryString != null) && 
                (controllerContext.HttpContext.Request.QueryString.AllKeys.Contains(RouteParameterName)))
            {
                var tempPath = platPath.Replace("~/", string.Format("~/{0}/", controllerContext.HttpContext.Request.QueryString[RouteParameterName]));

                if (FileExists(controllerContext, tempPath))
                {
                    platPath = tempPath;
                }
            }

            return platPath;
        }

        private string PlatformMasterPath(ControllerContext controllerContext, string path)
        {
            var platPath = path;

            if ((controllerContext.HttpContext.Request.QueryString != null) &&
                (controllerContext.HttpContext.Request.QueryString.AllKeys.Contains(RouteParameterName)))
            {
                var tempPath = string.Format("~/Views/Shared/{0}.Master", controllerContext.HttpContext.Request.QueryString[RouteParameterName]);

                if (FileExists(controllerContext, tempPath))
                {
                    platPath = tempPath;
                }
            }

            return platPath;
        }

        #region VirtualPathProvider

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var result = base.FindView(controllerContext, viewName, masterName, useCache);
            return result;
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return new WebFormView(controllerContext, PlatformViewPath(controllerContext, partialPath));
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var view = new WebFormView(controllerContext, PlatformViewPath(controllerContext, viewPath), PlatformMasterPath(controllerContext, masterPath));
            return view;
        }

        #endregion
    }
}
