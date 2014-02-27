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
    [PerfTraceFilter]
    public class ProgressWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Actions for grades.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Actions for courses.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressWidgetController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gradeActions">The grade actions.</param>
        /// <param name="courseActions">The course actions.</param>
        public ProgressWidgetController(BizSC.IBusinessContext context, BizSC.IGradeActions gradeActions, BizSC.ICourseActions courseActions)
        {
            Context = context;
            GradeActions = gradeActions;
            CourseActions = courseActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Gets the summarize progress report.
        /// </summary>
        /// <returns>
        /// ViewResult that renders a summarized list of Announcements.
        /// </returns>
        public ActionResult Summary(Models.Widget widget)
        {
            //ActionResult result = null;

            //var model = new ProgressWidget();

            //var enrollment = CourseActions.GetEnrollment(Context.CurrentUser.Id, Context.EntityId);

            //if (null != enrollment)
            //{
            //    model.OverallGrade = enrollment.OverallGrade;
            //    model.PercentGraded = GradeActions.GetPercentGraded(enrollment);

            //    var itemsDueToday = GradeActions.GetGrades(Context.CurrentUser.Id, Context.EntityId, BizSC.GradedItemStatus.Any, DateTime.MinValue, DateTime.Now.EndOfDay());
            //    var completeItemsDueToday = GradeActions.GetGrades(Context.CurrentUser.Id, Context.EntityId, BizSC.GradedItemStatus.HasBeenSubmittedOrGraded, DateTime.MinValue, DateTime.Now.EndOfDay());
                
            //    if (null != itemsDueToday && null != completeItemsDueToday)
            //    {
            //        model.Due = itemsDueToday.Count();
            //        model.Complete = completeItemsDueToday.Count();
            //    }
            //}

            //ViewData.Model = model;
            //result = View("Scorecard");


            var model = new AllProgress();

            model.EnrollmentId = Context.EnrollmentId;
            ViewData.Model = model;
            return View("StudentGradebook");

        }

        /// <summary>
        /// Displays a complete view of the gradebook.
        /// </summary>
        /// <returns>
        /// gradebook view.
        /// </returns>
        public ActionResult ViewAll(Models.Widget widget)
        {
            var model = new AllProgress();

            model.EnrollmentId = Context.EnrollmentId;
            ViewData.Model = model;
            return View("Gradebook");
        }

        #endregion
    }
}
