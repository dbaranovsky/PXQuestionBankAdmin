using System;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Common.Logging;
using Macmillan.PXQBA.Web.ActionResults;
using Macmillan.PXQBA.Web.Helpers;
using NLog;

namespace Macmillan.PXQBA.Web.Controllers
{
    public abstract class MasterController: Controller
    {
        private readonly IProductCourseManagementService productCourseManagementService;
        private readonly IUserManagementService userManagementService;

        protected MasterController()
        {
        }

        protected MasterController(IProductCourseManagementService productCourseManagementService, IUserManagementService userManagementService)
        {
            this.productCourseManagementService = productCourseManagementService;
            this.userManagementService = userManagementService;
        }

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

        /// <summary>
        /// Update current course in session
        /// </summary>
        /// <param name="courseId"></param>
        protected void UpdateCurrentCourse(string courseId)
        {
            if (CourseHelper.NeedGetCourse(courseId))
            {
                CourseHelper.CurrentCourse = productCourseManagementService.GetProductCourse(courseId, true);
                UserCapabilitiesHelper.Capabilities = userManagementService.GetUserCapabilities(courseId);
            }
        }
    }
}