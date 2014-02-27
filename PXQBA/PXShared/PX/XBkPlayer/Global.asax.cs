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

using Bfw.Common.Patterns.Unity;

namespace XBkPlayer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("RALogin/RAStudent/RAg/RAgLocal.asmx");

            routes.MapRoute("PlayerAPIAction", "XBkPlayer/{action}",
                new { controller = "XBP", action = "GetCustomQuestion" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "XBP", action = "Index", id = UrlParameter.Optional, __px__routename = "Default" } // Parameter defaults
            );

            routes.MapRoute(
              "DefaultHome", // Route name
              "", // URL with parameters
              new { controller = "XBP", action = "Index", id = UrlParameter.Optional, __px__routename = "DefaultHome" } // Parameter defaults
           );
        }

        protected void Application_Start()
        {
            ConfigureServiceLocator();
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            ControllerBuilder.Current.SetControllerFactory(ServiceLocator.Current.GetInstance<IControllerFactory>());
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());

            var logger = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ILogger>();
            logger.Log("Platform-X Application Start Event", Bfw.Common.Logging.LogSeverity.Information, new List<string>() { "Application Status" });   
        }
        protected void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();

            ServiceLocator.SetLocatorProvider(() => locator);
        }
    }
}