using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Profile;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;

namespace Bfw.PXAP.Controllers
{
    public class ProgressController : Controller
    {
        [HttpPost]
        public JsonResult StartProcess()
        {
            ProgressModel progressModel = new ProgressModel() { ProcessId = 0, Percentage = 0, Status = "" };
            IProgressService svc = new ProgressService();
            string message = "";
            Int64 processId = svc.AddUpdateProcess(progressModel, out message);

            return Json(new
            {
                ProcessId = processId,
                Message = message
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProgress()
        {
            String sProcess = Request["processId"].ToString();

            Int64 iProcessId = 0;
            Int64.TryParse(sProcess, out iProcessId);

            IProgressService svc = new ProgressService();
            ProgressModel model = svc.GetProgress(iProcessId);



            //int progress = 0;
            //progress = model.Percentage;

            return Json(new
            {
                ProcessId = model.ProcessId,
                Percentage = model.Percentage,
                Status = model.Status
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
