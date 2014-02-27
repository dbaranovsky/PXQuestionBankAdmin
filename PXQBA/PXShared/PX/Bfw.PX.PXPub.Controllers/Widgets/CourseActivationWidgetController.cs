using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Mvc;

using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using System.Globalization;
using System.Web.Routing;
using System.Web;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provides all actions necessary to support the CourseActivationWidget
    /// </summary>
    [PerfTraceFilter]
    public class CourseActivationWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// The actions for getting gradable items.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }
        /// <summary>
        /// The actions for getting gradable items.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// The actions for getting course .
        /// </summary>
        /// <value>
        /// The Course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Access to an IAssignmnetActions implementation.
        /// </summary>
        protected BizSC.IAssignmentActions AssignmentActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.  
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        protected BizSC.IDomainActions DomainActions { get; set; }

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Depends on the IBusinessContext and IGradeActions interfaces.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gradeActions">The grade actions.</param>
        /// <param name="contentActions">The content actions.</param>
        public CourseActivationWidgetController(BizSC.IBusinessContext context, BizSC.IGradeActions gradeActions, BizSC.IContentActions contentActions, BizSC.IPageActions pageActions, BizSC.IAssignmentActions assignmentActions, BizSC.IEnrollmentActions enrollmentActions, BizSC.ICourseActions courseActions, BizSC.IDomainActions domainActions)
        {
            Context = context;
            GradeActions = gradeActions;
            ContentActions = contentActions;
            AssignmentActions = assignmentActions;
            EnrollmentActions = enrollmentActions;
            PageActions = pageActions;
            this.CourseActions = courseActions;
            this.DomainActions = domainActions;
        }

        public ActionResult Summary(Widget widget)
        {
            if (Context.Course.ToCourse().IsActivated || Context.AccessLevel == BizSC.AccessLevel.Student)
            {
                return new EmptyResult();
            }
            else
            {
                Models.Course course = this.CourseActions.GetCourseByCourseId(widget.EntityID).ToCourse();
                if (String.IsNullOrWhiteSpace(course.SchoolName))
                {
                    var domain = DomainActions.GetDomainById(Context.Course.Domain.Id);
                    course.SchoolName = domain.Name;
                }

                ViewData["PossibleAcademicTerms"] = CourseActions.ListAcademicTerms().Map(i => i.ToAcademicTerm()).ToList();
                ViewData["TimeZones"] = GetTimeZones();

                return View(course);
            }
        }

        [HttpGet]
        public JsonResult Refresh()
        {
            Models.Course course = new Course();
            try
            {
                course = this.CourseActions.GetCourseByCourseId(Context.Course.Id).ToCourse();
            }
            catch {}

            return Json(course, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewAll(Widget widget)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sort the time zones by putting the time zones with "US" on top.
        /// </summary>
        /// <returns>Time zones</returns>
        private List<TimeZoneInfo> GetTimeZones()
        {
            var timezones = TimeZoneInfo.GetSystemTimeZones();
            var timezonesUs = timezones.Filter(z => z.DisplayName.Contains("(US")).OrderByDescending(z => z.BaseUtcOffset);
            var timezonesNonUs = timezones.Filter(z => !z.DisplayName.Contains("(US"));
            return timezonesUs.Concat(timezonesNonUs).ToList();
        }
    }
}
