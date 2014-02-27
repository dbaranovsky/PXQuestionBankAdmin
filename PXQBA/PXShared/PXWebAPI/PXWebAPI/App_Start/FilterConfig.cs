using System.Web.Http;
using System.Web.Mvc;
using PXWebAPI.App_Start;
using ApiActionFilter = PXWebAPI.App_Start.ApiActionAttribute;

namespace PXWebAPI
{
	/// <summary>
	/// FilterConfig
	/// </summary>
	public class FilterConfig
	{
		/// <summary>
		/// RegisterGlobalFilters
		/// </summary>
		/// <param name="filters"></param>
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			//HandleErrorAttribute used in MVC , but does not handle exceptions thrown by Web API controllers
			filters.Add(new HandleErrorAttribute());
		}

		/// <summary>
		/// RegisterGlobalFilters
		/// </summary>
		/// <param name="config"> </param>
		public static void RegisterFilters(HttpConfiguration config)
		{
			config.Filters.Add(new OAuthValidationAttribute());
			config.Filters.Add(new ApiActionFilter());
			config.Filters.Add(new ExceptionHandlingAttribute());

		}
	}


}