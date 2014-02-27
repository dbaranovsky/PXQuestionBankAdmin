using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;
using Microsoft.Practices.Unity;

using UnityServiceLocator = Bfw.Common.Patterns.Unity.UnityServiceLocator;
using Bfw.PXAP.Components;

namespace PxAdminPortal
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

            routes.MapRoute(
             "AccountRoute", // Route name
             "Account/{action}", // URL with parameters
             new { controller = "Account", action = "LogOn" } // Parameter defaults
          );

            // handles urls like domain/dev/controllder/action
            routes.MapRoute(
               "EnvironmentBasedRoute", // Route name
               "{environment}/{controller}/{action}", // URL with parameters
               new { environment="DEV", controller = "Home", action = "Index" } // Parameter defaults
            );

          
        }

        private void SetupServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
            locator.Container.RegisterType<IUserRepository, UserRepository>();

			locator.Container.RegisterType<IPxWebUserRepository, PxWebUserRepository>();

            var resolver = new UnityDependencyResolver(locator.Container);

            ServiceLocator.SetLocatorProvider(() => locator);
            DependencyResolver.SetResolver(resolver);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            SetupServiceLocator();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.IsLocal)
            {
                //MvcMiniProfiler.MiniProfiler.Start();
            }

            var tracer = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ITraceManager>();
            var context = ServiceLocator.Current.GetInstance<Bfw.PXAP.Components.IApplicationContext>();

            var rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));

            if (rd != null)
            {
                object environmentParameter = rd.Values["environment"];

                if (environmentParameter != null)
                {
                    context.Environment = environmentParameter.ToString();
                }
            }

            if (tracer != null)
                tracer.StartTracing();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //MvcMiniProfiler.MiniProfiler.Stop();

            var tracer = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ITraceManager>();
            if (tracer != null)
                tracer.EndTracing();
        }
    }
}