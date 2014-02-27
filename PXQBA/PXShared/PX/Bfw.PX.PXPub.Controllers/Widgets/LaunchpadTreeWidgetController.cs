using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.XPath;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Models;
using Bfw.PX.Abstractions;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.PXPub.Models.ContentItem;
using AssignedItem = Bfw.PX.PXPub.Models.AssignedItem;
using Container = Bfw.PX.PXPub.Models.Container;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// LaunchpadTreeWidget settings in bfw_properties:
    /// bfw_showitemsonly : a value of 'assigned' would only show assigned chapters whereas a value of 'unassigned' would only show unassigned items in root level.
    /// bfw_disabledraganddrop : when set to true this option will disable drag and drop.
    /// bfw_disableediting : when set to true this option will disable editing.
    /// bfw_showcollapseunassigned : when set to true this option will show collapse unassigned option.
    /// bfw_collapseunassigned : when set to true with "bfw_showcollapseunassigned" being true this option will collapse unassigned by default.
    /// bfw_toggleduelater : when set to true this option will show allow users to hide/show items in root level that are due later (14 days by default).
    /// bfw_toggleduelaterdays : number of days value for bfw_toggleduelater
    /// bfw_togglepastdue : hide/show chapters that are past due
    /// bfw_launchpadtitle : launchapd title
    /// bfw_grayoutpastduelater : when set to true this option will gray out past due and dule later items in root level
    /// bfw_sortbyduedate : when set to true this option sorts the launchpad tree by due date
    /// </summary>
    public class LaunchpadTreeWidgetController : Controller, IPXWidget
    {
        internal readonly string DefaultToc;
        internal readonly string DefaultContainer;
        internal readonly string DefaultSubContainer;

        /// <summary>
        /// Contains helper functions related to the Tree Widget 
        /// </summary>
        protected ITreeWidgetHelper Helper { get; set; }

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
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }

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

        protected BizSC.IPageActions PageActions { get; set; }

        protected BizSC.IRubricActions RubricActions { get; set; }

        public LaunchpadTreeWidgetController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions,
            BizSC.IGradeActions gradeActions,
            BizSC.ICourseActions courseActions,
            BizSC.IEnrollmentActions enrollmentActions,
            BizSC.IRubricActions rubricAction,
            AssignmentCenterHelper assignmentCenterHelper,
            BizSC.IRSSFeedActions rssFeedActions,
            ContentHelper contentHelper,
            BizSC.IPageActions pageActions,
            ITreeWidgetHelper helper)
        {
            Context = context;
            ContentActions = contentActions;
            GradeActions = gradeActions;
            CourseActions = courseActions;
            EnrollmentActions = enrollmentActions;
            RubricActions = rubricAction;
            AssignmentCenterHelper = assignmentCenterHelper;
            PageActions = pageActions;
            Helper = helper;

            DefaultToc = helper.DefaultToc;
            DefaultContainer = helper.DefaultContainer;
            DefaultSubContainer = helper.DefaultSubContainer;
        }

        /// <summary>
        /// Does an assignment center opration on an item
        /// </summary>
        /// <param name="itemId">The item unique identifier.</param>
        /// <param name="targetId">The target unique identifier.</param>
        /// <param name="assignedItem">The assigned item.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="entityId">The entity unique identifier.</param>
        /// <param name="toc">The toc.</param>
        /// <returns></returns>
        public ActionResult ItemOperation(string itemId, string targetId,
                                          AssignedItem assignedItem, AssignmentCenterOperation operation, bool keepInGradebook = true,
                                          string toc = "syllabusfilter", string entityId = "")
        {
            return new JsonDataContractResult(AssignmentCenterHelper.ItemOperation(itemId, targetId, assignedItem, operation, keepInGradebook, toc: toc, entityId: entityId));
        }

        /// <summary>
        /// Persists the state of the tree. Returns JSON representation of tree
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="widgetId">The id of the widget the operation is being performed on. We need this so we can pull
        /// in the settings.</param>
        /// <param name="toc">TOC this item operation is happening on.</param>
        /// <returns>
        /// JSON object representing the new state of the category.
        /// </returns>
        public ActionResult SaveNavigationState(AssignmentCenterNavigationState state, string widgetId = "",
            string toc = "syllabusfilter", bool keepInGradebook = true)
        {
            return new JsonDataContractResult(AssignmentCenterHelper.SaveNavigationState(state, toc, keepInGradebook));
        }

        /// <summary>
        /// Persists the state of the tree. Returns HTML representation of changed item
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="widgetId">The widget unique identifier.</param>
        /// <returns>
        /// JSON object representing the new state of the category.
        /// </returns>
        public ActionResult SaveNavigationStateView(AssignmentCenterNavigationState state, string widgetId = "",
            string toc = "syllabusfilter", bool keepInGradebook = true)
        {
            var changes = AssignmentCenterHelper.SaveNavigationState(state, toc, keepInGradebook).Changes;

            return LoadItems(changes, widgetId, toc);
        }

        /// <summary>
        /// Updates the item status in the FNE header
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ActionResult UpdateItemStatus(string itemId)
        {
            TreeWidgetViewItem item = null;
            var items = LoadItemWithUpdatedParent(itemId, "1", "");
            if (items != null && items.Count > 0)
            {
                item = items.First();
            }
            if (item != null)
            {
                item.Item.UserAccess = Context.AccessLevel;
            }
            return View("~/Views/Shared/LaunchPadItemStatus.ascx", item);
        }

        public ActionResult UpdateAssignmentUnit(string unitId)
        {
            return View("~/Views/Shared/AssignmentUnitSelector.ascx");
        }

        public ActionResult LoadItem(string itemId, string widgetId)
        {
            Models.AssignmentCenterItem item = new Models.AssignmentCenterItem()
            {
                Id = itemId,
                Level = "1"
            };
            return LoadItems(new List<Models.AssignmentCenterItem>() { item }, widgetId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aitems"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        private ActionResult LoadItems(List<Models.AssignmentCenterItem> aitems, string widgetId = "", 
            string toc = "syllabusfilter")
        {
            var items = new List<TreeWidgetViewItem>();

            //TODO: Load widget and add setting back into item.  Refactor settings load.
            foreach (var aitem in aitems)
            {
                var updatedItems = LoadItemWithUpdatedParent(aitem.Id, aitem.Level, widgetId, toc);

                if (updatedItems != null)
                {
                    items.AddRange(updatedItems);
                }
            }

            return View("LaunchPadItem", items);
        }

        /// <summary>
        /// Loads an item with updated tree data.  If student calls this, top level subcontainer gets 
        /// enrollments completion data added/updated to it.  
        /// </summary>
        /// <param name="itemId">Item to load</param>
        /// <param name="level">Level of the item in the tree</param>
        /// <param name="widgetId">Not used</param>
        /// <returns>A loaded content item, and its subcontainer parent, if a student makes this call :(</returns>
        private List<TreeWidgetViewItem> LoadItemWithUpdatedParent(string itemId, string level, string widgetId = "",
            string toc = "syllabusfilter")
        {
            List<TreeWidgetViewItem> retval = new List<TreeWidgetViewItem>();
            TreeWidgetSettings settings = new TreeWidgetSettings();

            //We really should make the widgetid mandatory
            if (!widgetId.IsNullOrEmpty())
            {
                var widget = PageActions.GetWidget(widgetId).ToWidgetItem();
                SetTreeWidgetSettings(widget, settings);
            }

            var item = ContentActions.GetContent(Context.AccessLevel == AccessLevel.Student ? Context.EnrollmentId : Context.EntityId, itemId);

            if (item == null)
            {
                return null;
            }

            var contentItem = item.ToContentItem(ContentActions);
            settings.UserAccess = Context.AccessLevel;
            //TODO: Remove this once we have replaced instances of contentitem.useraccess with treewidgetviewitem.useraccess
            contentItem.UserAccess = Context.AccessLevel;

            //We use to call GetSyllabusFilterFromCategory to set parent id, which would just use syllabus filter as 
            //the default toc. CHANGE!
            retval.Add(new TreeWidgetViewItem(contentItem, settings, level.IsNullOrEmpty() ? 1 : int.Parse(level)));

            // getting grade for student's item
            if (Context.AccessLevel == BizSC.AccessLevel.Student)
            {
                var grade = GradeActions.GetGradesByEnrollment(Context.EnrollmentId,
                    new List<string>() { contentItem.Id }).FirstOrDefault();
                Helper.SetGrade(contentItem, grade);

                //Only want to add subcontainer to update if item isn't a top level item
                var subcontainer = GetSubcontainerWithUpdatedGrades(contentItem.GetSubContainer(toc));
                if (subcontainer != null)
                {
                    retval.Add(new TreeWidgetViewItem(subcontainer, settings, 1));
                }
            }

            return retval;
        }

        /// <summary>
        /// Returns a subcontainer contentitem (pxunit) with updated student completion data for a particular enrollment
        /// </summary>
        /// <param name="subcontainerId"></param>
        /// <returns></returns>
        private ContentItem GetSubcontainerWithUpdatedGrades(string subcontainerId, string toc = "syllabusfilter")
        {
            ContentItem parentItem = null;
            //If top level PXUnit
            if (!subcontainerId.Equals(DefaultSubContainer))
            {
                parentItem = ContentActions.GetContent(Context.EntityId, subcontainerId).ToContentItem(ContentActions,
                    false, null, false);
                parentItem.level = 1;
                parentItem.UserAccess = Context.AccessLevel;

                var itemsDue = Helper.GetAssignedContent(ContentActions, Context.EntityId, DefaultContainer.ToLowerInvariant(), parentItem.Id, toc);
                var grades = GradeActions.GetGradesByEnrollment(Context.EnrollmentId,
                    itemsDue.Select(i => i.Id).ToList());

                Helper.CalculateSubcontainerCompletionData(grades, itemsDue, parentItem);
            }

            return parentItem;
        }

        /// <summary>
        /// Get Assignment data
        /// </summary>
        /// <param name="contentItemId">The content item unique identifier.</param>
        /// <param name="widgetId">The widget unique identifier.</param>
        /// <returns></returns>
        public ActionResult ManagementCard(string contentItemId, string widgetId)
        {
            var settings = new TreeWidgetSettings();

            var widget = PageActions.GetWidget(widgetId).ToWidgetItem();

            SetTreeWidgetSettings(widget, settings);
            
            var dcContentItem = ContentActions.GetContent(Context.EntityId, contentItemId);

            var assignmentItem = Assign(dcContentItem);
            assignmentItem.Score.Possible = dcContentItem.MaxPoints > 0 ? dcContentItem.MaxPoints : assignmentItem.Score.Possible;
            assignmentItem.IsRemovable = dcContentItem.IsRemovable(settings.RemovableSetting);

            var contentItem = dcContentItem.ToContentItem(ContentActions, false, null, false);
            contentItem.MaxPoints = dcContentItem.MaxPoints;

            assignmentItem.FriendlyNameSourceType = contentItem.GetFriendlyItemContentType();

            ViewData["isRange"] = this.ContentActions.ListContentWithDueDates(Context.CourseId, contentItem.Id).Any(o => !o.AssignmentSettings.DueDate.Equals(contentItem.DueDate));
            ViewData["HiddenFromStudents"] = contentItem.HideFromStudents();
            ViewData["HasSubmissions"] = (contentItem.DueDate.Year > DateTime.MinValue.Year);
            ViewData["IsSandboxCourse"] = Context.Course.IsSandboxCourse;
            ViewData["ParentId"] = contentItem.ParentId; 
            ViewData["Removable_Tocs"] = settings.RemovableSetting.RemoveFromTocs;

            #region assignment units
            
            ViewData["ShowAssignmentUnitWorkflow"] = settings.ShowAssignmentUnitWorkflow;
            ViewData["AssignmentUnitToc"] = settings.AssignmentTOC; 

            //Only need to pull in assignment units if the application works off of assignment units (XBook)
            if (settings.ShowAssignmentUnitWorkflow)
            {

                ViewData["AssignmentUnitTemplateId"] = settings.UnitTemplateId;
                var selectedUnit = assignmentItem.SubContainerIds.FirstOrDefault(c => c.Toc == settings.AssignmentTOC);
                var selectedUnitId = String.Empty;
                if (selectedUnit != null)
                {
                    selectedUnitId = selectedUnit.Value;
                }
                assignmentItem.AssignmentUnits = AssignmentHelper.GetAssignmentUnits(ContentActions, Context.EntityId,
                                                                                     settings.Container, settings.SubContainer,
                                                                                     settings.AssignmentTOC, selectedUnitId);
            }

            #endregion

            ViewData.Model = assignmentItem;

            return View();
        }

        /// <summary>
        /// Show the Assign Screen
        /// </summary>
        /// <param name="cont"></param>
        /// <returns></returns>
        public AssignedItem Assign(BizDC.ContentItem cont, string toc = "syllabusfilter")
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
                assignment.Id = cont.Id;
                assignment.Title = cont.Title;
                assignment.SubTitle = cont.SubTitle;
                assignment.Type = cont.Type;
                assignment.SourceType = string.IsNullOrEmpty(cont.Subtype) ? cont.Type : cont.Subtype;
                assignment.IsSendReminder = false;

                if (null != cont.AssignmentSettings && (cont.AssignmentSettings.DueDate.Year != DateTime.MinValue.Year || cont.AssignmentSettings.Points > 0))
                {
                    assignment.DueDate = cont.AssignmentSettings.DueDate;
                    assignment.StartDate = cont.AssignmentSettings.StartDate;
                    assignment.Score = new Score() { Possible = cont.AssignmentSettings.Points };
                    assignment.Category = cont.AssignmentSettings.Category;
                    assignment.CompletionTrigger = (Models.CompletionTrigger)cont.AssignmentSettings.CompletionTrigger;
                    assignment.IsGradeable = cont.AssignmentSettings.IsAssignable;
                    assignment.IsAllowLateSubmission = cont.AssignmentSettings.AllowLateSubmission;
                    assignment.IsHighlightLateSubmission = cont.AssignmentSettings.IsHighlightLateSubmission;
                    assignment.IsAllowLateGracePeriod = cont.AssignmentSettings.IsAllowLateGracePeriod;
                    assignment.IsAllowExtraCredit = cont.AssignmentSettings.IsAllowExtraCredit;
                    assignment.LateGraceDuration = cont.AssignmentSettings.LateGraceDuration;
                    assignment.LateGraceDurationType = cont.AssignmentSettings.LateGraceDurationType;
                    assignment.IsSendReminder = cont.AssignmentSettings.IsSendReminder;
                    assignment.GradeRule = (Models.GradeRule)cont.AssignmentSettings.GradeRule;
                    assignment.SubmissionGradeAction = (Models.SubmissionGradeAction)cont.AssignmentSettings.SubmissionGradeAction;

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
                    assignment.IsImportant = cont.Categories.Filter(x => x.Id == category).Any();
                }

                if (assignment.DueDate.ToShortDateString() == DateTime.MinValue.ToShortDateString() && !(cont.Subtype.ToLower().Equals("pxunit")))
                {
                    var parentUnit = ContentActions.GetContent(Context.EntityId, cont.GetSubContainer(toc));
                    if (parentUnit != null)
                    {
                        assignment.Category = parentUnit.UnitGradebookCategory;
                    }
                }
                else if (null != cont.AssignmentSettings && (!cont.AssignmentSettings.Category.IsNullOrEmpty()))
                {
                    assignment.Category = cont.AssignmentSettings.Category;
                }

                assignment.GradeBookWeights = Context.Course.ToGradeBookWeights();

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

                assignment.Containers = cont.Containers.Map(c => new Container(c.Toc, c.Value)).ToList();
                assignment.SubContainerIds = cont.SubContainerIds.Map(c => new Container(c.Toc, c.Value)).ToList();
            }

            assignment.AssignTabSettings = Context.Course.AssignTabSettings.ToAssignTabSettings();
            assignment.CourseType = Context.Course.CourseType;
            return assignment;
        }

        /// <summary>
        /// Rename content
        /// </summary>
        /// <param name="itemId">The item unique identifier.</param>
        /// <param name="description">The description.</param>
        /// <param name="thumbnail">The thumbnail.</param>
        /// <param name="title">The title.</param>
        /// <param name="subtitle">The subtitle.</param>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult EditContentSave(string itemId, string description, string thumbnail, string title, string subtitle, string window)
        {
            var source = ContentActions.GetContent(Context.EntityId, itemId);
            source.Title = title;
            source.SubTitle = subtitle;
            source.Description = description;

            // Fix for localhost development
            source.Thumbnail = thumbnail.Replace("http://dev.worthpublishers.com", string.Empty);

            ContentActions.StoreContent(source);

            return Content(itemId + "|" + title + "|" + description + "|" + thumbnail + "|" + subtitle);
        }

        /// <summary>
        /// Edit content
        /// </summary>
        /// <param name="contentId">The content unique identifier.</param>
        /// <param name="newParentId">The new parent unique identifier.</param>
        /// <param name="level">The level.</param>
        /// <param name="source">The source.</param>
        /// <param name="mode">The rename/copy mode</param>
        /// <returns></returns>
        public ActionResult EditContentView(string contentId, string newParentId, string level, string source, string mode)
        {
            if (contentId.IsNullOrEmpty())
            {
                throw new Exception("The contentId value cannot be null or empty!");
            }

            if (mode.IsNullOrEmpty())
            {
                throw new Exception("The mode value cannot be null or empty!");
            }

            if (mode != "rename" && mode != "copy")
            {
                throw new Exception("The mode value should be rename or copy!");
            }

            var contentItem = ContentActions.GetContent(Context.EntityId, contentId);
            ViewData.Model = contentItem.ToContentItem(ContentActions);

            ViewData["courseType"] = Context.Course.CourseType;
            ViewData["courseId"] = Context.CourseId;
            ViewData["newParentId"] = newParentId;
            ViewData["level"] = level;
            ViewData["mode"] = mode;
            ViewData["sourceWindow"] = source;

            return View();
        }

        /// <summary>
        /// Load up children of the specified node
        /// </summary>
        /// <param name="itemId">The item unique identifier.</param>
        /// <param name="itemLevel">The item level.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="isLoadFullTree">if set to <c>true</c> [is load full tree].</param>
        /// <param name="widgetId">The widget unique identifier.</param>
        /// <returns></returns>
        public ActionResult LoadChildrenForParentItem(string itemId, string itemLevel = "1", string operation = "load",
            bool isLoadFullTree = false, string widgetId = "")
        {
            TreeWidgetSettings settings = new TreeWidgetSettings();
            ContentItem parent = null;
            Bfw.PX.PXPub.Models.Widget widget = null;

            settings.IsSandboxCourse = Context.Course.IsSandboxCourse;

            //WidgetId is passed in so that the toc and containerid can be pulled in.  Why not just pass those values 
            //into the url as params?
            widget = widgetId.IsNullOrEmpty() ? this.PageActions.GetWidget(System.Configuration.ConfigurationManager.AppSettings["DefaultLaunchPadWidget"]).ToWidgetItem()
                                              : this.PageActions.GetWidget(widgetId).ToWidgetItem();

            settings.UseProductCourse = widget.UseProductCourse;

            //Needs to be before the check for unit browser and assessment browser for now because their settings arn't
            //defined in DLAP
            SetTreeWidgetSettings(widget, settings);

            parent = ContentActions.GetContent(settings.UseProductCourse ? Context.ProductCourseId : Context.EntityId, itemId).ToContentItem(ContentActions, false, null, false);
            var children = Helper.GetContainersItems(ContentActions, Context, settings, settings.Container, parent.Id, Convert.ToInt32(itemLevel));

            // checking for settings for quiz browser widget and unit browser widget
            if (widget.Properties.ContainsKey("bfw_launchpadunitbrowser") ||
                widget.Properties.ContainsKey("bfw_launchpadassessmentbrowser"))
            {
                if (widget.Properties.ContainsKey("bfw_launchpadunitbrowser"))
                {
                    children = children.Filter(i => i.Item.Type.ToLower() == "pxunit").ToList();
                }
                else if (widget.Properties.ContainsKey("bfw_launchpadassessmentbrowser"))
                {
                    children = children.Filter(i => i.Item.Type.ToLower() == "pxunit" ||
                        i.Item.Type.ToLower() == "quiz" || i.Item.Type.ToLower() == "assessment").ToList();
                    settings.QuizBrowser = true;
                }

                settings.AllowEditing = false;
                settings.ShowDescription = false;
                settings.OpenContentOnClick = true;

                return View("DisplayItem", children);
            }

            if (itemId != null)
            {
                SetCompletionStatus(children);

                if (itemLevel == "1" && settings.ShowBrowseMoreResources)
                {
                    var browseLink = new TreeWidgetViewItem(new ContentItem()
                    {
                        Id = itemId + "_ChapterResourcesLinksFixed",
                        Title = (parent as PxUnit) != null ? (parent as PxUnit).UnitChapter : string.Empty,
                        Type = "ChapterResourcesLinksFixed",
                        ParentId = itemId,
                        ReadOnly = true,
                        UserAccess = Context.AccessLevel,
                    }, settings, 1);

                    children.Add(browseLink);
                    browseLink.Item.SetSyllabusFilterCategory(itemId, settings.TOC, sequence:null);
                }
            }

            if ((widget.Title != null && widget.Title.ToLower().Equals("ebook")) || widgetId == "PX_MENU_ITEM_EBOOK")
            {
                children = children.Filter(i => i.Item is PxUnit || (i.Item.Categories.ToList().Any(c => c.Id == "bfw_faceplate_filter" && c.Text == "ebook"))).ToList();
            }

            return View("DisplayItem", children);
        }

        /// <summary>
        /// Sets the tree widget settings.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="showPastDueToggle">if set to <c>true</c> [show past due toggle].</param>
        /// <param name="showDueLaterToggle">if set to <c>true</c> [show due later toggle].</param>
        public void SetTreeWidgetSettings(Bfw.PX.PXPub.Models.Widget widget, TreeWidgetSettings settings,
            bool showPastDueToggle = false, bool showDueLaterToggle = false)
        {
            bool showAssignmentUnitFlow;
            if (widget.TryGetProperty("bfw_assignment_unit_flow", out showAssignmentUnitFlow, Convert.ToBoolean))
                settings.ShowAssignmentUnitWorkflow = showAssignmentUnitFlow;

            bool collapsedUnassigned;
            if (widget.TryGetProperty("bfw_collapseunassigned", out collapsedUnassigned, Convert.ToBoolean))
                settings.CollapseUnassigned = collapsedUnassigned;

            bool disableDragDrog;
            if (widget.TryGetProperty("bfw_disabledraganddrop", out disableDragDrog, Convert.ToBoolean))
                settings.AllowDragDrop = !disableDragDrog;

            bool disableEditing;
            if (widget.TryGetProperty("bfw_disableediting", out disableEditing, Convert.ToBoolean))
                settings.AllowEditing = !disableEditing;

            bool greyOutPastDue;
            if (widget.TryGetProperty("bfw_grayoutpastduelater", out greyOutPastDue, Convert.ToBoolean))
                settings.GreyoutPastDue = greyOutPastDue;

            bool hideStudentDateData;
            if (widget.TryGetProperty("bfw_hidedatestudentdata", out hideStudentDateData, Convert.ToBoolean))
                settings.ShowStudentDateData = !hideStudentDateData;

            string launchpadTitle;
            if (widget.TryGetProperty("bfw_launchpadtitle", out launchpadTitle))
                settings.Title = launchpadTitle;

            bool openContentOnClick;
            if (widget.TryGetProperty("bfw_opencontentonclick", out openContentOnClick, Convert.ToBoolean))
                settings.OpenContentOnClick = openContentOnClick;

            bool removeOnUnassign;
            if (widget.TryGetProperty("bfw_removeOnUnassign", out removeOnUnassign, Convert.ToBoolean))
                settings.RemoveOnUnassign = removeOnUnassign;

            bool browseMoreResource;
            if (widget.TryGetProperty("bfw_showbrowsemoreresource", out browseMoreResource, Convert.ToBoolean))
                settings.ShowBrowseMoreResources = browseMoreResource;

            bool showCollapsedUnassigned;
            if (widget.TryGetProperty("bfw_showcollapseunassigned", out showCollapsedUnassigned, Convert.ToBoolean))
                settings.ShowCollapsedUnassigned = showCollapsedUnassigned;

            bool showdescription;
            if (widget.TryGetProperty("bfw_showdescription", out showdescription, Convert.ToBoolean))
                settings.ShowDescription = showdescription;

            bool showexpandiconatalllevels;
            if (widget.TryGetProperty("bfw_showexpandiconatalllevels", out showexpandiconatalllevels, Convert.ToBoolean))
                settings.ShowExpandIconAtAllLevels = showexpandiconatalllevels;

            string showFilter;
            if (widget.TryGetProperty("bfw_showitemsonly", out showFilter))
                settings.ShowOnlyFilter = showFilter;

            bool sortByDueDate;
            if (widget.TryGetProperty("bfw_sortbyduedate", out sortByDueDate, Convert.ToBoolean))
                settings.SortByDueDate = sortByDueDate;

            bool splitAssigned;
            if (widget.TryGetProperty("bfw_splitassigned", out splitAssigned, Convert.ToBoolean))
                settings.SplitAssigned = splitAssigned;

            string toc;
            if (widget.TryGetProperty("bfw_toc", out toc))
                settings.TOC = toc;
            else
                settings.TOC = DefaultToc;

            string subcontainerId, containerId;
            if (widget.TryGetProperty("bfw_subcontainer_id", out subcontainerId))
                settings.SubContainer = subcontainerId;
            else
                settings.SubContainer = DefaultSubContainer;

            if (widget.TryGetProperty("bfw_container_id", out containerId))
                settings.Container = containerId;
            else
                settings.Container = DefaultContainer;

            string assignmentToc;
            if (widget.TryGetProperty("bfw_assignmenttoc", out assignmentToc))
                settings.AssignmentTOC = assignmentToc;

            bool dueLaterEnabled;
            if (showDueLaterToggle &&
                widget.TryGetProperty("bfw_toggleduelater", out dueLaterEnabled, Convert.ToBoolean))
            {
                settings.AllowDueLater = dueLaterEnabled;
            }

            int dueLaterDays;
            settings.DueLaterDays = 14;
            if (widget.TryGetProperty("bfw_toggleduelaterdays", out dueLaterDays, Convert.ToInt32))
                settings.DueLaterDays = dueLaterDays;

            bool togglePastDue;
            if (widget.TryGetProperty("bfw_togglepastdue", out togglePastDue, Convert.ToBoolean) && showPastDueToggle)
                settings.AllowPastDue = togglePastDue;

            bool fneonlylearningcurve;
            if (widget.TryGetProperty("bfw_fneonlylearningcurve", out fneonlylearningcurve, Convert.ToBoolean))
                settings.FneOnlyLearningCurve = fneonlylearningcurve;

            bool scrollOnOpen;
            if (widget.TryGetProperty("bfw_scrollonopen", out scrollOnOpen, Convert.ToBoolean))
                settings.ScrollOnOpen = scrollOnOpen;

            bool closeAllOnOpen;
            if (widget.TryGetProperty("bfw_closeallonopen", out closeAllOnOpen, Convert.ToBoolean))
                settings.CloseAllOnOpen = closeAllOnOpen;

            settings.WidgetId = widget.Id;
            settings.EntityId = Context.EntityId;
            settings.UserAccess = Context.AccessLevel;
            settings.CourseId = Context.Course.Id;
            settings.IsSandboxCourse = Context.Course.IsSandboxCourse;
            settings.ProductType = Context.ProductType.ToLower() + "_contenttreewidget";
            settings.UnitTemplateId = Helper.GetUnitTemplateId(ContentActions);

            settings.RemovableSetting = GetRemovableSetting(widget);
        }

        /// <summary>
        /// Sets the completion status.
        /// </summary>
        /// <param name="items">The items.</param>
        private void SetCompletionStatus(List<TreeWidgetViewItem> items, IEnumerable<BizDC.Grade> grades = null)
        {
            if (Context.AccessLevel == AccessLevel.Student)
            {
                if (grades == null)
                {
                    grades = GradeActions.GetGradesByEnrollment(Context.EnrollmentId,
                                                                    items.Select(i => i.Item.Id).ToList());
                }

                foreach (var item in items)
                {
                    var grade = grades.FirstOrDefault(g => item.Item.Id == g.GradedItem.Id);
                    if (grade != null)
                    {
                        Helper.SetGrade(item.Item, grade);
                    }
                }
            }
        }

        /// <summary>
        /// Get a list of item from grades
        /// </summary>
        /// <param name="grades"></param>
        /// <param name="filterPxUnit">If true, do not return item with type = "Folder"</param>
        /// <returns></returns>
        public List<BizDC.ContentItem> GetGradableItems(IEnumerable<BizDC.Grade> grades, bool filterPxUnit)
        {
            if (grades.IsNullOrEmpty())
                return new List<BizDC.ContentItem>();

            var gradedItems = grades.Map(g => g.ItemId).ToList();
            var itemsDue =
                ContentActions.GetItems(Context.EnrollmentId, gradedItems);
            return filterPxUnit ? itemsDue.Where(item => item.Subtype == null || item.Subtype.ToLower() != "pxunit").ToList() : itemsDue.ToList();
        }

        #region RemovableSetting

        /// <summary>
        /// Parses the removable types (as shown below)
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <returns></returns>
        private static RemovableSetting GetRemovableSetting(Widget widget)
        {
            var isRemovable = true;

            bool switchOn;
            widget.TryGetProperty("bfw_removable_switch", out switchOn, Convert.ToBoolean);
            if (widget.Properties.ContainsKey("bfw_removable_switch"))
            {
                isRemovable = switchOn;
            }

            String query;
            widget.TryGetProperty("bfw_removable_xpath_query", out query, Convert.ToString);

            String tocs;
            widget.TryGetProperty("bfw_remove_from_toc", out tocs, Convert.ToString);

            return new RemovableSetting()
            {
                Switch = isRemovable,
                XPathQueryFilter = query,
                RemoveFromTocs = tocs
            };
        }

        #endregion

        #region IPXWidget Members

        public ActionResult Summary(Widget widget)
        {
            return Summary(widget);
        }

        /// <summary>
        /// Shows the login status of the currently logged in user. If the user is
        /// anonymous then they are considered to be not authenticated.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget, string toc = "syllabusfilter")
        {
            var unitsList = AssignmentCenterHelper.LoadContainerData("Launchpad", "", "faceplate", toc);
            unitsList.AddRange(AssignmentCenterHelper.LoadContainerData("Launchpad", "PX_MULTIPART_LESSONS", "faceplate", toc));
            var pxUnit = new PxUnit();

            foreach (var item in unitsList)
            {
                pxUnit.AddAssociatedItem(item);
            }

            pxUnit.Id = "PX_MULTIPART_LESSONS";
            pxUnit.UserAccess = Context.AccessLevel;
            ViewData.Model = pxUnit;
            return View();
        }

        /// <summary>
        /// Shows the login status of the currently logged in user. If the user is
        /// anonymous then they are considered to be not authenticated.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <returns></returns>
        public ActionResult Index(Bfw.PX.PXPub.Models.Widget widget)
        {
            TreeWidgetRoot root = new TreeWidgetRoot();
            TreeWidgetSettings settings = new TreeWidgetSettings();
            root.Settings = settings;

            bool showTogglePastDue = false;
            bool showToggleDueLater = false;

            if (string.IsNullOrWhiteSpace(widget.Template))
            {
                widget = this.PageActions.GetWidget(widget.Id).ToWidgetItem();
            }

            //TODO: Find a way to avoid doing this twice.
            SetTreeWidgetSettings(widget, settings, showTogglePastDue, showToggleDueLater);

            settings.UseProductCourse = widget.UseProductCourse;

            //This is essentially getting the subcontainer items in two calls.  One for legacy format (where 
            //subcontainerid = PX_MULTIPART_LESSONS???) and a second for where the subcontainerid is set to string.Empty
            var subcontainers = Helper.GetContainersItems(ContentActions, Context, settings, settings.Container, "").ToList();
            subcontainers.AddRange(Helper.GetContainersItems(ContentActions, Context, settings, settings.Container, settings.SubContainer));

            #region Extract
            if (subcontainers.Any(i => (i.Item.DueDate <= DateTime.Now.GetCourseDateTime() && (i.Item.DueDate.Year > 1))))
            {
                showTogglePastDue = true;
                settings.PastDueCount = subcontainers.Count(i => (i.Item.DueDate <= DateTime.Now.GetCourseDateTime() && (i.Item.DueDate.Year > 1)));
            }

            int dueLaterDays;
            settings.DueLaterDays = 14;
            if (widget.TryGetProperty("bfw_toggleduelaterdays", out dueLaterDays, Convert.ToInt32))
                settings.DueLaterDays = dueLaterDays;

            if (subcontainers.Any(i => ((i.Item.DueDate.DayOfYear - DateTime.Now.GetCourseDateTime().DayOfYear) > settings.DueLaterDays
                                         && (i.Item.StartDate.Year == DateTime.MinValue.Year || (i.Item.StartDate.DayOfYear - DateTime.Now.GetCourseDateTime().DayOfYear) > settings.DueLaterDays))))
            {
                showToggleDueLater = true;
                settings.DueLaterCount = subcontainers.Count(i => ((i.Item.DueDate.DayOfYear - DateTime.Now.GetCourseDateTime().DayOfYear) > settings.DueLaterDays
                                                                   && (i.Item.StartDate.DayOfYear - DateTime.Now.GetCourseDateTime().DayOfYear) > settings.DueLaterDays));
            }

            settings.ShowWidgetTitles = subcontainers.Any(i => (i.Item.DueDate.Year > 1 || i.Item.IsAssigned));

            SetTreeWidgetSettings(widget, settings, showTogglePastDue, showToggleDueLater);

            // show move or copy
            bool unitBrowser;
            if (widget.TryGetProperty("bfw_launchpadunitbrowser", out unitBrowser) && unitBrowser)
            {
                settings.ShowDescription = false;
                settings.OpenContentOnClick = true;
                subcontainers = subcontainers.Where(i => i.Item.Type.ToLower() == "pxunit").ToList();
            }

            // check to see whether we want to only show assessments
            bool assessmentBrowser;
            if (widget.TryGetProperty("bfw_launchpadassessmentbrowser", out assessmentBrowser) && assessmentBrowser)
            {
                settings.ShowDescription = false;
                settings.OpenContentOnClick = true;
                settings.QuizBrowser = true;
                subcontainers = subcontainers.Where(i => i.Item.Type.ToLower() == "pxunit" || i.Item.Type.ToLower() == "quiz" ||
                    i.Item.Type.ToLower() == "assessment").ToList();
            }


            root.Items.AddRange(subcontainers);

            if (Context.AccessLevel == BizSC.AccessLevel.Student && settings.ShowStudentDateData)
            {
                var grades = GradeActions.GetGradesByEnrollment(Context.EnrollmentId, null);
                var itemsDue = GetGradableItems(grades, true);

                var assignedSubcontainerIds = Helper.GetSubcontainerItemIds(itemsDue, settings.TOC);

                foreach (var sid in assignedSubcontainerIds)
                {
                    var subcontainer = subcontainers.Where(s => s.Item.Id == sid).Select(sc => sc.Item).FirstOrDefault();
                    Helper.CalculateSubcontainerCompletionData(grades, itemsDue.Where(t => t.GetSubContainer(settings.TOC) != null &&
                        t.GetSubContainer(settings.TOC).Equals(subcontainer.Id)), subcontainer);
                }

                foreach (var subcontainer in subcontainers.Where(s => !(s.Item is PxUnit) && s.Item.IsAssigned))
                {
                    SetCompletionStatus(new List<TreeWidgetViewItem>() { subcontainer }, grades);
                }

            }
            #endregion Extract

            //CHANGE!
            if (settings.SortByDueDate)
                root.Items.Sort(TreeWidgetViewItem.DueDateComparer);
            else
                root.Items = root.Items.OrderBy(i => i.Item.Sequence).ToList();
            ViewData.Model = root;

            return View();
        }

        /// <summary>
        /// Shows all data related to the currently logged in user
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget widget)
        {
            return View();
        }
        #endregion IPXWidget Members
    }
}