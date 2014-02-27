using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;
using Microsoft.IdentityModel.Protocols.Saml2;

using Bfw.Common.Collections;
using Bfw.Common.Patterns.Unity;
using BFW.Shared.Saml;

namespace SamlTest
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
                "CourseSectionHome", // Route name
                "{course}/{section}/{courseid}", // URL with parameters
                new { controller = "Home", action = "Index", __px__routename = "CourseSectionHome" } // Parameter defaults   
            );

            routes.MapRoute(
                "CourseSectionDefault", // Route name
                "{course}/{section}/{courseid}/{controller}/{action}/{id}", // URL with parameters
                new { controller = "CourseWidget", action = "Index", id = UrlParameter.Optional, section = "bcs" } // Parameter defaults            
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ConfigureServiceLocator();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();

            var resolver = new UnityDependencyResolver(locator.Container);

            DependencyResolver.SetResolver(resolver);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));

            if (rd == null)
            {
                return;
            }
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));

            if (rd == null)
            {
                return;
            }

            System.Diagnostics.Trace.WriteLine("Application_PostAuthenticateRequest");

            if (!HttpContext.Current.Request.Path.Contains("saml/post/ac"))
            {
                var module = SamlAuthenticationModule.Current;
                var biz = ServiceLocator.Current.GetInstance<Bfw.PX.Biz.ServiceContracts.IBusinessContext>();
                biz.Initialize();
            }
        }
    }
}