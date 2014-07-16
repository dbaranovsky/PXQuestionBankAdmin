using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Macmillan.PXQBA.Business;
using Macmillan.PXQBA.Business.QuestionParserModule;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Business.QuestionParserModule.QML;
using Macmillan.PXQBA.Business.QuestionParserModule.QTI;
using Macmillan.PXQBA.Business.QuestionParserModule.Respondus;
using Macmillan.PXQBA.Business.Services.Automapper;

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

            SetupQuestionParsers();
        }

        private void SetupQuestionParsers()
        {
            QuestionParserProvider.Clear();

            QuestionParserProvider.AddParser(new RespondusQuestionParser());
            QuestionParserProvider.AddParser(new QMLQuestionPaser());
            QuestionParserProvider.AddParser(new QTIQuestionParser());
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                var user = HttpContext.Current.User.Identity.Name;
                var businessContext = DependencyResolver.Current.GetService(typeof (IContext)) as IContext;
                //TODO: need uncomment when switching to dlap
                businessContext.Initialize(user);
            }
        }

    }
}
