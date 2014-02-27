using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Bfw.Common
{
	public partial class ExceptionExtentions
	{
		/// <summary>
		/// Returns a string that contains all information about the exception including HTTP request context information.
		/// </summary>
		/// <param name="ex">Exception object.</param>
		/// <param name="businessErrMsg"> </param>
		/// <param name="traceEventType"> </param>
		/// <param name="exceptionCategory"> </param>
		/// <returns>A String containing all Exception information.</returns>
		public static string LogBusinessException(this BusinessException ex, string businessErrMsg, TraceEventType traceEventType, string exceptionCategory)
		{
			var request = HttpContext.Current.Request;
			var sb = new StringBuilder();
			sb.AppendLine(businessErrMsg);
			sb.AppendLine(ex.Message + ": " + ex.InnerException);
			sb.AppendLine("------------------------------------------------------------------------------");
			sb.AppendLine("STACK TRACE:");
			sb.AppendLine(ex.StackTrace);
			sb.AppendLine("------------------------------------------------------------------------------");
			AddRequestBody(sb, request);
			sb.AppendLine("------------------------------------------------------------------------------");

			AddRequestInfo(sb, request);

			var errMsg = sb.ToString();

			Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(errMsg, exceptionCategory, 1, 1, traceEventType);

			return errMsg;
		}

		/// <summary>
		/// Log Business Exception that contains all information about the exception including HTTP request context information.
		/// </summary>
		/// <param name="ex">Exception object.</param>
		/// <param name="method"> </param>
		/// <param name="exceptionCategory"> </param>
		/// <returns>A String containing all Exception information.</returns>
		public static string LogException(this Exception ex, string method, string exceptionCategory)
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

			var errMsg = sb.ToString();

			Logger.Write(errMsg, exceptionCategory, 1, 1, TraceEventType.Error);

			return errMsg;
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
	public static partial class ExceptionExtentions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="businessErrMsg"> </param>
		/// <param name="traceEventType"> </param>
		/// <returns></returns>
		public static BusinessException BusinessException(this Exception ex, string businessErrMsg, TraceEventType traceEventType)
		{
			return new BusinessException(businessErrMsg, traceEventType);
		}


	}

	/// <summary>
	/// 
	/// </summary>
	public class BusinessException : Exception
	{
		/// <summary>
		/// Severity
		/// </summary>
		public TraceEventType Severity { get; set; }


		/// <summary>
		/// Severity
		/// </summary>
		public string BusinessMessage { get; set; }

		/// <summary>
		/// BusinessException
		/// </summary>
		/// <param name="businessErrMsg"> </param>
		/// <param name="severity"> </param>
		/// <param name="ex"> </param>
		public BusinessException(String businessErrMsg, TraceEventType severity, Exception ex)
			: base(ex.Message, ex.InnerException)
		{
			InitBusinessException(businessErrMsg, severity);
		}


		/// <summary>
		/// BusinessException
		/// </summary>
		/// <param name="businessErrMsg"> </param>
		/// <param name="severity"> </param>
		/// <returns></returns>
		public BusinessException(String businessErrMsg, TraceEventType severity)
			: base(businessErrMsg)
		{
			InitBusinessException(businessErrMsg, severity);
		}

		/// <summary>
		/// BusinessException
		/// </summary>
		/// <param name="businessErrMsg"> </param>
		/// <param name="severity"> </param>
		/// <returns></returns>
		private void InitBusinessException(String businessErrMsg, TraceEventType severity)
		{
			BusinessMessage = businessErrMsg;
			Severity = severity;
		}


	}

}