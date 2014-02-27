using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Web.Mvc;

using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    public class HealthCheckController : Controller
    {
        public IAdminActions AdminActions { get; set; }

        public HealthCheckController(IAdminActions adminActions)
        {
            AdminActions = adminActions;
        }

        public ActionResult Index()
        {
            var dlapStatus = AdminActions.GetStatus();

            ViewData["DlapStatus"] = dlapStatus;

            return View();
        }
    }
}
