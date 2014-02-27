using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Direct.Services;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provides all actions necessary to support the FneMenuWidget.
    /// </summary>
    [PerfTraceFilter]
    public class FneMenuWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// The Courses actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the page actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Depends on the IBusinessContext and ICourseActions interfaces.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="courseActions">The course actions.</param>
        /// <param name="enrollmentActions">The enrollment actions.</param>
        public FneMenuWidgetController(BizSC.IBusinessContext context, BizSC.ICourseActions courseActions, BizSC.IEnrollmentActions enrollmentActions, BizSC.IPageActions pageActions)
        {
            Context = context;
            CourseActions = courseActions;
            EnrollmentActions = enrollmentActions; 
            PageActions = pageActions;
        }

        #region IPXWidget Members

        public ActionResult Summary(Models.Widget widget)
        {
            widget = PageActions.GetWidget(widget.Id).ToWidgetItem();
            
            return View("Summary", widget);
        }

        public ActionResult ViewAll(Models.Widget model)
        {
            return View("Summary", model);
        }

        #endregion
    }
}
