using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PXWebAPI_TestClient
{
	public static class WebApiConfig
	{


		public static void Register(HttpConfiguration config)
		{
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

		}


		public static void ConfigureApiFormatters(HttpConfiguration config)
		{
			var xmlFormatter = new System.Net.Http.Formatting.XmlMediaTypeFormatter();
			var jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
			
			jsonFormatter.SerializerSettings = new JsonSerializerSettings
						                   	{
						                   		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
						                   		NullValueHandling = NullValueHandling.Ignore,
						                   		MissingMemberHandling = MissingMemberHandling.Ignore,
						                   		Error = delegate(object sender, ErrorEventArgs args)
						                   		        	{
						                   		        		new List<string>().Add(args.ErrorContext.Error.Message);
						                   		        		args.ErrorContext.Handled = true;
						                   		        	}
						                   	};




			var formUrlEncodedFormatter = new System.Net.Http.Formatting.FormUrlEncodedMediaTypeFormatter();

			if (!config.Formatters.Contains(xmlFormatter)) config.Formatters.Add(xmlFormatter);
			if (!config.Formatters.Contains(jsonFormatter)) config.Formatters.Add(jsonFormatter);
			if (!config.Formatters.Contains(formUrlEncodedFormatter)) config.Formatters.Add(formUrlEncodedFormatter);
			//config.Formatters.XmlFormatter.UseXmlSerializer = true;
		}
	}
}
