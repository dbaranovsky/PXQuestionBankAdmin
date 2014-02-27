using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Components;

namespace Bfw.PX.PXPub.Controllers {
    [PerfTraceFilter]
    public class CustomSettingsController : Controller
    {
        public CustomSettingsController()
        {

        }

        public ActionResult Quiz(Models.ContentItem contentItem)
        {
            ViewData.Model = contentItem;
            return View();
        }

        public ActionResult FrameComponent(Models.ContentItem contentItem)
        {
            ViewData.Model = contentItem;
            return View();
        }

        public ActionResult Default(Models.ContentItem contentItem)
        {
            return new EmptyResult();
           // ViewData.Model = contentItem;
            //return View();
        }

    }
}
