using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging;


namespace PXWebAPI.App_Start
{

	/// <summary>
	/// PxWebApiExceptionHandlingAttribute
	/// </summary>
	public class PxApiExceptionHandlingAttribute : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext context)
		{

			if (context.Exception is BusinessException)
			{
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
				{
					Content = new StringContent(context.Exception.Message),
					ReasonPhrase = "Exception"
				});

			}

			// HandleErrorAttribute used in MVC , but does not handle exceptions thrown by Web API controllers.
			// NotImplExceptionFilterAttribute exception will be thrown when requested method or operation is not implemented
			// It converts NotImplementedException exceptions into HTTP status code 501, Not Implemented
			if (context.Exception is NotImplementedException)
			{
				context.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
			}


			//TODO:
			//Log Critical errors
			Debug.WriteLine(context.Exception);

			throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
			{
				Content = new StringContent("An error occurred, please try again or contact the administrator."),
				ReasonPhrase = "Critical Exception"
			});
		}


		/// <summary>
		/// Returns a string that contains all information about the exception including HTTP request context information.
		/// </summary>
		/// <param name="ex">Exception object.</param>
		/// <returns>A String containing all Exception information.</returns>
		public static void LogBusinessException(BusinessException ex)
		{
			var request = HttpContext.Current.Request;
			var sb = new StringBuilder();
			sb.AppendLine(ex.Message + ": " + ex.InnerException);
			sb.AppendLine("------------------------------------------------------------------------------");
			sb.AppendLine("STACK TRACE:");
			sb.AppendLine(ex.StackTrace);
			sb.AppendLine("------------------------------------------------------------------------------");
			AddRequestBody(sb, request);
			sb.AppendLine("------------------------------------------------------------------------------");
			AddRequestInfo(sb, request);
			Logger.Write(sb.ToString(), "PxWebApi", 1, 1, TraceEventType.Warning);


		}

		/// <summary>
		/// Log Business Exception that contains all information about the exception including HTTP request context information.
		/// </summary>
		/// <param name="ex">Exception object.</param>
		/// <param name="method"> </param>
		/// <returns>A String containing all Exception information.</returns>
		public static void LogException(Exception ex, string method)
		{
			HttpRequest request = HttpContext.Current.Request;
			var sb = new StringBuilder();
			sb.AppendLine(method);
			sb.AppendLine(ex.GetType().Name + ": " + ex.Message);
			sb.AppendLine("------------------------------------------------------------------------------");
			if (ex.InnerException != null)
			{
				sb.AppendLine(ex.InnerException.GetType().Name + ": " + ex.InnerException.Message);
				sb.AppendLine(ex.InnerException.StackTrace);
				sb.AppendLine("--------------------- End of inner exception stack trace -----------------------");
			}
			sb.AppendLine("STACK TRACE:");
			AddStackTrace(sb, ex);
			sb.AppendLine("------------------------------------------------------------------------------");
			AddRequestBody(sb, request);
			sb.AppendLine("------------------------------------------------------------------------------");
			AddRequestInfo(sb, request);
			Logger.Write(sb.ToString(), "PxWebApi", 1, 1, TraceEventType.Error);

		}

		private static void AddStackTrace(StringBuilder contents, Exception ex)
		{
			contents.AppendLine(ex.StackTrace);
		}

		private static void AddRequestBody(StringBuilder contents, HttpRequest request)
		{
			contents.AppendLine("REQUEST CONTEXT:");
			contents.Append(request.RequestContext.ToString());
		}

		private static void AddRequestInfo(StringBuilder contents, HttpRequest request)
		{
			contents.AppendLine("REQUEST INFO:");
			contents.AppendLine("  HttpMethod " + request.HttpMethod);
			contents.AppendLine("  RequestType " + request.RequestType);
			contents.AppendLine("  RouteData " + request.RequestContext.RouteData.Values);
			contents.AppendLine("  RawUrl " + request.RawUrl);

			contents.AppendLine("  Application path: " + request.ApplicationPath);
			if (request.Browser != null)
			{
				contents.AppendLine("  Browser name: " + request.Browser.Browser);
				contents.AppendLine("  Is a crawler: " + request.Browser.Crawler.ToString(CultureInfo.InvariantCulture));
			}
			contents.AppendLine("  Content type: " + request.ContentType);
			contents.AppendLine("  HTTP verb: " + request.HttpMethod);

			if (request.UrlReferrer != null) contents.AppendLine("  Referrer: " + request.UrlReferrer);

			contents.AppendLine("  User agent: " + request.UserAgent);

			if (request.UserHostAddress != null) contents.AppendLine("  User IP: " + request.UserHostAddress);

			contents.AppendLine("  User Host Name: " + request.UserHostName);

			contents.AppendLine("------------------------------------------------------------------------------");
			contents.AppendLine("HEADERS PARAMETERS:");
			AddRequestParameters(contents, request.Headers);
			contents.AppendLine("------------------------------------------------------------------------------");
			contents.AppendLine("QUERY STRING PARAMETERS:");
			AddRequestParameters(contents, request.QueryString);
			contents.AppendLine("------------------------------------------------------------------------------");
			contents.AppendLine("FORM PARAMETERS:");
			AddRequestParameters(contents, request.Form);
			contents.AppendLine("------------------------------------------------------------------------------");
			contents.AppendLine("COOKIES:");
			AddCookies(contents, request.Cookies);
			contents.AppendLine("------------------------------------------------------------------------------");
			contents.AppendLine("SERVER VARIABLES:");
			AddRequestParameters(contents, request.ServerVariables);
			contents.AppendLine("------------------------------------------------------------------------------");

		}

		private static void AddRequestParameters(StringBuilder contents, NameValueCollection paramCollection)
		{
			if (paramCollection == null) return;
			for (int i = 0; i < paramCollection.Count; i++)
			{
				if (!string.IsNullOrEmpty(paramCollection.Get(i)))
				{
					contents.AppendLine(string.Concat(paramCollection.GetKey(i), " : ", paramCollection.Get(i)));
				}
			}
		}

		private static void AddCookies(StringBuilder contents, HttpCookieCollection cookieCollection)
		{
			if (cookieCollection == null) return;
			for (int i = 0; i < cookieCollection.Count; i++)
			{
				contents.AppendLine(string.Concat(cookieCollection.GetKey(i), " : ", cookieCollection.Get(i).Value));
			}
		}

	}


	/// <summary>
	/// 
	/// </summary>
	public static class ExceptionExtentions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static BusinessException BusinessException(this Exception ex)
		{
			return new BusinessException(ex);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class BusinessException : Exception
	{
		public BusinessException(Exception ex)
		{
			//LOG THE ERROR HERE
		}
	}



}