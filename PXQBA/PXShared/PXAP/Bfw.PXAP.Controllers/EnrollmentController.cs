
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Profile;
using Bfw.Common.JqGridHelper;

using Bfw.PXAP.Components;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;

namespace Bfw.PXAP.Controllers
{
    // Controller for the move Enrollment action
    public class EnrollmentController : ApplicationController
    {
        public EnrollmentController(IApplicationContext context) : base(context) { }

        public ActionResult Copy()
        {
            return View();
        }

        public ActionResult Index()
        {
            EnrollmentModel enrollmentModel = new EnrollmentModel();
            this.ViewData.Model = enrollmentModel;
            return View();
        }

        /// <summary>
        /// This is the method called by the AJAX form submit
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EnrollStudents()
        {
            //need the progress model here in order to make sure the process updates properly as the metadata is applied
            String sProcess = Request["processId"].ToString();

            Int64 iProcess = 0;
            Int64.TryParse(sProcess, out iProcess);

            //gather the form post data          
            String entityId = Request["EntityId"].ToString();
            int StudentCount = int.Parse(Request["StudentCount"].ToString());
          
            IEnrollmentService svc = new EnrollmentService(Context);
            svc.EnrollStudent(entityId, StudentCount, iProcess);            
            return Json(new
            {
                ProcessId = iProcess
            }, JsonRequestBehavior.DenyGet);
        }


    }
}
