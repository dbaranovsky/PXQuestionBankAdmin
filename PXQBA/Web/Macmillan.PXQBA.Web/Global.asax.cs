﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using Macmillan.PXQBA.Business;
using Macmillan.PXQBA.Business.Automapper;

namespace Macmillan.PXQBA.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private AutomapperConfigurator automapperConfigurator;

        protected void Application_Start()
        {
            automapperConfigurator = DependencyResolver.Current.GetService(typeof(AutomapperConfigurator)) as AutomapperConfigurator;
            automapperConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                var user = HttpContext.Current.User.Identity.Name;
                var businessContext = DependencyResolver.Current.GetService(typeof (IContext)) as IContext;
                businessContext.InitializeSession(user);
            }
        }

    }
}