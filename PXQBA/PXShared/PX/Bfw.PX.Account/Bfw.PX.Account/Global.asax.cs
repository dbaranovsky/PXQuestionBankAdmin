using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

using Bfw.Common.Collections;
using Bfw.Common.Patterns.Unity;
using Bfw.Common.SSO;

using Bfw.PX.Account.Abstract;

namespace PXLogin
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("RALogin/RAStudent/RAg/RAgLocal.asmx");
            routes.IgnoreRoute("RAg/RAgLocal.asmx");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        public static void RegisterViewEngines(ViewEngineCollection viewEngines)
        {
            viewEngines.Clear();
            viewEngines.Add(ServiceLocator.Current.GetInstance<IViewEngine>());
        }

        protected void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        #region Event Handlers

        protected void Application_Start()
        {
            ConfigureServiceLocator();
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            RegisterViewEngines(ViewEngines.Engines);
            ControllerBuilder.Current.SetControllerFactory(ServiceLocator.Current.GetInstance<IControllerFactory>());

            var logger = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ILogger>();
            logger.Log("Platform-X Account Application Start Event", Bfw.Common.Logging.LogSeverity.Information, new List<string>() { "Application Status" });
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var context = ServiceLocator.Current.GetInstance<IRequestContext>();
            var logger = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ILogger>();
            context.Logger = logger;
            context.Logger.Log("RequestContext logger set", Bfw.Common.Logging.LogSeverity.Information);
            context.Init();
            
            var rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            string routeName = "yo";
            if (rd != null)
            {
                if (rd.Values["controller"] != null)
                {
                    routeName = rd.Values["controller"].ToString().ToLower();
                }
                else
                {
                    routeName = "nope 2";
                }
            }
            else
            {
                routeName = "nope 1";
            }
            HttpCookie cookie = new HttpCookie("routeName", routeName);
            HttpContext.Current.Response.Cookies.Add(cookie);
            if (routeName == "Request")
            {
                HttpContext.Current.Response.Redirect("~/Account/Login");
            }
        }

        #endregion
    }
}
