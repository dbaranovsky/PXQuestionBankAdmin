using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Aspose.Words.Lists;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Models;
using AssignedItem = Bfw.PX.PXPub.Models.AssignedItem;
using AssignmentCenterItem = Bfw.PX.PXPub.Models.AssignmentCenterItem;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.PXPub.Models.ContentItem;
using TocCategory = Bfw.PX.PXPub.Models.TocCategory;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class FacePlateController : Controller
    {
        /// <summary>
        /// Contains business layer context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }


        /// <summary>
        /// Grade actions
        /// </summary>
        protected BizSC.IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Course Actions
        /// </summary>
        protected BizSC.ICourseActions CourseActions { get; set; }


        /// <summary>
        /// Enrollment Actions
        /// </summary>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }


        /// <summary>
        /// Access to an IBookmarkActions implementation.
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
        /// The rss feed actions.
        /// </summary>
        /// <value>
        /// The rss feed actions.
        /// </value>
        protected BizSC.IRSSFeedActions RSSFeedActions { get; set; }

        /// <summary>
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }

        protected BizSC.IPageActions PageActions { get; set; }

        public FacePlateController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions,
            BizSC.IGradeActions gradeActions,
            BizSC.ICourseActions courseActions,
            BizSC.IEnrollmentActions enrollmentActions,
            BizSC.IRubricActions rubricAction,
            AssignmentCenterHelper assignmentCenterHelper,
            BizSC.IRSSFeedActions rssFeedActions,
            ContentHelper contentHelper,
            BizSC.IPageActions pageActions)
        {
            Context = context;
            ContentActions = contentActions;
            GradeActions = gradeActions;
            CourseActions = courseActions;
            EnrollmentActions = enrollmentActions;
            RubricActions = rubricAction;
            AssignmentCenterHelper = assignmentCenterHelper;
            RSSFeedActions = rssFeedActions;
            ContentHelper = contentHelper;
            PageActions = pageActions;
        }

        
        private bool HasSubmission(List<ContentItem> items)
        {
            foreach (var item in items)
            {
                var grades = GradeActions.GetGrades(Context.EntityId, new List<string>() { item.Id });
                grades = grades.ToList().Where(e => e.Attempts > 0);
                if (grades.Count() > 0)
                    return true;

                if (item is PxUnit)
                {
                    var unit = (PxUnit)item;
                    if (HasSubmission(unit.GetAssociatedItems()))
                        return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Item Heirachy HasSubmission
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns></returns>
        public ActionResult ItemHeirachyHasSubmission(string contentItemId)
        {
            List<BizDC.ContentItem> data = null;
            var dcContentItem = data != null ? data.FirstOrDefault(i => i.Id == contentItemId) : null;
            var contentItem = new ContentItem();

            if (dcContentItem != null)
            {
                contentItem = dcContentItem.ToContentItem(ContentActions, false, data, false);
            }
            else
            {
                dcContentItem = ContentActions.GetContent(Context.EntityId, contentItemId);
            }

            contentItem = dcContentItem.ToContentItem(ContentActions, false, data, false);
            contentItem.MaxPoints = dcContentItem.MaxPoints;
            bool hasSubmission = HasSubmission(new List<ContentItem>() { contentItem });
            return Content(hasSubmission.ToString());
        }


        /// <summary>
        /// Show the Assign Screen
        /// </summary>
        /// <param name="cont"></param>
        /// <returns></returns>
        public AssignedItem Assign(BizDC.ContentItem cont)
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
                }
            };
            AssignedItemMapper.PopulateGradeSettings(assignment, cont.ItemDataXml);
            if (null != cont)
            {
                //var cont = ContentActions.GetContent(Context.EntityId, item.Content.Id);
                //assignment.Syllabus = AssignmentCenterHelper.FindFilter("PX_ASSIGNMENT_CENTER_SYLLABUS", true, false);
                //assignment.Syllabus.ChildrenFilterSections.RemoveAll(i => i.Id.ToLowerInvariant().Contains("workspace"));

                assignment.Id = cont.Id;
                assignment.Title = cont.Title;
                assignment.Type = cont.Type;
                //assignment.SyllabusFilter = cont.ToContentItem(ContentActions).SyllabusFilter;


                assignment.SourceType = string.IsNullOrEmpty(cont.Subtype) ? cont.Type : cont.Subtype;

                assignment.IsSendReminder = false;

                if (null != cont.AssignmentSettings && (cont.AssignmentSettings.DueDate.Year != DateTime.MinValue.Year || cont.AssignmentSettings.Points > 0))
                {
                    assignment.DueDate = cont.AssignmentSettings.DueDate;
                    assignment.StartDate = cont.AssignmentSettings.StartDate;
                    assignment.Score = new Score() { Possible = cont.AssignmentSettings.Points };
                    assignment.Category = cont.AssignmentSettings.Category;
                    assignment.CompletionTrigger = (CompletionTrigger)cont.AssignmentSettings.CompletionTrigger;
                    assignment.SubmissionGradeAction = (SubmissionGradeAction)cont.AssignmentSettings.SubmissionGradeAction;
                    assignment.IsGradeable = cont.AssignmentSettings.IsAssignable;
                    assignment.IsAllowLateSubmission = cont.AssignmentSettings.AllowLateSubmission;
                    assignment.IsHighlightLateSubmission = cont.AssignmentSettings.IsHighlightLateSubmission;
                    assignment.IsAllowLateGracePeriod = cont.AssignmentSettings.IsAllowLateGracePeriod;
                    assignment.IsAllowExtraCredit = cont.AssignmentSettings.IsAllowExtraCredit;
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

                if (null != cont.AssignmentSettings && (!cont.AssignmentSettings.Category.IsNullOrEmpty()))
                {
                    assignment.Category = cont.AssignmentSettings.Category;
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

                assignment.IsContentCreateAssign = true;


                //List of allowed content items which can be created from the Assign tab
                assignment.RelatedTemplates = cont.RelatedTemplates.Map(ci => ci.ToRelatedTemplate()).ToList();
            }

            assignment.AssignTabSettings = Context.Course.AssignTabSettings.ToAssignTabSettings();
            assignment.CourseType = Context.Course.CourseType;
            return assignment;
        }

        /// <summary>
        /// Show the Move or Copy dialog
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowMoveOrCopy(string sourceId)
        {
            ViewData["sourceId"] = sourceId;
            var assignedWidget = this.PageActions.GetWidget("PX_LAUNCHPAD_MOVECOPY_WIDGET").ToWidgetItem();
            return View(new List<Widget> { assignedWidget });
        }

        /// <summary>
        /// Show the existing assessments dialog
        /// </summary>
        /// <returns></returns>
        public ActionResult AddQuestionToExistingAssessment(string questionId)
        {
            ViewData["questionId"] = questionId;
            var assignedWidget = this.PageActions.GetWidget("PX_LAUNCHPAD_ASSESSMENT_BROWSER_WIDGET").ToWidgetItem();
            return View(new List<Widget> { assignedWidget });
        }

        public ActionResult MoveContentItem(string contentId, string newParentId, string previousParentId, string toc = "syllabusfilter")
        {
            List<AssignmentCenterItem> changes = new List<AssignmentCenterItem>();
            var node = new AssignmentCenterItem();

            node.Id = contentId;
            node.ParentId = newParentId;

            if (previousParentId == "")
            {
                var content = ContentActions.GetContent(Context.EntityId, contentId).ToContentItem(ContentActions, false, null, false);
                previousParentId = content.GetSyllabusFilterFromCategory(toc);
            }

            node.PreviousParentId = previousParentId;
            var newParent = ContentActions.GetContent(Context.EntityId, newParentId).ToContentItem(ContentActions, true, null, false);
            UpdateContainer(newParentId, node, newParent, toc);
            var maxSequence = ((PxUnit)newParent).GetAssociatedItems().Max(i => i.Sequence);
            node.Sequence = Context.Sequence(maxSequence, "");

            changes.Add(node);
            ContentActions.UpdateAssignmentCenterItems(newParentId, changes.Map(c => c.ToAssignmentCenterItem()), toc);

            var dcParents = ContentHelper.GetParentHeirachy(node.Id, TreeCategoryType.ManagementCard, toc);
            var parents = dcParents.Map(biz => biz.ToContentItem(ContentActions, false)).ToList();

            return Content(parents.Last().Id);
        }

        private static void UpdateContainer(string newParentId, AssignmentCenterItem node, ContentItem newParent, string toc = "syllabusfilter")
        {
            //the new subcontainerid is the new parent's subcontainer id if it exists. Otherwise it is the new new parent's id
            if (newParent != null)
            {
                node.Containers = newParent.Containers;
                node.SubContainerIds = newParent.SubContainerIds;
                node.SetContainer(!string.IsNullOrWhiteSpace(newParent.GetContainer(toc)) ? newParent.GetContainer(toc) : "Launchpad", toc);
                var subcontainer = newParent.GetSubContainer(toc);
                node.SetSubContainer(!string.IsNullOrWhiteSpace(subcontainer)
                                          ? subcontainer
                                          : newParentId, toc);
            }
        }
        /// <summary>
        /// Updates the item status in the FNE header
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ActionResult UpdateItemStatus(string itemId)
        {
            return View("~/Views/Shared/LaunchPadItemStatus.ascx");
        }

        /// <summary>
        /// Copy content
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newParentId"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult CopyContent(ContentItem item, string newParentId, string level)
        {
            var myMaterialsId = System.Configuration.ConfigurationManager.AppSettings["MyMaterials"];
            var myMaterials = new Biz.DataContracts.TocCategory()
            {
                Id = myMaterialsId,
                ItemParentId = string.Format("{0}_{1}", myMaterialsId, Context.CurrentUser.Id),
                Sequence = "",
                Text = myMaterialsId
            };

            var newItem = ContentActions.CopyItem(Context.CourseId, 
                    item.Id, 
                    Guid.NewGuid().ToString(), 
                    newParentId,
                    new List<Biz.DataContracts.TocCategory> { 
                            myMaterials
                    }, 
                    false,
                    item.Title, 
                    item.SubTitle, 
                    item.Description, 
                    includePoints: false);

            var source = ContentActions.GetContent(Context.EntityId, item.Id);
            var ci = source.ToContentItem(ContentActions, false);

            if (ci is DocumentCollection)
            {
                var sourceCollection = ci as DocumentCollection;

                foreach (var document in sourceCollection.Documents)
                {
                    ContentHelper.CopyDocumentResource(newItem.Id, document, Context.CourseId); 
                }
            }

            if (ci is LinkCollection)
            {
                var sourceCollection = ci as LinkCollection;

                foreach (var link in sourceCollection.Links)
                {
                    ContentHelper.StoreLink(newItem.Id, link.Title, link.Url, Context.CourseId); 
                }                
            }

            return new JsonDataContractResult(newItem.ToAssignment(ContentActions).ToAssignmentCenterItem());
        }
        
        public bool RemoveItemMyMaterial(string contentId, string toc = "syllabusfilter")
        {
            var ci = ContentActions.GetContent(Context.EntityId, contentId);
            ci.Categories.Clear();
            string category = System.Configuration.ConfigurationManager.AppSettings["MyMaterials"];
            ci.SetContainer("", toc);
            ci.SetSubContainer("", toc);
            ContentActions.StoreContent(ci, Context.EntityId, ignoreCategory: category);

            return true;
        }

        public ActionResult FacePlateLaunchPadTab()
        {
            var assignedWidget = this.PageActions.GetWidget("PX_LAUNCHPAD_ASSIGNED_WIDGET").ToWidgetItem();
           // var unassignedWidget = this.PageActions.GetWidget("PX_LAUNCHPAD_UNASSIGNED_WIDGET").ToWidgetItem();


            return View(new List<Widget> { assignedWidget });
        }
    }
}
