using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Controllers
{
    public class TestViewEngine : WebFormViewEngine
    {
        public TestViewEngine() : base()
        {
            ViewLocationFormats = new[]
            {
                "~/Views/%1/%2/{0}.aspx", 
                "~/Views/%1/%2/{0}.ascx", 
                "~/Views/%1/Shared/{0}.aspx", 
                "~/Views/%1/Shared/{0}.ascx", 
                "~/Views/%1/Shared/%2/{0}.aspx", 
                "~/Views/%1/Shared/%2/{0}.ascx", 
                "~/Views/%1/Shared/DisplayTemplates/{0}.aspx", 
                "~/Views/%1/Shared/DisplayTemplates/{0}.ascx", 
                "~/Views/%1/Shared/DisplayTemplates/%2/{0}.aspx", 
                "~/Views/%1/Shared/DisplayTemplates/%2/{0}.ascx", 
                //"~/Views/%1/Shared/EditorTemplates/{0}.aspx", 
                //"~/Views/%1/Shared/EditorTemplates/{0}.ascx", 
                //"~/Views/%1/Shared/GradingTemplates/{0}.aspx", 
                //"~/Views/%1/Shared/GradingTemplates/{0}.ascx",
            };

            PartialViewLocationFormats = ViewLocationFormats;
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            var routeData = controllerContext.Controller.ControllerContext.RouteData;
            return base.CreatePartialView(controllerContext, 
                ReplaceHolder(partialPath, routeData));
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {

            var routeData = controllerContext.Controller.ControllerContext.RouteData;
            return base.CreateView(controllerContext, ReplaceHolder(viewPath, routeData),
                ReplaceHolder(masterPath, routeData));
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            var routeData = controllerContext.Controller.ControllerContext.RouteData;
            return base.FileExists(controllerContext,
                ReplaceHolder(virtualPath, routeData));
        }

        private string ReplaceHolder(string path, RouteData data)
        {
            var app = data.Values["app"].ToString();
            object folder;
            if (data.Values.TryGetValue("folder", out folder))
                return path.Replace("%1", app).Replace("%2", folder.ToString());
            else 
                return path.Replace("%1", app).Replace("%2/", string.Empty);
        }
    }
}

    
