using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Bfw.Common;
using Bfw.Common.Collections;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Configuration;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provides all actions necessary to support the AssignmentWidget
    /// </summary>
    [PerfTraceFilter]
    public class AssignmentWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// The actions for getting gradable items.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }
        /// <summary>
        /// The actions for getting gradable items.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Access to an IAssignmnetActions implementation.
        /// </summary>
        protected BizSC.IAssignmentActions AssignmentActions { get; set; }

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Gets or sets the Assignment Center Helper
        /// </summary>
        protected IAssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// Depends on the IBusinessContext and IGradeActions interfaces.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gradeActions">The grade actions.</param>
        /// <param name="contentActions">The content actions.</param>
        public AssignmentWidgetController(BizSC.IBusinessContext context, BizSC.IGradeActions gradeActions, BizSC.IContentActions contentActions, 
                                          BizSC.IAssignmentActions assignmentActions, IAssignmentCenterHelper assigmentCenterHelper)
        {
            Context = context;
            GradeActions = gradeActions;
            ContentActions = contentActions;
            AssignmentActions = assignmentActions;
            AssignmentCenterHelper = assigmentCenterHelper;
        }

        /// <summary>
        /// Returns upcoming assignments widget for TOC (Home page)
        /// </summary>
        /// <returns></returns>
        public ActionResult LaunchPad()
        {
            var items = new List<BizDC.ContentItem>();

            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                items = AssignmentCenterHelper.GetDueAssignmentsForInstructor(Context.EntityId, 7).ToList();
            }
            else 
            {
                items = AssignmentCenterHelper.GetDueAssignmentsForStudent(7, false, false).ToList();
            }

            if (items != null && items.Count > 0)
            {
                UpcommingActivitiesModel model = new UpcommingActivitiesModel();

                model.CountOfAssignments = items.Count(i => i.AssignmentSettings.DueDate <= DateTime.Now.AddDays(7).ToUniversalTime());

                if (model.CountOfAssignments == 0)
                {
                    model.DueInDays = "no";
                }
                else
                {
                    model.DueInDays = model.CountOfAssignments.ToString();
                }

                ViewData.Model = model;
            }
           
            return View();
        }

        public ActionResult CreateAssignment()
        {
            var assignments = new List<ContentItem>();
            foreach( var item in ContentActions.GetAssignmentFolders())
            {
                assignments.Add(item.ToContentItem(ContentActions));
            }
            ViewData["assignments"] = assignments;
            return View();
        }

        public void SetAssignmentFolder(string itemId, string folderId)
        {
            var items = ContentActions.GetItems(Context.EntityId, new List<string>() { itemId });
            var item = items.FirstOrDefault();

            item.AssignmentFolderId = folderId;
            ContentActions.StoreContent(item);
        }

        #region IPXWidget Members

        /// <summary>
        /// Gets a summarized list of all Assignments for the current entity and date and
        /// renders them in a view.
        /// </summary>
        /// <returns>ViewResult that renders a summarized list of Announcements.</returns>
        public ActionResult Summary(Models.Widget widget)
        {
            var rootFolder = ConfigurationManager.AppSettings["assignmentCenterRootFolder"];

            var entityId = string.IsNullOrEmpty(Context.EnrollmentId) ? Context.EntityId : Context.EnrollmentId;

            var items = ContentActions.ListContentWithDueDates(entityId, string.Empty);
            var permissionedItems = new List<BizDC.ContentItem>();
            var accessLevel = Context.AccessLevel.ToString().ToLowerInvariant();

            foreach (var item in items)
            {
                if (item.Subtype != null && item.Subtype.ToLowerInvariant() == "pxunit")
                {
                    continue;
                }

                var ci = item.ToContentItem(ContentActions);

                if (ci.Visibility.Elements(accessLevel).Count() != 0)
                {
                    permissionedItems.Add(item);
                }
            }

            var model = new AssignmentWidget(DateTime.Now.GetCourseDateTime());

            if (!permissionedItems.IsNullOrEmpty())
            {
                var all = permissionedItems.Map(m => m.ToAssignedItem());

                model.Groups = new List<AssignedItemGroup>() { GetToday(all, model), GetThisWeek(all, model), GetNextWeek(all, model) };
                model.All = all.OrderBy(a => a.DueDate);

                foreach (var group in model.Groups)
                {
                    if (!group.Assignments.IsNullOrEmpty())
                    {
                        foreach (var item in group.Assignments)
                        {
                            model.HasData = true;
                            break;
                        }

                        if (model.HasData)
                            break;
                    }
                }
            }

            ViewData["CourseType"] = Context.Course.CourseType;
            ViewData.Model = model;
            return View(items);
        }

        /// <summary>
        /// This method is used to get the assigned item for today.
        /// </summary>
        /// <param name="all">All the Assigned item.</param>
        /// <param name="model">This is the AssignmentWidget model.</param>
        /// <returns></returns>
        private AssignedItemGroup GetToday(IEnumerable<AssignedItem> all, AssignmentWidget model)
        {
            var startDate = model.ReferenceDate;
            var firstDefaultDate = String.Empty;            
            var assignments = AssignmentsForRange(all, model.ReferenceDate, model.ReferenceDate.EndOfDay());

            if (!assignments.IsNullOrEmpty())
            {
                assignments = assignments.OrderBy(a => a.DueDate).ThenBy(c => c.Title).ToList();

                foreach (AssignedItem assignment in assignments)
                {
                    TimeSpan ts = assignment.DueDate.Subtract(startDate);

                    if (ts.Hours >= 1)
                    {
                        assignment.DueDateDisplay = string.Format("in {0} hours", ts.Hours);
                    }
                    else if (ts.Minutes >= 1)
                    {
                        assignment.DueDateDisplay = string.Format("in {0} minutes", ts.Minutes);
                    }
                    else if (ts.Seconds >= 1)
                    {
                        assignment.DueDateDisplay = string.Format("in {0} seconds", ts.Seconds);
                    }
                }

                firstDefaultDate = string.Format("the first {0}", assignments.First().DueDateDisplay);
            }
            else
            {
                assignments = new List<AssignedItem>();
            }
                        
            var today = new AssignedItemGroup(
                    assignments
                )
            {
                Title = "today",
                Type = "today",
                DefaultDisplay = "There are currently no assignments due today.",
                FirstDefaultDate = firstDefaultDate,
                CountOfAssignments = assignments.Count()
            };
           
            return today;
        }

        /// <summary>
        /// This method is used to get the assigned item for this week.
        /// </summary>
        /// <param name="all">All the Assigned item.</param>
        /// <param name="model">This is the AssignmentWidget model.</param>
        /// <returns></returns>
        private AssignedItemGroup GetThisWeek(IEnumerable<AssignedItem> all, AssignmentWidget model)
        {            
            var firstDefaultDate = String.Empty;
            var twa = AssignmentsForRange(all,
                        model.ReferenceDate.StartOfDay(),
                        model.ReferenceDate.EndOfWeek(DayOfWeek.Sunday).EndOfDay()
                );
            
            if (!twa.IsNullOrEmpty())
            {
                twa = twa.Filter(d => !d.DueDate.InRange(model.ReferenceDate.StartOfDay(), model.ReferenceDate.EndOfDay()));
                twa = twa.OrderBy(a => a.DueDate).ThenBy(c => c.Title).ToList();

                if (!twa.IsNullOrEmpty())
                {
                    foreach (AssignedItem assignment in twa)
                    {
                        if (assignment.DueDate.ToString("MM/dd/yy") == DateTime.Today.AddDays(1).ToString("MM/dd/yy"))
                        {
                            assignment.DueDateDisplay = string.Format("Tomorrow, {0}", assignment.DueDate.ToString("h:mm tt"));
                        }
                        else
                        {
                            assignment.DueDateDisplay = assignment.DueDate.ToString("dddd, h:mm tt");
                        }
                    }

                    firstDefaultDate = string.Format("the first is due {0}", twa.First().DueDateDisplay);
                }
            }
            else
            {
                twa = new List<AssignedItem>();
            }

            var thisWeek = new AssignedItemGroup(
                twa
            )
            {
                Title = "this week",
                Type = "thisweek",
                DefaultDisplay = "There are currently no assignments due this week.",
                FirstDefaultDate = firstDefaultDate,
                CountOfAssignments = twa.Count()
            };

            return thisWeek;
        }

        /// <summary>
        /// This method is used to get the assigned item for next week.
        /// </summary>
        /// <param name="all">All the Assigned item.</param>
        /// <param name="model">This is the AssignmentWidget model.</param>
        /// <returns></returns>
        private AssignedItemGroup GetNextWeek(IEnumerable<AssignedItem> all, AssignmentWidget model)
        {
            var firstDefaultDate = String.Empty;
            var start = model.ReferenceDate.AddDays(7.0).StartOfWeek(DayOfWeek.Sunday).StartOfDay();
            var end = model.ReferenceDate.AddDays(7.0).EndOfWeek(DayOfWeek.Sunday).EndOfDay();
            
            var assignments = AssignmentsForRange(all, start, end);

            if (!assignments.IsNullOrEmpty())
            {
                assignments = assignments.OrderBy(a => a.DueDate).ThenBy(c => c.Title).ToList();

                foreach (AssignedItem assignment in assignments)
                {
                    assignment.DueDateDisplay = assignment.DueDate.ToString("dddd, h:mm tt");
                }

                firstDefaultDate = string.Format("the first is due {0}", assignments.First().DueDateDisplay);
            }
            else
            {
                assignments = new List<AssignedItem>();
            }

            var nextWeek = new AssignedItemGroup(assignments)
            {
                Title = "next week",
                Type = "nextweek",
                DefaultDisplay = "There are currently no assignments due next week.",
                FirstDefaultDate = firstDefaultDate,
                CountOfAssignments = assignments.Count()
            };           

            return nextWeek;
        }

        private AssignedItemGroup GetImportantAssignments(IEnumerable<AssignedItem> all, AssignmentWidget model)
        {
            var firstDefaultDate = String.Empty;
            var rootFolder = ConfigurationManager.AppSettings["assignmentCenterRootFolder"];
            var entityId = string.IsNullOrEmpty(Context.EnrollmentId) ? Context.EntityId : Context.EnrollmentId;
            var items = ContentActions.ListContentWithDueDates(entityId, string.Empty);
            var category = System.Configuration.ConfigurationManager.AppSettings["IsImportant"];
            List<AssignedItem> removalItem = new List<AssignedItem>();

            var start = model.ReferenceDate;
            var end = DateTime.MaxValue;

            var importantAssignmentsRange = AssignmentsForRange(all, start, end);


            var importantAssignments = (from item in items
                                        where item.Categories.FirstOrDefault(c => c.Id == category) != null
                                       select item).Map(m => m.ToAssignedItem()).OrderBy(i => i.DueDate).ToList();

            if (!importantAssignments.IsNullOrEmpty() && !importantAssignmentsRange.IsNullOrEmpty())
            {
                foreach(var assignment in importantAssignments)
                {
                    IEnumerable<AssignedItem> matchingAssignments = null;
                    matchingAssignments = importantAssignmentsRange.Filter(a => a.Id == assignment.Id);
                    if (matchingAssignments.IsNullOrEmpty())
                    {
                        removalItem.Add(assignment);
                        continue;
                    }
                    assignment.DueDateDisplay = assignment.DueDate.ToString("MMMM d, h:mm tt");
                }
                firstDefaultDate = string.Format("the first is due {0}", importantAssignments.First().DueDateDisplay);
            }
            else
            {
                importantAssignments = new List<AssignedItem>();
            }

            foreach (AssignedItem item in removalItem)
            {
                importantAssignments.Remove(item);
            }

            var importantAssignmentGroup = new AssignedItemGroup(
                   importantAssignments
               )
            {
                Title = "important",
                Type = "important",
                DefaultDisplay = "There are currently no important assignments",
                FirstDefaultDate = firstDefaultDate,
            };


            return importantAssignmentGroup;
        }

        /// <summary>
        /// Return a subset of a set of assignments whose due date falls within a given range.
        /// </summary>
        /// <param name="set">The set of assignments to search.</param>
        /// <param name="fromDate">Assignments with due dates prior to this argument will be filtered out.</param>
        /// <param name="toDate">Assignments with due dates after this argument will be filtered out.</param>
        /// <returns></returns>
        private static IEnumerable<AssignedItem> AssignmentsForRange(IEnumerable<AssignedItem> set, DateTime fromDate, DateTime toDate)
        {
            IEnumerable<AssignedItem> matchingOrdered = null;

            if (!set.IsNullOrEmpty())
            {
                matchingOrdered = set.Filter(a => a.DueDate.InRange(fromDate, toDate));
            }

            return matchingOrdered;
        }

        /// <summary>
        /// Renders all assignemnts
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return RedirectToAction("Index", "AssignmentCenter");
        }

        /// <summary>
        /// Returns upcoming assignment view of the assignments (Start page)
        /// </summary>
        /// <returns></returns>
        public ActionResult View(IEnumerable<BizDC.ContentItem> items)
        { 
            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                items = AssignmentCenterHelper.GetDueAssignmentsForInstructor(Context.EntityId, 15);
            }
            else
            {
                items = AssignmentCenterHelper.GetDueAssignmentsForStudent(15, true, false);
            }

            var all = items.Map(m => m.ToAssignedItem());

            var model = new AssignmentWidget(DateTime.Now.GetCourseDateTime());
            model.Groups = new List<AssignedItemGroup>() { GetToday(all, model), GetThisWeek(all, model), GetNextWeek(all, model) };
            model.All = all.OrderBy(a => a.DueDate).ThenBy(c => c.Title);

            foreach (var group in model.Groups)
            {
                if (!group.Assignments.IsNullOrEmpty())
                {
                    foreach (var item in group.Assignments)
                    {
                        model.HasData = true;
                        break;
                    }

                    if (model.HasData)
                    {
                        break;
                    }
                }
            }            

            ViewData.Model = model;
            ViewData["CourseType"] = Context.Course.CourseType;

            return View("Summary");
        }

        /// <summary>
        /// Returns the dialog to modify the important assignments of the course
        /// </summary>
        /// <returns></returns>
        public ActionResult OnBeforeEdit()
        {
            var rootFolder = ConfigurationManager.AppSettings["assignmentCenterRootFolder"];
            var entityId = string.IsNullOrEmpty(Context.EnrollmentId) ? Context.EntityId : Context.EnrollmentId;
            var items = ContentActions.ListContentWithDueDates(entityId, string.Empty);
            var category = System.Configuration.ConfigurationManager.AppSettings["IsImportant"];

            var permissionedItems = new List<BizDC.ContentItem>();
            var accessLevel = Context.AccessLevel.ToString().ToLowerInvariant();

            foreach (var item in items)
            {
                if (item.Subtype != null && item.Subtype.ToLowerInvariant() == "pxunit")
                {
                    continue;
                }

                var ci = item.ToContentItem(ContentActions);

                if (ci.Visibility.Elements(accessLevel).Count() != 0)
                {
                    permissionedItems.Add(item);
                }
            }

            var importantAssignments = (from item in permissionedItems
                                         where item.Categories.FirstOrDefault(c => c.Id == category) != null
                                        select item).Map(m => m.ToAssignedItem()).OrderBy(i => i.DueDate).ToList();
            ViewData.Model = importantAssignments;
            ViewData["AssignmentCount"] = permissionedItems.Count();
            return View("EditorModal");
        }

        /// <summary>
        /// Searches the assignments for the autocomplete textbox in EditorModal
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchAssignments(string searchText)
        {
            var rootFolder = ConfigurationManager.AppSettings["assignmentCenterRootFolder"];
            var entityId = string.IsNullOrEmpty(Context.EnrollmentId) ? Context.EntityId : Context.EnrollmentId;
            var accessLevel = Context.AccessLevel.ToString().ToLowerInvariant();

            var items = ContentActions.ListContentWithDueDates(entityId, string.Empty);
            var assignmentItems = new List<AssignedItem>();

            foreach (var item in items)
            {
                if (item.Subtype != null && item.Subtype.ToLowerInvariant() == "pxunit")
                {
                    continue;
                }

                var ci = item.ToContentItem(ContentActions);

                if (ci.Visibility.Elements(accessLevel).Count() != 0)
                {
                    assignmentItems.Add(item.ToAssignedItem());
                }
            }

            var searchedItems = (from item in assignmentItems
                                 where item.Title.ToLowerInvariant().Contains(searchText.ToLowerInvariant())
                                 select new { item.Id, item.Title });

            //return Json(new { id = "1", title = "first title" });
            return Json(searchedItems);
        }

        /// <summary>
        /// Saves the important assignments from Assignment Editor Modal
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveImportantAssignments(string importantAssignments, string removedAssignments, string pageName, string WidgetZoneID, string WidgetTemplateID, string WidgetID, string PrevSeq, string NextSeq)
        {
            if (!importantAssignments.IsNullOrEmpty())
            {
                var lstImportantAssignments = importantAssignments.Split(',').ToList();
                //mark the assignments as important            
                AssignmentActions.AssignmentImportant(lstImportantAssignments, true);            
            }

            if (!removedAssignments.IsNullOrEmpty())
            {
                var lstRemovedAssignments = removedAssignments.Split(',').ToList();
                //remove the important flag on the removed assignments
                AssignmentActions.AssignmentImportant(lstRemovedAssignments, false);
            }

            return Json(new { Result = "Success", Mode = "EDIT", OldWidgetID = WidgetID, ErrorMes = "", ZoneId = WidgetZoneID, WidgetTemplateID = WidgetTemplateID, WidgetId = "" });
        }

        #endregion
    }
}
