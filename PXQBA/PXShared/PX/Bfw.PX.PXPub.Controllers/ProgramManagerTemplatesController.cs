using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Configuration;

using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Web.Script.Serialization;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class ProgramManagerTemplatesController : Controller
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// The Course Actions.
        /// </summary>
        /// <value>
        /// The Course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }
        
        /// <summary>
        /// Access to an IContentActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// Initializes a new instance of the <see cref="ProgramManagerTemplatesController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ProgramManagerTemplatesController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, BizSC.ICourseActions courseActions)
        {
            Context = context;
            ContentActions = contentActions;
            CourseActions = courseActions;
        }

        /// <summary>
        /// Launches the Program Manager Templates Tab.
        /// </summary>        
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Show Copy To My Templates.
        /// </summary>        
        /// <returns></returns>
        public ActionResult ShowCopyToMyTemplates(string courseId)
        {
            Course model = CourseActions.GetCourseByCourseId(courseId).ToCourse();
            return View("~/Views/ProgramManagerTemplates/CopyToMyTemplates.ascx", model);
        }

        /// <summary>
        /// Show Create My Templates.
        /// </summary>        
        /// <returns></returns>
        public ActionResult ShowCreateMyTemplate()
        {
            return View("~/Views/ProgramManagerTemplates/CreateMyTemplates.ascx");
        }
        
    }
}