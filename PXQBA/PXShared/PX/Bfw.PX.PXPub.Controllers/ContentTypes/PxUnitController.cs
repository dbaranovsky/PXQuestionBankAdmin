using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;

using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class PxUnitController : Controller
    {
        protected ITreeWidgetHelper treeHelper;

        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context. 
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Actions for nav items.
        /// </summary>
        /// <value>
        /// The navigation actions. 
        /// </value>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Actions for grade.
        /// </summary>
        /// <value>
        /// The grade actions. 
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }

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
        /// Initializes a new instance of the <see cref="PxUnitController"/> class.
        /// </summary>
        /// <param name="context">The context. </param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="helper">The helper. </param>
        /// <param name="navActions">The nav actions. </param>
        /// <param name="gradeActions">The grade actions. </param>
        /// <param name="assignmentCenterHelper">The assignment center helper. </param>
        public PxUnitController(BizSC.IBusinessContext context, BizSC.IContentActions contActions, ContentHelper helper, 
            BizSC.INavigationActions navActions, BizSC.IGradeActions gradeActions, AssignmentCenterHelper assignmentCenterHelper,
            ITreeWidgetHelper treeWHelper)
        {
            Context = context;
            ContentActions = contActions;
            ContentHelper = helper;
            NavigationActions = navActions;
            GradeActions = gradeActions;
            AssignmentCenterHelper = assignmentCenterHelper;
            treeHelper = treeWHelper;
        }

        /// <summary>
        /// Saves a new Lesson and updates and existing Lesson, as well as it's description.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="dueDate">The due date.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SaveLesson(PxUnit content, string behavior, DateTime dueDate)
        {
            ActionResult result = null;
            ContentView model = null;
            var toc = treeHelper.DefaultToc;

            switch (behavior.ToLowerInvariant())
            {
                case "cancel":
                    var idToLoad = string.IsNullOrEmpty(content.Id) ? content.ParentId : content.Id;

                    try
                    {
                        model = ContentHelper.LoadContentView(idToLoad, ContentViewMode.Preview, false, toc);
                    }
                    catch { }
                    if (model.Content.Type.ToLower() == "content404")
                    {
                        result = new EmptyResult();
                    }
                    else
                    {
                        result = View("DisplayItem", model);
                    }
                    break;
                case "save & open":
                case "add":
                case "save":
                    if (ModelState.IsValid)
                    {
                        content.DueDate = dueDate;
                        content.AssociatedToCourse = SetAssociatedCourseFlag(content, behavior.ToLowerInvariant());
                        ContentHelper.SetVisibility(content.Visibility, true, "student|instructor");
                        ContentHelper.StoreModule(content, Context.EntityId);
                        
                        model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, false, toc);
                        ViewData["isShowDelete"] = false;
                        result = View("DisplayItem", model);
                    }
                    else
                    {
                        content.EnvironmentUrl = Context.EnvironmentUrl;
                        content.CourseInfo = Context.Course.ToCourse();
                        content.EnrollmentId = Context.EnrollmentId;
                        content.Status = string.IsNullOrEmpty(content.Id) ? ContentStatus.New : ContentStatus.Existing;
                        content.Description = System.Web.HttpUtility.HtmlDecode(content.Description);
                        result = View("CreateContent", content);
                    }
                    break;
                case "remove":
                    content.AssociatedToCourse = "";
                    ContentHelper.StoreModule(content, Context.EntityId);
                    result = RedirectToAction("Index", "AssignmentCenter");
                    break;
                default:
                    result = RedirectToAction("Index", "AssignmentCenter");
                    break;
            }

            return result;
        }

        /// <summary>
        /// Adds the grade book category.
        /// </summary>
        /// <param name="content">The content.</param>
        public void AddGradeBookCategory(ContentItem content)
        {
            string newCategory = content.Title;
            string newGradebookCategoryId = AssignmentCenterHelper.AddGradeBookCategoryToCourse(newCategory: newCategory);
            content.GradeBookWeightCategoryId = newGradebookCategoryId;
        }

        /// <summary>
        /// Sets the associated course flag.
        /// </summary>
        /// <param name="content">The content. </param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        private string SetAssociatedCourseFlag(PxUnit content, string behavior)
        {
            if (behavior == "add" || behavior == "save" || behavior == "save & open")
            {
                return Context.EntityId;
            }
            else if (behavior == "remove")
            {
                content.AssociatedToCourse = "";
            }

            return content.AssociatedToCourse;
        }

    }
}

