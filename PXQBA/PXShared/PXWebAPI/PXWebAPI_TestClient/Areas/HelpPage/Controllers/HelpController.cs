using System;
using System.Web.Http;
using System.Web.Mvc;
using PXWebAPI_TestClient.Areas.HelpPage.Models;

namespace PXWebAPI_TestClient.Areas.HelpPage.Controllers
{
	/// <summary>
	/// The controller that will handle requests for the help page.
	/// </summary>
	public class HelpController : Controller
	{
		public HelpController()
			: this(GlobalConfiguration.Configuration)
		{
		}

		public HelpController(HttpConfiguration config)
		{
			Configuration = config;
		}

		public HttpConfiguration Configuration { get; private set; }

		public System.Web.Mvc.ActionResult Index()
		{
			return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
		}

		[System.Web.Mvc.HttpGet]
		public System.Web.Mvc.ActionResult Api(object apiId)
		{
			if (!String.IsNullOrEmpty(apiId.ToString()))
			{
				HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId.ToString());
				
				if (apiModel != null)
				{
					return View(apiModel);
				}
			}

			return View("Error");
		}
	}
}