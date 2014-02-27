using System;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Routing;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Microsoft.Practices.ServiceLocation;

using Bfw.PX.PXPub.Models;
using System.Linq;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class TestController : Controller
    {
        public ActionResult index()
        {
            var doc = new DocumentToView()
            {
                ItemId = "Copy__1__bsi__8435E700_...B02__8918__C69AAA5F2D45",
                Url = "",
                HighlightType = 1,
                HighlightDescription = "Description",
                AllowComments = true,
                DisciplineId = "6696",
                NoteId = "00000000-0000-0000-0000-000000000000"
            };

            return View(doc);
        }
        public ActionResult iframe()
        {
            return View();
        }
    }
}
