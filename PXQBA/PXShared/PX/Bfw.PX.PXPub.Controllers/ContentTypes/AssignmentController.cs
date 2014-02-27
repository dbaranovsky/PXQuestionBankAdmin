using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.Common.JqGridHelper;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using Submission = Bfw.PX.PXPub.Models.Submission;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class AssignmentController : Controller
    {
        #region Properties
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IUserActions implementation.
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Access to a content helper object.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Actions for nav items.  
        /// </summary>
        /// <value>
        /// The navigation actions.
        /// </value>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Access to an IGradeActions implementation.  
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Gets or sets the resource map actions.
        /// </summary>
        /// <value>
        /// The resource map actions.
        /// </value>
        protected BizSC.IResourceMapActions ResourceMapActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.  
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the note actions.
        /// </summary>
        /// <value>
        /// The note actions.
        /// </value>
        protected BizSC.INoteActions NoteActions { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected IPxGradeBookActions GradeBookActions { get; set; }

        #endregion Properties
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="helper">The helper.</param>
        /// <param name="navActions">The nav actions.</param>
        /// <param name="gradeActions">The grade actions.</param>
        /// <param name="enrollmentActions">The enrollment actions.</param>
        /// <param name="resourceMapActions">The resource map actions.</param>
        /// <param name="noteActions">The note actions.</param>
        /// 
        public AssignmentController(BizSC.IBusinessContext context, BizSC.IUserActions userActions, BizSC.IContentActions contActions, ContentHelper helper, BizSC.INavigationActions navActions, BizSC.IGradeActions gradeActions, BizSC.IEnrollmentActions enrollmentActions, BizSC.IResourceMapActions resourceMapActions, INoteActions noteActions, AssignmentCenterHelper assignmentCenterHelper,
                                    IPxGradeBookActions gradebookactions)
        {
            Context = context;
            UserActions = userActions;
            ContentActions = contActions;
            ContentHelper = helper;
            NavigationActions = navActions;
            GradeActions = gradeActions;
            EnrollmentActions = enrollmentActions;
            ResourceMapActions = resourceMapActions;
            NoteActions = noteActions;
            AssignmentCenterHelper = assignmentCenterHelper;
            GradeBookActions = gradebookactions;
        }

        /// <summary>
        /// Saves a new assignment or updates and existing one
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>A view to display</returns>
        [ValidateInput(false)]
        public ActionResult SaveAssignment(Assignment content, string behavior, Assign assign, string toc = "syllabusfilter")
        {
            ActionResult result;
            ContentView model;

            switch (behavior.ToLowerInvariant())
            {
                case "cancel":
                    var idToLoad = string.IsNullOrEmpty(content.Id) ? content.ParentId : content.Id;
                    model = ContentHelper.LoadContentView(idToLoad, ContentViewMode.Preview, false, toc);
                    ViewData.Model = model;
                    result = View("DisplayItem", model);
                    break;

                case "save":
                    if (!ViewData.ModelState.IsValid)
                    {
                        content.EnvironmentUrl = Context.EnvironmentUrl;
                        content.CourseInfo = Context.Course.ToCourse();
                        content.EnrollmentId = Context.EnrollmentId;
                        content.Status = string.IsNullOrEmpty(content.Id) ? ContentStatus.New : ContentStatus.Existing;
                        content.Description = System.Web.HttpUtility.HtmlDecode(content.Description);
                        ViewData.Model = content;
                        result = View("CreateContent");
                        break;
                    }

                    if (content.DueDate.Year == DateTime.MinValue.Year)
                    {
                        var form = new FormCollection(ControllerContext.HttpContext.Request.Form);
                        var dd = form["DueDate"];

                        if (null != dd)
                        {
                            DateTime dt;
                            DateTime.TryParse(dd, out dt);
                            content.DueDate = dt;
                        }
                    }

                    ContentHelper.StoreAssignment(content, Context.EntityId);
                    if (assign != null && assign.DueDate.Year > DateTime.MinValue.Year)
                    {
                        assign.Id = content.Id;
                        AssignmentCenterHelper.AssignItem(assign, toc);
                    }
                    model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, false, toc);
                    ViewData.Model = model;
                    result = View("DisplayItem", model);
                    break;
                default:
                    result = View();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Saves a new dropbox or updates and existing one
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>A view to display</returns>
        [ValidateInput(false)]
        public ActionResult SaveDropbox(Dropbox content, string behavior, Assign assign, string toc = "syllabusfilter")
        {
            ActionResult result;
            ContentView model;

            switch (behavior.ToLowerInvariant())
            {
                case "cancel":
                    var idToLoad = string.IsNullOrEmpty(content.Id) ? content.ParentId : content.Id;
                    model = ContentHelper.LoadContentView(idToLoad, ContentViewMode.Preview, false, toc);
                    ViewData.Model = model;
                    result = View("DisplayItem", model);
                    break;

                case "save":
                    if (!ViewData.ModelState.IsValid)
                    {
                        content.EnvironmentUrl = Context.EnvironmentUrl;
                        content.CourseInfo = Context.Course.ToCourse();
                        content.EnrollmentId = Context.EnrollmentId;
                        content.Status = string.IsNullOrEmpty(content.Id) ? ContentStatus.New : ContentStatus.Existing;
                        content.Description = System.Web.HttpUtility.HtmlDecode(content.Description);
                        ViewData.Model = content;
                        result = View("CreateContent");
                        break;
                    }

                    if (content.DueDate.Year == DateTime.MinValue.Year)
                    {
                        var form = new FormCollection(ControllerContext.HttpContext.Request.Form);
                        var dd = form["DueDate"];

                        if (null != dd)
                        {
                            DateTime dt;
                            DateTime.TryParse(dd, out dt);
                            content.DueDate = dt;
                        }
                    }

                    ContentHelper.StoreAssignment(content, Context.EntityId);
                    if (assign != null && assign.DueDate.Year > DateTime.MinValue.Year)
                    {
                        assign.Id = content.Id;
                        AssignmentCenterHelper.AssignItem(assign, toc);
                    }
                    model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, false, toc);
                    ViewData.Model = model;
                    result = View("DisplayItem", model);
                    break;
                default:
                    result = View();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Adds the link to collection by storing it in Agilix.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="linkCollid">The link collid.</param>
        /// <param name="linkTitle">The link title.</param>
        /// <param name="linkUrl">The link URL.</param>
        /// <param name="isExternalLink">The is external link.</param>
        /// <returns>A view to display</returns>
        public ActionResult AddLinkToCollection(Assignment content, string linkCollid, string linkTitle,
            string linkUrl, bool? isExternalLink)
        {
            if (string.IsNullOrEmpty(linkTitle))
            {
                ModelState.AddModelError("linkTitle", "You must specify a title");
            }
            else if (string.IsNullOrEmpty(linkUrl))
            {
                ModelState.AddModelError("linkTitle", "You must specify a link URL");
            }
            else
            {
                isExternalLink = isExternalLink ?? false;

                if (isExternalLink.Value)
                {
                    if (linkUrl.StartsWith("www.") || !linkUrl.StartsWith("http://"))
                        linkUrl = "http://" + linkUrl;
                }

                ContentHelper.StoreAssignmentLink(content.Id, linkCollid, linkTitle, linkUrl, Context.EntityId);
            }

            var model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, true);

            return View("DisplayItem", model);
        }

        /// <summary>
        /// Removes the resources.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="docCollectionId">The doc collection id.</param>
        /// <param name="linkCollectionId">The link collection id.</param>
        /// <param name="docIds">The doc ids.</param>
        /// <param name="linkIds">The link ids.</param>
        /// <returns>A view to display</returns>
        public ActionResult RemoveResources(Assignment content, string docCollectionId, string linkCollectionId,
            string[] docIds, string[] linkIds)
        {
            var itemIds = new List<string>();

            if (docIds != null && docIds.Length > 0)
            {
                foreach (var id in docIds)
                {
                    itemIds.Add(id);
                }
            }

            if (linkIds != null && linkIds.Length > 0)
            {
                foreach (var id in linkIds)
                {
                    itemIds.Add(id);
                }
            }

            if (itemIds.Count > 0)
            {
                ContentActions.RemoveContents(Context.EntityId, itemIds);
            }

            var model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, true);

            var result = View("DisplayItem", model);

            return result;
        }

        /// <summary>
        /// Gets the name of the parent.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <returns>String</returns>
        private string GetParentName(string parentId)
        {
            var ci = ContentActions.GetContent(Context.EntityId, parentId);

            if (ci.Subtype == "pxunit")
            {
                if (ci.Properties.ContainsKey("bfw_syllabusfilter") && !string.IsNullOrEmpty(ci.Properties["bfw_syllabusfilter"].Value.ToString()))
                {
                    string bfw_syllabusfilter = ci.Properties["bfw_syllabusfilter"].Value.ToString();

                    return ci.Title + "<br /> " + GetParentName(bfw_syllabusfilter);

                }
            }

            return ci.Title;
        }

        /// <summary>
        /// Gets the toc tool tip.
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="isAssignmentCenter">if set to <c>true</c> [is assignment center].</param>
        /// <returns>A view to display</returns>
        public ActionResult GetTocToolTip(string assignmentId, bool isAssignmentCenter)
        {
            var child = NavigationActions.LoadNavigation(Context.EntityId, assignmentId, "");
            var tocItem = child.ToTocItem();

            if (child.Properties.ContainsKey("bfw_syllabusfilter") && !string.IsNullOrEmpty(child.Properties["bfw_syllabusfilter"].Value.ToString()))
            {
                //string bfw_syllabusfilter = child.Properties["bfw_syllabusfilter"].Value.ToString();
                tocItem.ParentLesson = "";// GetParentName(bfw_syllabusfilter);               
            }
            return View("Tooltip", tocItem);
        }

        /// <summary>
        /// Gets the students submission info.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="sidx">The sidx.</param>
        /// <param name="sord">The sord.</param>
        /// <param name="page">The page.</param>
        /// <param name="rows">The rows.</param>
        /// <returns>A view to display</returns>
        public ActionResult GetStudentsSubmissionInfo(Assignment content, string sidx, string sord, int page, int rows)
        {
            var item = ContentActions.GetContent(Context.EntityId, content.Id).ToContentItem(ContentActions);
            string action = "ViewAssignment";
            string controller = "Assignment";
            
            var submissions = GradeActions.GetStudentsSubmissionInfo(Context.EntityId, content.Id, EnrollmentActions);

            if (submissions != null && submissions.Any())
            {
                var model = from submission in submissions.AsQueryable()
                            select new
                            {
                                Id = submission.EnrollmentId,
                                Url = Url.Action(action, controller,
                                                new { id = content.Id, eId = submission.EnrollmentId }),
                                StudentName = submission.StudentFullName,
                                SubmittedDate = submission.SubmittedDate.ToString("g"),
                                SubDate = submission.SubmittedDate,
                                Comments = NoteActions.GetNoteCount(PxHighlightType.WritingAssignment, Context.CourseId, content.Id, "", Context.CurrentUser.Id, submission.EnrollmentId),
                                Grade = submission.Grade.RawAchieved > 0 ? submission.Grade.RawAchieved + "/" + submission.Grade.RawPossible : "None",
                                SubGrade = submission.Grade.RawAchieved > 0 ? submission.Grade.RawAchieved.ToString() : "None",
                                Status = submission.SubmissionStatus == SubmissionStatus.NotGraded ? "Not Graded" : submission.SubmissionStatus.ToString(),
                            };

                var result = model.ToJqGridData(page, rows, sidx + " " + sord, "",
                                                new[] {
                                                          "Id","Url","StudentName", "SubmittedDate", "Comments", "Grade", "SubGrade", "Status"
                                                      });

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        /// <summary>
        /// Gets the students.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="eid">The eid.</param>
        /// <returns>
        /// A json objects of students or null
        /// </returns>
        public JsonResult GetStudents(string id, string eid)
        {
            var submissions = GradeActions.GetStudentsSubmissionInfo(Context.EntityId, id, EnrollmentActions).AsQueryable();

            if (submissions.Any())
            {
                var model = from submission in submissions
                            select new
                            {
                                Eid = submission.EnrollmentId,
                                Text = submission.StudentFullName,
                                Value = Url.Action("ViewAssignment", "Assignment", new { id = id, eId = submission.EnrollmentId })
                            };

                var students = Json(model.ToList(), JsonRequestBehavior.AllowGet);

                return students;
            }

            return null;
        }

        /// <summary>
        /// Gets the assignment items.
        /// </summary>
        /// <param name="HasReferences">if set to <c>true</c> [has references].</param>
        /// <param name="HasRubrics">if set to <c>true</c> [has rubrics].</param>
        /// <returns>
        /// A json object of Assignment items
        /// </returns>
        public JsonResult GetAssignmentItems(bool HasReferences, bool HasRubrics)
        {
            var Items = new List<string>() { "Writing", "Details" };
            if (HasReferences) Items.Add("References");
            if (HasRubrics) Items.Add("Rubrics");

            var assignmentItems = new List<SelectListItem>();

            foreach (var i in Items)
            {
                assignmentItems.Add(new SelectListItem() { Text = i, Value = i });
            }

            assignmentItems[0].Selected = true;
            return Json(assignmentItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Composes the dropbox.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <returns>A view to display</returns>
        public ActionResult ComposeDropbox(string id, string path, string name, string toc = "syllabusfilter")
        {
            var model = new Dropbox();
            if (!string.IsNullOrEmpty(id))
            {
                var contentView = ContentHelper.LoadContentView(id, ContentViewMode.Preview, toc);
                model = (Dropbox)contentView.Content;
                model.EnrollmentId = Context.EnrollmentId;
                if (model.Submission == null) model.Submission = new Submission();
                model.Submission.Name = name;

                if (!string.IsNullOrEmpty(path))
                {
                    var resource = ContentActions.GetResource(Context.EnrollmentId, path);

                    using (var sw = new System.IO.StreamReader(resource.GetStream()))
                    {
                        model.Submission.Body = sw.ReadToEnd();
                        string comment = string.Empty;
                        resource.ExtendedProperties.TryGetValue(ResourceExtendedProperty.Comment.ToString(), out comment);
                        model.Submission.Notes = comment;
                    }

                    if (model.AssignmentStatus != AssignmentStatus.Submitted && model.AssignmentStatus != AssignmentStatus.Graded)
                    {
                        model.AssignmentStatus = AssignmentStatus.Saved;
                    }

                    model.Submission.ResourcePath = path;
                }
                else
                {
                    if (model.AssignmentStatus != AssignmentStatus.Submitted && model.AssignmentStatus != AssignmentStatus.Graded)
                    {
                        model.AssignmentStatus = AssignmentStatus.New;
                    }
                }
            }
            return View("~/Views/Shared/EditorTemplates/DropboxWriter.ascx", model);
        }

        /// <summary>
        /// Views the assignment submission.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="eid">The eid.</param>
        /// <returns>A view of Assignment submissions</returns>
        public ActionResult ViewAssignmentSubmission(string id, string eid)
        {
            var sub = new Submission();

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(eid))
            {
                sub = GradeActions.GetStudentSubmission(eid, id).ToSubmission();
            }

            return View("~/Views/Shared/DisplayTemplates/AssignmentPartials/AssignmentSubmission.ascx", sub);
        }

        /// <summary>
        /// Saves Assignment Submission
        /// </summary>
        /// <param name="content">The content</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>A json object of the submission</returns>
        [ValidateInput(false)]
        public JsonResult StoreAssignmentSubmission(Assignment content, string behavior)
        {
            var returnData = new Dictionary<string, string>();
            var status = "";
            string url;

            if (content.Submission == null || string.IsNullOrEmpty(content.Submission.ResourcePath))
            {
                var resId = Guid.NewGuid().ToString("N");
                url = string.Format("Templates/Data/XmlResources/Documents/Assignments/{0}.pxres", resId);
            }
            else
            {
                url = content.Submission.ResourcePath;
            }

            switch (behavior.ToLowerInvariant())
            {
                case "submit":
                case "submit for grade":
                case "resubmit":
                    ContentItem ciItem = ContentActions.GetContent(Context.EntityId, content.Id);
                    if (content.Submission == null || !content.Submission.Body.IsNullOrEmpty() || (ciItem.Subtype.ToLowerInvariant() == "eportfolio" && content.Submission.Body.IsNullOrEmpty()))
                    {
                        status = "submitted";
                        var submission = new BizDC.Submission
                        {
                            ItemId = content.Id,
                            Body = content.Submission.Body,
                            SubmissionType = SubmissionType.Assignment,
                            SubmittedDate = DateTime.Today
                        };


                        GradeActions.AddStudentSubmission(Context.EntityId, submission);
                        SaveSubmission(content, url, status);
                    }
                    else
                    {
                        status = "nosubmissionsaved";
                    }
                    break;
                case "save":
                case "save as":
                    status = "saved";
                    SaveSubmission(content, url, status);
                    break;
                case "cancel":
                    status = "canceled";
                    break;

            }
            //Adds the item as updated by student
            ContentHelper.AddItemForReview(content.Id);

            returnData.Add("status", status);
            returnData.Add("path", url);
            returnData.Add("submitteddate", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            return Json(returnData);
        }

        /// <summary>
        /// Saves Dropbox Submission
        /// </summary>
        /// <param name="content">The content</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>A json object of the submission</returns>
        [ValidateInput(false)]
        public JsonResult StoreDropboxSubmission(Dropbox content, string behavior)
        {
            var returnData = new Dictionary<string, string>();
            var status = "";
            string url;

            if (content.Submission == null || string.IsNullOrEmpty(content.Submission.ResourcePath))
            {
                var resId = Guid.NewGuid().ToString("N");
                url = string.Format("Templates/Data/XmlResources/Documents/Assignments/{0}.pxres", resId);
            }
            else
            {
                url = content.Submission.ResourcePath;
            }

            ContentItem ciItem = ContentActions.GetContent(Context.EntityId, content.Id);
            status = "submitted";
            var submission = new BizDC.Submission
            {
                ItemId = content.Id,
                Body = content.Submission.Body,
                SubmissionType = SubmissionType.Assignment,
                SubmittedDate = DateTime.Today,
                Notes = content.Submission.Notes
            };


            GradeActions.AddStudentSubmission(Context.EntityId, submission);
            SaveSubmission(content, url, status);

            //Adds the item as updated by student
            ContentHelper.AddItemForReview(content.Id);

            returnData.Add("status", status);
            returnData.Add("path", url);
            returnData.Add("submitteddate", DateTime.Today.ToShortDateString());
            return Json(returnData);
        }


        /// <summary>
        /// Saves the submission.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="url">The URL.</param>
        /// <param name="status">The status.</param>
        private void SaveSubmission(Assignment content, string url, string status)
        {
            string enrollmentId = Context.EnrollmentId;

            var resource = new XmlResource
            {
                Status = ResourceStatus.Normal,
                Url = url,
                EntityId = enrollmentId,
                Title = content.Submission.Name,
                Body = content.Submission.Body
            };

            resource.ExtendedProperties.Add(ResourceExtendedProperty.AssignmentId.ToString(), content.Id);
            resource.ExtendedProperties.Add(ResourceExtendedProperty.Status.ToString(), status);
            resource.ExtendedProperties.Add(ResourceExtendedProperty.WordCount.ToString(), content.Submission.WordCount);
            ContentActions.StoreResources(new List<Resource> { resource });
            ResourceMapActions.AddResourceMap(resource, content.Id, "Assignment");
        }

        /// <summary>
        /// Saves the submission.
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="docIds">The doc ids.</param>
        public void SaveSubmission(string assignmentId, string docIds)
        {
            ResourceMapActions.AddResourceMap(docIds, assignmentId, "Assignment");
        }


        /// <summary>
        /// Saves Assignment Submission.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>String</returns>
        public string StoreAssignmentGrade(Assignment content, string behavior)
        {
            var status = "";
            var response = content.ToTeacherResponse(behavior);
            switch (behavior.ToLowerInvariant())
            {
                case "save":
                    status = "saved";
                    break;
                case "submit":
                    status = "submitted";
                    break;
                case "unsubmit":
                    status = "unsubmitted";
                    break;
            }

            //Check for decimals
            if (response.PointsAssigned.ToString().IndexOf(".") != -1)
            {
                status = "Error:Decimal Score is not allowed";
            }

            //Check for grading limits
            else if (response.PointsAssigned < 0 || response.PointsAssigned > content.PossibleScore)
            {
                status = "Error:Please enter grade between 0 and " + response.PointsPossible.ToString();
            }
            else
            {
                GradeActions.AddTeacherResponse(content.EnrollmentId, content.Id, response);
            }

            return status;
        }

        /// <summary>
        /// Gets the submission status.
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <returns>A view of the submission status</returns>
        public ActionResult GetSubmissionStatus(string assignmentId)
        {
            if (Context.AccessLevel == AccessLevel.Student)
            {
                var ci = ContentActions.GetContent(Context.EntityId, assignmentId);
                var grades = GradeActions.GetGradesByEnrollment(Context.EnrollmentId, new List<string>() { ci.Id }).FirstOrDefault();
                return grades.Attempts > 0 ? Content(SubmissionStatus.Completed.ToString()) : Content("Not Completed");
            }
            else if (Context.AccessLevel == AccessLevel.Instructor)
            {
                var ci = ContentActions.GetContent(Context.EntityId, assignmentId);
                var grades = GradeActions.GetGrades(Context.EntityId, new List<string>() { ci.Id });
                grades = grades.ToList().Where(e => e.Attempts > 0);
                int gradedItemsCount = grades.Count();

                var students = EnrollmentActions.GetEntityEnrollments(Context.EntityId).Filter(e => e.Flags.Split(',').Contains("Participate")).Map(e => e.ToStudent()).ToList();
                string studentCount = students.Count.ToString();
                string completedCount = gradedItemsCount.ToString();
                double percent = double.Parse(completedCount) / double.Parse(studentCount) * 100;
                if (double.IsNaN(percent))
                    percent = 0;

                string percentage = String.Format("{0:0.00}", percent);
                String status = String.Format("{0} of {1} students ({2}%) completed ", completedCount, studentCount, percentage);
                return Content(status);

            }

            return new EmptyResult();
        }

        /// <summary>
        /// Gets the submission status.
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <returns>A view of the submission status</returns>
        public ActionResult GetSubmissionStatusForManagementCard(string assignmentId)
        {
            if (Context.AccessLevel == AccessLevel.Student)
            {
                var ci = ContentActions.GetContent(Context.EntityId, assignmentId);
                var grades = GradeActions.GetGradesByEnrollment(Context.EnrollmentId, new List<string>() { ci.Id }).FirstOrDefault();

                return grades.Attempts > 0 ? Content(SubmissionStatus.Completed.ToString()) : Content("Not Completed");
            }
            else if (Context.AccessLevel == AccessLevel.Instructor)
            {
                var ci = ContentActions.GetContent(Context.EntityId, assignmentId);
                var grades = GradeActions.GetGrades(Context.EntityId, new List<string>() { ci.Id });
                int gradedItemsCount = 0;
                var avgScore = string.Empty;
                double allSumGrades = 0;

                foreach (var grade in grades)
                {
                    if (grade != null 
                        && ((grade.Status & GradeStatus.Completed) == GradeStatus.Completed 
                                || (grade.Status & GradeStatus.ShowScore) == GradeStatus.ShowScore))
                    {
                        gradedItemsCount++;
                        allSumGrades += grade.Achieved;
                        
                        //special case for learning curve:
                        if (ci.FacetMetadata.ContainsKey("meta-content-type") &&
                            ci.FacetMetadata["meta-content-type"].Contains("LearningCurve"))
                        {
                            if (grade.Achieved <= 0)
                            {
                                gradedItemsCount--;
                                allSumGrades -= grade.Achieved;
                            }
                        }
                    }
                }
                avgScore = String.Format("<br/>Avg Score: {0}", gradedItemsCount > 0 ? (allSumGrades / gradedItemsCount).ToString() : "-");
                var students = EnrollmentActions.GetEntityEnrollments(Context.EntityId).Filter(e => e.Flags.Split(',').Contains("Participate")).Map(e => e.ToStudent()).ToList();
                string studentCount = students.Count.ToString();
                string completedCount = gradedItemsCount.ToString();
                double percent = double.Parse(completedCount) / double.Parse(studentCount) * 100;

                if (double.IsNaN(percent))
                {
                    percent = 0;
                }

                string percentage = String.Format("{0:0.00}", percent);
                String status = String.Format("{0} of {1} students ({2}%) completed{3}", completedCount, studentCount, percentage, avgScore);
                return Content(status);
            }

            return new EmptyResult();
        }

        public List<Resource> GetStudentSubmittedResources(string assignmentId)
        {
            return ResourceMapActions.GetResourcesForItem(assignmentId).ToList();


        }

        /// <summary>
        /// Gets the student dropbox submitted document.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>View</returns>
        public ActionResult GetStudentDropboxDocuments(string id, bool isAllowSubmission)
        {
            var model = new Dropbox();
            var resource = ResourceMapActions.GetResourcesForItem(id).LastOrDefault();
            var resources = new List<object>();
            var submissionInfo = GradeActions.GetStudentSubmissionInfo(Context.EntityId, id, Context.EnrollmentId, EnrollmentActions);
            var latestVersion = 0;

            if (submissionInfo.Any())
            {
                latestVersion = submissionInfo.Max(x => x.Version);
            }
            if (resource != null)
            {
                string status, comment = "", filename = "", filesize = "";
                bool allowReSubmission = false;
                resource.ExtendedProperties.TryGetValue(ResourceExtendedProperty.Status.ToString(), out status);
                resource.ExtendedProperties.TryGetValue(ResourceExtendedProperty.Comment.ToString(), out comment);
                resource.ExtendedProperties.TryGetValue(ResourceExtendedProperty.FileName.ToString(), out filename);
                resource.ExtendedProperties.TryGetValue(ResourceExtendedProperty.FileSize.ToString(), out filesize);

                var response = GradeActions.GetTeacherResponse(Context.EnrollmentId, id);
                var grade = GradeActions.GetGradesByEnrollment(Context.EnrollmentId, new List<string> { id });

                status = "Submitted";
                if (response.Status != BizDC.GradeStatus.None && response.ScoredVersion != 0)
                {
                    status = "Graded";
                }
                if (response.Status.ToString().IndexOf(BizDC.GradeStatus.AllowResubmission.ToString()) > -1)
                {
                    allowReSubmission = true;
                }
                model.SubmitedDocTitle = resource.Name;
                model.SubmitedDocDate = resource.ModifiedDate.ToString("g");
                model.SubmitedDocComment = comment;
                model.Id = id;
                model.StudentSubmittedFilename = filename;
                model.StudentSubmittedFileSize = filesize;
                model.StudentSubmittedFileEditUrl = Url.Action("ComposeDropbox", "Assignment", new { id = id, path = resource.Url, name = resource.Name });
                model.StudentSubmitStatus = status.ToLowerInvariant();
                model.AllowReSubmission = allowReSubmission;
                if (grade.Count() > 0)
                {
                    model.PointsAssigned = grade.FirstOrDefault().Achieved;
                }
                model.PointsPossible = response.PointsPossible;
                model.TeacherComment = response.TeacherComment;
                model.IsAllowSubmission = isAllowSubmission;
                ViewData["TeacherAttachments"] = response.TeacherAttachments;
                long strLength = 0;
                if (response.ResourceStream != null)
                {
                    strLength = response.ResourceStream.Length;
                }
                model.TeacherAttachmentFileSize = Convert.ToInt32(strLength) / 1000 + "K";
            }

            ViewData["timeZone"] = Context.Course.GetCourseTimeZoneAbbreviation();

            return View("~/Views/Shared/DropboxSubmittedDocument.ascx", model);
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public string DeleteDocs(string eid, string resourceIdList)
        {
            ContentActions.RemoveResources(new List<string>(resourceIdList.Split(',')));
            return "success";
        }

        public ActionResult ViewReflectionBody(string id, string eid, string toc = "syllabusfilter")
        {
            var resource = ResourceMapActions.GetResourcesForItem(id, eid).FirstOrDefault();
            if (resource == null)
            {
                return null;
            }

            var xmlResource = new XmlResource();
            var submission = GradeActions.GetStudentSubmissionInfo(Context.EntityId, id, Context.EnrollmentId, EnrollmentActions).FirstOrDefault();
            if (submission != null)
            {
                var submissionId = submission.ItemId;
                xmlResource = (XmlResource)ContentActions.GetResource(Context.EnrollmentId, resource.Url);
                submission.Body = ResourceMapActions.GetResourceContent(xmlResource);
                var newSubmission = SubmissionMapper.ToSubmission(submission);
                return View("~/Views/ReflectionSubmissionBody.ascx", newSubmission);
            }
            else
            {
                xmlResource = (XmlResource)ContentActions.GetResource(eid, resource.Url);
                xmlResource.Body = ResourceMapActions.GetResourceContent(xmlResource);
            }
            ResourceDocument rd = ResourceDocumentMapper.ToResourceDocument(xmlResource);
            rd.body = xmlResource.Body;
            return View("~/Views/Browser/ReflectionBody.ascx", rd);
        }

        public ActionResult ComposeReflectionAssignment(string id, string path, string name, string toc = "syllabusfilter")
        {
            string enrollmentId = Context.EnrollmentId;
            
            var model = new Assignment();
            var reflectionResource = ResourceMapActions.GetResourcesForItem(id).FirstOrDefault();
            if (path == null && reflectionResource != null)
            {
                path = reflectionResource.Url;
            }

            if (!string.IsNullOrEmpty(id))
            {
                var contentView = ContentHelper.LoadContentView(id, ContentViewMode.Preview, toc);
                model = (Assignment)contentView.Content;
                model.EnrollmentId = enrollmentId;
                if (model.Submission == null) model.Submission = new Submission();
                model.Submission.Name = name;

                if (!string.IsNullOrEmpty(path))
                {
                    var resource = ContentActions.GetResource(enrollmentId, path);

                    using (var sw = new System.IO.StreamReader(resource.GetStream()))
                    {
                        model.Submission.Body = sw.ReadToEnd();
                    }

                    if (model.AssignmentStatus != AssignmentStatus.Submitted && model.AssignmentStatus != AssignmentStatus.Graded)
                    {
                        model.AssignmentStatus = AssignmentStatus.Saved;
                    }

                    model.Submission.ResourcePath = path;
                }
                else
                {
                    if (model.AssignmentStatus != AssignmentStatus.Submitted && model.AssignmentStatus != AssignmentStatus.Graded)
                    {
                        model.AssignmentStatus = AssignmentStatus.New;
                    }
                }
            }
            return View("~/Views/Shared/EditorTemplates/ReflectionAssignmentWriter.ascx", model);
        }

        public ActionResult AssignmentItemCount()
        {
            var bizContentItemsToc = ContentActions.ListChildren(Context.EntityId, "PX_PROJECT_ID", 5, "bfw_toc_projects", false);
            int count = 0;
            foreach (var i in bizContentItemsToc.Map(i => i.ToContentItem(ContentActions)).ToList())
            {
                var dueDateFormatted = i.DueDate.ToString().Substring(0, i.DueDate.ToString().IndexOf(' '));

                if (i.IsAssigned)
                {
                    count++;
                }
            }


            return Content(count.ToString());

        }

        #region Implementaion
        public Dictionary<string, string> ConvertListToDictionary(List<string> data)
        {
            var dict = new Dictionary<string, string>();
            if (null == data)
            {
                return dict;
            }
            for (int i = 0; i< data.Count; i = i + 2)
            {
                if (!string.IsNullOrEmpty(data[i])) 
                {
                    dict[data[i]] = data[i + 1];
                }
            }
            return dict;
        }
        #endregion Implementaion
    }
}
