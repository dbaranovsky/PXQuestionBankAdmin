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

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provides all actions necessary to support the CourseWidget.
    /// </summary>
    [PerfTraceFilter]
    public class CourseWidgetController : Controller, IPXWidget
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
        /// Depends on the IBusinessContext and ICourseActions interfaces.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="courseActions">The course actions.</param>
        /// <param name="enrollmentActions">The enrollment actions.</param>
        public CourseWidgetController(BizSC.IBusinessContext context, BizSC.ICourseActions courseActions, BizSC.IEnrollmentActions enrollmentActions)
        {
            Context = context;
            CourseActions = courseActions;
            EnrollmentActions = enrollmentActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Gets a summarized list of all Courses for the current entity and
        /// renders them in a view.
        /// </summary>
        /// <returns>
        /// ViewResult that renders a summarized list of Courses
        /// </returns>
        public ActionResult Summary(Models.Widget widget)
        {
            ActionResult result = null;
            result = View();
            return result;
        }

        /// <summary>
        /// Lists all Courses
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            ActionResult result = null;
            ViewData.Model = AllCourses();
            result = View("CourseWidget" );
            return result;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete()
        {
            string id = RouteData.Values["id"].ToString();
            Course course = new Course();

            if (id != "-1")
            {
                course = CourseActions.GetCourseByCourseId(id).ToCourse();
                CourseActions.DeleteCourses(ConvertToDataContractCourses(course));

                ViewData.Model = AllCourses();
            }

            return View("ViewAll");
        }

        /// <summary>
        /// Fill an CourseWidget model with all Courses.
        /// </summary>
        /// <returns></returns>
        protected CourseWidget AllCourses()
        {
            var biz = CourseActions.ListCourses();
            var model = new CourseWidget();

            List<Course> courses = new List<Course> ();

            if (!biz.IsNullOrEmpty())
            {
                courses = biz.Map(b => b.ToCourse()).ToList();
            }

            model.Courses = new List<Course>();

            if (Context.CurrentUser != null)
            {
                foreach (var item in courses)
                {
                    if (!String.IsNullOrEmpty(EnrollmentActions.GetUserEnrollmentId(Context.CurrentUser.Id, item.Id)))
                    {
                        model.Courses.Add(item);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Mosts the recent.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        protected CourseWidget MostRecent(int n)
        {
            var biz = CourseActions.ListCourses();
            var model = new CourseWidget();

            if (!biz.IsNullOrEmpty())
            {
                model.Courses = biz.Map(b => b.ToCourse()).ToList();
            }

            return model;
        }

        /// <summary>
        /// Converts to data contract courses.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        List<BizDC.Course> ConvertToDataContractCourses(Course course)
        {
            List<BizDC.Course> courses = new List<Bfw.PX.Biz.DataContracts.Course>();
            BizDC.Course bizCourse = course.ToCourse();
            bizCourse.Domain = Context.Domain;
            courses.Add(bizCourse);
            return courses;
        }

        #endregion
    }
}
