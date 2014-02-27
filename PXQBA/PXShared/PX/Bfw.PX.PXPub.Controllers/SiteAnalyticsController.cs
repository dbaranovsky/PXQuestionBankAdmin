using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Bfw.PX.PXPub.Models;
using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    public class SiteAnalyticsController : Controller
    {
        private IBusinessContext Context { get; set; }

        public SiteAnalyticsController(IBusinessContext context)
        {
            Context = context;
        }

        public ActionResult AnalyticsBase()
        {
            ActionResult result = null;
            var provider = ConfigurationManager.AppSettings["AnalyticsProvider"];
            var model = new SiteAnalytics();

            ConfigureSiteAnalytics(model);

            switch (provider.ToLowerInvariant())
            {
                case "googleanalytics":
                    ConfigureGoogleAnalytics(model);
                    result = View("GoogleAnalytics");
                    break;

                default:
                    result = new EmptyResult();
                    break;
            }

            ViewData.Model = model;

            return result;
        }

        private void ConfigureSiteAnalytics(SiteAnalytics model)
        {
            var path = Url.RouteUrl("ProductHome");

            model.RequestDomain = Request.Url.Host;
            model.RequestPath = path;

            model.CustomParams["loggedin"] = (!Context.IsAnonymous).ToString();

            if (Context.AccessLevel == AccessLevel.Instructor)
            {
                model.CustomParams["type"] = "instructor";
            }
            else if (Context.AccessLevel == AccessLevel.Student)
            {
                model.CustomParams["type"] = "student";
            }
            else
            {
                model.CustomParams["type"] = "unentitled";
            }
        }

        private void ConfigureGoogleAnalytics(SiteAnalytics model)
        {
            model.SiteKey = ConfigurationManager.AppSettings["GoogleSiteKey"];
        }
    }
}
