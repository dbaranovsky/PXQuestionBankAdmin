using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Bfw.Common.Logging;
using Bfw.PX.Biz.ServiceContracts;


namespace Bfw.PX.PXPub.Controllers
{
    public class LogController : Controller
    {        
        protected ILogger Logger;

        public LogController(ILogger logger)
        {
            Logger = logger;     
        }

        public JsonResult LogJSError (string errorName, string errorMessage)
        {
            try
            {
                var sb = new StringBuilder();
                
                sb.AppendLine("Javascript Client Error : " + errorName);
                sb.AppendLine("------------------------------------------------------------------------------");
                sb.AppendLine(errorMessage);
                
                var errMsg = sb.ToString();
                var category = new List<string> {"PXWeb_JS"};                

                Logger.Log(errMsg, LogSeverity.Error, category);

                return Json(new { status = "true" });
            }
            catch
            {
                return Json(new { status = "false" });
            }
        }

    }
}
