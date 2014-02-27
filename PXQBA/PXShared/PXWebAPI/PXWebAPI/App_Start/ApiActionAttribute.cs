using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using Bfw.Common;
using Newtonsoft.Json.Linq;

namespace PXWebAPI.App_Start
{
	/// <summary>
	/// ValidationActionFilter
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class ApiActionAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext context)
		{
			var user = HttpContext.Current.User;

			//NOTE: You can skip authorization if your controller action has this attribute:[System.Web.Http.AllowAnonymous]
			var bAllowAnonymous = context.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
									  || context.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

			if (!user.Identity.IsAuthenticated & !bAllowAnonymous)
			{
				var strValidationMessage = "User is not Authenticated to execute this Action:" + context.ActionDescriptor.ActionName;
				var businessExc = new BusinessException(strValidationMessage, TraceEventType.Warning);

				businessExc.LogBusinessException(strValidationMessage, TraceEventType.Warning, "PXWebAPI");
				throw businessExc;
			}

			var modelState = context.ModelState;
			if (modelState.IsValid) return;
			var errors = new JObject();
			foreach (var key in modelState.Keys)
			{
				var state = modelState[key];
				if (!state.Errors.Any()) continue;
				errors[key] = state.Errors.First().ErrorMessage;
				Trace.Write(errors[key]);
			}
			context.Response = context.Request.CreateResponse<JObject>(HttpStatusCode.BadRequest, errors);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public override void OnActionExecuted(HttpActionExecutedContext context)
		{
			if (context.Exception != null) context.Exception.LogException(context.ActionContext.ActionDescriptor.ActionName, "PXWebAPI");

			var modelState = context.ActionContext.ModelState;
			if (modelState.IsValid) return;
			var errors = new JObject();
			foreach (var key in modelState.Keys)
			{
				var state = modelState[key];
				if (state.Errors.Any())
				{
					errors[key] = state.Errors.First().ErrorMessage;
				}
			}

			context.Response = context.Request.CreateResponse<JObject>(HttpStatusCode.BadRequest, errors);

		}


	}
}
