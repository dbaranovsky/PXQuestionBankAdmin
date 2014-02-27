using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json.Linq;

namespace Bfw.PXWebAPI.Controllers
{
	/// <summary>
	/// ValidationActionFilter
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ApiActionFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext context)
		{
			var modelState = context.ModelState;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public override void OnActionExecuted(HttpActionExecutedContext context)
		{

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
