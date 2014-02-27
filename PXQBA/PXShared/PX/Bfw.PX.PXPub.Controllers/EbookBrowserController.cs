using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Configuration;

using Bfw.Common.Collections;
using Bfw.Common;

using Bfw.PX.Abstractions;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers {
    [PerfTraceFilter]
    public class EbookBrowserController : Controller {
        
        protected BizSC.IBusinessContext Context { get; set; }
        protected ContentHelper ContentHelper { get; set; }
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the 
        /// </summary>
        /// <param name="context">The context.</param>
        public EbookBrowserController( BizSC.IBusinessContext context, ContentHelper contentHelper,
            IContentActions contentActions )
        {
            Context = context;
            ContentHelper = contentHelper;
            ContentActions = contentActions;
        }

        public ActionResult Index( string itemId, string category )
        {
            EbookBrowser model = new EbookBrowser();
            model.Courseid = Context.CourseId;
            model.Id = itemId;
            model.CategoryId = category;
            model.UserAccess = Context.AccessLevel;
            return PartialView("Index", model);
        }

        public ActionResult EbookSelection()
        {
            var ebook = ContentActions.ListContent(Context.EntityId, "Ebook").Map(c => c.ToEbook()).ToList();
            ViewData["accessLevel"] = Context.AccessLevel; 
            return PartialView("EbookSelection", ebook);
        }

        public ActionResult Selection()
        {
            return View("Selection");
        }
    }
}
