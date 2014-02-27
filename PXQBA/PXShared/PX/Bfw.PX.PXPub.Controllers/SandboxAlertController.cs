using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Widget = Bfw.PX.PXPub.Models.Widget;
using BizDc = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class SandboxAlertController : Controller
    {
        protected BizSC.IBusinessContext Context { get; set; }
        
        /// <summary>
        /// course actions
        /// </summary>
        protected BizSC.IContentActions CourseActions { get; set; }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        public SandboxAlertController(BizSC.IContentActions courseActions, BizSC.IBusinessContext context, BizSC.IContentActions content)
        {
            CourseActions = courseActions;
            Context = context;
            ContentActions = content;
        }

        public ActionResult Index()
        {
            ViewData["isSandboxCourse"] = Context.Course.IsSandboxCourse;
            ViewData["isCourseUpdated"] = true;
            return View("SandboxAlert");
        }

        public ActionResult ViewAll()
        {
            throw new NotImplementedException();
        }
    }
}
