using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Bfw.Common;
using Bfw.PXWebAPI.Models.Response;

namespace PXWebAPI.App_Start
{

	/// <summary>
	/// PxWebApiExceptionHandlingAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
	public class ExceptionHandlingAttribute : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext context)
		{
			var ex = context.Exception;
			var action = context.ActionContext.ActionDescriptor.ActionName;
			var errMsg = ex.LogException(action, "PxWebAPI");

			var t = context.ActionContext.ActionDescriptor.ReturnType;

			//dynamic objResponse = t.Assembly.CreateInstance(t.FullName);

			var objResponse = new Response { status_code = -1, error_message = errMsg };

			context.Response = context.Request.CreateResponse(HttpStatusCode.OK, objResponse);

		}


	}



}