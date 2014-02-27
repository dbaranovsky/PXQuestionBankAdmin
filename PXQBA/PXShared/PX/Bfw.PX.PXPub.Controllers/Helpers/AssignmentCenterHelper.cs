using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public class AssignmentCenterHelper : IAssignmentCenterHelper
    {
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
        /// Access to a content helper object.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected IContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the assignment actions.
        /// </summary>
        /// <value>
        /// The assignment actions.
        /// </value>
        protected BizSC.IAssignmentActions AssignmentActions { get; set; }

        /// <summary>
        /// Gets or sets the course actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }


        /// <summary>
        /// Gets or sets the grade actions.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the rubric actions.
        /// </summary>
        /// <value>
        /// The rubric actions.
        /// </value>
        protected BizSC.IRubricActions RubricActions { get; set; }

        /// <summary>
        /// A private constant for the Intructor workspace.
        /// </summary>
        private const string PX_ASSIGNMENT_CENTER_SYLLABUS_INSTRUCTOR_WORKSPACE = "PX_ASSIGNMENT_CENTER_SYLLABUS_INSTRUCTOR_WORKSPACE";

        /// <summary>
        /// A private constant for the NO Category Id.
        /// </summary>
        private const string PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY = "PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY";

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentCenterHelper"/> class.
        /// </summary>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="context">The context.</param>
        /// <param name="contentHelper">The content helper.</param>
        /// <param name="assignmentActions">The assignment actions.</param>
        public AssignmentCenterHelper(BizSC.IContentActions contentActions, BizSC.IBusinessContext context, IContentHelper contentHelper, BizSC.IAssignmentActions assignmentActions, BizSC.ICourseActions courseActions, BizSC.IGradeActions gradeActions, BizSC.IEnrollmentActions enrollmentActions, BizSC.IRubricActions rubricActions)
        {
            ContentActions = contentActions;
            Context = context;
            ContentHelper = contentHelper;
            AssignmentActions = assignmentActions;
            CourseActions = courseActions;
            GradeActions = gradeActions;
            EnrollmentActions = enrollmentActions;
            RubricActions = rubricActions;
        }

        /// <summary>
        /// Finds a Filter by Id.
        /// </summary>
        /// <param name="filterId">The filter id.</param>
        /// <param name="generateDefaultIfEmpty">if set to <c>true</c> [generate default if empty].</param>
        /// <param name="isLoadData">if set to <c>true</c> [is load data].</param>
        /// <returns></returns>
        public AssignmentCenterFilterSection FindFilter(string filterId, bool generateDefaultIfEmpty, bool isLoadData)
        {
            if (!string.IsNullOrEmpty(filterId))
            {
                var root = ContentActions.GetContent(Context.EntityId, filterId);
                if (root != null)
                {
                    if (root.Subtype.ToLowerInvariant() != "assignmentcenterfiltersection")
                        return null;

                    return root.ToAssignmentCenterFilterSection(ContentActions, Context, isLoadData, null, true);
                }
                else
                {
                    if (generateDefaultIfEmpty)
                    {
                        var section = new AssignmentCenterFilterSection();
                        section.ChildrenFilterSections = new List<AssignmentCenterFilterSection>();
                        section.ChildrenFilterSections.Add(new AssignmentCenterFilterSection() { Id = PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY, Title = "Uncategorized" });
                        return section;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Generates the root.
        /// </summary>
        /// <returns></returns>
        public AssignmentCenterFilterSection GenerateRoot()
        {
            var root = new AssignmentCenterFilterSection
            {
                ParentId = "PX_SETTINGS",
                Id = "PX_ASSIGNMENT_CENTER_SYLLABUS",
                Title = "Syllabus - Weeks",
                SyllabusType = "weeks"
            };

            var nc = new AssignmentCenterFilterSection
            {
                ParentId = root.Id,
                Id = PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY,
                Title = "Uncategorized"
            };

            ContentHelper.SetVisibility(nc.Visibility, true, "student|instructor");
            root.ChildrenFilterSections.Add(nc);

            ContentHelper.StoreAssignmentCenterFilterSection(root, Context.EntityId);
            return root;
        }


        /// <summary>
        /// Gets the syllabus.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="ignoreIfDeleted">if set to <c>true</c> [ignore if deleted].</param>
        /// <returns></returns>
        public AssignmentCenterFilterSection GetSyllabus(List<ContentItem> items, bool ignoreIfDeleted)
        {
            AssignmentCenterFilterSection syllabus = new AssignmentCenterFilterSection();

            using (Context.Tracer.StartTrace("AssignmentCenterHelper.GetSyllabus"))
            {
                var root = ContentActions.GetContent(Context.EntityId, "PX_ASSIGNMENT_CENTER_SYLLABUS");
                if (root != null)
                {
                    syllabus = root.ToAssignmentCenterFilterSection(ContentActions, Context, true, items, true);
                    if (ignoreIfDeleted && syllabus.ParentId == "PX_DELETED")
                    {
                        return new AssignmentCenterFilterSection();
                    }

                    // Move uncategorized filter to top of the list.
                    syllabus.ChildrenFilterSections =
                        syllabus.ChildrenFilterSections.OrderBy(f => f.Id != PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY).ToList();

                    if (Context.AccessLevel == BizSC.AccessLevel.Student)
                    {
                        syllabus.ChildrenFilterSections.RemoveAll(f => f.Id == PX_ASSIGNMENT_CENTER_SYLLABUS_INSTRUCTOR_WORKSPACE);
                    }
                }
                else
                {
                    try
                    {
                        if (!items.IsNullOrEmpty())
                        {
                            if (items.Any(i => i.Categories.Any(c => c.Id.ToUpper().Contains(PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY))))
                            {
                                if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
                                {
                                    syllabus = GenerateRoot();
                                }
                            }
                        }
                    }
                    catch { }
                }

                foreach (var filter in syllabus.ChildrenFilterSections)
                {
                    SetItemsByFilter(filter, items);
                }
            }

            return syllabus;
        }

        public AssignmentCenterFilterSection GetSyllabusForAssignmentPad(Course course)
        {
            AssignmentCenterFilterSection syllabus = new AssignmentCenterFilterSection
            {
                Id = "PX_ASSIGNMENT_CENTER_SYLLABUS"
            };

            syllabus.ChildrenFilterSections = new List<AssignmentCenterFilterSection>();

            var completed = new AssignmentCenterFilterSection
            {
                Id = "PX_ASSIGNMENT_CENTER_SYLLABUS_AP_COMPLETED_ITEMS",
                Title = "Completed"
            };

            var thisWeek = new AssignmentCenterFilterSection
            {
                Id = "PX_ASSIGNMENT_CENTER_SYLLABUS_AP_THIS_WEEK",
                Title = "Due This Week"
            };

            var twoWeeks = new AssignmentCenterFilterSection
            {
                Id = "PX_ASSIGNMENT_CENTER_SYLLABUS_AP_TWO_WEEKS",
                Title = "Due in Two Weeks"
            };

            var nextMonth = new AssignmentCenterFilterSection
            {
                Id = "PX_ASSIGNMENT_CENTER_SYLLABUS_AP_NEXT_MONTH",
                Title = "Due in the Next Month"
            };

            var endOfClass = new AssignmentCenterFilterSection
            {
                Id = "PX_ASSIGNMENT_CENTER_SYLLABUS_AP_END_CLASS",
                Title = "Due by End of Class"
            };

            var passedDue = new AssignmentCenterFilterSection
            {
                Id = "PX_ASSIGNMENT_CENTER_SYLLABUS_AP_PAST_DUE",
                Title = "Past Due"
            };

            var noDueDate = new AssignmentCenterFilterSection
            {
                Id = "PX_ASSIGNMENT_CENTER_SYLLABUS_AP_NO_DUE_DATE",
                Title = "No Due Date"
            };

            completed.AddChildItems(course.CompletedItems);
            thisWeek.AddChildItems(course.DueThisWeek);
            twoWeeks.AddChildItems(course.DueTwoWeeks);
            nextMonth.AddChildItems(course.DueNextMonth);
            endOfClass.AddChildItems(course.EndOfClass);
            passedDue.AddChildItems(course.PassedDueDate);
            noDueDate.AddChildItems(course.NoDueDate);

            syllabus.ChildrenFilterSections = new List<AssignmentCenterFilterSection>
            {
                completed, thisWeek, twoWeeks, nextMonth, endOfClass, passedDue, noDueDate
            };

            return syllabus;
        }

        /// <summary>
        /// Sets the items by filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="items">The items.</param>
        public void SetItemsByFilter(AssignmentCenterFilterSection filter, List<ContentItem> items)
        {
            if (items.IsNullOrEmpty() && filter.ItemCount() == 0)
            {
                return;
            }

            if (items == null)
            {
                items = new List<ContentItem>();
            }

            foreach (var contentItem in items)
            {
                if (string.IsNullOrEmpty(contentItem.SyllabusFilter) && (contentItem.StartDate.ToShortDateString() != DateTime.MinValue.ToShortDateString() && contentItem.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString()))
                {
                    bool isNoCategory = true;
                    foreach (var tocCategory in contentItem.Categories)
                    {
                        if (tocCategory.Id.ToLowerInvariant().Contains("syllabus"))
                        {
                            isNoCategory = false;
                            break;
                        }
                    }
                    if (isNoCategory)
                        contentItem.SyllabusFilter = PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY;
                }
            }

            var contentItems = items.Where(l => (!(l is PxUnit) && l.SyllabusFilter == filter.Id));//.Where(l => l.DueDate >= start && l.DueDate <= end);
            filter.AddChildItems(contentItems.ToList());
        }

        /// <summary>
        /// Assigns the category date overload.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <returns></returns>
        public AssignmentCenterFilterSection AssignCategoryDate(string categoryId, DateTime startDate, DateTime endDate,
            bool updateChildren, string toc)
        {
            var category = FindFilter(categoryId, false, false);
            return AssignCategoryDate(category, startDate, endDate, updateChildren, toc);
        }

        /// <summary>
        /// Assigns the category date orver load.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <returns></returns>
        public AssignmentCenterFilterSection AssignCategoryDate(AssignmentCenterFilterSection category,
            DateTime startDate, DateTime endDate, bool updateChildren, string toc)
        {
            if (updateChildren)
            {
                foreach (var item in category.GetChildItems())
                {
                    if (item.StartDate.Year > 1900 && item.StartDate < startDate)
                    {
                        startDate = item.StartDate;
                    }
                    if (item.DueDate > endDate)
                    {
                        endDate = item.DueDate;
                    }

                    if (item is PxUnit)
                    {
                        if (item.StartDate.Year <= 1900 || item.DueDate.Year <= 1900)
                        {
                            item.StartDate = startDate;
                            item.DueDate = endDate;
                            AssignLessonDate(item.Id, startDate, endDate, true, "", toc);
                        }
                    }
                    else
                    {
                        if (item.DueDate.Year <= 1900)
                        {
                            item.DueDate = endDate;
                            AssignItem(item.Id, DateTime.MinValue, endDate, "");
                        }
                    }
                }
            }

            category.StartDate = startDate;
            category.DueDate = endDate;
            ContentHelper.StoreAssignmentCenterFilterSection(category, Context.EntityId);
            return category;
        }

        /// <summary>
        /// Assigns the lesson date.
        /// </summary>
        /// <param name="lessonId">The lesson id.</param>
        /// <param name="lessonStartDate">The lesson start date.</param>
        /// <param name="lessonEndDate">The lesson end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        public void AssignLessonDate(string lessonId, DateTime lessonStartDate, DateTime lessonEndDate,
            bool updateChildren, string groupId, string toc)
        {
            string applicableEntityId = string.IsNullOrEmpty(groupId) ? Context.EntityId : groupId;
            var lessonItem = ContentActions.GetContent(applicableEntityId, lessonId);
            AssignLessonDate(lessonItem.ToPxUnit(ContentActions, toc), lessonStartDate, lessonEndDate, updateChildren,
                applicableEntityId, toc);
        }

        /// <summary>
        /// Determines whether [is valid date] [the specified date time].
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>
        ///   <c>true</c> if [is valid date] [the specified date time]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidDate(DateTime dateTime)
        {
            if (dateTime.Year != DateTime.MinValue.Year && dateTime.Year != DateTime.MaxValue.Year)
                return true;

            return false;
        }


        /// <summary>
        /// Assign a lesson date.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="pxunit">The module.</param>
        /// <param name="lessonStartDate">The lesson start date.</param>
        /// <param name="lessonEndDate">The lesson end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        public void AssignLessonDate(AssignmentCenterFilterSection filter, PxUnit module, DateTime lessonStartDate,
            DateTime lessonEndDate, bool updateChildren, string groupId, string toc)
        {
            string applicableEntityId = string.IsNullOrEmpty(groupId) ? Context.EntityId : groupId;

            if (filter != null)
            {
                DateTime catStartDate = filter.StartDate;
                DateTime catEndDate = filter.DueDate;
                bool isUpdate = false;

                if (IsValidDate(lessonStartDate))
                {
                    if (IsValidDate(catStartDate))
                    {
                        if (lessonStartDate < catStartDate)
                        {
                            catStartDate = lessonStartDate;
                            isUpdate = true;
                        }
                    }
                    else
                    {
                        catStartDate = lessonStartDate;
                        isUpdate = true;
                    }
                }

                if (IsValidDate(lessonEndDate))
                {
                    if (IsValidDate(catEndDate))
                    {
                        if (lessonEndDate > catEndDate)
                        {
                            catEndDate = lessonEndDate;
                            isUpdate = true;
                        }
                    }
                    else
                    {
                        catEndDate = lessonEndDate;
                        isUpdate = true;
                    }
                }

                // if anything has changed with category date then update it.
                if (isUpdate)
                {
                    AssignCategoryDate(filter, catStartDate, catEndDate, false, toc);
                }
            }

            var childItems = new List<ContentItem>();
            childItems.AddRange(module.GetAssociatedItems());

            if (updateChildren)
            {
                foreach (var item in childItems)
                {
                    if (item.DueDate < lessonStartDate && item.DueDate.Year > 1900)
                        lessonStartDate = item.DueDate;

                    if (item.DueDate > lessonEndDate && item.DueDate.Year > 1900)
                        lessonEndDate = item.DueDate;
                }

                foreach (var item in childItems.Where(item => item.DueDate < lessonStartDate || item.DueDate > lessonEndDate))
                {
                    AssignItem(item.Id, DateTime.MinValue, lessonEndDate, applicableEntityId);
                }
            }

            var ci = module.ToContentItem(applicableEntityId);
            AssignItem(ci, lessonStartDate, lessonEndDate, applicableEntityId);
        }

        /// <summary>
        /// Handles the assign item date.
        /// </summary>
        /// <param name="ai">The ai.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        public void HandleAssignItemDate(AssignedItem ai, DateTime startDate, DateTime endDate, bool updateChildren,
            string groupId, string toc)
        {
            string applicableEntityId = string.IsNullOrEmpty(groupId) ? Context.EntityId : groupId;
            var contentItem = ai.ToContentItem(applicableEntityId, ContentActions);

            if (contentItem.Subtype != null && contentItem.Subtype.ToLowerInvariant() == "pxunit")
            {
                var module = contentItem.ToPxUnit(ContentActions, toc);
                module.AssignedItem = ai;
                AssignLessonDate(module, ai.StartDate, ai.DueDate, true, applicableEntityId, toc);
            }
            else
            {
                AssignAssignmentCenterItemDate(contentItem, ai.DueDate, applicableEntityId, toc);
            }
        }

        /// <summary>
        /// Assigns the lesson date.
        /// </summary>
        /// <param name="pxunit">The module.</param>
        /// <param name="lessonStartDate">The lesson start date.</param>
        /// <param name="lessonEndDate">The lesson end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        public void AssignLessonDate(PxUnit module, DateTime lessonStartDate, DateTime lessonEndDate, bool updateChildren,
            string groupId, string toc)
        {
            string applicableEntityId = string.IsNullOrEmpty(groupId) ? Context.EntityId : groupId;
            var category = module.Categories.Where(c => c.Id.Contains("PX_ASSIGNMENT_CENTER_SYLLABUS_")).FirstOrDefault();
            AssignmentCenterFilterSection filter = null;

            if (category != null && !string.IsNullOrEmpty(category.Id))
            {
                filter = FindFilter(category.Id, false, false);
            }

            AssignLessonDate(filter, module, lessonStartDate, lessonEndDate, updateChildren, applicableEntityId, toc);
        }

        /// <summary>
        /// Assigns the assignment center item date.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="pxunit">The module.</param>
        /// <param name="item">The item.</param>
        /// <param name="endDate">The end date.</param>
        public void AssignAssignmentCenterItemDate(AssignmentCenterFilterSection filter, PxUnit module,
            BizDC.ContentItem item, DateTime endDate, string groupId, string toc)
        {
            string applicableEntityId = string.IsNullOrEmpty(groupId) ? Context.EntityId : groupId;
            if (module != null)
            {
                DateTime moduleStartDate = (module.StartDate.Year == DateTime.MinValue.Year || module.StartDate.Year == DateTime.MaxValue.Year) ? DateTime.MinValue : Convert.ToDateTime(module.StartDate.ToShortDateString());
                DateTime moduleEndDate = (module.DueDate.Year == DateTime.MinValue.Year || module.DueDate.Year == DateTime.MaxValue.Year) ? DateTime.MaxValue : Convert.ToDateTime(module.DueDate.ToShortDateString());
                DateTime dueDate = Convert.ToDateTime(endDate.ToShortDateString());

                if (dueDate.Year != DateTime.MinValue.Year && dueDate.Year != DateTime.MaxValue.Year)
                {
                    DateTime updateStart = module.StartDate;
                    DateTime updateEnd = module.DueDate;
                    bool isUpdate = false;

                    if (dueDate < moduleStartDate)
                    {
                        updateStart = endDate;
                        isUpdate = true;
                    }

                    if (dueDate > moduleEndDate)
                    {
                        updateEnd = endDate;
                        isUpdate = true;
                    }

                    if (module.StartDate.Year == DateTime.MinValue.Year || module.StartDate.Year == DateTime.MinValue.Year)
                    {
                        updateStart = endDate;
                        isUpdate = true;
                    }

                    if (module.DueDate.Year == DateTime.MinValue.Year || module.DueDate.Year == DateTime.MinValue.Year)
                    {
                        updateEnd = endDate;
                        isUpdate = true;
                    }

                    if (isUpdate)
                    {
                        AssignLessonDate(module, updateStart, updateEnd, false, applicableEntityId, toc);
                    }
                }
            }
            else if (filter != null)
            {
                DateTime filterStartDate = (filter.StartDate.Year == DateTime.MinValue.Year || filter.StartDate.Year == DateTime.MaxValue.Year) ? DateTime.MinValue : Convert.ToDateTime(filter.StartDate.ToShortDateString());
                DateTime filterEndDate = (filter.DueDate.Year == DateTime.MinValue.Year || filter.DueDate.Year == DateTime.MaxValue.Year) ? DateTime.MaxValue : Convert.ToDateTime(filter.DueDate.ToShortDateString());
                DateTime dueDate = Convert.ToDateTime(endDate.ToShortDateString());

                if (dueDate.Year != DateTime.MinValue.Year && dueDate.Year != DateTime.MaxValue.Year)
                {
                    DateTime updateStart = filter.StartDate;
                    DateTime updateEnd = filter.DueDate;
                    bool isUpdate = false;

                    if (dueDate < filterStartDate)
                    {
                        updateStart = endDate;
                        isUpdate = true;
                    }

                    if (dueDate > filterEndDate)
                    {
                        updateEnd = endDate;
                        isUpdate = true;
                    }

                    if (filter.StartDate.Year == DateTime.MinValue.Year || filter.StartDate.Year == DateTime.MinValue.Year)
                    {
                        updateStart = endDate;
                        isUpdate = true;
                    }

                    if (filter.DueDate.Year == DateTime.MinValue.Year || filter.DueDate.Year == DateTime.MinValue.Year)
                    {
                        updateEnd = endDate;
                        isUpdate = true;
                    }

                    if (isUpdate)
                    {
                        AssignCategoryDate(filter, updateStart, updateEnd, false, toc);
                    }
                }
            }

            AssignItem(item, endDate, endDate, applicableEntityId);
        }

        /// <summary>
        /// Assigns the assignment center item date.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="endDate">The end date.</param>
        public void AssignAssignmentCenterItemDate(BizDC.ContentItem item, DateTime endDate, string groupId,
            string toc)
        {
            string applicableEntityId = string.IsNullOrEmpty(groupId) ? Context.EntityId : groupId;
            PxUnit module = null;
            AssignmentCenterFilterSection filter = null;
            var moduleCategory = item.Categories.Where(c => c.Id.Contains("MODULE_") ||
                (c.Id == "syllabusfilter" && c.ItemParentId.Contains("MODULE_"))).FirstOrDefault();

            if (moduleCategory != null)
            {
                module = ContentActions.GetContent(applicableEntityId, moduleCategory.ItemParentId).ToPxUnit(ContentActions, toc);
            }
            else
            {
                var filterId = "";
                var propValue = item.Properties["bfw_syllabusfilter"];
                if (propValue != null && propValue.Value != null)
                {
                    filterId = item.Properties["bfw_syllabusfilter"].Value.ToString();
                }
                else
                {
                    var category = item.Categories.Where(c => c.Id.Contains("PX_ASSIGNMENT_CENTER_SYLLABUS_")).FirstOrDefault();
                    filterId = (category != null) ? category.Id : "";
                }

                GetSyllabus(new List<ContentItem>() { item.ToContentItem(ContentActions, false) }, false);
                filter = FindFilter(filterId, true, false);
            }

            AssignAssignmentCenterItemDate(filter, module, item, endDate, applicableEntityId, toc);
        }


        /// <summary>
        /// Assigns the assignment center item date.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="endDate">The end date.</param>
        public void AssignAssignmentCenterItemDate(string itemId, DateTime endDate, string groupId, string toc)
        {
            var a = ContentActions.GetContent(Context.EntityId, itemId);
            AssignAssignmentCenterItemDate(a, endDate, groupId, toc);
        }

        /// <summary>
        /// Assigns an item.
        /// </summary>
        /// <param name="item">item to be assigned</param>
        public void AssignItem(Assign item, string toc = "syllabusfilter")
        {
            var ai = ContentActions.GetContent(Context.EntityId, item.Id).ToAssignedItem();

            ai.Score = new Score() { Possible = item.Points };
            if (ai.Score.Possible < 0 || ai.Score.Possible > 100)
            {
                throw new Exception("Points should be between 0 and 100");
            }

            ai.DueDate = item.DueDate;

            ai.CompletionTrigger = item.CompletionTrigger;

            ai.Category = item.GradebookCategory;
            ai.SyllabusFilter = item.SyllabusFilter;
            ai.IsGradeable = item.IsGradeable;
            ai.IsAllowLateSubmission = item.IsAllowLateSubmission;
            ai.IsHighlightLateSubmission = item.IsHighlightLateSubmission;
            ai.IsAllowLateGracePeriod = item.IsAllowLateGracePeriod;
            ai.IsAllowExtraCredit = item.IsAllowExtraCredit;
            ai.LateGraceDuration = item.LateGraceDuration;
            ai.LateGraceDurationType = item.LateGraceDurationType;
            ai.IsSendReminder = item.IsSendReminder;

            ai.SubmissionGradeAction = item.SubmissionGradeAction.HasValue ? item.SubmissionGradeAction.Value : SubmissionGradeAction.Default;
            ai.GradeRule = !string.IsNullOrEmpty(item.GradeRule) ? (GradeRule)Enum.Parse(typeof(GradeRule), item.GradeRule) : GradeRule.Last;

            if (item.IsSendReminder)
            {
                ai.ReminderEmail = new ReminderEmail { AssignmentId = item.Id, Body = item.ReminderBody, Subject = item.ReminderSubject, DaysBefore = item.ReminderDurationCount, DurationType = item.ReminderDurationType, AssignmentDate = Context.Course.UtcRelativeAdjust(DateTime.Now) };
            }

            ai.IncludeGbbScoreTrigger = item.IncludeGbbScoreTrigger;


            HandleAssignItemDate(ai, ai.StartDate, ai.DueDate, true, "", toc);
        }

        /// <summary>
        /// Assigns an item.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        public void AssignItem(string itemId, DateTime startDate, DateTime endDate, string groupId)
        {
            string applicableEntityId = string.IsNullOrEmpty(groupId) ? Context.EntityId : groupId;
            var item = ContentActions.GetContent(Context.EntityId, itemId);
            AssignItem(item, startDate, endDate, applicableEntityId);
        }

        /// <summary>
        /// Assigns an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        public void AssignItem(BizDC.ContentItem item, DateTime startDate, DateTime endDate, string groupId)
        {
            var assignedItem = item.ToAssignedItem();


            if (string.IsNullOrEmpty(assignedItem.SyllabusFilter))
            {
                assignedItem.SyllabusFilter = PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY;
            }
            else if (assignedItem.SyllabusFilter == PX_ASSIGNMENT_CENTER_SYLLABUS_INSTRUCTOR_WORKSPACE && (startDate.Year > 1900 && endDate.Year > 1900))
            {
                assignedItem.SyllabusFilter = PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY;
            }

            assignedItem.StartDate = startDate;
            assignedItem.DueDate = endDate;

            var ai = assignedItem.ToAssignedItem();
            ai.GroupId = groupId;
            AssignmentActions.Assign(ai);
        }


        public List<ContentItem> LoadContainerData(string containerId, string subContainerId, string mode, string toc)
        {
            List<BizDC.ContentItem> items;
            List<ContentItem> contentItems;
            using (Context.Tracer.StartTrace("AssignmentCenterController.LoadContainerData"))
            {

                items = CourseActions.LoadContainerData(Context.Course, containerId, subContainerId, toc).ToList();

                contentItems = items.Map(c => c.ToContentItem(this.ContentActions)).ToList();
                //TODO: implement caching

                contentItems.Sort(CourseController.ContentItemSort);
            }
            return contentItems;
        }
    
        /// <summary>
        /// Adds the grade book category to course.
        /// </summary>
        /// <param name="newCategory">The new category.</param>
        /// <returns></returns>
        public string AddGradeBookCategoryToCourse(string newCategory)
        {
            string result = string.Empty;

            try
            {
                var categoryList = Context.Course.GradeBookCategoryList.Map(o => o.ToGradeBookWeightCategory()).ToList();
                GradeBookWeightCategory gbbCategoryItem = categoryList.FirstOrDefault(o => o.Text.Equals(newCategory));

                var newSequence = string.Empty;
                var lastSequence = string.Empty;

                GradeBookWeightCategory lastCategoryItem = categoryList.LastOrDefault();
                if (lastCategoryItem != null)
                {
                    lastSequence = lastCategoryItem.Sequence;
                }

                newSequence = String.IsNullOrEmpty(lastSequence)? "a" 
                                                                : Context.Sequence(lastSequence, string.Empty);

                if (gbbCategoryItem == null)
                {
                    int maxCategory = 0;
                    if (categoryList.Count > 0)
                    {
                        maxCategory = categoryList.Max(c =>
                        {
                            int max = 0;
                            int.TryParse(c.Id, out max);
                            return max + 1;
                        });
                    }

                    gbbCategoryItem = new GradeBookWeightCategory()
                    {
                        Id = Convert.ToString(maxCategory),
                        Text = newCategory,
                        Weight = "0",
                        Sequence = newSequence
                    };

                    categoryList.Add(gbbCategoryItem);
                }

                Context.Course.GradeBookCategoryList = new List<Biz.DataContracts.GradeBookWeightCategory>();

                foreach (var gradeBookWeightCategory in categoryList)
                {
                    Context.Course.GradeBookCategoryList.Add(new BizDC.GradeBookWeightCategory()
                    {
                        Id = gradeBookWeightCategory.Id,
                        Text = gradeBookWeightCategory.Text,
                        Weight = (gradeBookWeightCategory.Weight == null) ? "0" : gradeBookWeightCategory.Weight,
                        DropLowest = (gradeBookWeightCategory.DropLowest == null) ? "0" : gradeBookWeightCategory.DropLowest,
                        Sequence = (gradeBookWeightCategory.Sequence == null) ? "0" : gradeBookWeightCategory.Sequence
                    });
                }

                CourseActions.UpdateCourse(Context.Course);

                result = gbbCategoryItem.Id;
            }
            catch { }

            return result;
        }

        /// <summary>
        /// Adds the grade book category automatic unit.
        /// </summary>
        /// <param name="unitId">The unit unique identifier.</param>
        /// <param name="categoryId">The category unique identifier.</param>
        /// <returns></returns>
        public bool AddGradeBookCategoryToUnit(string unitId, string categoryId)
        {
            if (!String.IsNullOrEmpty(unitId))
            {
                return ContentActions.SetGradebookCategoryFor(unitId, categoryId, Context.EntityId);
            }

            return false;
        }

        /// <summary>
        /// Set the dates for  each item
        /// </summary>
        /// <param name="course">The course.</param>
        /// <param name="pxunit">The module.</param>
        /// <param name="selectedContentId">The selected content id.</param>
        /// <param name="selectedLessonId">The selected lesson id.</param>
        void SetAssignedDateForLessonItems(Course course, PxUnit module, string selectedContentId, string selectedLessonId)
        {
            using (Context.Tracer.StartTrace("SetAssignedDateForLessonItems"))
            {
                foreach (var subItem in module.GetAssociatedItems())
                {
                    subItem.UserAccess = module.UserAccess;

                    if (selectedContentId == subItem.Id)
                    {
                        course.SelectedContentId = subItem.Id;
                        course.SelectedLessonId = module.Id;
                        selectedContentId = module.Id;
                    }

                    if (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
                    {
                        var submission = GradeActions.GetStudentSubmissionInfo(Context.EntityId, subItem.Id, Context.EnrollmentId, EnrollmentActions).FirstOrDefault();

                        if (submission != null && submission.SubmittedDate.Year != DateTime.MinValue.Year)
                        {
                            subItem.SubmittedDate = submission.SubmittedDate;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// We always want to select and item to be displayed.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <param name="selectedLessonId">The selected lesson id.</param>
        /// <param name="selectedContentId">The selected content id.</param>
        void SetDefaultSelectedContentId(Course course, string selectedLessonId, string selectedContentId)
        {
            if (course.Units.Count > 0 && string.IsNullOrEmpty(selectedLessonId) && string.IsNullOrEmpty(selectedContentId))
            {
                //grab the module from no due date
                var lessonToShow = new PxUnit();
                if (course.DueThisWeek.Count > 0)
                {
                    if (course.DueThisWeek.Exists(i => i.Type == "pxunit"))
                        lessonToShow = (PxUnit)course.DueThisWeek.FindLast(i => i.Type == "pxunit");
                }

                //grab any
                if (lessonToShow == null)
                {
                    lessonToShow = course.Units.First();
                }

                course.SelectedLessonId = lessonToShow.Id;

                if (lessonToShow.GetAssociatedItems().Count > 0)
                {
                    course.SelectedContentId = lessonToShow.GetAssociatedItems().First().Id;
                }
                else
                {
                    if (lessonToShow.GetAssociatedItems().Count > 0)
                    {
                        course.SelectedContentId = lessonToShow.GetAssociatedItems().First().Id;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the assigned date.
        /// </summary>
        /// <param name="contentItem">The content item.</param>
        /// <param name="parentId">The parent id.</param>
        void SetAssignedDate(ContentItem contentItem, string parentId)
        {
            BizDC.ContentItem assignedItem = null;
            BizDC.AssignmentSettings assignedDueDate = null;

            using (Context.Tracer.StartTrace("SetAssignedDate"))
            {
                assignedItem = ContentActions.GetContent(parentId, contentItem.Id);
                assignedDueDate = assignedItem.AssignmentSettings;

                if (assignedDueDate != null)
                {
                    contentItem.DueDate = assignedDueDate.DueDate;
                }

                if (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
                {
                    var submission = GradeActions.GetStudentSubmissionInfo(Context.EntityId, contentItem.Id, Context.EnrollmentId, EnrollmentActions).FirstOrDefault();
                    if (submission != null && submission.SubmittedDate.Year != DateTime.MinValue.Year)
                    {
                        contentItem.SubmittedDate = submission.SubmittedDate;
                    }
                }
            }
        }

        public List<AssignmentCenterItem> ItemOperation(string itemId, string targetId, AssignedItem assignedItem,
                                                        AssignmentCenterOperation operation, bool keepInGradebook, string toc = "syllabusfilter", string entityId = "")
        {
            List<AssignmentCenterItem> changes = new List<AssignmentCenterItem>();

            switch (operation)
            {
                case AssignmentCenterOperation.NewItem:
                    changes = NewItem(itemId, targetId, toc);
                    break;

                case AssignmentCenterOperation.Move:
                    changes = MoveItem(itemId, targetId, "", toc);
                    break;

                case AssignmentCenterOperation.DatesAssigned:
                    changes = AssignDatesToItem(itemId,
                                                new AssignmentCenterItem()
                                                {
                                                    Id = assignedItem.Id,
                                                    StartDate = assignedItem.StartDate,
                                                    EndDate = assignedItem.DueDate,
                                                    SubmissionGradeAction = assignedItem.SubmissionGradeAction
                                                },
                                                entityId, toc);
                    break;

                case AssignmentCenterOperation.DatesUnAssigned:
                    changes = UnAssignDatesToItem(itemId, new AssignmentCenterItem()
                    {
                        Id = assignedItem.Id,
                        Points = assignedItem.MaxPoints == null ? 0 : assignedItem.MaxPoints.Value
                    }, toc, keepInGradebook);
                    break;

                default:
                    break;
            }

            return changes;
        }

        /// <summary>
        /// Persists the state of the category.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="toc">TOC to save the navigation state against</param>
        /// <returns>
        /// JSON object representing the new state of the category.
        /// </returns>
        public AssignmentCenterNavigationState SaveNavigationState(AssignmentCenterNavigationState state, string toc = "syllabusfilter", bool keepInGradebook = true)
        {
            switch (state.Operation)
            {
                case AssignmentCenterOperation.Move:
                    MoveItem(state, toc);
                    break;

                case AssignmentCenterOperation.DatesAssigned:
                    AssignDatesToItem(state, toc);
                    break;

                case AssignmentCenterOperation.MoveAndAssign:
                    AddItemToAssignmentUnit(state);
                    break;

                case AssignmentCenterOperation.DatesUnAssigned:
                    UnAssignItem(state, toc, keepInGradebook);
                    break;

                case AssignmentCenterOperation.PointsAssigned:
                    AssignPointsToItem(state, toc);
                    break;

                case AssignmentCenterOperation.NewItem:
                    NewItem(state, toc);
                    break;
                    
                case AssignmentCenterOperation.Removed:
                    RemoveItem(state, toc);
                    break;

                case AssignmentCenterOperation.RemoveOnUnassign:
                    UnassignAndRemoveItems(state);
                    break;

                case AssignmentCenterOperation.Copy:
                    CopyItem(state, toc);
                    break;

                case AssignmentCenterOperation.ShowOrHideFromStudents:
                    ShowOrHideFromStudents(state, toc);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Handles the case when an item is removed.
        /// </summary>
        /// <param name="ci">The content item.</param>
        /// <param name="removeFromCategoryId">The remove from category unique identifier.</param>
        /// <param name="tocContainer">The toc container.</param>
        /// <param name="removeFromTocs">The remove from tocs.</param>
        /// <returns>
        /// Updated state for the tree.
        /// </returns>
        internal BizDC.ContentItem RemoveItem(BizDC.ContentItem ci, string removeFromCategoryId, string tocContainer, string removeFromTocs)
        {
            if (ci == null || String.IsNullOrEmpty(removeFromTocs)) return ci;

            var removalFlag = ConfigurationManager.AppSettings["FaceplateRemoved"];

            var parentContainer = ci.GetContainer(tocContainer);
            //which subcontainer are the children in
            //if parent is at root, children are inside parent's id
            //otherwise children are inside parent's subcontainerid
            var subcontainer = ci.GetSubContainer(tocContainer);
            var parentSubcontainer = subcontainer.IsNullOrEmpty() ||
                                     subcontainer == "PX_MULTIPART_LESSONS"
                                     ? ci.Id
                                     : subcontainer;
            ci.InAssignmentCenter = false;

            if ((!string.IsNullOrWhiteSpace(ci.Type) && ci.Type.ToLower() == "pxunit") ||
                (!string.IsNullOrEmpty(ci.Subtype) && ci.Subtype.ToLower() == "pxunit"))
            {
                var children = ContentActions.GetContainerItemsForParent(Context.EntityId, parentContainer, parentSubcontainer, ci.Id, tocContainer).ToList();

                foreach (var subItem in children)
                {
                    removeFromTocs.Split(',').ToList().ForEach(filter =>
                    {
                        subItem.SetContainer(removalFlag, filter);
                        subItem.SetSubContainer(ci.Id, filter);
                    });
                        
                    subItem.AssignmentSettings.DueDate = DateTime.MinValue;
                    subItem.AssignmentSettings.StartDate = DateTime.MinValue;
                }

                if (!children.IsNullOrEmpty() && children.Any())
                {
                    ContentActions.StoreContents(children);
                }
            }

            ci.ParentId = removalFlag; //parent id must be set to remove this item from the gradebook

            removeFromTocs.Split(',').ToList().ForEach(filter =>
            {
                ci.SetSyllabusFilterCategory(removalFlag, filter);
                ci.SetContainer(removalFlag, filter);
            });

            if (ci.AssignmentSettings != null)
            {
                ci.AssignmentSettings.DueDate = DateTime.MinValue;
                ci.AssignmentSettings.StartDate = DateTime.MinValue;
            }
            ci.Properties["bfw_syllabusfilter"] = new BizDC.PropertyValue() { Type = Bfw.PX.Biz.DataContracts.PropertyType.String, Value = "" };

            ContentActions.StoreContent(ci);

            return ci;
        }

        /// <summary>
        /// Handles the case when an item is removed.
        /// </summary>
        /// <param name="state">State of the tree in which an item has been removed.</param>
        /// <param name="toc">The toc.</param>
        /// <returns>
        /// Updated state for the tree.
        /// </returns>
        private AssignmentCenterNavigationState RemoveItem(AssignmentCenterNavigationState state, string toc)
        {
            var node = state.FindItem(state.Changed.Id);
            if (node == null) return state;

            var ci = ContentActions.GetContent(Context.EntityId, node.Id);

            foreach (var filter in toc.Split(','))
            {
                UnAssignItem(state, filter);

                RemoveItem(ci, state.Changed.ParentId, filter, state.RemoveFrom);
            }

            return state;
        }

        /// <summary>
        /// Move item without navigation state
        /// </summary>
        /// <param name="contentId">The content unique identifier.</param>
        /// <param name="newParentId">The new parent unique identifier.</param>
        /// <param name="previousParentId">The previous parent unique identifier.</param>
        /// <param name="toc">The toc.</param>
        /// <param name="container">The container.</param>
        /// <param name="subContainerId">The sub container unique identifier.</param>
        /// <returns>
        /// Top level parent of the item
        /// </returns>
        public List<AssignmentCenterItem> MoveItem(string contentId, string newParentId, string previousParentId, string toc,
                                                   string container = "", string subContainerId = "")
        {
            var node = ContentActions.GetContent(Context.EntityId, contentId)
                                      .ToContentItem(ContentActions, true, null, false, toc)
                                      .ToAssignmentCenterItem(toc);

            //set up assignment center item
            node.Id = contentId;
            node.PreviousParentId = node.ParentId;
            node.ParentId = newParentId;

            return MoveItem(node, toc, container, subContainerId);
        }

        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="toc">The toc.</param>
        /// <param name="container">The container.</param>
        /// <param name="subContainerId">The sub container unique identifier.</param>
        /// <param name="assignParents">if set to <c>true</c> [assign parents].</param>
        /// <returns></returns>
        public List<AssignmentCenterItem> MoveItem(AssignmentCenterItem node, string toc,
                                                   string container = "", string subContainerId = "",
                                                   bool assignParents = true)
        {
            //get the new parent's children to determine top sequence of the item's siblings
            var newParent = (PxUnit)ContentActions.GetContent(Context.EntityId, node.ParentId)
                                                  .ToContentItem(ContentActions, true, null, false, toc);
            var maxSequence = (newParent).GetAssociatedItems().Max(i => i.Sequence);
            node.Sequence = Context.Sequence(maxSequence, "");

            //save changes
            List<AssignmentCenterItem> changes = new List<AssignmentCenterItem>();
            changes.AddRange(UpdateContainer(node, toc, container, subContainerId));
            changes.Add(node);
            ContentActions.UpdateAssignmentCenterItems(node.ParentId, changes.Map(c => c.ToAssignmentCenterItem()), toc);

            if (assignParents)
            {
                var dcParents =
                    ContentHelper.GetParentHeirachy(node.Id, TreeCategoryType.ManagementCard, toc).Map(
                        i => i.ToContentItem(ContentActions, false)).Map(i => i.ToAssignmentCenterItem(toc));

                changes.AddRange(dcParents);
            }

            return changes;
        }

        private void LoadAllChildren(AssignmentCenterItem parent, List<AssignmentCenterItem> items, string toc)
        {
            var list = new List<string>();
            list.Add(parent.Id);

            List<AssignmentCenterItem> children = ContentActions.GetChildItems(Context.EntityId, list, toc).Map(
                o => o.ToAssignment(ContentActions)).Map(p => p.ToAssignmentCenterItem(toc)).ToList();

            if (!children.IsNullOrEmpty())
            {
                children.ForEach(o => o.ParentId = parent.Id);

                parent.Children = children;
                foreach (var child in children)
                {
                    LoadAllChildren(child, children, toc);
                }
            }
        }

        public List<AssignmentCenterItem> NewItem(string contentId, string parentId, string toc)
        {
            List<AssignmentCenterItem> changes = new List<AssignmentCenterItem>();
            var node = ContentActions.GetContent(Context.EntityId, contentId).ToContentItem(ContentActions, false, null).ToAssignmentCenterItem(toc);

            //set up assignment center item
            node.Id = contentId;

            node.ParentId = parentId;
            
            if (parentId != null && !String.IsNullOrEmpty(parentId))
            {
                var children = ContentActions.ListChildren(Context.EntityId, parentId, 1, toc).ToList();

                if (children.Any())
                {
                    string belowSequence = children.Min(o =>
                    {
                        var cat = o.Categories.FirstOrDefault(c => c.Id == toc);
                        return cat != null ? cat.Sequence : null;
                    });

                    if (belowSequence != null)
                    {
                        node.Sequence = Context.Sequence(string.Empty, belowSequence);                    
                    }
                }

                LoadAllChildren(node, new List<AssignmentCenterItem>(), toc);
            }

            //save changes            
            changes.AddRange(UpdateContainer(node, toc));
            changes.Add(node);
            ContentActions.UpdateAssignmentCenterItems(parentId, changes.Map(c => c.ToAssignmentCenterItem()), toc);

            //var dcParents = ContentHelper.GetParentHeirachy(node.Id, TreeCategoryType.ManagementCard).Map(i => i.ToContentItem(ContentActions, false)).Map(i => i.ToAssignmentCenterItem());

            //changes.AddRange(dcParents);
            return changes;
        }

        /// <summary>
        /// Gets upcoming assignments for instructor view without submissions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BizDC.ContentItem> GetDueAssignmentsForInstructor(string entityId, DateTime startDate, DateTime endDate)
        {
            var fromDate = startDate.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ");
            var toDate = endDate.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ");

            // Getting a list of assignments within the date range
            var items = ContentActions.ListContentWithDueDatesWithinRange(entityId, fromDate, toDate);

            if (!items.IsNullOrEmpty())
            {
                items = items.Where(i => i.Subtype.ToLowerInvariant() != "pxunit");
            }

            return items;
        }

        /// <summary>
        /// Gets upcoming assignments for instructor view without submissions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BizDC.ContentItem> GetDueAssignmentsForInstructor(string entityId, int days)
        {
            var fromDate = DateTime.Now.AddHours(-12);
            var toDate = DateTime.Now.AddDays(days);

            return GetDueAssignmentsForInstructor(entityId, fromDate, toDate);
        }

        /// <summary>
        /// Gets upcoming assignments for student view 
        /// <param name="days"></param>
        /// <param name="showCompleted"></param>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BizDC.ContentItem> GetDueAssignmentsForStudent(int days, bool showCompleted, bool showPastDue)
        {
            IEnumerable<BizDC.Grade> grades = GetDueGradesForStudent(days, showCompleted, showPastDue);
            var items = grades.Select(i => i.GradedItem).ToList();
            var contentItems = ContentActions.GetItems(Context.EnrollmentId, items.Select(i => i.Id).ToList());

            return contentItems.Filter(i =>
            {
                var c = i.ToContentItem(ContentActions);
                return !c.HiddenFromStudents && !c.ApplyRestrictedAccess();
            });
        }

        /// <summary>
        /// Gets assigned items with grades for student view
        /// <param name="days"></param>
        /// <param name="showCompleted"></param>
        /// </summary>
        /// <returns></returns>
        private IEnumerable<BizDC.Grade> GetDueGradesForStudent(int days, bool showCompleted, bool showPastDue)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone);
            var offSet = (int)(tz.BaseUtcOffset.TotalMinutes);

            var adjustment = tz.GetAdjustment(DateTime.Now.Year);
            if (adjustment != null)
            {
                offSet += (int)adjustment.DaylightDelta.TotalMinutes;
            }

            return GradeActions.GetDueSoonItemsWithGrades(null, Context.EnrollmentId, offSet, showCompleted, showPastDue, days);
        }

        #region Assignment Unit

        /// <summary>
        /// Moves the and assign item.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private AssignmentCenterNavigationState AddItemToAssignmentUnit(AssignmentCenterNavigationState state)
        {
            if (state.UnitContainer == null)
                return state;

            var node = state.FindItem(state.Changed.Id) ?? state.Changed;

            if (node != null)
            {
                if (state.Changes == null)
                {
                    state.Changes = new List<AssignmentCenterItem>();
                }

                var contentId = state.Changed.Id;
                var previousParentId = state.Changed.ParentId;
                var toc = state.UnitContainer.Toc;
                var unitId = state.UnitContainer.UnitId;
                
                var changedItems = new List<AssignmentCenterItem>();
                var contentItem = ContentActions.GetContent(Context.EntityId, contentId)
                                                .ToContentItem(ContentActions);
                if (contentItem != null)
                {
                    var configFaceplateRemove = ConfigurationManager.AppSettings["FaceplateRemoved"];
                    
                    /* set containers on assigned item */
                    // Do not update containers of assigned item in the same Assignment Unit
                    if (contentItem.GetContainer(toc) == configFaceplateRemove // check to see if this item was removed 
                        || contentItem.GetSubContainer(toc) != unitId) // or assigned into different Assignment Unit
                    {   
                        node.PreviousParentId = previousParentId;
                        node.ParentId = unitId;

                        changedItems = MoveItem(node, toc, string.Empty, unitId, false);
                    }
                }

                /* set due date on assigned item */
                var changedNode = (changedItems.Count > 0)? changedItems.FirstOrDefault(i => i.Id == contentId): node;
               
                if (changedNode != null)
                {
                    changedNode.StartDate = state.Changed.StartDate;
                    changedNode.EndDate = state.Changed.EndDate;
                    changedNode.SubmissionGradeAction = state.Changed.SubmissionGradeAction;
                    changedNode.Points = (state.Changed.Points > 0) ? state.Changed.Points : state.Changed.DefaultPoints;
                    changedNode.DefaultPoints = state.Changed.DefaultPoints;
                    if (String.Equals(changedNode.Type, typeof(PxUnit).Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        changedNode.UnitGradebookCategory = state.UnitContainer.CategoryId;
                    }
                    else
                    {
                        changedNode.GradebookCategory = state.UnitContainer.CategoryId;
                    }
                    
                    state.Changes.AddRange(ProcessAssignment(contentId, changedNode, false, toc, true, Context.EntityId)); 
                }

                /* set due dates on assigned item's children */
                state.Changes.AddRange(UpdateChildren(changedNode, toc));
            }

            return state;
        }

        /// <summary>
        /// Updates the children.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="toc">The toc.</param>
        /// <param name="toUnassign">if set to <c>true</c> [automatic unassign].</param>
        /// <returns></returns>
        private List<AssignmentCenterItem> UpdateChildren(AssignmentCenterItem parent, string toc, bool toUnassign = false)
        {
            var processedItems = new List<AssignmentCenterItem>();
            AssignDueDatesToChildren(parent, processedItems, parent.UnitGradebookCategory, toUnassign);

            var processedChildren = processedItems.Except(new List<AssignmentCenterItem>(){parent}).ToList();

            if (toUnassign)
            {
                this.ContentActions.UnAssignAssignmentCenterItems(string.Empty, processedChildren.Map(o => o.ToAssignmentCenterItem()), toc, true);
            }
            else
            {
                this.ContentActions.UpdateAssignmentCenterItems(string.Empty, processedChildren.Map(o => o.ToAssignmentCenterItem()), toc, Context.EntityId);
            }

            return processedItems;
        } 

        /// <summary>
        /// Assigns due dates to children.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="processedItems">The processed items.</param>
        /// <param name="gradebookCategory">The gradebook category.</param>
        /// <param name="isUnAssigned">if set to <c>true</c> [is function assigned].</param>
        private void AssignDueDatesToChildren(AssignmentCenterItem parent, List<AssignmentCenterItem> processedItems, 
                                              string gradebookCategory, bool isUnAssigned = false)
        {
            processedItems.Add(parent);

            if (parent.Children == null)
                return;

            foreach (var node in parent.Children)
            {
                //update item
                if (isUnAssigned)
                {
                    node.StartDate = DateTime.MinValue;
                    node.EndDate = DateTime.MinValue;
                }
                else
                {
                    if (node.Type != null && node.Type.ToLower() == "pxunit")
                    {
                        node.StartDate = parent.StartDate;
                    }

                    node.EndDate = parent.EndDate;

                    node.GradebookCategory = gradebookCategory;
                }

                AssignDueDatesToChildren(node, processedItems, gradebookCategory, isUnAssigned);
            }
        }

        #endregion
        
        /// <summary>
        /// Handles the case when an item is moved.
        /// </summary>
        /// <param name="state">State of the tree in which an item has been moved.</param>
        /// <param name="toc">The toc.</param>
        /// <returns>
        /// Updated state for the tree.
        /// </returns>
        private AssignmentCenterNavigationState MoveItem(AssignmentCenterNavigationState state, string toc)
        {
            var node = state.FindItem(state.Changed.Id);

            if (node == null)
            {
                node = state.Changed;
            }

            var changes = MoveItemGetChanges(state, node, toc);

            state.Changes = changes.OrderBy(o => o.Sequence).ToList();

            if (!changes.IsNullOrEmpty())
            {
                ContentActions.UpdateAssignmentCenterItems(state.Category.Id, changes.Map(c => c.ToAssignmentCenterItem()), toc);
                ContentActions.UpdateAssignmentCenterCategory(Context.EntityId, state.Category.ToAssignmentCenterCategory());
            }

            return state;
        }

        private IList<AssignmentCenterItem> MoveItemGetChanges(AssignmentCenterNavigationState state,
                                                               AssignmentCenterItem node, string toc,
                                                               string container = "", string subContainerId = "")
        {
            var min = string.Empty;
            var max = string.Empty;

            List<AssignmentCenterItem> changes = new List<AssignmentCenterItem>();

            if (node != null)
            {
                if (state.Above != null)
                {
                    min = state.Above.Sequence;
                }

                if (state.Below != null)
                {
                    max = state.Below.Sequence;
                }

                node.Sequence = Context.Sequence(min, max);

                if (!string.IsNullOrWhiteSpace(state.Changed.ParentId))
                {
                    node.ParentId = state.Changed.ParentId;
                    node.Parent = state.Changed.Parent;
                }
                else
                {
                    node.ParentId = Context.Course.CourseType == "FACEPLATE" ? "PX_MULTIPART_LESSONS" : state.Category.Id;
                }

                changes.AddRange(UpdateContainer(node, toc, container, subContainerId));
                state.Changed = node;

                changes.AddRange(AdjustAssignmentDates(state.Category, node));
                changes.Add(node);
            }

            var parent = state.FindItem(state.Changed.ParentId);

            while (parent != null)
            {
                if (ExpandOrContractParentDates(parent, node, true))
                {
                    if (!changes.ToList().Exists(i => i.Id == parent.Id))
                    {
                        changes.Add(parent);
                    }
                }

                parent = state.FindItem(parent.ParentId);
            }

            var item = ContentActions.GetContent(Context.EntityId, node.Id);
            //if this is a new/copied item, it does not exist yet. we do not need to modify the parent IDs
            if (item != null)
            {
                var oldParentId = item.GetSyllabusFilterFromCategory(toc);

                if (oldParentId != state.Changed.ParentId)
                {
                    var oldParentAssignmentCenterItem = state.FindItem(oldParentId);

                    while (oldParentAssignmentCenterItem != null)
                    {
                        if (ExpandOrContractParentDates(oldParentAssignmentCenterItem, node, false))
                        {
                            if (!changes.Contains(oldParentAssignmentCenterItem))
                            {
                                changes.Add(oldParentAssignmentCenterItem);
                            }
                        }

                        oldParentAssignmentCenterItem = state.FindItem(oldParentAssignmentCenterItem.ParentId);
                    } //TODO: handle case where the parent is not in the state
                }
            }
            return changes;
        }

        private bool ExpandOrContractParentDates(AssignmentCenterItem parent, AssignmentCenterItem node, bool isIncludeCurrentItem)
        {
            if (parent == null)
            {
                return false;
            }

            var startDate = GetLowestSiblingDate(parent.Children, node, isIncludeCurrentItem);
            var endDate = GetHighestSiblingDate(parent.Children, node, isIncludeCurrentItem);

            bool isModified = false;

            if (parent.StartDate != startDate)
            {
                parent.StartDate = startDate;
                isModified = true;
            }

            if (parent.EndDate != endDate)
            {
                parent.EndDate = endDate;
                isModified = true;
            }

            return isModified;
        }

        private DateTime GetLowestSiblingDate(List<AssignmentCenterItem> siblings, AssignmentCenterItem current, bool isIncludeCurrentItem)
        {
            DateTime date = DateTime.MaxValue;
            bool isModified = false;

            if (siblings != null)
            {
                foreach (var item in siblings)
                {
                    if (item.Id == current.Id && !isIncludeCurrentItem)
                    {
                        continue;
                    }

                    var endDate = null == item.Type || item.Type.ToLower() == "pxunit" ? item.StartDate : item.EndDate;

                    if (endDate.Year > DateTime.MinValue.Year && endDate < DateTime.MaxValue && endDate < date)
                    {
                        isModified = true;
                        date = endDate;
                    }
                }
            }

            if (isModified)
            {
                return date;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        private DateTime GetHighestSiblingDate(List<AssignmentCenterItem> siblings, AssignmentCenterItem current, bool isIncludeCurrentItem)
        {
            DateTime date = DateTime.MinValue;
            bool isModidifed = false;

            if (siblings != null)
            {
                foreach (var item in siblings)
                {
                    if (item.Id == current.Id && !isIncludeCurrentItem)
                    {
                        continue;
                    }

                    if (item.EndDate.Year > DateTime.MinValue.Year && item.EndDate < DateTime.MaxValue && item.EndDate > date)
                    {
                        date = item.EndDate;
                        isModidifed = true;
                    }
                }
            }

            if (isModidifed)
            {
                return date;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        internal List<AssignmentCenterItem> UpdateContainer(AssignmentCenterItem node, string toc, string container = "", string subContainerId = "")
        {
            List<AssignmentCenterItem> changes = new List<AssignmentCenterItem>();
            if (string.IsNullOrWhiteSpace(container) || string.IsNullOrWhiteSpace(subContainerId))
            {
                var newParent = ContentActions.GetContent(Context.EntityId, node.ParentId);
                if (newParent != null)
                {
                    container = newParent.GetContainer(toc);
                    var subcontainer = newParent.GetSubContainer(toc);
                    subContainerId = string.IsNullOrWhiteSpace(subcontainer) ||
                                     subcontainer == "PX_MULTIPART_LESSONS"
                                         ? newParent.Id
                                         : subcontainer;
                }
                if (string.IsNullOrWhiteSpace(container))
                {
                    container = "Launchpad";
                }
                if (string.IsNullOrWhiteSpace(subContainerId))
                {
                    subContainerId = "PX_MULTIPART_LESSONS";
                }
            }

            if (subContainerId != node.GetSubContainer(toc) || container != node.GetContainer(toc))
            {
                node.SetContainer(container, toc);
                node.SetSubContainer(subContainerId, toc);

                //if node has children, also update children
                if (node.Children != null && node.Children.Count > 0)
                {
                    foreach (var child in node.Children)
                    {
                        //if this node has a real parent, copy that parent's subcontainer
                        //else the subcontainer is the id of the node
                        string childSubcontainer = subContainerId != "PX_MULTIPART_LESSONS"
                            ? node.GetSubContainer(toc)
                            : node.Id;
                        changes.AddRange(UpdateContainer(child, toc, container, childSubcontainer));
                        changes.Add(child);
                    }
                }
            }

            return changes;
        }

        /// <summary>
        /// Handles the case when an item is moved.
        /// </summary>
        /// <param name="state">State of the tree in which an item has been copied.</param>
        /// <returns>Updated state for the tree.</returns>
        private AssignmentCenterNavigationState CopyItem(AssignmentCenterNavigationState state, string toc)
        {
            var node = state.FindItem(state.Changed.Id);

            List<AssignmentCenterItem> changes = new List<AssignmentCenterItem>();

            if (node != null)
            {

                //first create a new item as a copy of the existing item
                var item = ContentActions.GetContent(Context.EntityId, node.Id);

                item.Id = Guid.NewGuid().ToString();
                ContentActions.StoreContent(item);

                node.ParentId = state.Changed.ParentId;
                node.Parent = state.Changed.Parent;
                node.Id = item.Id;

                changes.AddRange(MoveItemGetChanges(state, node, toc));
                changes.Add(node);
            }

            if (!changes.IsNullOrEmpty())
            {
                state.Changes = changes.ToList();
                ContentActions.UpdateAssignmentCenterItems(state.Category.Id, changes.Map(c => c.ToAssignmentCenterItem()), toc);
                ContentActions.UpdateAssignmentCenterCategory(Context.EntityId, state.Category.ToAssignmentCenterCategory());
            }

            return state;
        }

        /// <summary>
        /// Handles the case when an item is assigned without being moved.
        /// </summary>
        /// <param name="state">State of the tree in which an item has been assigned.</param>
        /// <param name="toc">The toc.</param>
        /// <returns>
        /// Updated state for the tree.
        /// </returns>
        private AssignmentCenterNavigationState UnAssignItem(AssignmentCenterNavigationState state, string toc, bool keepInGradebook = true)
        {
            var node = state.Changed != null ? state.FindItem(state.Changed.Id) : null;

            // node in category selected, roll up the dates.
            if (node != null)
            {
                string changedId = state.Changed.Id;

                if (!keepInGradebook)
                {
                    node.GradebookCategory = string.Empty;
                }

                IList<AssignmentCenterItem> changes = GetAllChildren(node.Children).Where(c => c.EndDate.Year > 1).ToList();
                changes.Add(node);

                ContentActions.UnAssignAssignmentCenterItems(state.Category.Id, changes.Map(c => c.ToAssignmentCenterItem()), toc, keepInGradebook);

                var parent = state.FindItem(state.Changed.ParentId);

                if (parent != null)
                {
                    ExpandOrContractParentDates(parent, node, false);
                    state.Changed = parent;
                    state.IsIgnoreChildren = true;

                    if (parent.StartDate.Year == DateTime.MinValue.Year && (parent.EndDate.Year == DateTime.MinValue.Year || parent.EndDate == DateTime.MaxValue))
                    {
                        UnAssignItem(state, toc);
                    }
                    else
                    {
                        AssignDatesToItem(state, toc);
                    }

                    if (state.Changes.Count > 0)
                    {
                        changes = changes.Concat(state.Changes).ToList();
                    }

                    changes.Add(parent);
                }

                state.Changes = changes.Distinct().ToList();
                state.Changed = state.FindItem(changedId);
            }

            return state;
        }

        /// <summary>
        /// Unassigns the and remove item.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private AssignmentCenterNavigationState UnassignAndRemoveItems(AssignmentCenterNavigationState state)
        {
            var node = state.FindItem(state.Changed.Id);
            if (node != null)
            {
                state.Changes = UnAssignDatesToItem(node.Id, node, state.Toc, false, true);

                var ci = ContentActions.GetContent(Context.EntityId, node.Id);
                RemoveItem(ci, state.Changed.ParentId, state.Toc, state.RemoveFrom);
            }

            return state;
        }

        private IList<AssignmentCenterItem> GetAllChildren(IList<AssignmentCenterItem> childItems)
        {
            var changedItems = new List<AssignmentCenterItem>();

            if (!childItems.IsNullOrEmpty())
            {
                // move down the tree
                foreach (AssignmentCenterItem item in childItems)
                {
                    if (!item.Children.IsNullOrEmpty())
                    {
                        changedItems.AddRange(GetAllChildren(item.Children));
                    }
                }

                changedItems.AddRange(childItems);
            }

            return changedItems;
        }

        /// <summary>
        /// Assigns dates to a content item given a new due date
        /// ASSUME: Item cannot have children
        /// ASSUME:
        /// </summary>
        /// <param name="itemid">The itemid.</param>
        /// <param name="item">The item.</param>
        /// <param name="entityId">The entity unique identifier.</param>
        /// <param name="toc">The toc.</param>
        /// <returns>
        /// Returns the item plus all parents that have been updated with a new date range
        /// </returns>
        private List<AssignmentCenterItem> AssignDatesToItem(string itemid, AssignmentCenterItem item, string entityId, string toc)
        {
            return ProcessAssignment(itemid, item, false, toc, true, entityId);
        }

        /// <summary>
        /// UnAssigns dates to a content item given a new due date
        /// ASSUME: Item cannot have children
        /// ASSUME:
        /// </summary>
        /// <param name="itemid">The itemid.</param>
        /// <param name="item">The item.</param>
        /// <param name="toc">The toc.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <returns>
        /// Returns the item plus all parents that have been updated with a new date range
        /// </returns>
        private List<AssignmentCenterItem> UnAssignDatesToItem(string itemid, AssignmentCenterItem item, string toc, bool keepInGradebook, bool updateChildren = false)
        {
            var changes = ProcessAssignment(itemid, item, true, toc, keepInGradebook);

            if (!updateChildren) return changes;

            item.StartDate = item.EndDate = DateTime.MinValue;

            changes.AddRange(UpdateChildren(item, toc, true));

            return changes;
        }

        /// <summary>
        /// Assigns (unassigns/moves) item
        /// </summary>
        /// <param name="itemid">The itemid.</param>
        /// <param name="item">The item.</param>
        /// <param name="isUnAssigned">if set to <c>true</c> [is function assigned].</param>
        /// <param name="toc">The toc that the assignment item belongs to</param>
        /// <param name="entityId">The entity unique identifier.</param>
        /// <returns></returns>
        public List<AssignmentCenterItem> ProcessAssignment(string itemid, AssignmentCenterItem item, 
                                                            bool isUnAssigned, string toc, bool keepInGradebook, string entityId = "")
        {
            if (entityId.IsNullOrEmpty())
            {
                entityId = Context.EntityId;
            }

            //get the item
            var ci = ContentActions.GetContent(entityId, itemid)
                                   .ToContentItem(ContentActions, true, null, false, toc);

            var node = ci.ToAssignmentCenterItem(toc);
            //strip out title (save space)
            node.Title = string.Empty;

            var listAssignedItems = new List<AssignmentCenterItem> {node};

            //update item
            if (isUnAssigned)
            {
                this.ContentActions.UnAssignAssignmentCenterItems(string.Empty, listAssignedItems.Map(o => o.ToAssignmentCenterItem()), toc, keepInGradebook);
            }
            else
            {
                node.StartDate = item.StartDate;
                node.EndDate = item.EndDate;
                node.SubmissionGradeAction = item.SubmissionGradeAction;
                node.GradebookCategory = item.GradebookCategory;
                if (item.Points.HasValue)
                {
                    node.Points = item.Points;
                }
                if (item.DefaultPoints.HasValue)
                {
                    node.DefaultPoints = item.DefaultPoints;
                }
                node.UnitGradebookCategory = item.UnitGradebookCategory;

                this.ContentActions.UpdateAssignmentCenterItems(string.Empty, listAssignedItems.Map(o => o.ToAssignmentCenterItem()), toc, entityId);
            }

            //get parents of item (with children)
            var dcParents = ContentHelper.GetParentHeirachy(node.Id, TreeCategoryType.ManagementCard, toc, entityId);
            var topParent = dcParents.LastOrDefault(o => !o.Id.Equals("PX_MULTIPART_LESSONS"));
            var allChildren = ContentActions.ListContentWithDueDates(entityId, topParent.GetContainer(toc), topParent.Id, toc)
                                            .Map(o => o.ToContentItem(ContentActions, true, null, false, toc))
                                            .Map(o => o.ToAssignmentCenterItem(toc)).ToList();

            var result = new List<AssignmentCenterItem>();

            foreach (var parent in dcParents)
            {
                if (parent.Id == node.Id)
                {
                    //don't update the node again
                    continue;
                }
                var startDate = DateTime.MinValue;
                var endDate = DateTime.MinValue;
                var startDateTZ = new DateTimeWithZone(startDate, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false);
                var endDateTZ = new DateTimeWithZone(endDate, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false);

                var dueChildren = allChildren.Where(o => o.EndDate.ToShortDateString() != DateTime.MinValue.ToShortDateString()
                                                         && o.ParentId == parent.Id).ToList();

                if (!dueChildren.IsNullOrEmpty())
                {
                    foreach (var child in dueChildren)
                    {
                        //update children with new due dates previously updated
                        var childParent = dcParents.FirstOrDefault(p => p.Id == child.Id);

                        if (childParent != null)
                        {
                            child.StartDate = childParent.AssignmentSettings.StartDate;
                            child.EndDate = childParent.AssignmentSettings.DueDate;
                            child.StartDateTZ = childParent.AssignmentSettings.StartDateTZ;
                            child.EndDateTZ = childParent.AssignmentSettings.DueDateTZ;
                        }

                        //get start date and end date based on whether child is a folder
                        if (!child.Children.IsNullOrEmpty())
                        {
                            if (child.StartDate < startDate || startDate.Year == DateTime.MinValue.Year)
                            {
                                startDate = child.StartDate;
                                startDateTZ = new DateTimeWithZone(startDate, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false);
                            }
                        }
                        else
                        {
                            if (child.EndDate < startDate || startDate.Year == DateTime.MinValue.Year)
                            {
                                startDate = child.EndDate;
                                startDateTZ = new DateTimeWithZone(startDate, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false);
                            }
                        }

                        if (child.EndDate > endDate || endDate.Year == DateTime.MinValue.Year)
                        {
                            endDate = child.EndDate;
                            endDateTZ = new DateTimeWithZone(endDate, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false); 
                        }
                    }

                    if (isUnAssigned && startDate.Year == DateTime.MinValue.Year && endDate.Year == DateTime.MinValue.Year)
                    {
                        parent.AssignmentSettings.StartDate = DateTime.MinValue;
                        parent.AssignmentSettings.DueDate = DateTime.MinValue;
                        parent.AssignmentSettings.StartDateTZ = new DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false);
                        parent.AssignmentSettings.DueDateTZ = new DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false);
                    }
                    else
                    {
                        parent.AssignmentSettings.StartDate = startDate.Year != DateTime.MinValue.Year ? startDate : parent.AssignmentSettings.StartDate;
                        parent.AssignmentSettings.DueDate = endDate.Year != DateTime.MinValue.Year ? endDate : parent.AssignmentSettings.DueDate;
                        parent.AssignmentSettings.StartDateTZ = startDate.Year != DateTime.MinValue.Year ? startDateTZ : parent.AssignmentSettings.StartDateTZ;
                        parent.AssignmentSettings.DueDateTZ = endDate.Year != DateTime.MinValue.Year ? endDateTZ : parent.AssignmentSettings.DueDateTZ;
                    }
                }

                result.Add(parent.ToContentItem(ContentActions).ToAssignmentCenterItem(toc));
            }

            //store parents
            ContentActions.UpdateAssignmentCenterItems(String.Empty, result.Map(o => o.ToAssignmentCenterItem()), toc, entityId);
            
            //return only the nodes to update, not their children
            result.ForEach(r =>
            {
                r.Children = null;
                r.Title = "";
            });

            return result;
        }

        /// <summary>
        /// Handles the case when an item is assigned without being moved.
        /// </summary>
        /// <param name="state">State of the tree in which an item has been assigned.</param>
        /// <returns>Updated state for the tree.</returns>
        private AssignmentCenterNavigationState AssignDatesToItem(AssignmentCenterNavigationState state, string toc)
        {
            var entityId = state.EntityId;

            if (entityId.IsNullOrEmpty())
            {
                entityId = Context.EntityId;
            }

            var node = state.Changed != null ? state.FindItem(state.Changed.Id) : null;
            if (node != null && node.Points < 1)
            {
                node.GradebookCategory = "-1";
                state.Changed.GradebookCategory = "-1";
            }

            // node in category selected, roll up the dates.
            if (node != null)
            {
                if (!node.Children.IsNullOrEmpty() && state.IsIgnoreChildren == false)
                {
                    AdjustUnitDateRange(node, node.Children);
                }

                IList<AssignmentCenterItem> changes = AdjustAssignmentDates(state.Category, node);
                changes.Add(node);
                node.Points = state.Changed.Points;
                node.DefaultPoints = state.Changed.DefaultPoints;
                node.GradebookCategory = state.Changed.GradebookCategory;

                var ci = ContentActions.GetContent(entityId, state.Changed.Id);

                if (state.Changed.SubmissionGradeAction != SubmissionGradeAction.NotSet)
                {
                    node.SubmissionGradeAction = state.Changed.SubmissionGradeAction;
                }
                else if (ci.AssignmentSettings.SubmissionGradeAction != BizDC.SubmissionGradeAction.NotSet)
                {
                    node.SubmissionGradeAction = (Bfw.PX.PXPub.Models.SubmissionGradeAction)ci.AssignmentSettings.SubmissionGradeAction;
                }

                UpdateGradeBookCategorySequence(state, ci, node);

                if (!string.IsNullOrEmpty(state.Changed.Type) && state.Changed.Type.ToLowerInvariant() == "pxunit")
                {
                    if (String.IsNullOrEmpty(ci.UnitGradebookCategory))
                    {
                        string newCategory = System.Web.HttpUtility.HtmlEncode(state.Changed.RawTitle);
                        string newGradebookCategoryId = this.AddGradeBookCategoryToCourse(newCategory: newCategory);

                        if (!String.IsNullOrEmpty(newGradebookCategoryId))
                        {
                            ci.UnitGradebookCategory = newGradebookCategoryId;
                            ContentActions.StoreContent(ci, Context.EntityId);

                            node.UnitGradebookCategory = ci.UnitGradebookCategory;
                        }
                    }
                }

                if (!node.Children.IsNullOrEmpty() && state.IsIgnoreChildren == false)
                {
                    var childChanges = AdjustAssignmentDatesByUnit(node, node.Children);
                    if (!childChanges.IsNullOrEmpty())
                    {
                        changes = changes.Concat(childChanges).ToList();
                    }
                }

                var parent = state.FindItem(node.ParentId);
                if (ExpandOrContractParentDates(parent, node, true))
                {
                    if (!changes.ToList().Exists(i => i.Id == parent.Id))
                    {
                        changes.Add(parent);
                    }
                }

                // if unit got assigned item folders also get assigned
                var changedUnits = new List<AssignmentCenterItem>();
                if (state.Changed.Type != null && state.Changed.Type.ToLowerInvariant() == "pxunit")
                {
                    foreach (var change in changes)
                    {
                        parent = change.Parent;
                        if (ExpandOrContractParentDates(parent, change, true))
                        {
                            if (!changes.ToList().Exists(i => i.Id == parent.Id))
                            {
                                changedUnits.Add(parent);
                            }
                        }
                    }

                    changedUnits.ForEach(o => changes.Add(o));
                }

                state.Changes = changes.ToList();

                ContentActions.UpdateAssignmentCenterItems(state.Category.Id, changes.Map(c => c.ToAssignmentCenterItem()), toc, entityId);
            }

            // category was selected
            if (node == null)
            {
                //AdjustCategoryDateRange(state.Category, state.Category.Items);
                IList<AssignmentCenterItem> changes = AdjustAssignmentDatesByCategory(state.Category, state.Category.Items);

                if (!changes.IsNullOrEmpty())
                {
                    ContentActions.UpdateAssignmentCenterItems(state.Category.Id, changes.Map(c => c.ToAssignmentCenterItem()), toc, entityId);
                }
            }

            // this is always executed because if node is null, then it was the category that changed, otherwise we 
            // should save to cover the case where the node's new dates trickled up.
            ContentActions.UpdateAssignmentCenterCategory(entityId, state.Category.ToAssignmentCenterCategory());

            // TODO: implement return only changed nodes.

            return state;
        }

        /// <summary>
        /// Update gradebook category sequence when 
        /// - Application is FacePlate
        /// and one of the following condition is met
        /// 1) Item sequence is empty
        /// 2) Item has not been assigned
        /// 3) item gradebook category has been changed
        /// </summary>
        /// <param name="state">State of the tree in which an item has been assigned.</param>
        /// <param name="ci">Content item</param>
        /// <param name="assignmentCenterItem">Assignment cetner item with changes</param>
        private void UpdateGradeBookCategorySequence(AssignmentCenterNavigationState state, BizDC.ContentItem ci,
            AssignmentCenterItem assignmentCenterItem)
        {
            if (Context.Course.CourseType != CourseType.FACEPLATE.ToString())
                return;

            var maxCategorySequence = string.Empty;

            if (ci.AssignmentSettings.CategorySequence.IsNullOrEmpty() || ci.AssignmentSettings.DueDate.Year == DateTime.MinValue.Year ||
                ci.AssignmentSettings.Category != state.Changed.GradebookCategory)
            {
                var gradeBookWeights = GradeActions.GetGradeBookWeights(Context.CourseId);
                if (null == gradeBookWeights)
                    return;

                var categories = gradeBookWeights.GradeWeightCategories;
                if (categories.IsNullOrEmpty())
                    return;

                var category = categories.FirstOrDefault(o => o.Id.Equals(assignmentCenterItem.GradebookCategory));
                if (category != null && !category.Items.IsNullOrEmpty())
                {
                    var lastItems = category.Items.LastOrDefault();

                    if (lastItems != null)
                    {
                        var idList = new List<string> { lastItems.Id };

                        var items = ContentActions.GetItems(Context.EntityId, idList);
                        if (!items.IsNullOrEmpty())
                        {
                            maxCategorySequence = items.First().AssignmentSettings.CategorySequence;
                        }
                    }
                }
                

                var newCategorySequence = Context.Sequence(maxCategorySequence, string.Empty);
                assignmentCenterItem.CategorySequence = newCategorySequence;
            }
        }

        /// <summary>
        /// Handles the case when an item is assigned without being moved.
        /// </summary>
        /// <param name="state">State of the tree in which an item has been assigned.</param>
        /// <returns>Updated state for the tree.</returns>
        private AssignmentCenterNavigationState AssignPointsToItem(AssignmentCenterNavigationState state, string toc)
        {
            var node = state.Changed != null ? state.FindItem(state.Changed.Id) : null;

            if (node != null)
            {
                node.Points = state.Changed.Points;
                node.DefaultPoints = state.Changed.DefaultPoints;
                List<AssignmentCenterItem> changes = new List<AssignmentCenterItem>() { node };


                ContentActions.UpdateAssignmentCenterItems(state.Category.Id, changes.Map(c => c.ToAssignmentCenterItem()), toc);
                state.Changes = changes.ToList();
            }

            return state;
        }


        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <param name="state">State of the tree in which an item has been added.</param>
        /// <returns>Updated state for the tree.</returns>
        private AssignmentCenterNavigationState NewItem(AssignmentCenterNavigationState state,
            string toc)
        {
            var node = state.FindItem(state.Changed.Id);

            var biz = ContentActions.GetContent(Context.EntityId, state.Changed.Id);
            var item = biz.ToContentItem(ContentActions, false);

            if (node != null)
            {
                node.Title = item.Title;
                node.Type = item.Type;
                node.Points = item.MaxPoints;

                if (item.Type.ToLowerInvariant() == "pxunit")
                {
                    node.State = "closed";
                    node.Level = "2";
                    var changes = NewItem(node.Id, node.ParentId, toc);
                    state.Changes = changes;
                    return state;
                }
                else
                {
                    node.State = "barren";
                    state.Changes = MoveItemGetChanges(state, node, toc).ToList();

                    if (!state.Changes.IsNullOrEmpty())
                    {
                        ContentActions.UpdateAssignmentCenterItems(state.Category.Id, state.Changes.Map(c => c.ToAssignmentCenterItem()), toc);
                    }
                }
            }



            //biz.InAssignmentCenter = true;
            //biz.Sequence = state.Changed.Sequence;
            //biz.Container = state.Changed.Container;
            //biz.SubContainerId = state.Changed.SubcontainerId;
            //ContentActions.StoreContent(biz);

            return state;
        }

        private void FlattenTree(AssignmentCenterItem root, List<AssignmentCenterItem> children)
        {
            if (!root.Children.IsNullOrEmpty())
            {
                children.AddRange(root.Children);

                foreach (var child in root.Children)
                {
                    FlattenTree(child, children);
                }
            }
        }

        /// <summary>
        /// Checks to make sure that the assignment dates of the changed item are inside the 
        /// ranges set by their parent items.  If any dates need to be adjusted, then the
        /// returned list will contain those items with their corrected dates
        /// </summary>
        /// <param name="changed"></param>
        /// <returns></returns>
        private IList<AssignmentCenterItem> AdjustAssignmentDates(AssignmentCenterCategory category, AssignmentCenterItem changed)
        {
            var items = new List<AssignmentCenterItem>();
            AssignmentCenterItem parent = changed.Parent;
            var current = changed;
            bool hasChanged = false;
            DateTime newStart;
            DateTime newEnd;

            // move up the tree an fix any assignment dates
            while (parent != null)
            {
                var startDate = parent.StartDate;
                var endDate = parent.EndDate;

                var children = from c in GetAllChildren(parent.Children)
                               where !c.Type.ToLower().Equals("pxunit") && !c.EndDate.Equals(DateTime.MinValue)
                               select c;

                if (children.Count() > 0)
                {
                    startDate = children.Min(o => o.EndDate);
                    endDate = children.Max(o => o.EndDate);
                }

                if (parent.StartDate != startDate)
                {
                    parent.StartDate = startDate;
                    hasChanged = true;
                }

                if (parent.EndDate != endDate)
                {
                    parent.EndDate = endDate;
                    hasChanged = true;
                }

                /*
                if (AdjustStartDate(current.StartDate, startDate, parent.EndDate, out newStart, out newEnd))
                {
                    hasChanged = true;
                    parent.StartDate = startDate; // newStart;
                    parent.EndDate = newEnd;
                }

                if (AdjustEndDate(current.EndDate, startDate, parent.EndDate, out newStart, out newEnd))
                {
                    hasChanged = true;
                    parent.EndDate = newEnd;
                }
                */

                if (hasChanged)
                {
                    items.Add(parent);
                }

                current = parent;
                parent = parent.Parent;
                hasChanged = false;
            }

            if (AdjustStartDate(current.StartDate, category.StartDate, category.EndDate, out newStart, out newEnd))
            {
                category.StartDate = newStart;
                category.EndDate = newEnd;
            }

            if (AdjustEndDate(current.EndDate, category.StartDate, category.EndDate, out newStart, out newEnd))
            {
                category.StartDate = newStart;
                category.EndDate = newEnd;
            }

            return items;
        }
        /// <summary>
        /// Adjusts the given range based on the given current date.
        /// </summary>
        /// <param name="current">Date that may be causing the range to change.</param>
        /// <param name="rangeStart">Original start of the range.</param>
        /// <param name="rangeEnd">Original end of the range.</param>
        /// <param name="newStart">New start of the range.</param>
        /// <param name="newEnd">New end of the range.</param>
        /// <returns>true if the range has been changed, false otherwise.</returns>
        private bool AdjustStartDate(DateTime current, DateTime rangeStart, DateTime rangeEnd, out DateTime newStart, out DateTime newEnd)
        {
            var hasChanged = false;
            newStart = rangeStart;
            newEnd = rangeEnd;

            if (current.Year != DateTime.MinValue.Year && !current.InRange(rangeStart, rangeEnd))
            {
                if (current < newStart)
                {
                    hasChanged = true;
                    newStart = current;
                }
            }

            if (current.Year != DateTime.MinValue.Year && newStart.Year == DateTime.MinValue.Year)
            {
                hasChanged = true;
                newStart = current;
            }

            return hasChanged;
        }

        /// <summary>
        /// Adjusts the given range based on the given current date.
        /// </summary>
        /// <param name="current">Date that may be causing the range to change.</param>
        /// <param name="rangeStart">Original start of the range.</param>
        /// <param name="rangeEnd">Original end of the range.</param>
        /// <param name="newStart">New start of the range.</param>
        /// <param name="newEnd">New end of the range.</param>
        /// <returns>true if the range has been changed, false otherwise.</returns>
        private bool AdjustEndDate(DateTime current, DateTime rangeStart, DateTime rangeEnd, out DateTime newStart, out DateTime newEnd)
        {
            var hasChanged = false;
            newStart = rangeStart;
            newEnd = rangeEnd;

            if (current.Year != DateTime.MinValue.Year && !current.InRange(rangeStart, rangeEnd))
            {
                hasChanged = true;

                if (current < rangeStart)
                {
                    newStart = current;
                }

                if (current > rangeEnd)
                {
                    newEnd = current;
                }

                if (rangeStart.Year == DateTime.MinValue.Year)
                {
                    newStart = newEnd;
                }
            }

            return hasChanged;
        }

        private void AdjustCategoryDateRange(AssignmentCenterCategory category, IList<AssignmentCenterItem> childItems)
        {
            DateTime newStart;
            DateTime newEnd;

            if (!childItems.IsNullOrEmpty())
            {
                // move down the tree an fix any assignment dates
                foreach (AssignmentCenterItem item in childItems)
                {
                    if (AdjustStartDate(item.StartDate, category.StartDate, category.EndDate, out newStart, out newEnd))
                    {
                        category.StartDate = newStart;
                        category.EndDate = newEnd;
                    }

                    if (AdjustEndDate(item.EndDate, category.StartDate, category.EndDate, out newStart, out newEnd))
                    {
                        category.StartDate = newStart;
                        category.EndDate = newEnd;
                    }

                    AdjustCategoryDateRange(category, item.Children);
                }
            }
        }

        private void AdjustUnitDateRange(AssignmentCenterItem unit, IList<AssignmentCenterItem> childItems)
        {
            DateTime newStart;
            DateTime newEnd;

            if (!childItems.IsNullOrEmpty())
            {
                // move down the tree an fix any assignment dates
                foreach (AssignmentCenterItem item in childItems)
                {
                    if (unit.StartDate == unit.EndDate)
                    {
                        if (item.ToAssignmentCenterItem().IsAssigned || item.DefaultPoints > 0.0)
                        {
                            item.StartDate = unit.StartDate;
                            item.EndDate = unit.EndDate;
                        }
                    }
                    else
                    {
                        if (AdjustStartDate(item.StartDate, unit.StartDate, unit.EndDate, out newStart, out newEnd))
                        {
                            unit.StartDate = newStart;
                            unit.EndDate = newEnd;
                        }

                        if (AdjustEndDate(item.EndDate, unit.StartDate, unit.EndDate, out newStart, out newEnd))
                        {
                            unit.StartDate = newStart;
                            unit.EndDate = newEnd;
                        }
                    }

                    AdjustUnitDateRange(unit, item.Children);
                }
            }
        }

        private IList<AssignmentCenterItem> AdjustAssignmentDatesByCategory(AssignmentCenterCategory category, IList<AssignmentCenterItem> childItems)
        {
            var changedItems = new List<AssignmentCenterItem>();

            if (!childItems.IsNullOrEmpty())
            {
                bool hasChanged = false;

                // move down the tree an fix any assignment dates
                foreach (AssignmentCenterItem item in childItems)
                {
                    if (item.EndDate.Year == DateTime.MinValue.Year || (item.StartDate != category.StartDate && item.EndDate != category.EndDate))
                    {
                        hasChanged = true;
                        item.StartDate = category.StartDate;
                        item.EndDate = category.EndDate;
                    }

                    if (hasChanged)
                    {
                        changedItems.Add(item);
                    }

                    if (!item.Children.IsNullOrEmpty())
                    {
                        changedItems.AddRange(AdjustAssignmentDatesByCategory(category, item.Children));
                    }

                    hasChanged = false;
                }
            }

            return changedItems;
        }

        private IList<AssignmentCenterItem> AdjustAssignmentDatesByUnit(AssignmentCenterItem unit, IList<AssignmentCenterItem> childItems)
        {
            var changedItems = new List<AssignmentCenterItem>();

            if (!childItems.IsNullOrEmpty())
            {
                bool hasChanged = false;

                // move down the tree an fix any assignment dates
                foreach (AssignmentCenterItem item in childItems)
                {
                    if (item.Points == 0.0)
                    {
                        item.Points = item.DefaultPoints;
                    }

                    if (String.IsNullOrEmpty(item.GradebookCategory))
                    {
                        item.GradebookCategory = unit.UnitGradebookCategory;
                    }

                    //manually force date update
                    if (item.ToAssignmentCenterItem().IsAssigned)
                    {
                        hasChanged = true;
                        item.StartDate = unit.StartDate;
                        item.EndDate = unit.EndDate;
                    }

                    if (hasChanged)
                    {
                        if (!changedItems.Exists(i => i.Id == item.Id))
                        {
                            changedItems.Add(item);
                        }
                    }

                    if (!item.Children.IsNullOrEmpty())
                    {
                        changedItems.AddRange(AdjustAssignmentDatesByUnit(unit, item.Children));
                    }

                    hasChanged = false;
                }
            }

            return changedItems;
        }

        #region DEPRECATED FUNCTIONALITY

        /// <summary>
        /// Shows the or hide from students.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private AssignmentCenterNavigationState ShowOrHideFromStudents(AssignmentCenterNavigationState state,
            string toc)
        {
            var node = state.FindItem(state.Changed.Id);
            state.Changes = new List<AssignmentCenterItem>() { node };

            if (node != null)
            {
                List<BizDC.ContentItem> allItems = new List<BizDC.ContentItem>();
                List<BizDC.ContentItem> parentItems = new List<BizDC.ContentItem>();

                ShowOrHideItem(node, allItems, !state.Changed.isVisibleToStudents);

                if (!allItems.IsNullOrEmpty())
                {
                    ContentActions.StoreContents(allItems);
                }

                if (state.Changed.isVisibleToStudents)
                {
                    //set item's parents to show
                    var treeType = (Context.Course.CourseType == Bfw.PX.Biz.DataContracts.CourseType.FACEPLATE.ToString())
                                       ? TreeCategoryType.ManagementCard
                                       : TreeCategoryType.Assignment;

                    parentItems = ContentHelper.GetParentHeirachy(node.Id, treeType, toc);
                    var parentUpdatedItems = (from c in parentItems where c.HiddenFromStudents select c).ToList();

                    foreach (var item in parentUpdatedItems)
                    {
                        item.HiddenFromStudents = false;
                    }

                    if (!parentUpdatedItems.IsNullOrEmpty())
                    {
                        ContentActions.StoreContents(parentUpdatedItems);
                    }

                    var parentUpdatedAssignedItems = parentUpdatedItems.Map(o => o.ToAssignment(ContentActions).ToAssignmentCenterItem(toc)).ToList();

                    parentUpdatedAssignedItems.ForEach(o =>
                    {
                        o.isVisibleToStudents = true;
                    });

                    state.Changes.AddRange(parentUpdatedAssignedItems);
                }
            }

            return state;
        }

        /// <summary>
        /// Shows the item.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="allItems">All items.</param>
        private void ShowOrHideItem(AssignmentCenterItem node, List<BizDC.ContentItem> allItems, bool hide)
        {
            var ci = ContentActions.GetContent(Context.EntityId, node.Id);

            // treats special cases such as Browse more resources
            if (ci == null)
            {
                return;
            }

            ci.HiddenFromStudents = hide;
            node.isVisibleToStudents = !hide;

            allItems.Add(ci);

            //if (node.Children != null)
            //{
            //    foreach (var child in node.Children)
            //    {
            //        ShowOrHideItem(child, allItems, hide);
            //    }
            //}
        }
        #endregion
    }
}
