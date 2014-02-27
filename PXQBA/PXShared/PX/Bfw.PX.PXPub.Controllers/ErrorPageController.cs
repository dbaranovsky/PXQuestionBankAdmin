using Bfw.Common.Exceptions;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace Bfw.PX.PXPub.Controllers
{
    public class ErrorPageController : Controller
    {
        private bool bShowErrors = false;
        public ErrorPageController()
        {
            string ShowDetailErrors = ConfigurationManager.AppSettings["ShowDetailErrors"];
            if (!string.IsNullOrEmpty(ShowDetailErrors)) bShowErrors = bool.Parse(ShowDetailErrors);
        }

        public ActionResult Error404()
        {
            ViewData["ShowErrorDetails"] = bShowErrors;

            return View("Index");
        }

        public ViewResult Error500(HttpException exception)
        {
            var view = View("Error500", exception);

            ViewData["ShowErrorDetails"] = bShowErrors;
            return view;
        }

        /* Action method to display error page on exception*/
        public ActionResult DisplayError(Exception exception)
        {
            string exceptionType = string.Empty;
            string messageTitleKey = "_MessageTitle";
            string messageKey = "_Message";

            PXException pxException = exception as PXException;
        
            /* Default User friendly Error message */
            Models.ErrorData errorData = new Models.ErrorData();
            errorData.DisplayMessageTitle = GetMessageFromConfig("Default" + messageTitleKey, "We’re sorry—there’s a technical issue with this page.");
            errorData.DisplayMessage = GetMessageFromConfig("Default" + messageKey, "We had a problem completing your request.");             
            errorData.Exception = exception;
            errorData.ShowDetailErrors = false;

            errorData.ShowDetailErrors = bShowErrors;

            /* Logic to get custom message based on the exception type */
            if (pxException != null)
            {
                exceptionType = pxException.PXExceptionType;
                /* Reading the custom message from web.config */
                if (!string.IsNullOrEmpty(exceptionType))
                {
                    errorData.DisplayMessageTitle = GetMessageFromConfig(exceptionType + messageTitleKey, errorData.DisplayMessageTitle);
                    errorData.DisplayMessage = GetMessageFromConfig(exceptionType + messageKey, errorData.DisplayMessage);
                }
            }

            return View("Error", errorData);
        }

        /* Reads value from web.config and retuns default if config option does not exist */
        private string GetMessageFromConfig(string Key, string Default)
        {
            string Message = System.Configuration.ConfigurationManager.AppSettings[Key];
            if (string.IsNullOrEmpty(Message))
                Message = Default;             
            return Message;
        }
    }
}
