using Macmillan.PXQBA.Common.Logging;
using Macmillan.PXQBA.Web.ActionResults;
using System;
using System.Web.Mvc;

namespace Macmillan.PXQBA.Web.Controllers
{
    public abstract class MasterController: Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string controllerName = filterContext.Controller.GetType().ToString();

            StaticLogger.LogError(String.Format("Web exception: controler='{0}',action='{1}'.", controllerName, actionName),
                filterContext.Exception);
            filterContext.ExceptionHandled = false;   
        }

        protected ActionResult JsonCamel(object data)
        {
            return new JsonCamelCaseResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}