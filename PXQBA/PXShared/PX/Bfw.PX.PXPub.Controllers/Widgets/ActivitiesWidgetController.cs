using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Windows.Forms;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDc = Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Controllers.Helpers;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class ActivitiesWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// Implementing interfaces to utilize functionality
        /// </summary>        
        protected IBusinessContext Context { get; set; }
        protected IActivitiesWidgetActions ActivitiesWidgetActions { get; set; }
        protected IGradeActions GradeActions { get; set; }
        protected IContentActions ContentActions { get; set; }
        /// <summary>
        /// Access to an INavigationActions implementation.
        /// </summary>
        protected INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Access to an IAssignmnetActions implementation.
        /// </summary>
        protected IAssignmentActions AssignmentActions { get; set; }

        /// <summary>
        /// Access to an IRubricActions implementation.
        /// </summary>
        protected IRubricActions RubricActions { get; set; }
        /// <summary>
        /// Access to a content helper object.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// Gets or sets the course actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Exposes the related functions for 'Course Sharing' functionality across PX
        /// </summary>
        protected ISharedCourseActions SharedCourseActions { get; set; }

        protected IUserActions UserActions { get; set; }

        protected IPageActions PageActions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Business Context Interface</param>        
        /// <param name="activitiesWidgetActions">Page Actions Interface</param>
        /// <param name="gradeActions">Grade Actions Interface</param>        
        public ActivitiesWidgetController(IBusinessContext context, IActivitiesWidgetActions activitiesWidgetActions, IGradeActions gradeActions, IContentActions contentActions, INavigationActions navigationActions,
           IAssignmentActions assignmentActions, IRubricActions rubricActions, ContentHelper contentHelper, AssignmentCenterHelper assignmentCenterHelper, ICourseActions courseActions, ISharedCourseActions sharedCourseActions, IUserActions userActions, IPageActions pageActions)
        {
            Context = context;
            ActivitiesWidgetActions = activitiesWidgetActions;
            GradeActions = gradeActions;
            ContentActions = contentActions;
            NavigationActions = navigationActions;
            AssignmentActions = assignmentActions;
            RubricActions = rubricActions;
            ContentHelper = contentHelper;
            AssignmentCenterHelper = assignmentCenterHelper;
            GradeActions = gradeActions;
            CourseActions = courseActions;
            SharedCourseActions = sharedCourseActions;
            UserActions = userActions;
            PageActions = pageActions;
        }

        /// <summary>
        /// Initial view for activities widget.
        /// Loads activities from DLAP by meta-content-type. Grouped by meta-topic.
        /// </summary>
        /// <returns>Returns summary view.</returns>
        public ActionResult Summary(Models.Widget widget)
        {
            var widgetProperties = widget.Properties;
            ActivitiesWidget model = new ActivitiesWidget();

            //Fix old lc PX-4059
            ActivitiesWidgetResults activitiesResult = ActivitiesWidgetActions.LoadActivitiesByType("LearningCurve");
            var invalidLC = activitiesResult.Activities.Where(i => i.ParentId != "PX_MANIFEST" && i.AssessmentSettings != null && i.AssignmentSettings.meta_bfw_Assigned);
            foreach (BizDc.ContentItem ci in invalidLC)
            {
                ci.ParentId = "PX_MANIFEST";
                ContentActions.StoreContent(ci);
            }
            //End fix 

            model.isSortable = false;


            if (activitiesResult != null)
            {
                model = activitiesResult.ToActivitiesWidget(Context, GradeActions, ContentActions, model.isSortable);
            }
            model.EnrollmentId = Context.EnrollmentId;
            model.Userspace = Context.Domain.Userspace.ToLowerInvariant();
            model.UserAccess = Context.AccessLevel;

            string subtype = "";
            string coursetype = "";
            if (Context.Course.SubType != null)
            {
                subtype = Context.Course.SubType.ToLowerInvariant();
                coursetype = Context.Course.CourseType.ToLowerInvariant();
            }


            ViewData.Model = model;
            ViewData["UseEasyXdm"] = !Context.Course.EnableArgaUrlMapping;

            return View();
        }

        public ActionResult ViewAll(Models.Widget model)
        {

            return View();
        }

        public ActionResult ActivitiesSorted()
        {


            ActivitiesWidget model = new ActivitiesWidget();
            ActivitiesWidgetResults activitiesResult = ActivitiesWidgetActions.LoadActivitiesByType("LearningCurve");

            model = activitiesResult.ToActivitiesWidget(Context, GradeActions, ContentActions, true);

            model.isSortable = true;
            model.UserAccess = Context.AccessLevel;
            model.EnrollmentId = Context.EnrollmentId;
            model.Userspace = Context.CurrentUser.DomainId;
            ViewData.Model = model;

            ViewData["UseEasyXdm"] = !Context.Course.EnableArgaUrlMapping;
            return View("Summary");

        }

        public ActionResult Assign(string itemId, int dueYear, int dueMonth, int dueDay, int dueHour, int dueMinute, string dueAmpm, string behavior, bool? isMultipartLessons,
            string completionTrigger, string gradebookCategory, string syllabusFilter, int points, string rubricId, bool isGradeable, bool isAllowLateSubmission, bool isSendReminder,
            int reminderDurationCount, string reminderDurationType, string reminderSubject, string reminderBody, int IncludeGbbScoreTrigger, bool isHighlightLateSubmission, bool isAllowLateGracePeriod,
            long lateGraceDuration, string lateGraceDurationType, string CalculationTypeTrigger, bool isAllowExtraCredit, string groupId, string persistSorting)
        {
            bool persistSortingFlag = bool.Parse(persistSorting.ToLowerInvariant());

            ContentWidgetController contentWidgetController = new ContentWidgetController(Context, NavigationActions, ContentActions, AssignmentActions, CourseActions, ContentHelper, AssignmentCenterHelper, GradeActions, RubricActions, SharedCourseActions, UserActions);
            contentWidgetController.AssignItem(itemId, dueYear, dueMonth, dueDay, dueHour, dueMinute, dueAmpm, behavior, isMultipartLessons, completionTrigger, gradebookCategory, syllabusFilter, points, rubricId, isGradeable, isAllowLateSubmission, isSendReminder, reminderDurationCount, reminderDurationType, reminderSubject, reminderBody, IncludeGbbScoreTrigger, isHighlightLateSubmission, isAllowLateGracePeriod, lateGraceDuration, lateGraceDurationType, CalculationTypeTrigger, isAllowExtraCredit, groupId, "");

            ActivitiesWidgetResults activitiesResult = ActivitiesWidgetActions.LoadActivitiesByType("LearningCurve");
            
            ActivitiesWidget model = activitiesResult.ToActivitiesWidget(Context, GradeActions, ContentActions, persistSortingFlag);
            model.isSortable = persistSortingFlag;

            model.EnrollmentId = Context.EnrollmentId;
            model.Userspace = Context.Domain.Userspace.ToLowerInvariant();
            model.UserAccess = Context.AccessLevel;

            ViewData.Model = model;
            ViewData["UseEasyXdm"] = !Context.Course.EnableArgaUrlMapping;

            return View("Summary");
        }

        public ActionResult UpdateDueDateColumn(string itemId)
        {
            ActivitiesWidgetResults activitiesResult = new ActivitiesWidgetResults();
            var item = ContentActions.GetContent(Context.EntityId, itemId);
            activitiesResult.Activities = new List<BizDc.ContentItem>{item};
            var activities = activitiesResult.ToActivitiesWidget(Context, GradeActions, ContentActions, false);
            var activity = item.ToActivity(Context, ContentActions);

            var result = activities.GroupedActivities.Values.First().First();
            return View("DueDateColumn", result);
        }
    }

}
