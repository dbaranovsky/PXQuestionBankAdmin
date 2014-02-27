using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Patterns.Unity;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Web.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;
using Microsoft.Practices.ServiceLocation;
using PXWebAPI.App_Start;

namespace PXWebAPI
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	/// <summary>
	/// 
	/// </summary>
	public class WebApiApplication : System.Web.HttpApplication
	{

		protected void Application_Start()
		{
			System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;

			//Start Web API framework to use the Unity.WebAPI DependencyResolver to resolve components listed in Unity section of web.config
			Bootstrapper.Initialise();

			FederatedAuthentication.ServiceConfigurationCreated += OnServiceConfigurationCreated;

			AreaRegistration.RegisterAllAreas();

			WebApiConfig.ConfigErrorDetailsPolicy();

			WebApiConfig.Register(GlobalConfiguration.Configuration);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Include };

			ConfigureOauth();
			ConfigureWhiteListOfConsumers();

			Logger.Write("PXWebAPI Application_Start", "PxWebAPI", 1, 1, TraceEventType.Information);
		}

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			var tracer = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ITraceManager>();

			if (tracer != null) tracer.EndTracing();
		}



		protected void Application_Error(object sender, EventArgs e)
		{
			//var logger = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ILogger>();

			var ex = Server.GetLastError();

			if (IsDebug()) Debug.WriteLine(ex);

			ex = Server.GetLastError();

			Logger.Write("PXWebAPI Application_Error:" + ex.Message, "PxWebAPI", 1, 1, TraceEventType.Error);

			var httpException = ex as HttpException ?? new HttpException(500, ex.Message, ex);

			// clear error on server
			Server.ClearError();
			if (httpException.GetHttpCode() != 500) return;
			HttpContext.Current.Response.Clear();
			HttpContext.Current.ClearError();

			var routeData = new RouteData();
			routeData.Values.Add("controller", "ErrorPage");
			string action;
			switch (httpException.GetHttpCode())
			{
				case 404:
					action = "Error404";
					break;

				default:
					action = "Error500";
					break;
			}

			HttpContext.Current.Response.TrySkipIisCustomErrors = true;
			HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
		}

		protected void OnServiceConfigurationCreated(object sender, ServiceConfigurationCreatedEventArgs e)
		{
			var sessionTransforms = new List<CookieTransform>(new CookieTransform[]
            {
                new DeflateCookieTransform(),
                new RsaEncryptionCookieTransform(e.ServiceConfiguration.ServiceCertificate),
                new RsaSignatureCookieTransform(e.ServiceConfiguration.ServiceCertificate)
            });

			var sessionHandler = new SessionSecurityTokenHandler(sessionTransforms.AsReadOnly());
			e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(sessionHandler);

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
		{
			var rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
			if (rd == null) return;

			InitializeSessionManager();

			var tracer = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ITraceManager>();

			var bizContext = ServiceLocator.Current.GetInstance<IBusinessContext>();
			tracer.StartTracing();

			var currentRoute = rd.Route.ToString();

			var routeTraceName = string.Format("{0} -> {1}", currentRoute, HttpContext.Current.Request.Url);

			bizContext.Tracer.StartTrace(routeTraceName);

			bizContext.Initialize();
		}

		protected void InitializeSessionManager()
		{
			var userName = ConfigurationManager.AppSettings["DlapUserName"];
			var password = ConfigurationManager.AppSettings["DlapUserPassword"];
			var userId = ConfigurationManager.AppSettings["DlapUserId"];

			var sessionManager = ServiceLocator.Current.GetInstance<ISessionManager>();
			var session = sessionManager.StartNewSession(userName, password, false, userId);

			sessionManager.CurrentSession = session;
		}

		protected void ConfigureOauth()
		{
			var oAuthRepo = new BLTI.OAuthKeyRepository();
			Application["oauthkeys"] = oAuthRepo;
		}

		protected void ConfigureWhiteListOfConsumers()
		{
			var whiteListOfConsumersSettings = ConfigurationManager.GetSection("whiteListOfConsumers") as NameValueCollection;

			if (whiteListOfConsumersSettings == null) return;

			Application["whiteListOfConsumers"] = whiteListOfConsumersSettings.AllKeys.ToDictionary(key => key, key => whiteListOfConsumersSettings[key]);

		}


		private bool IsDebug()
		{
			bool debug = true;

			var compilation = System.Configuration.ConfigurationManager.GetSection("system.web/compilation") as System.Web.Configuration.CompilationSection;

			if (compilation != null)
			{
				debug = compilation.Debug;
			}

			return debug;
		}
	}
}