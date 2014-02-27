using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PXWebAPI.App_Start
{
	/// <summary>
	/// WebApiConfig
	/// </summary>
	public static class WebApiConfig
	{
		/// <summary>
		/// Register
		/// </summary>
		/// <param name="config"></param>
		public static void Register(HttpConfiguration config)
		{
			RouteConfig.RegisterRoutes(config);

			RouteConfig.RegisterRoutes(RouteTable.Routes);


			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			FilterConfig.RegisterFilters(config);

			BundleConfig.RegisterBundles(BundleTable.Bundles);

			MediaFormattersConfig.RegisterMediaFormatters(config);

		}

		/// <summary>
		/// ConfigErrorDetailsPolicy
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static void ConfigErrorDetailsPolicy()
		{
			var errConfig = ConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
			if (errConfig == null) return;

			IncludeErrorDetailPolicy errorDetailPolicy;
			switch (errConfig.Mode)
			{
				case CustomErrorsMode.RemoteOnly:
					errorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
					break;
				case CustomErrorsMode.On:
					errorDetailPolicy = IncludeErrorDetailPolicy.Never;
					break;
				case CustomErrorsMode.Off:
					errorDetailPolicy = IncludeErrorDetailPolicy.Always;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = errorDetailPolicy;
		}


	}
}
