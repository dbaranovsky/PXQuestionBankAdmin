using System.Web.Mvc;


namespace Bfw.PX.PXPub.Controllers.Helpers
{
	public class AjaxMethod : ActionMethodSelectorAttribute
	{
		public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
		{	
			return controllerContext.HttpContext.Request.IsAjaxRequest();
		}
	}
}
