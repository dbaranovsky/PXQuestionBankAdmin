using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.Common;

namespace PXWebAPI
{
	/// <summary>
	/// RouteConfig
	/// </summary>
	public class RouteConfig
	{
		/// <summary>
		/// RegisterRoutes
		/// </summary>
		/// <param name="routes"></param>
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
			routes.Ignore("{*tinymce}", new { tinymce = @"(.*/)?tiny_mce_gzip.ashx(/.*)?" });
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("RALogin/RAStudent/RAg/RAgLocal.asmx");
			routes.IgnoreRoute("RootLogout.html");
			routes.IgnoreRoute("crossdomain.xml");

			routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Bfw.PXWebAPI.Controllers" });

		}

		/// <summary>
		/// Register
		/// </summary>
		/// <param name="config"></param>
		public static void RegisterRoutes(HttpConfiguration config)
		{
			try
			{
		//enrollment
				config.Routes.MapHttpRoute(
					name: "GetEnrollableDomains",
					routeTemplate: "enrollment/getenrollabledomains",
					defaults: new { controller = "Enrollment", action = "EnrollableDomains" }
				);

				config.Routes.MapHttpRoute(
					name: "GetUserEnrollmentId",
					routeTemplate: "enrollment/getuserenrollmentid",
					defaults: new { controller = "Enrollment", action = "UserEnrollmentId" }
				);
	
				config.Routes.MapHttpRoute(
					name: "GetEnrollees",
					routeTemplate: "enrollment/getenrollees",
					defaults: new { controller = "Enrollment", action = "Enrollees" }
				);


		//user
				config.Routes.MapHttpRoute(
					name: "GetUserEnrollments",
					routeTemplate: "user/getenrollments/{id}",
					defaults: new { controller = "User", action = "Enrollments" }
				);

				config.Routes.MapHttpRoute(
					name: "GetUserEnrollment",
					routeTemplate: "user/getenrollment/{id}",
					defaults: new { controller = "User", action = "Enrollment" }
				);

				config.Routes.MapHttpRoute(
					name: "GetUserEnrollmentDetails",
					routeTemplate: "user/enrollment/details/{id}",
					defaults: new { controller = "User", action = "Enrollment" }
				);


				config.Routes.MapHttpRoute(
					name: "GetUserDetails",
					routeTemplate: "user/getdetails/{id}",
					defaults: new { controller = "User", action = "Details" }
				);

		//grades routing
				config.Routes.MapHttpRoute(
					name: "GetGradesByEnrollment",
					routeTemplate: "grades/getgradesbyenrollment/{id}",
					defaults: new { controller = "Grades", action = "GradesByEnrollment" }
				);
				
				config.Routes.MapHttpRoute(
					name: "GetGradeDetails",
					routeTemplate: "grades/getdetails/{id}",
					defaults: new { controller = "Grades", action = "Details" }
				);

		//domain routing
				config.Routes.MapHttpRoute(
					name: "GetDomainDetails",
					routeTemplate: "domain/getdetails/{id}",
					defaults: new { controller = "Domain", action = "Details" }
				);

		//course routing
				config.Routes.MapHttpRoute(
				name: "GetCourseTerms",
				routeTemplate: "course/getterms",
				defaults: new { controller = "Course", action = "Terms" }
                );

				config.Routes.MapHttpRoute(
				name: "GetInstructors",
				routeTemplate: "course/getinstructors",
				defaults: new { controller = "Course", action = "Instructors" }
				);

				config.Routes.MapHttpRoute(
				name: "GetInstructorCourseList",
				routeTemplate: "course/getinstructorcourselist",
				defaults: new { controller = "Course", action = "InstructorCourseList" }
				);


				config.Routes.MapHttpRoute(
				name: "GetCourseDetails",
				routeTemplate: "course/getdetails/{id}",
				defaults: new { controller = "Course", action = "Details" }
				);
		
		//
				config.Routes.MapHttpRoute(
					name: "API Default",
					routeTemplate: "{controller}/{action}/{id}",
					defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional }
				);

				config.Routes.MapHttpRoute("ActionApi", "{controller}/{action}/{id}", new { id = RouteParameter.Optional });

			}
			catch (Exception ex)
			{
				ex.LogException("RegisterRoutes", "1");
			}

		}
	}
}