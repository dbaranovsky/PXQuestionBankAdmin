using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace PXWebAPI
{
	/// <summary>
	/// ValidationActionFilter
	/// </summary>
	/// 
	[AttributeUsage(AttributeTargets.Method)]
	public class ValidationActionFilter : ActionFilterAttribute
	{

		public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext context)
		{
			var modelState = context.ModelState;
			if (!modelState.IsValid)
			{
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


	//public class CustomAuthorizeAttribute : ActionFilterAttribute
	//{
	//    public MyUserTypes UserType { get; set; }

	//    public override void OnActionExecuting(ActionExecutingContext filterContext)
	//    {
	//        myUser user = ( (CustomControllerBase)filterContext.Controller ).User;

	//        if (!user.isAuthenticated)
	//        {
	//            filterContext.RequestContext.HttpContext.Response.StatusCode = 401;
	//        }
	//    }
	//}



	//public class CustomAuthorizeAttribute : AuthorizeAttribute
	//{
	//    private MyUser User { get; set; }

	//    public override void OnAuthorization(AuthorizationContext filterContext)
	//    {
	//        //Lazy loads the user in the controller.
	//        User = ( (MyControllerBase)filterContext.Controller ).User;

	//        base.OnAuthorization(filterContext);
	//    }

	//    protected override bool AuthorizeCore(HttpContextBase httpContext)
	//    {
	//        bool isAuthorized = false;
	//        string retLink = httpContext.Request.Url.AbsolutePath;

	//        if (User != null)
	//        {
	//            isAuthorized = User.IsValidated;
	//        }

	//        if (!isAuthorized)
	//        {
	//            //If the current request is coming in via an AJAX call,
	//            //simply return a basic 401 status code, otherwise, 
	//            //redirect to the login page.
	//            if (httpContext.Request.IsAjaxRequest())
	//            {
	//                httpContext.Response.StatusCode = 401;
	//            }
	//            else
	//            {
	//                httpContext.Response.Redirect("/login?retlink=" + retLink);
	//            }
	//        }

	//        return isAuthorized;
	//    }
	//}

}