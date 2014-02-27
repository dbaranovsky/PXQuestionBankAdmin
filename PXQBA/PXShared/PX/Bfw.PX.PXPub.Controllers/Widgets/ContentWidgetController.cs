using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Implements all actions necessary to populate the views of the content widget.
    /// </summary>
    [PerfTraceFilter]
    public class ContentWidgetController : Controller, IPXWidget, IContentWidgetHelper
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an INavigationActions implementation.
        /// </summary>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation.
        /// </summary>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Access to an IAssignmnetActions implementation.
        /// </summary>
        protected BizSC.IAssignmentActions AssignmentActions { get; set; }

        /// <summary>
        /// Access to an IRubricActions implementation.
        /// </summary>
        protected BizSC.IRubricActions RubricActions { get; set; }

        /// <summary>
        /// Gets or sets the children IDs.
        /// </summary>
        /// <value>
        /// The children IDs.
        /// </value>
        private List<String> ChildrenIds { get; set; }

        /// <summary>
        /// Gets or sets the next items.
        /// </summary>
        /// <value>
        /// The next items.
        /// </value>
        private List<ContentItem> NextItems { get; set; }

        /// <summary>
        /// Access to a content helper object.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected IContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected IAssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// Gets or sets the grade actions.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }


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

        protected IUserActions UserActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentWidgetController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="navActions">The nav actions.</param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="assignmentActions">The assignment actions.</param>
        /// <param name="helper">The helper.</param>
        /// <param name="assignmentCenterHelper">The assignment center helper.</param>
        /// <param name="gradeActions">The grade actions.</param>
        public ContentWidgetController(BizSC.IBusinessContext context, BizSC.INavigationActions navActions, BizSC.IContentActions contActions,
            BizSC.IAssignmentActions assignmentActions, BizSC.ICourseActions courseActions,
            IContentHelper helper, IAssignmentCenterHelper assignmentCenterHelper, BizSC.IGradeActions gradeActions,
            BizSC.IRubricActions rubricActions, BizSC.ISharedCourseActions sharedCourseActions, IUserActions userActions)
        {
            Context = context;
            NavigationActions = navActions;
            ContentActions = contActions;
            AssignmentActions = assignmentActions;
            ContentHelper = helper;
            AssignmentCenterHelper = assignmentCenterHelper;
            GradeActions = gradeActions;
            RubricActions = rubricActions;
            CourseActions = courseActions;            
            SharedCourseActions = sharedCourseActions;
            UserActions = userActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Populates the Summary view with data required to show a navigable tree of content with an optional
        /// content item in the view pane
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            return View();
        }

        /// <summary>
        /// Action providing the display of the widget when it is in 'View All' mode.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View();
        }

        #endregion


        /// <summary>
        /// Returns a view responsible for displaying the specified item in the specified mode.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="includeToc">The include toc.</param>
        /// <param name="includeDiscussion">Whether the result should include discussion.</param>
        /// <param name="includeNavigation">Whether the result should include navigation.</param>
        /// <param name="readOnly">Whether the result should be read only.</param>
        /// <param name="hasParentLesson">Whether the item has parent lessons.</param>
        /// <param name="isMultipartLessons">Is the request from multipart lessons.</param>
        /// <param name="isStudentView">Is this a student view.</param>
        /// <param name="isBeingEdited">Is the item being edited.</param>
        /// <param name="commentId">The comment ID.</param>
        /// <param name="renderFNE">Wraps the item in an FNE window</param>
        /// <param name="toc">Specifies which TOC this item belongs to (applicable only for items with multiple TOCs)</param>
        /// <returns></returns>
        public ActionResult DisplayItem(string id,
            ContentViewMode mode,
            bool? includeToc,
            bool? includeDiscussion,
            bool? includeNavigation,
            bool? readOnly,
            Guid? hasParentLesson,
            bool? isMultipartLessons,
            bool? isStudentView,
            bool? isBeingEdited,
            string commentId,
            bool? isStudentUpdated,            
            bool getChildrenGrades = false,
            string category = "",
            string groupId = "",
            bool isStart = false,
            bool renderFNE = false,
            string externalUrl = "",
            bool renderDialog = false,
            string toc = "syllabusfilter",
            bool isRelatedContent = false)
        {

            ViewData["courseType"] = Context.Course.CourseType.ToLowerInvariant();
            ViewData["IsSandboxCourse"] = Context.Course.IsSandboxCourse;
            ViewData["accessLevel"] = Context.AccessLevel;

            ViewData["showDueTime"] = true;
            ViewData["timeZone"] = Context.Course.GetCourseTimeZoneAbbreviation();

            if (string.IsNullOrEmpty(id) || id == "externalUrl")
            {
                if (!renderFNE)
                {
                    ViewData["message"] = "This item has no content";
                    return View("EmptyContent");
                }
                else
                {
                    var fneModel = new ContentView();
                    fneModel.ActiveMode = mode;
                    fneModel.AllowedModes = mode;
                    fneModel.Url = externalUrl;
                    return View("~/Views/Shared/FneWindow.ascx", fneModel);
                }
            }

            ViewData["isStartActiviy"] = isStart;
            var itemArr = id.Split(',');
            var firstItemId = itemArr[0];

            ActionResult result;

            ViewData["Category"] = category;

            var categoryId = string.Empty;
            if (!category.IsNullOrEmpty())
            {
                if (category.IndexOf("enrollment_") > -1)
                {
                    categoryId = category.Substring(11);
                    if (isStudentUpdated != null && isStudentUpdated.Value)
                    {
                        ContentHelper.FlagItemAsReviewed(id, categoryId);
                    }
                }
            }

            if (renderFNE && Context.AccessLevel == AccessLevel.Student)
            {
                getChildrenGrades = true;
                //get grades for item when in student view and inside an FNE window
                //the FNE window requires the grades to display student score.
            }

            // If the content needs to be marked as read, that has to happen before
            // calling LoadContentView in order for it to show as completed at the top 
            // right of the FNE window as soon as it is viewed by the student.
            //var ci = ContentActions.GetContent(Context.CourseId, firstItemId).ToContentItem(ContentActions);            
            var ci = ContentActions.GetContent(Context.CourseId, firstItemId);
            if (ci != null)
                this.MarkContentAsReadIfNotLate(firstItemId, ci);

            var model = ContentHelper.LoadContentView(firstItemId, mode, false, false, toc, getChildrenGrades, categoryId);
            model.Toc = toc;
            model.IsRelatedContent = isRelatedContent;
            model.Content.NoteId = commentId;
            model.GroupId = groupId;

            //If the course has disabled comments, it should disable comments for all content items
            model.Content.AllowComments = model.Content.AllowComments && !Context.Course.DisableComments;

            // The Parent Id will be the Temporary folder when the item in initially created.
            if (model.Content.ParentId == ContentActions.TemporaryFolder)
            {
                model.Content.Status = ContentStatus.New;
            }


            if (!readOnly.HasValue && Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
            {
                model.Content.ReadOnly = true;
            }

            if (model.Content is Quiz)
            {
                var quiz = (Quiz)model.Content;
                var ai = ContentActions.GetContent(Context.EntityId, quiz.Id).ToAssignedItem();

                if (ai.IsAllowLateGracePeriod && ai.IsAllowLateSubmission)
                {
                    ViewData["GraceDueDate"] = AssignmentHelper.GetGraceDueDate(ai.DueDate, ai.LateGraceDuration, ai.LateGraceDurationType);
                }

                if (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
                {
                    model.Content.ReadOnly = true;
                }

                var grade = GradeActions.GetTeacherResponse(Context.EnrollmentId, quiz.Id);
                quiz.AllowResubmission = grade != null && grade.Status.HasFlag(BizDC.GradeStatus.AllowResubmission);
               
                if (isStudentView.HasValue && isStudentView.Value)
                {
                    Context.AccessLevel = Bfw.PX.Biz.ServiceContracts.AccessLevel.Student;
                    model = ContentHelper.LoadContentView(firstItemId, mode, toc);
                    quiz.Display = Quiz.DisplayType.Student;
                    Context.AccessLevel = Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
                    model.Content.ReadOnly = false;
                }
                else
                {
                    model.ActiveMode = mode;
                    if (model.ActiveMode == ContentViewMode.Preview)
                    {
                        quiz.ShowContentView = true;
                    }
                }
            }

            if (includeToc.HasValue && includeToc.Value)
            {
                model.TableOfContents = ContentHelper.LoadToc(Context.EntityId, firstItemId);
                ViewData["includeToc"] = true;
            }
            else
            {
                ViewData["includeToc"] = false;
            }

            if (readOnly.HasValue && readOnly.Value && model.Content != null)
            {
                model.Content.ReadOnly = true;
                model.AllowedModes = ContentViewMode.None;
            }

            model.IncludeDiscussion = (includeDiscussion.HasValue && includeDiscussion.Value);

            model.IncludeNavigation = includeNavigation.HasValue ? includeNavigation.Value : true;

            Context.Logger.Log(string.Format("Displayed Item {0}", firstItemId), Bfw.Common.Logging.LogSeverity.Information);
            
            if (hasParentLesson != null)
            {
                ViewData["hasParentLesson"] = ((Guid)hasParentLesson).ToString("N");
            }
            else
            {
                ViewData["hasParentLesson"] = null;
            }

            if (Context.CourseIsProductCourse)
            {
                model.AllowedModes &= ~(ContentViewMode.Assign) & ~(ContentViewMode.Settings) & ~(ContentViewMode.Create);
            }

            if (mode == ContentViewMode.Edit && isBeingEdited.HasValue)
            {
                model.Content.IsBeingEdited = isBeingEdited.Value;
            }

            if (model.Content is HtmlDocument)
            {

                model.Content.EntityId = !String.IsNullOrEmpty(Context.EnrollmentId) ? Context.EnrollmentId : Context.EntityId;
                model.Content.IsInstructor = (Context.AccessLevel == AccessLevel.Instructor);
                if (model.ActiveMode != mode)
                {
                    model.ActiveMode = mode;
                }
            }

            if (model.Content is HtmlQuiz)
            {
                var htmlquiz = model.Content as HtmlQuiz;
                htmlquiz.EntityId = !String.IsNullOrEmpty(Context.EnrollmentId) ? Context.EnrollmentId : Context.EntityId;
                htmlquiz.IsInstructor = (Context.AccessLevel == AccessLevel.Instructor);

                if (model.ActiveMode != mode)
                {
                    model.ActiveMode = mode;
                }
                //All xbookapp specific parameters
                htmlquiz.XBookAppParams.DlapCookie = Context.BhAuthCookieValue;
                htmlquiz.XBookAppParams.ProductCourseId = Context.ProductCourseId;
                htmlquiz.XBookAppParams.StudentOverride = Context.ImpersonateStudent.ToString().ToUpper();
                htmlquiz.XBookAppParams.ItemID = htmlquiz.Id;
                htmlquiz.XBookAppParams.BrainHoneyUrl = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["BrainHoneyUrl"]);
                htmlquiz.XBookAppParams.EnrollmentId = Context.EnrollmentId;

                //Getting the base url for the product to send to the xbookapp
                var fullurl = Request.Url.AbsoluteUri;
                var index_of_courseid = fullurl.IndexOf(Context.CourseId);
                var url_length = fullurl.Length;

                // remove all text after the '/product/' part of the px url
                var pxurlbase = fullurl.Remove(index_of_courseid, url_length - index_of_courseid);

                //add the css override parameter for xbookapp
                var cssUrl = Url.RouteUrl("CourseStyleCourseCss", new
                {
                    component = htmlquiz.XBookAppParams.ComponentName,
                    courseProductName = Context.Product.SubType,
                    courseType = Context.Course.CourseType
                }, Request.Url.Scheme);

                htmlquiz.XBookAppParams.CssOverride = HttpUtility.UrlEncode(cssUrl);
                htmlquiz.XBookAppParams.PXAppPath = pxurlbase;

                //Build the agilix xbookapp url to get the html quiz player
                htmlquiz.XBookAppParams.AgilixUrl = ConfigurationManager.AppSettings["XBookComponentURL"];
            }

			//Set the IsInstructor flag if it is an instructor login
	        if (model.Content is Quiz)
	        {
				model.Content.IsInstructor = (Context.AccessLevel == AccessLevel.Instructor);
	        }

            if (model.Content is DocumentCollection && Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                BizDC.Resource resourceDoc = ContentActions.GetResource(categoryId, string.Format("Templates/Data/{0}/index.html", model.Content.Id));
                model.Content.HostMode = (Context.Course.ToCourse().CourseType == CourseType.FACEPLATE) ? HostMode.FacePlate : HostMode.AssignmentCenter;
            }
            
            if (category.IsNullOrEmpty() && model.Content.UserAccess == AccessLevel.Student)
            {
                category = "enrollment_" + model.Content.EnrollmentId;
            }
            if (!category.IsNullOrEmpty())
            {
                
                if (category.IndexOf("enrollment_") > -1)
                {
                    model.Content.ApplicableEnrollmentId = category.Substring(11);
                    model.Content.ReadOnly = true;
                    ViewData["viewingAs"] = "student";
                }
                
                if (model.Content is DocumentCollection && Context.AccessLevel == BizSC.AccessLevel.Instructor)
                {
                    if (category.IndexOf("enrollment_") > -1)
                    {
                        model.Content.ApplicableEnrollmentId = category.Substring(11);
                    }
                    else
                    {
                        model.Content.ApplicableEnrollmentId = Context.EnrollmentId;
                    }
                    BizDC.Resource resourceDoc = ContentActions.GetResource(categoryId, string.Format("Templates/Data/{0}/index.html", model.Content.Id));
                    Document doc = new Document();
                    doc.Id = model.Content.GetExtendedProperty("DocId");
                    doc.FileName = model.Content.GetExtendedProperty("FileName");
                    DateTime dateUploaded;
                    DateTime.TryParse(model.Content.GetExtendedProperty("CreationDate"), out dateUploaded);
                    doc.Uploaded = dateUploaded;
                    Int64 size;
                    Int64.TryParse(model.Content.GetExtendedProperty("FileSize"), out size);
                    doc.Size = size;
                    ((DocumentCollection)model.Content).Documents.Add(doc);
                    model.Content.HostMode = (Context.Course.ToCourse().CourseType == CourseType.FACEPLATE) ? HostMode.FacePlate : HostMode.AssignmentCenter;
                }
            }
            if (Context.AccessLevel == BizSC.AccessLevel.Student)
            {
                if (model.Content is DocumentCollection)
                {
                    BizDC.Resource resourceDoc = null; 
                    if (Context.Course.CourseType != CourseType.FACEPLATE.ToString())
                    {
                        resourceDoc = ContentActions.GetResource(Context.EnrollmentId, string.Format("Templates/Data/{0}/index.html", model.Content.Id));
                    }

                    if (resourceDoc != null)
                    {
                        var desc = resourceDoc.GetStream();
                        using (var sw = new System.IO.StreamReader(desc))
                        {
                            ((DocumentCollection) model.Content).Description = sw.ReadToEnd();
                        }
                    }

                    if (model.ActiveMode != mode)
                    {
                        model.ActiveMode = mode;
                    }
                    model.Content.HostMode = (Context.Course.ToCourse().CourseType == CourseType.FACEPLATE) ? HostMode.FacePlate : HostMode.AssignmentCenter;

                }
            }

            if (Context.Course.ToCourse().CourseType == CourseType.FACEPLATE)
            {
                ViewData["courseType"] = "faceplate";
                ViewData["accessLevel"] = Context.AccessLevel;

                if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
                {
                    model.AllowedModes &= ~(ContentViewMode.Rubrics);
                    if (model.Content.ReadOnly == false && model.Content.Type != "ExternalContent" && !(model.Content is Dropbox))
                    {
                        model.AllowedModes = model.AllowedModes | ContentViewMode.Assign | ContentViewMode.Results | ContentViewMode.Settings;
                    }
                    else if (model.Content.ReadOnly == false)
                    {
                        model.AllowedModes = model.AllowedModes | ContentViewMode.Assign | ContentViewMode.Settings;

                    }
                    else if (model.Content.ReadOnly)
                    {
                        model.AllowedModes &= ~(ContentViewMode.Assign | ContentViewMode.Settings);
                    }

                }
            }

            var previewAsVisitorCookie = Request.Cookies[Context.PreviewAsVisitorCookieKey];
            var isPublicView = Context.IsPublicView;
            if (previewAsVisitorCookie != null)
            {
                isPublicView = true;
            }
            ViewData["IsPublicView"] = isPublicView;

            
            if (model.Content is Quiz)
            {
                model.AllowedModes = model.AllowedModes | ContentViewMode.Questions;

                //TODO: Remove this hack when site builder is properly setup to set not question content as externalcontent
                if (model.Content is HtmlQuiz)
                {
                    HtmlQuiz hQuiz = model.Content as HtmlQuiz;
                    if (hQuiz.Questions == null || hQuiz.Questions.Count < 1)
                    {
                        model.AllowedModes &= ~(ContentViewMode.Questions);
                    }
                }
            }
            if (model.Content != null)
            {
                model.Content.GroupId = model.GroupId;
            }

            // check if social commenting is included
            // If the course and the item being displayed can contain comments, display comments! (and a disable button)
            // If the course can and the item can not display the ability to add comments
            // Else dont show commenting at all! (just a stub to place the comments if they get enabled in all the right places)
            model.SocialCommenting = SocialCommentingState.None;// default social commenting is turned off

            if (Context.Course.SocialCommentingIntegration &&   // Both the course allows for social commenting
                model.Content.SocialCommentingIntegration)      //  and the item allows for social commenting
            {
                model.SocialCommenting = SocialCommentingState.Active;
            }
            else if (Context.Course.SocialCommentingIntegration)// Only the course has social commenting allowed
            {
                model.SocialCommenting = SocialCommentingState.DisabledByUser;
            }

            ViewData.Model = model;

            if (string.IsNullOrEmpty(firstItemId))
            {
                result = View("NoContent");
            }
            else
            {
                var context = ServiceLocator.Current.GetInstance<BizSC.IBusinessContext>();

                if (context.ImpersonateStudent && model.Content.Type.ToLowerInvariant() == "quiz")
                {
                    (model.Content as Quiz).OverrideDueDateReq = true;
                }

                if (renderFNE)
                {
                    result = View("~/Views/Shared/FneWindow.ascx", model);
                }
                else if (renderDialog)
                {
                    result = View("~/Views/Shared/LearningCurveWindow.ascx", model);
                }
                else
                {
                    result = View();
                }
            }

            ViewData["accessLevel"] = Context.AccessLevel;
            ViewData["IsSharedCourse"] = Context.IsSharedCourse;
            ViewData["course_type"] = Context.Course.CourseType;
            return result;
        }

        /// <summary>
        /// Mark the content as read if it is not late.
        /// </summary>
        /// <param name="firstItemId">The item ID.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        private void MarkContentAsReadIfNotLate(string firstItemId, BizDC.ContentItem ci)
        {
            if (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
            {
                var model = ci.ToContentItem(ContentActions);

                DateTime now = DateTime.Now.GetCourseDateTime();
                DateTime dueDate = model.DueDate;

                bool isLate = (now > dueDate);

                var ai = ci.ToAssignedItem();

                if (ai.IsAllowLateGracePeriod && ai.IsAllowLateSubmission)
                {
                    DateTime graceDueDate = AssignmentHelper.GetGraceDueDate(ci.AssignmentSettings);

                    if (graceDueDate >= now.GetCourseDateTime())
                    {
                        isLate = false;
                    }
                }


                if (model.Type.ToLowerInvariant() != "folder" &&
                    model.Type.ToLowerInvariant() != "assignment" &&
                    model.Type.ToLowerInvariant() != "pxunit" &&
                    model.Type.ToLowerInvariant() != "quiz" &&
                    model.Type.ToLowerInvariant() != "htmlquiz" &&
                    model.Type.ToLowerInvariant() != "widgetconfiguration" &&
                    model.Type.ToLowerInvariant() != "discussion" &&
                    ((model.Type.ToLowerInvariant() != "externalcontent" &&
                      model.Type.ToLowerInvariant() != "htmldocument") || Context.ProductType != "xbook") &&
                    //right now we don't want external content being graded for xbook
                    !model.Sco &&
                    !isLate)
                {
                    if (ai.CompletionTrigger != CompletionTrigger.Minutes)
                    {
                        var submissions = GradeActions.GetSubmissions(Context.EnrollmentId, firstItemId);
                        if (submissions == null ||
                            submissions.Count(o => o.SubmissionStatus == BizDC.SubmissionStatus.Graded) == 0)
                        {
                            ContentActions.MarkContentAsRead(firstItemId);
                        }
                    }
                }
            }
        }
    

        /// <summary>
        /// Get the ID of the toc.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns></returns>
        protected string GetTocId(string itemId)
        {
            // Id if found
            try
            {
                var navigationItem = NavigationActions.LoadNavigation(Context.EntityId, itemId, "");
                return navigationItem.Children[0].Id;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Verifies the content exists.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns></returns>
        public ActionResult GetContentInfo(string itemId)
        {
            // Id if found

            try
            {
                var newItem = ContentActions.GetContent(Context.EntityId, itemId);
                return Content(newItem.Id);
            }
            catch (Exception)
            {
                return Content("null");
            }
        }

        /// <summary>
        /// Marks the content as read.
        /// </summary>
        /// <param name="id">The content item ID.</param>
        /// <returns></returns>
        public ActionResult MarkContentAsRead(string id)
        {
            if (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
            {
                var submissions = GradeActions.GetSubmissions(Context.EnrollmentId, id);
                if (submissions == null || submissions.Count(o => o.SubmissionStatus == BizDC.SubmissionStatus.Graded) == 0)
                {
                    ContentActions.MarkContentAsRead(id);
                }
            }

            return Content("read");
        }


        /// <summary>
        /// Action for deleting an item.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <returns></returns>
        public ActionResult DeleteItem(string id, string parentId)
        {
            ContentActions.RemoveContent(Context.EntityId, id);
            //Add the parent item as updated by user
            ContentHelper.RemoveItemFromReview(id);
            ContentHelper.AddItemForReview(parentId);
            
            ViewData.Model = ContentHelper.LoadToc(Context.EntityId, parentId);
            return View("ExpandSection");
        }

        /// <summary>
        /// Action for hiding an item.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <returns></returns>
        public ActionResult HideItem(string id, string parentId)
        {
            List<BizDC.ContentItem> allItems = new List<BizDC.ContentItem>();
            ShowOrHideCurrentItem(id, allItems, true);
            ShowOrHideChildrenItem(id, parentId, true, ref allItems);
            if (!allItems.IsNullOrEmpty())
            {
                ContentActions.StoreContents(allItems);
            }
            //ViewData.Model = ContentHelper.LoadToc(Context.EntityId, id);
            return new EmptyResult();
        }

        /// <summary>
        /// Shows the or hide children item.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="hide">if set to <c>true</c> [hide].</param>
        private void ShowOrHideChildrenItem(string id, string parentId, bool hide, ref List<BizDC.ContentItem> allItems)
        {
            var children = ContentActions.ListChildren(Context.EntityId, id);
            foreach (var child in children)
            {
                if (!(hide && child.HiddenFromStudents))
                {

                    if (child.HiddenFromStudents != hide)
                    {
                        child.HiddenFromStudents = hide;
                        allItems.Add(child);
                    }
                    ShowOrHideChildrenItem(child.Id, child.DefaultCategoryParentId, hide, ref allItems);
                }
            }
        }

        /// <summary>
        /// Action for showing an item.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <returns></returns>
        public ActionResult ShowItem(string id, string parentId, string toc = "syllabusfilter")
        {
            List<BizDC.ContentItem> allItems = new List<BizDC.ContentItem>();
            ShowOrHideCurrentItem(id, allItems, false);
            ShowOrHideChildrenItem(id, parentId, false, ref allItems);
            if (!allItems.IsNullOrEmpty())
            {
                ContentActions.StoreContents(allItems);
            }
            var parentItems = ContentHelper.GetParentHeirachy(parentId, TreeCategoryType.TOC, toc);
            var parentUpdatedItems = (from c in parentItems where c.HiddenFromStudents == true select c).ToList();

            foreach (var item in parentUpdatedItems)
            {
                item.HiddenFromStudents = false;
            }

            if (!parentUpdatedItems.IsNullOrEmpty())
            {
                ContentActions.StoreContents(parentUpdatedItems);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Shows the or hide current item.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="allItems">All items.</param>
        /// <param name="hide">if set to <c>true</c> [hide].</param>
        private void ShowOrHideCurrentItem(string id, List<BizDC.ContentItem> allItems, bool hide)
        {
            var ci = ContentActions.GetContent(Context.EntityId, id);
            if (ci.HiddenFromStudents != hide)
            {
                ci.HiddenFromStudents = hide;
                allItems.Add(ci);
            }
        }

        /// <summary>
        /// Returns a view that shows the item's settings
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Settings(string id, bool isTemplateEditor = false)
        {
            ContentItem ci = new ContentItem();

            if (!string.IsNullOrEmpty(id))
            {
                var item = ContentActions.GetContent(Context.EntityId, id);
                ci = item.ToContentItem(ContentActions);
            }
            ci.EnrollmentId = Context.EnrollmentId;

            var model = new ContentView
            {
                ActiveMode = ContentViewMode.Settings,
                Content = ci,
                DomainUserSpace = Context.Domain.Userspace,
                IsTemplateEditor = isTemplateEditor
            };

            ViewData.Model = model;

            return View();
        }

        /// <summary>
        /// Returns a view that shows the item's settings
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ContentSettings(string id, bool isTemplateEditor = false)
        {
            var assignment = new AssignedItem()
            {
                Id = string.Empty,
                Title = string.Empty,
                DueDate = DateTime.Now.GetCourseDateTime(),
                Category = "0",
                Score = new Score()
                {
                    Correct = 0,
                    Possible = 0
                }
            };

            var content = new ContentItem();

            var model = new ContentView()
            {
                ActiveMode = ContentViewMode.Settings
            };

            if (!string.IsNullOrEmpty(id))
            {
                var cont = ContentActions.GetContent(Context.EntityId, id);
                content = cont.ToContentItem(ContentActions);
                content.IsAssignable = cont.AssignmentSettings.IsAssignable;
                assignment.Id = content.Id;
                assignment.Title = content.Title;
                assignment = SetAssignedItem(cont, true, content.Type);
            }

            model.Content = content;
            model.AssignedItem = assignment;
            ViewData.Model = model;
            var type = content.Type;

            if (content is HtmlQuiz)
                type = "Quiz";
            var contentTemplate = ContentActions.FindTemplateForType(type);
            var customSettings = (contentTemplate == null || string.IsNullOrEmpty(contentTemplate.CustomSettings)) ? "|" : contentTemplate.CustomSettings;

            ViewData["hasCustomSettings"] = customSettings != "|" && customSettings != "CustomSettings|Default";
            ViewData["CustomSettings"] = customSettings == "|" ? "" : customSettings;
            return View();
        }
        /// <summary>
        /// Action for showing more resources for an item.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns></returns>
        public ActionResult MoreResources(string id)
        {
            var model = ContentActions.FindRelatedItems(id, Context.ProductCourseId).Map(r => r.ToTaxonomyRelationship(ContentActions));
            ViewData.Model = model;

            return View();
        }

        /// <summary>
        /// Returns a view that shows the item's rubric settings.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public ActionResult Rubrics(ContentView item, string viewMode = "")
        {
            item.Content.EnrollmentId = Context.EnrollmentId;
            ViewData.Model = item;
            if (!viewMode.IsNullOrEmpty())
            {
                ViewData["viewMode"] = viewMode;
            }

            return View();
        }

        /// <summary>
        /// Returns a view that allows the user to view or adjust the assignment options
        /// for the specified piece of content
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="isMultipartLessons">The is multipart lessons.</param>
        /// <returns></returns>
        public ActionResult CreateAndAssign(ContentView item, bool? isMultipartLessons)
        {

            if (item != null && null != item.Content && !string.IsNullOrEmpty(item.Content.Id))
            {
                var content = new ContentItem();

                var model = new ContentView()
                {
                    ActiveMode = ContentViewMode.Create
                };
                AssignedItem assignment = new AssignedItem()
                {
                    Id = string.Empty,
                    Title = string.Empty,
                    DueDate = DateTime.Now.GetCourseDateTime(),
                    Category = "0",
                    Score = new Score()
                    {
                        Correct = 0,
                        Possible = item.Content.DefaultPoints
                    }
                };

                var cont = ContentActions.GetContent(Context.EntityId, item.Content.Id);
                content = cont.ToContentItem(ContentActions);
                content.IsAssignable = cont.AssignmentSettings.IsAssignable;
                assignment.Id = content.Id;
                assignment.Title = content.Title;

                if (null != cont.AssignmentSettings && cont.AssignmentSettings.DueDate.Year != DateTime.MinValue.Year)
                {
                    assignment.DueDate = cont.AssignmentSettings.DueDate;
                    assignment.Score = new Score() { Possible = cont.AssignmentSettings.Points };
                }
                model.Content = content;
                model.AssignedItem = assignment;
                ViewData.Model = model;
            }

            return View();
        }

        /// <summary>
        /// Action to assign an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public ActionResult AssignTab(ContentView item, bool IsContentCreateAssign, string sourceType)
        {
            var assignment = new AssignedItem()
            {
                Id = string.Empty,
                Title = string.Empty,
                DueDate = DateTime.MinValue,
                Category = "",
                Score = new Score()
                {
                    Correct = 0,
                    Possible = item.Content.DefaultPoints
                },
                IsContentCreateAssign = IsContentCreateAssign,
                Type = sourceType,
                SourceType = sourceType,
                IncludeGbbScoreTrigger = 1
            };

            AssignedItemMapper.PopulateGradeSettings(assignment, item.Content.ItemDataXml);
            assignment.Syllabus = AssignmentCenterHelper.FindFilter("PX_ASSIGNMENT_CENTER_SYLLABUS", true, false);
            assignment.Syllabus.ChildrenFilterSections.RemoveAll(i => i.Id.ToLowerInvariant().Contains("workspace"));
            assignment.GradeBookWeights = Context.Course.ToGradeBookWeights();//GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();

            if (null != item.Content && !string.IsNullOrEmpty(item.Content.Id))
            {
                string entityId = (string.IsNullOrEmpty(item.GroupId) || item.GroupId.ToLower() == "null" || item.GroupId == "groupIdValue") ? Context.EntityId : item.GroupId;

                var cont = ContentActions.GetContent(entityId, item.Content.Id);

                if (cont != null)
                {
                    assignment.Id = cont.Id;
                    assignment.Title = cont.Title;
                    assignment.Type = cont.Type;
                    assignment.SubType = cont.Subtype;
                    assignment.SyllabusFilter = cont.ToContentItem(ContentActions).SyllabusFilter;
                    assignment.Sco = cont.Sco;
                    if (cont.FacetMetadata.ContainsKey("meta-content-type"))
                    {
                        assignment.ContentType = cont.FacetMetadata["meta-content-type"];
                    }


                    assignment.SourceType = string.IsNullOrEmpty(cont.Subtype) ? cont.Type : cont.Subtype;
                    assignment.CompletionTrigger = CompletionTrigger.Submission; //set default completion trigger
                    assignment.IsSendReminder = false;

                    if (null != cont.AssignmentSettings && (cont.AssignmentSettings.DueDate.Year != DateTime.MinValue.Year || cont.AssignmentSettings.Points > 0))
                    {
                        assignment.DueDate = cont.AssignmentSettings.DueDate;
                        assignment.StartDate = cont.AssignmentSettings.StartDate;
                        assignment.Score = new Score() { Possible = cont.AssignmentSettings.Points };
                        assignment.Category = cont.AssignmentSettings.Category;
                        assignment.CompletionTrigger = (Models.CompletionTrigger)cont.AssignmentSettings.CompletionTrigger;
                        assignment.TimeToComplete = cont.AssignmentSettings.TimeToComplete;
                        assignment.PassingScore = cont.AssignmentSettings.PassingScore;
                        assignment.IsGradeable = cont.AssignmentSettings.IsAssignable;
                        assignment.IsAllowLateSubmission = cont.AssignmentSettings.AllowLateSubmission || cont.DueDateGrace != 0;
                        assignment.IsHighlightLateSubmission = cont.AssignmentSettings.IsHighlightLateSubmission;
                        assignment.IsAllowLateGracePeriod = cont.AssignmentSettings.IsAllowLateGracePeriod;
                        assignment.IsAllowExtraCredit = cont.AssignmentSettings.IsAllowExtraCredit;
                        assignment.LateGraceDuration = cont.AssignmentSettings.LateGraceDuration;
                        assignment.LateGraceDurationType = cont.AssignmentSettings.LateGraceDurationType;
                        assignment.IsSendReminder = cont.AssignmentSettings.IsSendReminder;
                        assignment.GradeRule = (Models.GradeRule)cont.AssignmentSettings.GradeRule;
                        assignment.IsItemLocked = cont.IsItemLocked;
                        assignment.SubmissionGradeAction = (SubmissionGradeAction)cont.AssignmentSettings.SubmissionGradeAction;

                        if (assignment.IsSendReminder)
                        {
                            assignment.ReminderEmail.AssignmentId = cont.AssignmentSettings.ReminderEmail.AssignmentId;
                            assignment.ReminderEmail.Subject = cont.AssignmentSettings.ReminderEmail.Subject;
                            assignment.ReminderEmail.Body = cont.AssignmentSettings.ReminderEmail.Body;
                            assignment.ReminderEmail.AssignmentDate = cont.AssignmentSettings.ReminderEmail.AssignmentDate;
                            assignment.ReminderEmail.DaysBefore = cont.AssignmentSettings.ReminderEmail.DaysBefore;
                            assignment.ReminderEmail.DurationType = cont.AssignmentSettings.ReminderEmail.DurationType;
                        }
    
                        var category = System.Configuration.ConfigurationManager.AppSettings["IsImportant"];
                        assignment.IsImportant = cont.Categories.Filter(x => x.Id == category).Count() > 0;
                    }

                    if (cont.Properties.ContainsKey("bfw_IncludeGbbScoreTrigger"))
                    {
                        assignment.IncludeGbbScoreTrigger = (int)cont.Properties["bfw_IncludeGbbScoreTrigger"].Value;
                    }


                    if (cont.Properties.ContainsKey("bfw_SendReminder"))
                    {
                        assignment.IsSendReminder = (Boolean)cont.Properties["bfw_SendReminder"].Value;
                    }

                    //assignment.IsContentCreateAssign = IsContentCreateAssign;


                    //List of allowed content items which can be created from the Assign tab
                    item.RelatedTemplates = cont.RelatedTemplates.Map(ci => ci.ToRelatedTemplate()).ToList();
                }
            }

            assignment.AssignTabSettings = Context.Course.AssignTabSettings.ToAssignTabSettings();
            assignment.CourseType = Context.Course.CourseType;

            //if (assignment.Score.Possible == 0.0) //Allow possible scores to be 0
            //{
            //    assignment.Score.Possible = item.Content.DefaultPoints;
            //}

            //Mark a flag to view the assigned item in read-only mode
            if (Context.IsCourseReadOnly)
            {
                assignment.IsReadOnly = true;
            }

            assignment.IsShowExtraCreditOption = Context.Course.IsAllowExtraCredit;
            item.AssignedItem = assignment;

            ViewData.Model = item;

            ViewData["courseId"] = Context.CourseId;
            ViewData["accessLevel"] = Context.AccessLevel;

            return View();
        }
        /// <summary>
        /// Action to assign an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public ActionResult Assign(ContentView item, bool IsContentCreateAssign, string sourceType)
        {
            var assignment = new AssignedItem()
            {
                Id = string.Empty,
                Title = string.Empty,
                DueDate = DateTime.MinValue,
                Category = "",
                Score = new Score()
                {
                    Correct = 0,
                    Possible = item.Content.DefaultPoints
                },
                IsContentCreateAssign = IsContentCreateAssign,
                Type = sourceType,
                SourceType = sourceType
            };

            if (null != item.Content && !string.IsNullOrEmpty(item.Content.Id))
            {
                var cont = ContentActions.GetContent(Context.EntityId, item.Content.Id);
                assignment.Syllabus = AssignmentCenterHelper.FindFilter("PX_ASSIGNMENT_CENTER_SYLLABUS", true, false);
                assignment.Syllabus.ChildrenFilterSections.RemoveAll(i => i.Id.ToLowerInvariant().Contains("workspace"));

                assignment.Id = cont.Id;
                assignment.Title = cont.Title;
                assignment.Type = cont.Type;
                assignment.SyllabusFilter = cont.ToContentItem(ContentActions).SyllabusFilter;

                assignment.SourceType = string.IsNullOrEmpty(cont.Subtype) ? cont.Type : cont.Subtype;
                assignment.IsSendReminder = false;
                assignment.IsItemLocked = cont.IsItemLocked;

                if (null != cont.AssignmentSettings && cont.AssignmentSettings.DueDate.Year != DateTime.MinValue.Year)
                {
                    assignment.DueDate = cont.AssignmentSettings.DueDate;
                    assignment.StartDate = cont.AssignmentSettings.StartDate;
                    assignment.Score = new Score() { Possible = cont.AssignmentSettings.Points };
                    assignment.Category = cont.AssignmentSettings.Category;
                    assignment.CompletionTrigger = (CompletionTrigger)cont.AssignmentSettings.CompletionTrigger;
                    assignment.IsGradeable = cont.AssignmentSettings.IsAssignable;
                    assignment.IsAllowLateSubmission = cont.AssignmentSettings.AllowLateSubmission;
                    assignment.IsHighlightLateSubmission = cont.AssignmentSettings.IsHighlightLateSubmission;
                    assignment.IsAllowLateGracePeriod = cont.AssignmentSettings.IsAllowLateGracePeriod;
                    assignment.LateGraceDuration = cont.AssignmentSettings.LateGraceDuration;
                    assignment.LateGraceDurationType = cont.AssignmentSettings.LateGraceDurationType;
                    assignment.IsSendReminder = cont.AssignmentSettings.IsSendReminder;
                    assignment.GradeRule = (Models.GradeRule)cont.AssignmentSettings.GradeRule;

                    if (assignment.IsSendReminder)
                    {
                        assignment.ReminderEmail.AssignmentId = cont.AssignmentSettings.ReminderEmail.AssignmentId;
                        assignment.ReminderEmail.Subject = cont.AssignmentSettings.ReminderEmail.Subject;
                        assignment.ReminderEmail.Body = cont.AssignmentSettings.ReminderEmail.Body;
                        assignment.ReminderEmail.AssignmentDate = cont.AssignmentSettings.ReminderEmail.AssignmentDate;
                        assignment.ReminderEmail.DaysBefore = cont.AssignmentSettings.ReminderEmail.DaysBefore;
                        assignment.ReminderEmail.DurationType = cont.AssignmentSettings.ReminderEmail.DurationType;
                    }
                    
                    var category = System.Configuration.ConfigurationManager.AppSettings["IsImportant"];
                    assignment.IsImportant = cont.Categories.Filter(x => x.Id == category).Count() > 0;
                }

                assignment.GradeBookWeights = Context.Course.ToGradeBookWeights();//GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();

                if (cont.Properties.ContainsKey("bfw_IncludeGbbScoreTrigger"))
                {
                    assignment.IncludeGbbScoreTrigger = (int)cont.Properties["bfw_IncludeGbbScoreTrigger"].Value;
                }


                if (cont.Properties.ContainsKey("bfw_SendReminder"))
                {
                    assignment.IsSendReminder = (Boolean)cont.Properties["bfw_SendReminder"].Value;
                }

                //assignment.IsContentCreateAssign = IsContentCreateAssign;


                //List of allowed content items which can be created from the Assign tab
                assignment.RelatedTemplates = cont.RelatedTemplates.Map(ci => ci.ToRelatedTemplate()).ToList();
            }

            assignment.AssignTabSettings = Context.Course.AssignTabSettings.ToAssignTabSettings();
            assignment.CourseType = Context.Course.CourseType;

            //if (assignment.Score.Possible == 0.0)
            //{
            //    assignment.Score.Possible = item.Content.DefaultPoints;
            //}

            var model = assignment;
            ViewData.Model = model;

            return View();
        }

        /// <summary>
        /// Returns a view that represents an expanded section comprised of the children of itemId.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="includeToc">Whether the TOC should be included in the returned view.</param>
        /// <param name="includeDiscussion">Whether the view should include discussion functionality.</param>
        /// <param name="category">The category.</param>
        /// <param name="fromAssignmentCenter">Whethter this action is being called from the Assignment Center.</param>
        /// <returns></returns>
        public ActionResult ExpandSection(string id, bool? includeToc, bool? includeDiscussion, string category, bool? fromAssignmentCenter,
                bool? IsEportfolioBrowser, bool? IsPresentationCourse, string UserAccess)
        {
            // fromAssignmentCenter will be true if My Materials in Assignmnet Center is selected else category should be empty.
            if (category == System.Configuration.ConfigurationManager.AppSettings["MyMaterials"] && (!fromAssignmentCenter.HasValue))
            {
                category = "";
            }
            bool isLoadStudentChild = true;
            var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, id, category, isLoadStudentChild);
            IEnumerable<TocItem> toc = new List<TocItem>();

            if (!bizNavigationItem.Children.IsNullOrEmpty())
            {
                string activeId = string.Empty;

                if (id == "PX_TOC")
                {
                    activeId = bizNavigationItem.Children[0].Id;
                    bizNavigationItem.Children[0] = NavigationActions.LoadNavigation(Context.EntityId, activeId, category);
                }
                toc = bizNavigationItem.Children.Map(ti => ti.ToTocItem(activeId, ContentHelper.ShowTocControls(), ContentHelper.GetTreeItems(category, ti.Id), ContentHelper.GetItemUpdates(category), Context));
            }

            ViewData.Model = toc;

            if (Context.IsSharedCourse)
            {
                var enrollmentId = Regex.Split(category, "enrollment_").ToList().Last();
                var sharedItems = SharedCourseActions.getSharedItems(enrollmentId);
                ViewData.Model = GetSharedTocs(sharedItems, toc.ToList());
            }

            if (includeToc.HasValue)
            {
                ViewData["includeToc"] = includeToc.Value;
            }

            if (includeDiscussion.HasValue)
            {
                ViewData["includeDiscussion"] = includeDiscussion.Value;
            }

            ViewData["category"] = category;
            ViewData["HasUserMaterials"] = ContentHelper.HasMyMaterials(category, toc);
            ViewData["IsEportfolioBrowser"] = IsEportfolioBrowser;
            ViewData["IsPresentationCourse"] = IsPresentationCourse;
            ViewData["UserAccess"] = UserAccess;
            var previewAsVisitorCookie = Request.Cookies[Context.PreviewAsVisitorCookieKey];
            var isPublicView = Context.IsPublicView;
            if (previewAsVisitorCookie != null)
            {
                isPublicView = true;
            }
            ViewData["IsPublicView"] = isPublicView;
            ViewData["IsReadOnly"] = Context.IsCourseReadOnly;

            return View();
        }

        /// <summary>
        /// Returns a view of a TOC tree expanded to a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <returns></returns>
        public ActionResult ExpandToItem(string itemId)
        {
            ViewData.Model = ContentHelper.LoadToc(Context.EntityId, itemId, null);
            return View("ExpandSection");
        }

        /// <summary>
        /// Creates a new content item of the given type, under the specific parent.
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="type">The type.</param>
        /// <param name="minSequence">The min sequence.</param>
        /// <param name="maxSequence">The max sequence.</param>
        /// <param name="hasParentLesson">The parent lesson, if there is one.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public ActionResult CreateContent(string parentId, string type, string minSequence, string maxSequence, Guid? hasParentLesson, string category)
        {
            ContentItem model = null;
            var sequence = Context.Sequence(minSequence.Trim(), maxSequence.Trim());
            var t = type.Trim().ToLowerInvariant();

            switch (t)
            {
                case "html":
                    model = new HtmlDocument()
                    {
                        ParentId = parentId
                    };
                    break;

                case "document":
                    model = new DocumentCollection()
                    {
                        ParentId = parentId
                    };
                    break;

                case "link":
                    model = new LinkCollection()
                    {
                        ParentId = parentId
                    };
                    break;

                case "folder":
                    model = new Folder()
                    {
                        ParentId = parentId
                    };
                    break;

                case "quiz":
                    model = new Quiz()
                    {
                        ParentId = parentId,
                        QuizType = QuizType.Assessment
                    };
                    break;

                case "homework":
                    model = new Quiz()
                    {
                        ParentId = parentId,
                        QuizType = QuizType.Homework
                    };
                    break;

                case "discussion":
                    model = new Discussion()
                    {
                        ParentId = parentId
                    };
                    break;

                case "rss feed":
                    model = new RssFeed()
                    {
                        ParentId = parentId
                    };
                    break;

                case "pxunit":
                case "module":
                    model = new PxUnit()
                    {
                        ParentId = Context.EntityId,
                        Id = Guid.NewGuid().ToString("N"),
                    };
                    break;

                default:
                    throw new ArgumentException("Type is a required parameter", "type");
            }

            model.EnvironmentUrl = Context.EnvironmentUrl;
            model.CourseInfo = Context.Course.ToCourse();
            model.EnrollmentId = Context.EnrollmentId;
            model.Status = ContentStatus.New;
            model.Sequence = sequence;

            if (!string.IsNullOrEmpty(category))
            {
                model.Categories = new List<TocCategory>() { new TocCategory() { Id = category, Text = category, Sequence = sequence, ItemParentId = parentId } };
            }

            if (hasParentLesson != null)
            {
                ViewData["hasParentLesson"] = ((Guid)hasParentLesson).ToString("N");
            }
            else
            {
                ViewData["hasParentLesson"] = null;
            }

            ViewData.Model = model;
            return View();
        }

        [HttpPost]
        public ActionResult AddGradebookCategory(string newCategory)
        {
            var categoryId = AssignmentCenterHelper.AddGradeBookCategoryToCourse(newCategory);

            return Json(new { gradebookId = categoryId });
        }

        [HttpPost]
        public ActionResult AddGradebookCategoryToUnit(string categoryId, string unitId)
        {
            var result = AssignmentCenterHelper.AddGradeBookCategoryToUnit(categoryId: categoryId, unitId: unitId);

            return result? Json("success") : Json("");
        }

        /// <summary>
        /// Assigns a date to an item in the assignment center.
        /// </summary>
        /// <param name="categoryId">The category ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult AssignAssignmentCenterItemDate(string categoryId, string itemId, DateTime startDate, 
            DateTime endDate, string type, string toc = "syllabusfilter")
        {
            endDate = endDate.AddHours(23);
            endDate = endDate.AddMinutes(59);
            var filter = new AssignmentCenterFilterSection();
            switch (type)
            {
                case "category":
                    filter = AssignmentCenterHelper.FindFilter(categoryId, true, true);
                    AssignmentCenterHelper.AssignCategoryDate(filter, startDate, endDate, true, toc);
                    return View("AssignmentCenterFilterSection", filter);
                case "pxunit":
                    AssignmentCenterHelper.AssignLessonDate(itemId, startDate, endDate, true, "", toc);
                    break;
                case "item":
                    AssignmentCenterHelper.AssignAssignmentCenterItemDate(itemId, endDate, Context.EntityId, toc);
                    break;
            }

            if (string.IsNullOrEmpty(filter.Id))
            {
                // Reload the filter; the dates may have changed.
                filter = AssignmentCenterHelper.FindFilter(categoryId, true, true);
                return View("AssignmentCenterFilterSection", filter);
            }
            else
            {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// Unassigns an item date.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns></returns>
        public ActionResult UnAssignItemDate(string itemId)
        {
            var a = ContentActions.GetContent(Context.EntityId, itemId);
            AssignmentActions.Unassign(a);
            return new EmptyResult();
        }

        /// <summary>
        /// Makes and item an assigned item.
        /// </summary>
        /// <param name="ai">The item to assign.</param>
        /// <param name="dueHour">The due hour.</param>
        /// <param name="dueMinute">The due minute.</param>
        /// <param name="dueAmpm">The due am/pm.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="isMultipartLessons">Whether this request is being made from multipart lessons.</param>
        /// <param name="isImportant">Whether this assignment is important.</param>
        /// <returns></returns>
        public JsonResult AssignItem(string itemId, int dueYear, int dueMonth, int dueDay, int dueHour, int dueMinute, string dueAmpm, string behavior, bool? isMultipartLessons,
            string completionTrigger, string gradebookCategory, string syllabusFilter, int points, string rubricId, bool isGradeable, bool isAllowLateSubmission, bool isSendReminder,
            int reminderDurationCount, string reminderDurationType, string reminderSubject, string reminderBody, int IncludeGbbScoreTrigger, bool isHighlightLateSubmission, bool isAllowLateGracePeriod,
            long lateGraceDuration, string lateGraceDurationType, string CalculationTypeTrigger, bool isAllowExtraCredit, 
            string groupId, string instructions, string timeToComplete = "", string passingScore = "", string toc = "syllabusfilter")
        {


            string applicableEntityId = string.IsNullOrEmpty(groupId) ? Context.EntityId : groupId;
            var result = new JsonResult();
            var ai = ContentActions.GetContent(applicableEntityId, itemId).ToAssignedItem();           

            switch (behavior.ToLower())
            {
                case "assign":
                    //if (ai.Score == null) ai.Score = new Score { Correct = 0.0, Possible = 0.0, Date = DateTime.Today };
                    ai.Score = new Score() { Possible = points };
                    if (ai.Score.Possible < 0 || ai.Score.Possible > 100)
                    {
                        result.Data = new Dictionary<string, string> { { "error", "Points should be between 0 and 100" } };
                        break;
                    }
                    if (points == 0)
                    {
                        gradebookCategory = "-1";
                    }

                    dueHour %= 12;
                    dueHour = (dueAmpm.ToLower() == "pm") ? dueHour + 12 : dueHour;
                    ai.DueDate = new DateTime(dueYear, dueMonth,
                                              dueDay, dueHour, dueMinute, 0);

                    //PRODUCTION CHANGE - allow due dates in the past
                    //if (ai.DueDate < DateTime.Now.GetCourseDateTime() && ai.DueDate.Year != DateTime.MinValue.Year)
                    //{
                    //    result.Data = new Dictionary<string, string> { { "error", "Due date/time cannot be lesser than current date/time." } };
                    //    break;
                    //}
                    int completionTriggerInt = (int)CompletionTrigger.Submission;
                    int.TryParse(completionTrigger, out completionTriggerInt);
                    ai.CompletionTrigger = (CompletionTrigger)completionTriggerInt;

                    if (ai.CompletionTrigger == CompletionTrigger.Minutes)
                    {
                        int timeToCompleteInt = 0;
                        int.TryParse(timeToComplete, out timeToCompleteInt);
                        ai.TimeToComplete = timeToCompleteInt;
                    }

                    if (ai.CompletionTrigger == CompletionTrigger.PassingScore)
                    {
                        double passingScoreDouble = 0;
                        double.TryParse(passingScore, out passingScoreDouble);

                        if (passingScoreDouble != 0)
                        {
                            ai.PassingScore = passingScoreDouble / 100;
                        }
                    }

                    ai.Category = gradebookCategory;
                    ai.SyllabusFilter = syllabusFilter;
                    ai.IsGradeable = isGradeable;
                    ai.IsAllowLateSubmission = isAllowLateSubmission;
                    ai.IsHighlightLateSubmission = isHighlightLateSubmission;
                    ai.IsAllowExtraCredit = isAllowExtraCredit;
                    ai.IsAllowLateGracePeriod = isAllowLateGracePeriod;
                    ai.LateGraceDuration = lateGraceDuration;
                    ai.LateGraceDurationType = lateGraceDurationType;
                    ai.IsSendReminder = isSendReminder;
                    ai.IsAllowExtraCredit = isAllowExtraCredit;
                    ai.Instructions = instructions;
                    //ai.GradeRule = !string.IsNullOrEmpty(SubmissionGradeAction) ? (GradeRule)Enum.Parse(typeof(GradeRule), SubmissionGradeAction) : GradeRule.Last;
                    ai.SubmissionGradeAction = !string.IsNullOrEmpty(CalculationTypeTrigger) ? (SubmissionGradeAction)Enum.Parse(typeof(SubmissionGradeAction), CalculationTypeTrigger) : SubmissionGradeAction.Default;

                    if (isSendReminder)
                    {
                        ai.ReminderEmail = new ReminderEmail { AssignmentId = itemId, Body = reminderBody, Subject = reminderSubject, DaysBefore = reminderDurationCount, DurationType = reminderDurationType, AssignmentDate = DateTime.Now.GetCourseDateTime() };
                    }

                    ai.IncludeGbbScoreTrigger = IncludeGbbScoreTrigger;

                    AssignmentCenterHelper.HandleAssignItemDate(ai, ai.StartDate, ai.DueDate, true, applicableEntityId, toc);
                    break;

                case "unassign":
                    AssignmentActions.Unassign(ai.ToContentItem(applicableEntityId, ContentActions));
                    ai.StartDate = DateTime.MinValue;
                    ai.DueDate = DateTime.MinValue;
                    ViewData["message"] = "unassign successful";
                    ai.IsImportant = false;
                    ai.IncludeGbbScoreTrigger = 0;
                    break;
            }

            var data = result.Data as Dictionary<string, string>;
            if (data == null || !data.ContainsKey("error"))
            {
                result.Data = new Dictionary<string, string>() { { "status", "success" }, { "behavior", behavior } };
            }
            return result;
        }

        /// <summary>
        /// Restore default instruction button on tinyMCE editor
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ActionResult RefreshDefaultItemDescription(string itemId)
        {
            var isDefaultInsutructions = false;
            var title = string.Empty;
            var description = string.Empty;

            //check whether defaults instructions, for an item (folder, eportfolio etc.), are available for this course
            if (Context.Course.Properties.ContainsKey("bfw_default_instructions"))
            {
                isDefaultInsutructions = Convert.ToBoolean(Context.Course.Properties["bfw_default_instructions"].Value.ToString());
            }

            if (isDefaultInsutructions)
            {
                if (Context.Course.Properties.ContainsKey("bfw_default_title"))
                {
                    title = Context.Course.Properties["bfw_default_title"].Value.ToString();
                }

                if (Context.Course.Properties.ContainsKey("bfw_default_description"))
                {
                    description = Context.Course.Properties["bfw_default_description"].Value.ToString();

                }
            }

            return Json(new { title = title, description = description });
        }

        /// <summary>
        /// Gets a start date and time given a date.
        /// </summary>
        /// <param name="dt">The date time.</param>
        /// <param name="isStartDate">if set to <c>true</c> [is start date].</param>
        /// <returns></returns>
        private DateTime GetStartDate(DateTime dt, bool isStartDate)
        {
            if (isStartDate)
            {
                return Convert.ToDateTime(dt.ToShortDateString() + " 12:00:00 AM");
            }
            else
            {
                return Convert.ToDateTime(dt.ToShortDateString() + " 11:59:59 PM");
            }
        }

        /// <summary>
        /// Lists the assignments by date.
        /// </summary>
        /// <param name="from">From date.</param>
        /// <param name="to">To date.</param>
        /// <param name="itemId">The item ID.</param>
        /// <returns></returns>
        public ActionResult ListAssignmentsByDate(int year, int month, int day, string itemId, string groupId)
        {
            string applicableEntityId = string.IsNullOrEmpty(groupId) ? Context.EntityId : groupId;
            DateTime fromDate;
            DateTime toDate;
            ++month;
            if (day < 1)
            {
                fromDate = new DateTime(year, month, 1);
                toDate = new DateTime(year, month, 1).EndOfMonth();
            }
            else
            {
                int previousDay = (day - 1) < 1 ? 1 : day;
                fromDate = new DateTime(year, month, previousDay);
                toDate = fromDate.AddHours(23);
                toDate = toDate.AddMinutes(59);
                toDate = toDate.AddSeconds(59);
            }

            var rootFolder = ConfigurationManager.AppSettings["assignmentCenterRootFolder"];
            var items = ContentActions.ListContentWithDueDates(applicableEntityId, string.Empty).Where(i => (i.Subtype == null || i.Subtype.ToLowerInvariant() != "pxunit"));

            if (!items.IsNullOrEmpty())
            {
                var all = items.Map(m => m.ToAssignedItem());
                var matchedItems = all.Filter(ai => ai.DueDate.InRange(fromDate, toDate));
                ViewData["assignments"] = matchedItems;
            }

            ViewData["fromDate"] = fromDate;
            ViewData["toDate"] = toDate;
            if (Context.Course.ToCourse().CourseType == CourseType.FACEPLATE)
            {
                ViewData["courseType"] = "faceplate";
            }

            return View();
        }



        /// <summary>
        /// Reorders the content.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="minSequence">The min sequence.</param>
        /// <param name="maxSequence">The max sequence.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public ActionResult ReorderContent(string id, string parentId, string minSequence, string maxSequence,string category)
        {
            var result = string.Empty;
            if (parentId == "-1")
            {
                return Content("0");

            }
            if (!string.IsNullOrEmpty(id))
            {
                var ci = ContentActions.GetContent(Context.EntityId, id);

                if (ci != null && Context.AccessLevel == BizSC.AccessLevel.Instructor &&
                    (category.IndexOf("enrollment_") == -1))
                {
                    var sequence = Context.Sequence(minSequence, maxSequence);
                    var cat = ci.Categories.FirstOrDefault(c => c.Id == category);

                    if (cat == null)
                    {
                        ci.Sequence = sequence;
                        ci.ParentId = parentId;
                        ci.DefaultCategoryParentId = parentId;
                        ci.DefaultCategorySequence = ci.Sequence;
                    }
                    else
                    {
                        cat.ItemParentId = parentId;
                        cat.Sequence = sequence;
                    }

                    result = ci.Sequence;
                    ContentActions.StoreContent(ci);

                }
            }
            return Content(result);
        }

        /// <summary>
        /// Returns the view containing the delete confirmation dialog.
        /// This confirmation dialog is used across the Px site to get the user confirmation before removing a content item from the TOC Navigation.
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteConfirmDialog(string removeMessage)
        {
            ViewData["removeMesage"] = removeMessage;
            return View();
        }

        /// <summary>
        /// Saves a resource to the TOC.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public ActionResult SaveResourceToTOC(string itemId, string parentId, string sequence, string category)
        {
            var result = string.Empty;

            var contentResource = ContentActions.GetContent(Context.EntityId, itemId, true);
            var contentToc = ContentActions.GetContent(Context.EntityId, parentId, true);

            if (contentResource != null && contentToc != null)
            {
                if (string.IsNullOrEmpty(category))
                {
                    contentResource.Sequence = sequence;
                    contentResource.ParentId = parentId;
                    contentResource.DefaultCategoryParentId = parentId;
                    contentResource.DefaultCategorySequence = sequence;
                }
                else if (!contentResource.Categories.IsNullOrEmpty())
                {
                    var cat = contentResource.Categories.FirstOrDefault(c => c.Id == category);
                    if (cat != null)
                    {
                        cat.ItemParentId = parentId;
                        cat.Sequence = sequence;
                    }
                }

                result = contentResource.Sequence;
                ContentActions.StoreContent(contentResource);
            }
            return Content(result);
        }

        /// <summary>

        /// Returns a list of child item IDs.
        /// </summary>
        /// <param name="id">The parent ID.</param>
        /// <returns></returns>
        public JsonResult ListChildrenIds(string id)
        {
            using (Context.Tracer.StartTrace("ContentWidget.ListChildrenIds"))
            {
                string excludeTypes = "htmldocument,externalcontent";

                List<String> excludes = excludeTypes.Split(',').ToList();

                if (ChildrenIds == null)
                {
                    ChildrenIds = new List<string>();
                }

                if (ChildrenIds.Count < 50)
                {
                    var content = ContentActions.GetContent(Context.EntityId, id);
                    if (content != null)
                    {
                        var modelType = content.ModelType().ToLower();
                        if (!content.Hidden)
                        {
                            if (excludes.Contains(modelType) || ChildrenIds.Count == 0)
                            {
                                ChildrenIds.Add(id);
                            }
                        }

                        if (ChildrenIds.Count < 50)
                        {
                            var items = ContentActions.ListChildren(Context.EntityId, id);

                            if (!items.IsNullOrEmpty())
                            {
                                foreach (var item in items)
                                {
                                    if (ChildrenIds.Count < 50)
                                    {
                                        ListChildrenIds(item.Id);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return this.Json(ChildrenIds);

        }

        /// <summary>
        /// Return a Category list for a model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="selectedCategory"></param>
        /// <returns></returns>
        public List<TocCategory> GetCategoryList(ContentWidget model, string selectedCategory)
        {
            var categories = new List<TocCategory>();

            try
            {
                if (Context.Course != null && !Context.Course.Categories.IsNullOrEmpty())
                {
                    categories = Context.Course.Categories.Map(c => c.ToTocCategory(selectedCategory)).ToList();

                    if (Context.CourseIsProductCourse)
                    {
                        categories.RemoveAll(cat => cat.Id == "my_materials");
                    }

                }
            }
            catch { }

            return categories;
        }

        /// <summary>
        /// Return try is a date is valid
        /// </summary>
        /// <param name="assignedDate"></param>
        /// <returns></returns>
        public ActionResult IsAssignDateValid(DateTime assignedDate)
        {
            var isValid = (assignedDate > DateTime.Now.GetCourseDateTime() || assignedDate.Year < 1900);
            return Content(isValid.ToString().ToLowerInvariant());
        }

        /// <summary>
        /// show the add gradebook screen
        /// </summary>
        /// <param name="contentItemid"></param>
        /// <param name="isIncludeAddNewOption"></param>
        /// <returns></returns>
        public ActionResult ShowAddGradeBookCateagory(string contentItemid, bool isIncludeAddNewOption = false)
        {
            ViewData["contentItemid"] = contentItemid;
            ViewData["isIncludeAddNewOption"] = isIncludeAddNewOption;
            return View();
        }


        public ActionResult SaveVisibility(ContentItem contentItem, string visibility, string restricted,
            DateTime DueDate)
        {
            var ci = ContentActions.GetContent(Context.EntityId, contentItem.Id);
            var node = XElement.Parse("<bfw_visibility />", LoadOptions.None);
            var roles = new XElement("roles");

            node.Add(roles);
            roles.Add(new XElement("instructor"));

            if (visibility != "hidefromstudent")
            {
                var student = (new XElement("student"));
                if (restricted == "restrictedbydate")
                {
                    var restriction = new XElement("restriction");
                    var endDateAttribute = new XAttribute("endate", DueDate.ToString());
                    var endDate = new XElement("date", endDateAttribute);

                    restriction.Add(endDate);
                    student.Add(restriction);
                }
                roles.Add(student);
            }

            ci.Visibility = node.ToString();
            ContentActions.StoreContent(ci);
            return new EmptyResult();
        }


        /// <summary>
        /// Action to assign an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private AssignedItem SetAssignedItem(BizDC.ContentItem cont, bool IsContentCreateAssign, string sourceType)
        {
            var assignment = new AssignedItem()
            {
                Id = string.Empty,
                Title = string.Empty,
                DueDate = DateTime.MinValue,
                Category = "",
                Score = new Score()
                {
                    Correct = 0,
                    Possible = cont.DefaultPoints
                },
                IsContentCreateAssign = IsContentCreateAssign,
                Type = sourceType,
                SourceType = sourceType
            };

            if (null != cont)
            {
                //var cont = ContentActions.GetContent(Context.EntityId, item.Content.Id);
                assignment.Syllabus = AssignmentCenterHelper.FindFilter("PX_ASSIGNMENT_CENTER_SYLLABUS", true, false);
                assignment.Syllabus.ChildrenFilterSections.RemoveAll(i => i.Id.ToLowerInvariant().Contains("workspace"));

                assignment.Id = cont.Id;
                assignment.Title = cont.Title;
                assignment.Type = cont.Type;
                assignment.SyllabusFilter = cont.ToContentItem(ContentActions).SyllabusFilter;


                assignment.SourceType = string.IsNullOrEmpty(cont.Subtype) ? cont.Type : cont.Subtype;
                assignment.IsSendReminder = false;

                if (null != cont.AssignmentSettings && cont.AssignmentSettings.DueDate.Year != DateTime.MinValue.Year)
                {
                    assignment.DueDate = cont.AssignmentSettings.DueDate;
                    assignment.StartDate = cont.AssignmentSettings.StartDate;
                    assignment.Score = new Score() { Possible = cont.AssignmentSettings.Points };
                    assignment.Category = cont.AssignmentSettings.Category;
                    assignment.CompletionTrigger = (CompletionTrigger)cont.AssignmentSettings.CompletionTrigger;
                    assignment.IsGradeable = cont.AssignmentSettings.IsAssignable;
                    assignment.IsAllowLateSubmission = cont.AssignmentSettings.AllowLateSubmission;
                    assignment.IsAllowExtraCredit = cont.AssignmentSettings.IsAllowExtraCredit;
                    assignment.IsHighlightLateSubmission = cont.AssignmentSettings.IsHighlightLateSubmission;
                    assignment.IsAllowLateGracePeriod = cont.AssignmentSettings.IsAllowLateGracePeriod;
                    assignment.LateGraceDuration = cont.AssignmentSettings.LateGraceDuration;
                    assignment.LateGraceDurationType = cont.AssignmentSettings.LateGraceDurationType;
                    assignment.IsSendReminder = cont.AssignmentSettings.IsSendReminder;
                    assignment.GradeRule = (Models.GradeRule)cont.AssignmentSettings.GradeRule;

                    if (assignment.IsSendReminder)
                    {
                        assignment.ReminderEmail.AssignmentId = cont.AssignmentSettings.ReminderEmail.AssignmentId;
                        assignment.ReminderEmail.Subject = cont.AssignmentSettings.ReminderEmail.Subject;
                        assignment.ReminderEmail.Body = cont.AssignmentSettings.ReminderEmail.Body;
                        assignment.ReminderEmail.AssignmentDate = cont.AssignmentSettings.ReminderEmail.AssignmentDate;
                        assignment.ReminderEmail.DaysBefore = cont.AssignmentSettings.ReminderEmail.DaysBefore;
                        assignment.ReminderEmail.DurationType = cont.AssignmentSettings.ReminderEmail.DurationType;
                    }                    
                    var category = System.Configuration.ConfigurationManager.AppSettings["IsImportant"];
                    assignment.IsImportant = cont.Categories.Filter(x => x.Id == category).Count() > 0;
                }

                assignment.GradeBookWeights = Context.Course.ToGradeBookWeights();//GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();

                if (cont.Properties.ContainsKey("bfw_IncludeGbbScoreTrigger"))
                {
                    assignment.IncludeGbbScoreTrigger = (int)cont.Properties["bfw_IncludeGbbScoreTrigger"].Value;
                }


                if (cont.Properties.ContainsKey("bfw_SendReminder"))
                {
                    assignment.IsSendReminder = (Boolean)cont.Properties["bfw_SendReminder"].Value;
                }

                //assignment.IsContentCreateAssign = IsContentCreateAssign;


                //List of allowed content items which can be created from the Assign tab
                assignment.RelatedTemplates = cont.RelatedTemplates.Map(ci => ci.ToRelatedTemplate()).ToList();
            }

            assignment.AssignTabSettings = Context.Course.AssignTabSettings.ToAssignTabSettings();
            assignment.CourseType = Context.Course.CourseType;

            //if (assignment.Score.Possible == 0.0)
            //{
            //    assignment.Score.Possible = cont.DefaultPoints;
            //}

            return assignment;
        }

        public ActionResult RestrictedContent(ContentItem contentItem)
        {
            ViewData["TimeZone"] = Context.Course.GetCourseTimeZoneAbbreviation();
            ViewData.Model = contentItem;

            return View();
        }

        private IList<TocItem> GetSharedTocs(IList<string> sharedItemIds, IList<TocItem> tocs)
        {
            var sharedTocs = new List<TocItem>();
            foreach (var toc in tocs)
            {
                if (sharedItemIds.Contains(toc.Id))
                {
                    if (!toc.Children.IsNullOrEmpty())
                    {
                        toc.Children = GetSharedTocs(sharedItemIds, toc.Children.ToList());
                    }
                    sharedTocs.Add(toc);
                }
            }
            return sharedTocs;
        }

        /// <summary>
        /// Store student's minute spent on a content
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="durationMilliseconds"></param>
        /// <returns></returns>
        public ActionResult StoreContentDuration(string itemId, long durationMilliseconds)
        {
            if (itemId.IsNullOrEmpty() || Context.EnrollmentId.IsNullOrEmpty())
                return new EmptyResult();

            var model = ContentActions.GetContent(Context.EntityId, itemId).ToContentItem(ContentActions);
            if (!model.Type.IsNullOrEmpty() && model.Type.ToLowerInvariant() != "folder" &&
                model.Type.ToLowerInvariant() != "assignment" &&
                model.Type.ToLowerInvariant() != "pxunit" &&
                model.Type.ToLowerInvariant() != "quiz" &&
                model.Type.ToLowerInvariant() != "widgetconfiguration")
            {
                var submissions = GradeActions.GetSubmissions(Context.EnrollmentId, itemId);
                if (submissions == null || submissions.Count(o => o.SubmissionStatus == BizDC.SubmissionStatus.Graded) == 0)
                {
                    int durationInSeconds = Convert.ToInt32(durationMilliseconds) / 1000;
                    var startDate = DateTime.UtcNow.AddMilliseconds(durationMilliseconds*(-1));
                    ContentActions.StoreContentDuration(itemId, Context.EnrollmentId, durationInSeconds, startDate);
                }
            }

            return new EmptyResult();
        }

        public ActionResult CrunchItBridgePage(string version, string bcourse, string file)
        {
            var url = String.Format("{0}?dataset={1}",
                bcourse,
                file);

            switch (version)
            {
                case "2":
                    url = String.Format("{0}{1}",
                        ConfigurationManager.AppSettings.Get("CrunchIt2Url"),
                        url);

                    break;

                case "3":
                    var crunchItHelper = new CrunchItHelper();
                    var Expiration = crunchItHelper.GetExpirationToken(900);

                    url = String.Format("{0}{1}&exp={2}",
                        ConfigurationManager.AppSettings.Get("CrunchIt3Url"),
                        url,
                        Expiration);

                    var Signature = crunchItHelper.SignWithHMACSHA256(url);

                    url = String.Format("{0}&sig={1}",
                        url,
                        Signature);

                    break;

                default:
                    throw new ApplicationException("You've provided non-existing CrunchIt version!");
            }

            return Redirect(url);
        }
    }
}