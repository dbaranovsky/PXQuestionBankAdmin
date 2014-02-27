using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Controllers
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
                "Default", // Route name
                "{controller}/{action}/{app}/{folder}", // URL with parameters
                new { controller = "Home", action = "Index", app = "PXPub", folder = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "CourseSectionHome", // Route name
                "{section}/{course}/{courseid}", // URL with parameters
                new { controller = "Home", action = "Index", __px__routename = "CourseSectionHome" } // Parameter defaults   
            );
        }

        //private static void RegisterViewEngines(ICollection<IViewEngine> engines)
        //{
        //    engines.Add(new WebFormViewEngine
        //    {
        //        PartialViewLocationFormats = new[] { "~/Views/DashboardCoursesWidget//{0}.ascx" },
        //        ViewLocationFormats = new[] { "~/Views/DashboardCoursesWidget//{0}.aspx" }
        //    });
        //}

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TestViewEngine());
            // Use LocalDB for Entity Framework by default
            //Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            HostingEnvironment.RegisterVirtualPathProvider(new ViewProvider());
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            //RegisterViewEngines(ViewEngines.Engines);
        }
    }
}