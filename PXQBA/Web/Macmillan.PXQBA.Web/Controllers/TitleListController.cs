using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class TitleListController : Controller
    {
        private readonly ITitleListManagementService titleListManagementService;

        public TitleListController(ITitleListManagementService titleListManagementService)
        {
            this.titleListManagementService = titleListManagementService;
        }

        //
        // GET: /TitleList/

        public ActionResult Index()
        {
            return View();
        }

    }
}
