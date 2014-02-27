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
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class AssignmentListWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// Contains business layer context information
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IProjectActions ProjectViewerActions { get; set; }

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        protected BizSC.IActivitiesWidgetActions ActivitiesWidgetActions { get; set; }
        protected BizSC.IGradeActions GradeActions { get; set; }
        protected BizSC.IContentActions ContentActions { get; set; }
        /// <summary>
        /// Access to an INavigationActions implementation.
        /// </summary>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Access to an IAssignmnetActions implementation.
        /// </summary>
        protected BizSC.IAssignmentActions AssignmentActions { get; set; }

        /// <summary>
        /// Access to an IRubricActions implementation.
        /// </summary>
        protected BizSC.IRubricActions RubricActions { get; set; }

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
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Exposes the related functions for 'Course Sharing' functionality across PX
        /// </summary>
        protected BizSC.ISharedCourseActions SharedCourseActions { get; set; }

        protected BizSC.IUserActions UserActions { get; set; }


        public AssignmentListWidgetController(BizSC.IBusinessContext context, BizSC.IActivitiesWidgetActions activitiesWidgetActions, BizSC.IGradeActions gradeActions, BizSC.IContentActions contentActions, BizSC.INavigationActions navigationActions,
           BizSC.IAssignmentActions assignmentActions, BizSC.IRubricActions rubricActions, ContentHelper contentHelper, AssignmentCenterHelper assignmentCenterHelper, BizSC.ICourseActions courseActions, BizSC.ISharedCourseActions sharedCourseActions, BizSC.IUserActions userActions, BizSC.IPageActions pageActions)
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

        public ActionResult Summary(Models.Widget widget)
        {
            CarouselResults model = new CarouselResults();

            var widgetProperties = widget.Properties;

            if (widgetProperties.ContainsKey("bfw_loadbytoc") && widgetProperties["bfw_loadbytoc"].Value.ToString().ToLowerInvariant() == "true")
            {

                var bizContentItemsToc = ContentActions.ListChildren(Context.EntityId, "PX_PROJECT_ID", 5, "bfw_toc_projects", false);

                foreach (var i in bizContentItemsToc.Map(i => i.ToContentItem(ContentActions)).ToList())
                {
                    var dueDateFormatted = i.DueDate.ToString().Substring(0, i.DueDate.ToString().IndexOf(' '));
                    if (i.IsAssigned)
                    {
                        model.CarouselItems.Add(i);
                    }
                }
            }
            else
            {
                var bizContentItems = ContentActions.ListContent(Context.EntityId, "Project").ToList();
           
                foreach (var i in bizContentItems.Map(i => i.ToContentItem(ContentActions)).ToList())
                {
                    var dueDateFormatted = i.DueDate.ToString().Substring(0, i.DueDate.ToString().IndexOf(' '));

                    var defaultDate = "1/1/0001";
                    if (i.IsAssigned)
                    {
                        model.CarouselItems.Add(i);
                    }
                }
            }

            model.UserAccess = Context.AccessLevel;
            model.CarouselItems = model.CarouselItems.OrderBy(i => i.DueDate).ToList();

            int startPosWithoutDueDate = model.CarouselItems.FindLastIndex(i => i.DueDate.Year == 0001 && i.IsAssigned == true);
            int endPosWithoutDueDate = model.CarouselItems.FindIndex(i => i.DueDate.Year == 0001 && i.IsAssigned == true);

            if (startPosWithoutDueDate > -1 && endPosWithoutDueDate > -1)
            {
                List<Models.ContentItem> listOfassignedItemsWithNoDueDate = new List<Models.ContentItem>(model.CarouselItems.GetRange(endPosWithoutDueDate, ((startPosWithoutDueDate - endPosWithoutDueDate) + 1)));

                foreach (Models.ContentItem content in listOfassignedItemsWithNoDueDate)
                {
                    model.CarouselItems.Remove(content);
                    model.CarouselItems.Add(content);
            
                }

            }
            ViewData.Model = model;
            
            return View();
        }

        public ActionResult ViewAll(Models.Widget model)
        {
            throw new NotImplementedException();
        }

        public ActionResult RefreshActivitiesListWidget(string itemId, int dueYear, int dueMonth, int dueDay, int dueHour, int dueMinute, string dueAmpm, string behavior, bool? isMultipartLessons,
           string completionTrigger, string gradebookCategory, string syllabusFilter, int points, string rubricId, bool isGradeable, bool isAllowLateSubmission, bool isSendReminder,
           int reminderDurationCount, string reminderDurationType, string reminderSubject, string reminderBody, int IncludeGbbScoreTrigger, bool isHighlightLateSubmission, bool isAllowLateGracePeriod,
           long lateGraceDuration, string lateGraceDurationType, string CalculationTypeTrigger, bool isAllowExtraCredit, string groupId, string instructions)
        {


            ContentWidgetController contentWidgetController = new ContentWidgetController(Context, NavigationActions, ContentActions, AssignmentActions, CourseActions, ContentHelper, AssignmentCenterHelper, GradeActions, RubricActions, SharedCourseActions, UserActions);
            contentWidgetController.AssignItem(itemId, dueYear, dueMonth, dueDay, dueHour, dueMinute, dueAmpm, behavior, isMultipartLessons, completionTrigger, gradebookCategory, syllabusFilter, points, rubricId, isGradeable, isAllowLateSubmission, isSendReminder, reminderDurationCount, reminderDurationType, reminderSubject, reminderBody, IncludeGbbScoreTrigger, isHighlightLateSubmission, isAllowLateGracePeriod, lateGraceDuration, lateGraceDurationType, CalculationTypeTrigger, isAllowExtraCredit, groupId, instructions);


            var widget = PageActions.GetWidget("PX_AssignmentList").ToWidgetItem();
            

            CarouselResults model = new CarouselResults();

            var widgetProperties = widget.Properties;

            if (widgetProperties.ContainsKey("bfw_loadbytoc") && widgetProperties["bfw_loadbytoc"].Value.ToString().ToLowerInvariant() == "true")
            {

                var bizContentItemsToc = ContentActions.ListChildren(Context.EntityId, "PX_PROJECT_ID", 5, "bfw_toc_projects", false);

                foreach (var i in bizContentItemsToc.Map(i => i.ToContentItem(ContentActions)).ToList())
                {
                    var dueDateFormatted = i.DueDate.ToString().Substring(0, i.DueDate.ToString().IndexOf(' '));
                    if (i.IsAssigned)
                    {
                        model.CarouselItems.Add(i);
                    }
                }
            }
            else
            {
                var bizContentItems = ContentActions.ListContent(Context.EntityId, "Project").ToList();

                foreach (var i in bizContentItems.Map(i => i.ToContentItem(ContentActions)).ToList())
                {
                    var dueDateFormatted = i.DueDate.ToString().Substring(0, i.DueDate.ToString().IndexOf(' '));
                    if (i.IsAssigned)
                    {
                        model.CarouselItems.Add(i);
                    }
                }
            }
            model.UserAccess = Context.AccessLevel;
            model.CarouselItems = model.CarouselItems.OrderBy(i => i.DueDate).ToList();
            ViewData.Model = model;
            
            return View("Summary");
        }
    }
}
