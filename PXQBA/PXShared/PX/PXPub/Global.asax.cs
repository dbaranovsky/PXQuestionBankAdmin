using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Patterns.Unity;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Web.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;
using Microsoft.Practices.ServiceLocation;

namespace PXPub
{

	public class MvcApplication : HttpApplication
	{
		private static string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["Bfw.PX.Comments.Data.Properties.Settings.PX_CommentsConnectionString"].ToString();

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
			routes.Ignore("{*tinymce}", new { tinymce = @"(.*/)?tiny_mce_gzip.ashx(/.*)?" });
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("RALogin/RAStudent/RAg/RAgLocal.asmx");
			routes.IgnoreRoute("RootLogout.html");
			routes.IgnoreRoute("crossdomain.xml");

			routes.MapRoute(
			"AdminMetaData",
			"{section}/{course}/{courseid}/MetaDataEditor/AdminMetaData/{id}",
			new { controller = "AdminMetaData", action = "MetaDataIndex", id = UrlParameter.Optional }
		);


			routes.MapRoute(
				"BHProxy",
				"brainhoney/{*path}",
				new { controller = "BHProxy", action = "Index", __px__routename = "BHProxy" }
			);

			routes.MapRoute(
				"HealthCheck",
				"healthcheck",
				new { controller = "HealthCheck", action = "Index", __px_routname = "HealthCheck" }
			);

			routes.MapRoute(
				"Error404",
				"ErrorPage/Error404",
				new { controller = "ErrorPage", action = "Error404", __px__routename = "Error404" }
			);

            routes.MapRoute(
                "LogError",
                "{section}/{course}/{courseid}/Log/LogJSError",
                new { controller = "Log", action = "LogJSError", __px__routename = "LogJSError" }
            );

			routes.MapRoute(
				"DirectProxy",
				"{section}/{course}/{courseid}/b/{*path}",
				new { controller = "BHProxy", action = "DirectProxy", __px__routename = "DirectProxy" }
			);

			routes.MapRoute(
				"PxHTSPlayer",
				"{section}/{course}/{courseid}/PxHTSPlayer/{*path}",
				new { controller = "Quiz", action = "GetHtsQuestionHtml", __px__routename = "PxHtsPlayer" }
			);

			routes.MapRoute(
				"PxHTSEditor",
				"BFWglobal/PxHTSEditor/{*path}",
				new { controller = "BHProxy", action = "Index", __px__routename = "PxHTSEditor" }
			);

			routes.MapRoute(
				"SessionExtended",
				"{section}/{course}/{courseid}/ac/se",
				new { controller = "Account", action = "SessionExtended", __px__routename = "SessionExtended" }
			);

			routes.MapRoute(
				"CourseList",
				"",
				new { controller = "CourseWidget", action = "ViewAll", __px__routename = "CourseList" }
			);

			routes.MapRoute(
				"SelectCourse",
				"{section}/{course}/{courseid}/SelectCourse",
				new { controller = "Home", action = "SelectCourse", __px__routename = "SelectCourse" }
			);

			routes.MapRoute(
				"EnrollCourse",
				"{section}/{course}/{courseid}/Enroll",
				new { controller = "Home", action = "Enroll", _px_routename = "EnrollCourse" }
			);

			routes.MapRoute(
				"Help",
				"{section}/{course}/{courseid}/Help",
				new { controller = "Home", action = "Help", __px__routename = "Help" }
			);

			routes.MapRoute(
				"Search",
				"{section}/{course}/{courseid}/Search",
				new { controller = "Search", action = "Index", __px__routename = "Search" }
			);

			routes.MapRoute(
				"InActiveCourse",
				"{section}/{course}/{courseid}/InActiveCourse",
				new { controller = "Home", action = "InActiveCourse", __px__routename = "InActiveCourse" }
			);

			routes.MapRoute(
				"CourseNotAdopted",
				"{section}/{course}/{courseid}/CourseNotAdopted",
				new { controller = "Home", action = "CourseNotAdopted", __px__routename = "CourseNotAdopted" }
			);

			routes.MapRoute(
				"MyBookmarks",
				"{section}/{course}/{courseid}/MyBookmarks",
				new { controller = "Bookmark", action = "Index", __px__routename = "MyBookmarks" }
			);

			routes.MapRoute(
				"TermsAndConditions",
				"{section}/{course}/{courseid}/Account/TermsAndConditions",
				new { controller = "Account", action = "TermsAndConditions", __px__routename = "TermsAndConditions" }
			);

			routes.MapRoute(
				"AssignmentCenter",
				"{section}/{course}/{courseid}/AssignmentCenter",
				new { controller = "AssignmentCenter", action = "Index", __px__routename = "AssignmentCenter" }
			);

			routes.MapRoute(
				"MyCourses",
				"{section}/{course}/{courseid}/MyCourses",
				new { controller = "Home", action = "MyCourses", __px__routename = "MyCourses" }
			);

			routes.MapRoute(
				"Login",
				"{section}/{course}/{courseid}/Account/Login",
				new { controller = "Account", action = "Login", __px__routename = "Login" }
			);

            routes.MapRoute(
                "UnauthenticatedTransfer",
                "{section}/{course}/{courseid}/ECommerce/UnauthenticatedTransfer",
                new { controller = "ECommerce", action = "UnauthenticatedTransfer", courseid = UrlParameter.Optional, __px__routename = "UnauthenticatedTransfer" }
			);            

			routes.MapRoute(
				"EcomUnauthenticated",
			   "{section}/{course}/{courseid}/ECommerce/Unauthenticated",
				new { controller = "ECommerce", action = "Unauthenticated", courseid = UrlParameter.Optional, __px__routename = "EcomUnauthenticated" }
			);

			routes.MapRoute(
				"QuestionAdmin",
				"{section}/{course}/{courseid}/QuestionAdmin",
				new { controller = "QuestionAdmin", action = "Index", __px__routename = "QuestionAdmin" }
			);

			routes.MapRoute(
				"QuestionAdminEditor",
				"{section}/{course}/{courseid}/QuestionAdmin/Editor/{id}",
				new { controller = "QuestionAdmin", action = "QuestionEditor", id = UrlParameter.Optional, __px__routename = "QuestionAdminEditor" }
			);

			routes.MapRoute(
				"QuestionAdminNewQuestion",
				"{section}/{course}/{courseid}/QuestionAdmin/NewQuestion/{questionType}/{quizId}",
				new { controller = "QuestionAdmin", action = "AddNewQuestion", questionType = UrlParameter.Optional, quizId = UrlParameter.Optional, __px__routename = "QuestionAdminNewQuestion" }
			);

			routes.MapRoute(
				"EcomEntitled",
			   "{section}/{course}/{courseid}/ECommerce/Entitled/{switchEnrollFromCourse}",
				new { controller = "ECommerce", action = "Entitled", courseid = UrlParameter.Optional, switchEnrollFromCourse = UrlParameter.Optional, __px__routename = "EcomEntitled" }
			);

			routes.MapRoute(
				"EcomNotEntitled",
			   "{section}/{course}/{courseid}/ECommerce/NotEntitled",
				new { controller = "ECommerce", action = "NotEntitled", courseid = UrlParameter.Optional, __px__routename = "EcomNotEntitled" }
			);

			routes.MapRoute(
				"EcomEnroll",
				"{section}/{course}/{courseid}/ECommerce/Enroll/{switchEnrollFromCourse}",
				new { controller = "ECommerce", action = "Enroll", courseid = UrlParameter.Optional, switchEnrollFromCourse = UrlParameter.Optional, __px__routename = "EcomEnroll" }
			);

			routes.MapRoute(
				"EcomJoin",
				"{section}/{course}/{courseid}/ECommerce/Join/{switchEnrollFromCourse}",
				new { controller = "ECommerce", action = "Join", courseid = UrlParameter.Optional, switchEnrollFromCourse = UrlParameter.Optional, __px__routename = "EcomJoin" }
			);

			routes.MapRoute(
				"EcomMyCourses",
				"{section}/{course}/{courseid}/ECommerce/CourseList",
				new { controller = "ECommerce", action = "CourseList", courseid = UrlParameter.Optional, __px__routename = "EcomMyCourses" }
			);

			routes.MapRoute(
				"Logout",
				"{section}/{course}/{courseid}/Account/Logout",
				new { controller = "Account", action = "Logout", __px__routename = "Logout" }
			);

			routes.MapRoute(
				"Assignment",
				"{section}/{course}/{courseid}/ContentWidget/DisplayItem/{id}",
				new { controller = "ContentWidget", action = "DisplayItem", id = UrlParameter.Optional, __px__routename = "Assignment" }
			);

			routes.MapRoute(
				"Skip",
				"{section}/{course}/{courseid}/item/geteq.ashx",
				new { controller = "Eq", action = "Index" }
			);

			routes.MapRoute(
				"FeaturedContentItem",
				"{section}/{course}/{courseid}/item/{id}",
				new { controller = "Content", action = "Index", id = UrlParameter.Optional, __px__routename = "FeaturedContentItem" }
			);

			routes.MapRoute(
				"FeaturedContentOverview",
				"{section}/{course}/{courseid}/FeaturedContentItem/{id}",
				new { controller = "Content", action = "Index", id = UrlParameter.Optional, __px__routename = "FeaturedContentOverview" }
			);

			routes.MapRoute(
				"DownloadDocument",
				"{section}/{course}/{courseid}/Downloads/Document/{id}",
				new { controller = "Download", action = "Document", id = UrlParameter.Optional, __px__routename = "DownloadDocument" }
			);

			routes.MapRoute(
			   "DownloadDropBoxDocument",
			   "{section}/{course}/{courseid}/Downloads/DropboxDocument/{id}",
			   new { controller = "Download", action = "DropboxDocument", id = UrlParameter.Optional, __px__routename = "DownloadDropBoxDocument" }
		   );

			routes.MapRoute(
			   "DownloadDropBoxTeacherDocument",
			   "{section}/{course}/{courseid}/Downloads/DropboxTeacherDocument/{id}",
			   new { controller = "Download", action = "DropboxTeacherDocument", id = UrlParameter.Optional, __px__routename = "DownloadDropBoxTeacherDocument" }
		   );

			routes.MapRoute(
				"CourseStyleCourseCss",
				"{section}/{course}/{courseid}/StyleCourseCss",
				new { controller = "Style", action = "CourseCss", __px__routename = "CourseStyleCourseCss" }
			);

			routes.MapRoute(
				"CourseStyleTitleCss",
				"{section}/{course}/{courseid}/StyleTitleCss",
				new { controller = "Style", action = "TitleCss", __px__routename = "CourseStyleTitleCss" }
			);

			routes.MapRoute(
				"CourseStyle",
				"{section}/{course}/{courseid}/Style/{*path}",
				new { controller = "Style", action = "Index", __px__routename = "CourseStyle" }
			);

			routes.MapRoute(
				"StoreContentDuration",
				"{section}/{course}/{courseid}/ContentWidget/StoreContentDuration",
				new { controller = "ContentWidget", action = "StoreContentDuration", __px__routename = "StoreContentDuration" }
			);

			routes.MapRoute(
				"Ping",
				"{section}/{course}/{courseid}/Home/Ping",
				new { controller = "Home", action = "Ping", __px__routename = "Ping" }
			);

			routes.MapRoute(
				"Scorecard",
				"{section}/{course}/{courseid}/Scorecard",
				new { controller = "ProgressWidget", action = "ViewAll", __px__routename = "Scorecard" }
			);

			routes.MapRoute(
				"CreateCourse", // Route Name
				"{section}/{course}/{courseid}/Course/ShowCreateCourse", // URL with parameters
				new { controller = "Course", action = "ShowCreateCourse", __px__routename = "CreateCourse" }
			);

            routes.MapRoute(
                "ShowCreateNameCourse", // Route Name
                "{section}/{course}/{productCourseId}/Course/ShowCreateNameCourse/{courseId}", // URL with parameters
                new { controller = "Course", action = "ShowCreateNameCourse", __px__routename = "ShowCreateNameCourse" }
            );

			routes.MapRoute(
				"CreateNewCourse",
				"{section}/{course}/{courseid}", // URL with parameters
				new { controller = "Home", action = "Index", __px__routename = "CreateNewCourse" } // Parameter defaults 
			);

			routes.MapRoute(
				"ProductHome", // Route name
				"{section}/{course}", // URL with parameters
				new { controller = "Home", action = "Index", __px__routename = "ProductHome" } // Parameter defaults   
			);

			routes.MapRoute(
				"CourseSectionHome", // Route name
				"{section}/{course}/{courseid}", // URL with parameters
				new { controller = "Home", action = "Index", __px__routename = "CourseSectionHome" } // Parameter defaults   
			);

			routes.MapRoute(
				"CourseCreationRedirect", // Route name
				"{section}/{course}/{courseid}", // URL with parameters
				new { controller = "Home", action = "Index", __px__routename = "CourseCreationRedirect" } // Parameter defaults   
			);

			routes.MapRoute(
				"IndexStart",
				"{section}/{course}/{courseid}/Start",
				new { controller = "Home", action = "IndexStart", __px__routename = "IndexStart" }
			);

			routes.MapRoute(
				"IndexDefault",
				"{section}/{course}/{courseid}/Home",
				new { controller = "Home", action = "IndexDefault", __px__routename = "IndexDefault" }
			);

            routes.MapRoute(
                "Dashboard",
                "{section}/{course}/{courseid}/Dashboard",
                new { controller = "Home", action = "IndexDashboard", __px__routename = "Dashboard" }
            );

            routes.MapRoute(
                "CourseSectionDefault", // Route name
                "{section}/{course}/{courseid}/{controller}/{action}/{id}", // URL with parameters
                new { controller = "CourseWidget", action = "Index", id = UrlParameter.Optional, section = "launchpad" } // Parameter defaults            
            );

			routes.MapRoute(
			   "Default", // Route name
			   "{controller}/{action}/{id}", // URL with parameters
			   new { controller = "Home", action = "Index", id = UrlParameter.Optional, __px__routename = "Default" } // Parameter defaults
			);

			routes.MapRoute(
			   "DefaultHome", // Route name
			   "", // URL with parameters
			   new { controller = "Home", action = "Index", id = UrlParameter.Optional, __px__routename = "DefaultHome" } // Parameter defaults
			);

			routes.MapRoute(
				"NotesSectionDefault",
				"{controller}/{action}/{id}",
				new { controller = "NoteLibrary", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				"NoteSettingsDefault",
				"{controller}/{action}/{id}",
				new { controller = "NoteSetting", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				"LinkLibraryDefault",
				"{controller}/{action}/{id}",
				new { controller = "LinkLibrary", action = "Index", id = UrlParameter.Optional }
			);

        }

		protected void Application_Start()
		{
			System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
			ConfigureServiceLocator();
			SetAsposeLicense();

			FederatedAuthentication.ServiceConfigurationCreated += OnServiceConfigurationCreated;

			AreaRegistration.RegisterAllAreas();
			RegisterRoutes(RouteTable.Routes);
			ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());

			var logger = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ILogger>();
			logger.Log("Platform-X Application Start Event", Bfw.Common.Logging.LogSeverity.Information, new List<string>() { "Application Status" });
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

		protected void OnServiceConfigurationCreated(object sender, ServiceConfigurationCreatedEventArgs e)
		{
			List<CookieTransform> sessionTransforms = new List<CookieTransform>(new CookieTransform[]
            {
                new DeflateCookieTransform(),
                new RsaEncryptionCookieTransform(e.ServiceConfiguration.ServiceCertificate),
                new RsaSignatureCookieTransform(e.ServiceConfiguration.ServiceCertificate)
            });

			SessionSecurityTokenHandler sessionHandler = new SessionSecurityTokenHandler(sessionTransforms.AsReadOnly());
			e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(sessionHandler);
		}

		public override string GetVaryByCustomString(HttpContext context, string custom)
		{
			var result = string.Empty;

			switch (custom)
			{
				case "debug":
					if (IsDebug())
					{
						result = Guid.NewGuid().ToString();
					}
					break;
                case "product":
                    var bussinessContext = ServiceLocator.Current.GetInstance<IBusinessContext>();
                    result = bussinessContext == null ? string.Empty : bussinessContext.ProductCourseId;
			        break;
				case "url":
					result = context.Request.RawUrl;
					break;
				default:
					result = base.GetVaryByCustomString(context, custom);
					break;
			}

			return result;
		}

		/// <summary>
		/// Converts PDF's and word documents into HTML documents.
		/// </summary>
		protected void SetAsposeLicense()
		{
			try
			{
				var assemblyPath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
				var trunkPath = new DirectoryInfo(assemblyPath).Parent.FullName;
				string asposeWordsLic = string.Format("{0}\\{1}", trunkPath, ConfigurationManager.AppSettings["AsposeWordsKeyFileName"]);
				string asposePdfLic = string.Format("{0}\\{1}", trunkPath, ConfigurationManager.AppSettings["AsposePdfKeyFileName"]);

				if (File.Exists(asposeWordsLic))
				{
					var wordLicense = new Aspose.Words.License();
					wordLicense.SetLicense(asposeWordsLic);
				}

				if (File.Exists(asposePdfLic))
				{
					var pdfLicense = new Aspose.Pdf.License();
					pdfLicense.SetLicense(asposePdfLic);
				}
			}
			catch
			{
				var logger = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ILogger>();
				logger.Log("Could not load the Aspose license", Bfw.Common.Logging.LogSeverity.Error, new List<string>() { "Application Status" });
			}
		}

		protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
		{
			var rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));

            if (rd == null || rd.Route.GetType().Name == "IgnoreRouteInternal" || rd.Values["action"].ToString().ToLower() == "logjserror") return;

			var tracer = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ITraceManager>();

			var bizContext = ServiceLocator.Current.GetInstance<IBusinessContext>();
			string routeName = "CourseSectionDefault";

			if (bizContext.Logger != null)
			{
				var pxReq = HttpContext.Current.Request.Cookies.Get("PXREQ");
				if (pxReq != null)
				{
					bizContext.Logger.CorrelationId = pxReq.Value;
				}
				if (rd.Values["__px__routename"] != null)
				{
					routeName = rd.Values["__px__routename"].ToString();
				}
			}

			var isProfiler = HttpContext.Current.Request.RawUrl.Contains("mini-profiler");

           

			// Unlike Ping, if you check for StoreContentDuration here a context will not be created which will cause error
			// in GetContent. However, like Ping we do check for storecontentduration when checking if it is an open route. 
			// (routeName != "Ping" && routeName != "StoreContentDuration")
			if (routeName != "Ping")
			{
				HttpCookie profilerCookie = HttpContext.Current.Request.Cookies["PXPROFILER"];
				if (profilerCookie != null && profilerCookie.Value.ToLowerInvariant() == bool.TrueString.ToLowerInvariant() && tracer != null)
				{
					tracer.StartTracing();
				}

				var routeTraceName = string.Format("{0} -> {1}", routeName, HttpContext.Current.Request.Url);
				bizContext.Tracer.StartTrace(routeTraceName);

				if (routeName != "CourseStyle" && routeName != "CourseStyleCourseCss" && !isProfiler)
				{
					bizContext.Initialize();
				}
			}

			var userAgent = HttpContext.Current.Request.UserAgent;
			if (userAgent == "Proxy Client Request")
			{
				//do something.
			}
			else if (routeName != "BHProxy" && routeName != "CourseStyle" && routeName != "StaticStyle" && routeName != "CourseStyleCourseCss" && routeName != "CourseStyleTitleCss" && !isProfiler)
			{
			    AvoidCachingAtRequest();
                RedirectIfNecessary(bizContext, rd, routeName);
			}
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
            var pxauth = System.Web.HttpContext.Current.Request.Cookies["pxauth"];

            if (pxauth != null && !string.IsNullOrEmpty(pxauth.Value))
            {
                var ticket = System.Web.Security.FormsAuthentication.Decrypt(pxauth.Value);
                if (ticket != null)
                {
                    if (ticket.Expired && !string.IsNullOrEmpty(ticket.Name))
                    {
                        System.Web.Security.FormsAuthentication.SetAuthCookie(ticket.Name, false);
                        var newCookie = System.Web.HttpContext.Current.Response.Cookies["pxauth"];
                        System.Web.HttpContext.Current.Request.Cookies.Set(newCookie);
                    }
                }
            }
		}

        private void RedirectIfNecessary(IBusinessContext bizContext, RouteData routeData, string routeName)
        {
            if (IsOpenRoute(routeName, routeData))
            {
                return;
            }

            if (IsQuestionAdminRoute(bizContext, routeData))
            {
                return;
            }

            RedirectIfEcommerce(bizContext, routeData, routeName);
            RedirectIfAnonymous(bizContext, routeData, routeName);
            RedirectIfNoEnrollment(bizContext, routeData);
            RedirectIfInActive(bizContext, routeData, routeName);
        }

	    /// This method is used to prevent caching of pages on the client side
        protected void AvoidCachingAtRequest()
        {
            //NOTE: Stopping IE from being a caching whore
            HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now);
            Response.Cache.SetValidUntilExpires(true);
        }

	    private bool IsQuestionAdminRoute(IBusinessContext bizContext, RouteData routeData)
		{

            if (((Request.UrlReferrer != null) && (Request.UrlReferrer.AbsoluteUri.ToLower().Contains("questionadmin")))
                || ((Request.Headers["QBA"] != null && Request.Headers["QBA"].Count() > 0)))
            { // if request coming from QBA then dont check route values
            }
            else
                if (!string.Equals(routeData.Values["controller"].ToString(), "QuestionAdmin", StringComparison.InvariantCultureIgnoreCase)) return false;
            
            if (routeData.Values["courseid"] == null) return false;

			//if (!bizContext.CourseIsProductCourse) return false;
			//if (bizContext.ProductCourseId != routeData.Values["courseid"].ToString()) return false;

			if (bizContext.AccessLevel != AccessLevel.Instructor) return false;

			if (bizContext.CurrentUser == null) return false;

			if (bizContext.CurrentUser.WebRights == null) return false;

			var QuestionAdminActions = ServiceLocator.Current.GetInstance<Bfw.PX.Biz.ServiceContracts.IQuestionAdminActions>();
			String instructorPermissionFlags = System.Configuration.ConfigurationManager.AppSettings["InstructorPermissionFlags"];
			QuestionAdminActions.CreateAdminUserEnrollment(instructorPermissionFlags);
			return true;

		}

		private string GetCourseOrDashboardUrl(IBusinessContext bizContext, RouteData routeData, string courseId)
		{
			var section = bizContext.Course.CourseSectionType.ToLowerInvariant();

			var url = string.Empty;
			if (courseId != null)
			{
				var rc = new RequestContext(new HttpContextWrapper(HttpContext.Current), routeData);
				var helper = new UrlHelper(rc);
				url = helper.RouteUrl("CourseSectionHome", new
				{
					course = bizContext.Course.SubType.ToLowerInvariant(),
					section = section,
					courseid = courseId
				});
			}

			return url;
		}
		
		private void RedirectIfAnonymous(IBusinessContext bizContext, RouteData routeData, string routeName)
		{
			if (!bizContext.IsPublicView && bizContext.IsAnonymous && !IsOpenRoute(routeName, routeData))
			{
				bizContext.Logger.Log("Redirecting user because they are anonymous and trying to access a non-ebook product", Bfw.Common.Logging.LogSeverity.Debug);
				var url = LoginUrl(bizContext, routeData);
				string redirPath = VirtualPathUtility.ToAbsolute("~/Scripts/other/login_redir.js");

				try
				{
					if (url != null)
					{
						Response.Write("<script type=\"text/javascript\" src=\"" + redirPath + "\"></script>");
						Response.Write("<script type=\"text/javascript\">set_cookie('RequestUrl', window.location, 1, '/'); window.location = '" + url + "';</script>");
						Response.End();
					}
				}
				catch (Exception ex)
				{
					bizContext.Logger.Log(string.Format("RedirectIfAnonymous encountered: {0}", ex.Message), Bfw.Common.Logging.LogSeverity.Error);
					Response.End();
				}
			}


		}

		/// <summary>
		/// This method is responsible for enforcing the ecommerce workflow depending on whether the user
		/// is authorized and entitled to the course or product being accessed.
		/// </summary>
		/// <param name="bizContext">initialized IBusinessContext</param>
		/// <param name="routeData">all routing data parsed from request</param>
		/// <param name="routeName">route that matched, if any</param>
		private void RedirectIfEcommerce(IBusinessContext bizContext, RouteData routeData, string routeName)
		{
			var rc = new RequestContext(new HttpContextWrapper(HttpContext.Current), routeData);
			var helper = new UrlHelper(rc);
			var redirectUrl = string.Empty;
		    string courseId;
			object cd = routeData.Values["courseid"];
			string controller = string.Empty;
			object controllerEntry = routeData.Values["controller"];
			string actionEntry = routeData.Values["action"].ToString();

		    if (cd != null && cd != UrlParameter.Optional)
			{
                courseId = cd.ToString();
			}
			else
			{
                courseId = bizContext.ProductCourseId;
			}

			if (controllerEntry != null && controllerEntry != UrlParameter.Optional)
			{
				controller = controllerEntry.ToString();
			}

			if (bizContext.IsPublicView)
			{
				//for public student presentation course
			}
			else if (bizContext.IsAnonymous && controllerEntry.ToString() != "ErrorPage")
			{
                if (courseId == null || courseId == bizContext.ProductCourseId)
                {
                    bizContext.Logger.Log("redirecting user becauase they are anonymous", Bfw.Common.Logging.LogSeverity.Debug);
                    //user is anonymous, and therefore Unauthenticated
                    redirectUrl = helper.RouteUrl("EcomUnauthenticated", new { courseid = courseId });
                }
                else
                {
                    ServerTransfer(helper.RouteUrl("UnauthenticatedTransfer"));
                }
			}
			else if (( bizContext.AccessLevel == AccessLevel.Student || bizContext.AccessLevel == AccessLevel.Instructor ) && routeName != "EcomEntitled")
			{
				//not anonymous and has an access level, therefore user is entitled
				if (bizContext.AccessLevel == AccessLevel.Student && bizContext.CourseIsProductCourse && routeName == "ProductHome" && actionEntry == "Index")
				{
					//redirectUrl = helper.RouteUrl("CourseSectionHome", new { course = "dashboard", section = "eportfolio", courseid = courseid });
				}
				else if ((bizContext.CourseIsProductCourse || !bizContext.IsCourseReadOnly &&
                    (string.IsNullOrEmpty(bizContext.EnrollmentId) || bizContext.EnrollmentStatus.IsNullOrEmpty() || Int32.Parse(bizContext.EnrollmentStatus) != (int)EnrollmentStatus.Active))
                    && (routeName != "CreateCourse" && routeName != "Dashboard"
                    && actionEntry != "ShowCreateNameCourse" && actionEntry != "UpdateCourse" && actionEntry != "GetContextCourse"
                    && actionEntry != "CreateCourseInDomain" && actionEntry != "Enroll" && actionEntry != "FindOnyxSchool" 
                    & actionEntry != "GetTerms" & actionEntry != "GetInstructors" & actionEntry != "Join" 
                    & actionEntry != "CourseList" & actionEntry != "NotEntitled" & actionEntry != "GetInstructorCourseList" 
                    & actionEntry != "EnrollCourse" 
                    && actionEntry != "EnrollmentConfirmation" && controller.ToLowerInvariant() != "domainselectiondialog" 
                    && actionEntry != "KeepAlive" && actionEntry != "SessionExtended" && actionEntry != "EditSandboxCourse"
                    && controller != "DashboardCoursesWidget" && controller != "InstructorConsoleWidget"
                    && controller != "AboutCourseWidget" && controller != "CourseActivationWidget"))
				{
					
					redirectUrl = helper.RouteUrl("EcomEntitled", new { courseid = courseId });
					
				}
			}
			else if (!bizContext.IsAnonymous && bizContext.AccessLevel == AccessLevel.None && routeName != "EcomNotEntitled")
			{
				//in this case, the user is authorized, but not entitled
                redirectUrl = helper.RouteUrl("EcomNotEntitled", new { courseid = courseId });
			}

		    if (bizContext.CurrentUser != null && bizContext.AccessLevel == AccessLevel.Student &&
                bizContext.Course.LmsIdRequired && bizContext.CurrentUser.ReferenceId.IsNullOrEmpty() && controller == "Home")
		    {
                redirectUrl = helper.RouteUrl("EcomJoin");
		    }

		    if (!string.IsNullOrEmpty(redirectUrl))
			{
				Response.Redirect(redirectUrl);
				Response.End();
			}
		}

		private void RedirectIfInActive(IBusinessContext bizContext, RouteData routeData, string routeName)
		{
			var courseActive = bizContext.Course != null
				&& !String.IsNullOrEmpty(bizContext.Course.ActivatedDate)
				&& (Convert.ToDateTime(bizContext.Course.ActivatedDate) != DateTime.MinValue)
                && (Convert.ToDateTime(bizContext.Course.ActivatedDate) != DateTime.MaxValue);
			if (bizContext.AccessLevel == AccessLevel.Student
                && (!courseActive && (bizContext.EnrollmentStatus.IsNullOrEmpty() || Int32.Parse(bizContext.EnrollmentStatus) != (int)EnrollmentStatus.Active))
                && !IsOpenRoute(routeName, routeData) 
                && !bizContext.ImpersonateStudent
                && (bizContext.Course.ProductCourseId != null 
                    && bizContext.Course.ProductCourseId != bizContext.Course.Id))
			{
				//students shouldn't be able to access inactive courses
				var url = InActiveOrNotAdoptedCourseUrl(bizContext, routeData);
				Response.Redirect(url, true);
			}

			if (bizContext.AccessLevel == AccessLevel.Student && bizContext.AccessType != AccessType.Adopter && !IsOpenRoute(routeName, routeData) && !bizContext.ImpersonateStudent && bizContext.Course != null && !routeName.Equals("EcomNotEntitled")) // && !bizContext.CourseIsProductCourse
			{
				var rc = new RequestContext(new HttpContextWrapper(HttpContext.Current), routeData);
				var helper = new UrlHelper(rc);
				var redirectUrl = helper.RouteUrl("EcomNotEntitled", new { courseid = bizContext.Course.Id }, Request.Url.Scheme);
				if (!string.IsNullOrEmpty(bizContext.AppDomainUrl) && Request.Url.Host.Contains("localhost"))
				{
					string find = ( Request.Url.IsDefaultPort ) ? string.Concat(Request.Url.Scheme, "://", Request.Url.Host) : string.Concat(Request.Url.Scheme, "://", Request.Url.Host, ":", Request.Url.Port);
					redirectUrl = redirectUrl.Replace(find, bizContext.AppDomainUrl);
				}

				Response.Redirect(redirectUrl, true);
			}
		}

		private bool IsOpenRoute(string routeName, RouteData routeData)
		{
			bool isOpen = false;
			try
			{
				isOpen = !( routeName != "Error404" && routeName != "HealthCheck" && routeName != "TermsAndConditions" && routeName != "InActiveCourse" && 
                    routeName != "CourseNotAdopted" && routeName != "PxHTSPlayer" && routeName != "PxHTSEditor" && routeName != "BHProxy" && 
                    routeName != "Login" && routeName != "Logout" && routeName != "CourseStyle" && routeName != "StaticStyle" && 
                    routeName != "CourseStyleCourseCss" && routeData.Values["controller"].ToString().ToLower() != "highlight" && 
                    routeData.Values["controller"].ToString().ToLower() != "header" && routeData.Values["action"].ToString().ToLower() != "ping" &&
                    routeData.Values["action"].ToString().ToLower() != "storecontentduration" && routeName != "EcomUnauthenticated" && routeName != "UnauthenticatedTransfer" &&
                    routeName != "EcomEntitled" && routeData.Values["controller"].ToString().ToLower() != "errorpage" &&
                    ( routeData.Values["controller"].ToString().ToLowerInvariant() != "Account" && 
                    routeData.Values["action"].ToString().ToLowerInvariant() != "auth" ) );
			}
			catch { };
			return isOpen;
		}
		private string InActiveOrNotAdoptedCourseUrl(IBusinessContext bizContext, RouteData routeData, string routeName = "InActiveCourse")
		{
			var url = string.Empty;
			string courseid = string.Empty;
			object cd = routeData.Values["courseid"];

			if (cd != null && cd != UrlParameter.Optional)
				courseid = cd.ToString();

			if (!string.IsNullOrEmpty(bizContext.ProductCourseId) && string.IsNullOrEmpty(courseid))
				routeData.Values["courseid"] = bizContext.ProductCourseId;

			var rc = new RequestContext(new HttpContextWrapper(HttpContext.Current), routeData);
			var helper = new UrlHelper(rc);

			return helper.RouteUrl(routeName);
		}

		private string LoginUrl(IBusinessContext bizContext, RouteData routeData)
		{
			string configUrlBasePublic = System.Configuration.ConfigurationManager.AppSettings["MarsUrlBasePublic"];

			string configLoginUrl = System.Configuration.ConfigurationManager.AppSettings["MarsPathLogin"];
			string configPathLogin = System.Configuration.ConfigurationManager.AppSettings["MarsPathLogin"];
			string configUrlPlatformValue = System.Configuration.ConfigurationManager.AppSettings["MarsUrlPlatformValue"];

			string url;
			string courseid = string.Empty;
			object cd = routeData.Values["courseid"];

			if (cd != null && cd != UrlParameter.Optional)
				courseid = cd.ToString();

			if (!string.IsNullOrEmpty(bizContext.ProductCourseId) && string.IsNullOrEmpty(courseid))
				routeData.Values["courseid"] = bizContext.ProductCourseId;

			var rc = new RequestContext(new HttpContextWrapper(HttpContext.Current), routeData);
			var helper = new UrlHelper(rc);

			if (!string.IsNullOrEmpty(configLoginUrl))
			{
				url = string.Format(configUrlBasePublic, configPathLogin, configUrlPlatformValue, Request.Url.Scheme + "://" + Request.Url.Host + helper.RouteUrl("CourseSectionHome"));
			}
			else
			{
				url = helper.RouteUrl("Login");
			}

			return url;
		}

		private void RedirectIfNoEnrollment(IBusinessContext bizContext, RouteData routeData)
		{
			string courseid = string.Empty;
			object cd = routeData.Values["courseid"];

            if (cd != null && cd != UrlParameter.Optional)
            {
                courseid = cd.ToString();
            }

            if (!bizContext.IsCourseReadOnly && !bizContext.IsPublicView && !string.IsNullOrEmpty(bizContext.ProductCourseId) && ((bizContext.EnrollmentId == null && !bizContext.IsAnonymous) || string.IsNullOrEmpty(courseid)))
            {
                routeData.Values["courseid"] = bizContext.ProductCourseId;

                if (courseid != bizContext.ProductCourseId && !(bizContext.AccessLevel == AccessLevel.Student))
                {
                    var rc = new RequestContext(new HttpContextWrapper(HttpContext.Current), routeData);
                    var helper = new UrlHelper(rc);
                    var url = helper.RouteUrl("CourseSectionHome");

                    bizContext.Logger.Log("Redirecting user because there is no enrollment for course", Bfw.Common.Logging.LogSeverity.Debug);

                    if (url != null)
                    {
                        Response.Redirect(url, true);
                    }
                }
            }
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var logger = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ILogger>();
			var ex = Server.GetLastError();

			ex.LogException("", "PxWeb");

			logger.Log(ex, Bfw.Common.Logging.LogSeverity.Error, new List<string>() { "Unhandled Exception" });

			HttpException httpException = ex as HttpException;
			if (httpException == null)
			{
				httpException = new HttpException(500, ex.Message, ex);
			}
			if (httpException != null)
			{
				// clear error on server
				Server.ClearError();
				if (httpException.GetHttpCode() == 500)
				{
					HttpContext.Current.Response.Clear();
					HttpContext.Current.ClearError();

					RouteData routeData = new RouteData();
					routeData.Values.Add("controller", "ErrorPage");
					string action;
					switch (httpException.GetHttpCode())
					{
						case 404:
							action = "Error404";
							break;

						// others if any

						default:
							action = "Error500";
							break;
					}
					routeData.Values.Add("action", action);
					routeData.Values.Add("exception", httpException);
					IController errorController = new ErrorPageController();
					errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));

					HttpContext.Current.Response.TrySkipIisCustomErrors = true;
					HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
				}
			}
		}

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			var tracer = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ITraceManager>();
			if (tracer != null)
				tracer.EndTracing();
		}

		/// <summary>
		/// Checks the compilation section in the web.config and returns true if the debug 
		/// attribute is set.
		/// </summary>
		/// <returns></returns>
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

        /// <summary>
        /// generates server transfer out of provided url
        /// </summary>
        /// <param name="url"></param>
        private void ServerTransfer(string url)
        {
            if (HttpRuntime.UsingIntegratedPipeline)
            {
                // IIS 7 integrated pipeline, does not work in VS dev server.
                HttpContext.Current.Server.TransferRequest(url, true);
            }
            else
            {
                // for VS dev server, does not work in IIS
                var cUrl = HttpContext.Current.Request.Url;
                // Create URI builder
                var uriBuilder = new UriBuilder(cUrl.Scheme, cUrl.Host, cUrl.Port, HttpContext.Current.Request.ApplicationPath);
                // Add destination URI
                uriBuilder.Path += url;
                // Because UriBuilder escapes URI decode before passing as an argument
                string path = HttpContext.Current.Server.UrlDecode(uriBuilder.Uri.PathAndQuery);
                // Rewrite path
                HttpContext.Current.RewritePath(path, true);
                IHttpHandler httpHandler = new MvcHttpHandler();
                // Process request
                httpHandler.ProcessRequest(HttpContext.Current);
            }
        }
	}
}