using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;

using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.Abstractions;
using Models = Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provided all actions required to implement the account widget which allows
    /// users to login, log out, and view their profile information
    /// </summary>
    [PerfTraceFilter]
    public class DashBoardWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// Access to the current business context information
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IUserActions implementation
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }
        
        /// <summary>
        /// The Course Actions.
        /// </summary>
        /// <value>
        /// The Course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Access to EporfolioCourseActions functionality
        /// </summary>
        protected BizSC.IEportfolioCourseActions EporfolioCourseActions { get; set; }

        /// <summary>
        /// Exposes the related functions for 'Course Sharing' functionality across PX
        /// </summary>
        protected BizSC.ISharedCourseActions SharedCourseActions { get; set; }

        /// <summary>
        /// Constructs a default AccountWidgetController. Depends on a business context
        /// and user actions implementation
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userActions">The user actions.</param>
        /// 
        protected BizSC.IDashboardActions DashboardActions { get; set; }


        public DashBoardWidgetController(BizSC.IBusinessContext context, BizSC.IUserActions userActions, BizSC.IEportfolioCourseActions eportfolioCourseActions,
            BizSC.ICourseActions courseActions, BizSC.ISharedCourseActions sharedCourseActions, BizSC.IDashboardActions dashboardActions)
        {
            Context = context;
            UserActions = userActions;
            EporfolioCourseActions = eportfolioCourseActions;
            CourseActions = courseActions;
            SharedCourseActions = sharedCourseActions;
            DashboardActions = dashboardActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Shows the login status of the currently logged in user. If the user is
        /// anonymous then they are considered to be not authenticated.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            var eportfolioData = new EportfolioDashboardData();
            eportfolioData = EporfolioCourseActions.GetEportfolioCourses();
            var model = new DashBoardWidget();

            model.CourseEportfolios.AddRange(eportfolioData.Eportfolio.Select(item => (new DashBoardWidgetDataLine() { Id = item.Course.Id, Name = item.Course.Title, Status = item.Status, Count = item.Count, URL = item.Course.Id.ToString(), Parent = GetParentTitle(item.Course.CourseTemplate, eportfolioData), SharedInstructors = item.SharedInstructors })));
            model.MyTemplates.AddRange(eportfolioData.EportfolioTemplate.Select(item => (new DashBoardWidgetDataLine() { Id = item.Course.Id, Name = item.Course.Title, Status = item.Status, Count = item.Count, URL = item.Course.Id.ToString() })));
            model.PublisherTemplates.AddRange(eportfolioData.EportfolioPublisherTemplate.Select(item => (new DashBoardWidgetDataLine() { Id = item.Course.Id, Name = item.Course.Title, Status = item.Status, Count = item.Count, URL = item.Course.Id.ToString() })));
			model.ProgramManagerTemplates.AddRange(eportfolioData.EportfolioProgramManagerTemplate.Select(item => (new DashBoardWidgetDataLine() { Id = item.Course.Id, Name = item.Course.Title, Status = item.Status, Count = item.Count, URL = item.Course.Id.ToString() })));

            ViewData.Model = model;
            return View();
        }

        string GetParentTitle(string parentId, EportfolioDashboardData data)
        {
            var parent  = data.EportfolioTemplate.Find(i => i.Course.Id == parentId);
            if ( parent == null )
                parent = data.EportfolioPublisherTemplate.Find(i => i.Course.Id == parentId);

            if ( parent != null )
                return parent.Course.Title;

            return "";
        }

        /// <summary>
        /// Show Copy To My Templates.
        /// </summary>        
        /// <returns></returns>
        public ActionResult ShowCopyToMyTemplates(string courseId)
        {
            Bfw.PX.PXPub.Models.Course model = CourseActions.GetCourseByCourseId(courseId).ToCourse();
            return View("~/Views/DashboardWidget/CopyToMyTemplates.ascx", model);
        }

        /// <summary>
        /// Shows all data related to the currently logged in user
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View();
        }

        /// <summary>
        /// Lists the eportfolios shared with instructor
        /// </summary>
        /// <returns></returns>
        public ActionResult SharedWithMeSummary()
        {
            var sharedCourses = SharedCourseActions.getSharedCourses(Context.CurrentUser.Id, true).Map(di => di.ToDashboardItem()).ToList();
            ViewData.Model = sharedCourses;
            return View();
        }

        public ActionResult DeleteDashboardCourse(string dashboardId, string dashboard_type)
        {

            string dashboardIdFromDb = string.Empty;
            dashboardIdFromDb = DashboardActions.GetDashboardId(Context.ProductCourseId, dashboard_type, Context.CurrentUser.ReferenceId);

            BizDC.Course course = new BizDC.Course();
            // 
            course = CourseActions.GetCourseByCourseId(dashboardId);


            if (dashboardId != null && dashboardIdFromDb != null && course != null && dashboardIdFromDb == dashboardId)
            {
                if (course.CourseOwner == Context.CurrentUser.Id)
                {
                    List<BizDC.Course> courseList = new List<BizDC.Course>();
                    courseList.Add(course);
                    CourseActions.DeleteCourses(courseList);

                }

            }
            return View();
        }
        #endregion
    }
}
