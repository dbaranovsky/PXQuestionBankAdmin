using System;
using System.Linq;
using System.Web.Mvc;

using Bfw.Common;
using Bfw.Common.Collections;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;


namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// 
    /// </summary>
    [PerfTraceFilter]
    public class QuickPanelController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Actions for courses.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickPanelController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public QuickPanelController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Action providing the display of the widget when it is in 'Summary' mode.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            return View("QuickPanel");
        }

        /// <summary>
        /// Action providing the display of the widget when it is in 'View All' mode.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View("QuickPanel");
        }
    }
}
