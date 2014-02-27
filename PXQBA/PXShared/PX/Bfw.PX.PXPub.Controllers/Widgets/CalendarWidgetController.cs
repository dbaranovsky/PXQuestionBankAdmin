using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web.Mvc;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using System.Web.Routing;
using System.Web;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provides all actions necessary to support the CalendarWidget
    /// </summary>
    [PerfTraceFilter]
    public class CalendarWidgetController : Controller, IPXWidget
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
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Gets or sets the Assignment Center Helper
        /// </summary>
        protected IAssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// The group actions.
        /// </summary>
        protected BizSC.IGroupActions GroupActions { get; set; }

        /// <summary>
        /// Depends on the IBusinessContext and IGradeActions interfaces.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gradeActions">The grade actions.</param>
        /// <param name="contentActions">The content actions.</param>
        public CalendarWidgetController(BizSC.IBusinessContext context, BizSC.IGradeActions gradeActions, BizSC.IContentActions contentActions, BizSC.IPageActions pageActions, IAssignmentCenterHelper assignmentCenterHelper, BizSC.IGroupActions groupActions)
        {
            Context = context;
            GradeActions = gradeActions;
            ContentActions = contentActions;
            PageActions = pageActions;
            AssignmentCenterHelper = assignmentCenterHelper;
            GroupActions = groupActions;
        }

        /// <summary>
        /// Returns list view of the assignments
        /// </summary>
        /// <returns></returns>
        public PartialViewResult AgendaFullView(string entityId)
        {
            var model = GetListModel(entityId);

            ViewData["ViewType"] = Context.AccessLevel;
            ViewData["timeZone"] = Context.Course.GetCourseTimeZoneAbbreviation();

            return PartialView(model);
        }

        /// <summary>
        /// Returns calendar view of the assignments
        /// </summary>
        /// <returns></returns>
        public PartialViewResult MonthFullView()
        {
            ViewData["ViewType"] = Context.AccessLevel;
            ViewData["DueLater"] = this.PageActions.GetInstructorConsoleLaunchPadSettings().DueLaterDays;

            return PartialView();
        }



        /// <summary>
        /// Get all assignments for the calendar in JSON format
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAssignments(string entityId, string firstDay, string lastDay)
        {
            JsonResult result = new JsonResult();
            var groups = new List<BizDC.Group>();

            DateTime startDate = DateTime.Parse(firstDay);
            DateTime endDate = DateTime.Parse(lastDay);

            if (startDate > endDate)
            {
                startDate = DateTime.Parse(lastDay);
                endDate = DateTime.Parse(firstDay);
            }

            if (entityId.Length == 0) {
                entityId = Context.EntityId;
            }

            var items = GetCalendarModel(entityId, startDate, endDate);

            if (Context.CourseId == entityId && Context.AccessLevel != BizSC.AccessLevel.Student)
            {
                groups = GroupActions.ListGroups(Context.CourseId).ToList();

                if (!groups.IsNullOrEmpty())
                {
                    groups.ForEach(g =>
                    {
                        var groupItems = GetCalendarModel(g.Id.ToString(), startDate, endDate);

                        items.ForEach(i =>
                        {
                            var dup = groupItems.Where(d => d.Id.Equals(i.Id) && d.AssignmentSettings.DueDate.Equals(i.AssignmentSettings.DueDate));

                            if (!dup.IsNullOrEmpty())
                            { 
                                dup.ToList().ForEach(r =>
                                {
                                    groupItems.Remove(r);
                                });                                
                            }
                        });

                        items.AddRange(groupItems);
                    });
                }
            }

            var assignments = from a in items
                              where a.AssignmentSettings.DueDate >= startDate && a.AssignmentSettings.DueDate <= endDate
                              select new CalendarAssignment
                              {
                                  itemid = a.Id,
                                  entityid = a.ActualEntityid,
                                  start = new Bfw.Common.DateTimeWithZone(a.AssignmentSettings.DueDate, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false).UniversalTime,
                                  originalstart = new Bfw.Common.DateTimeWithZone(GetOriginalDueDate(a, items), TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false).UniversalTime,
                                  title = a.Title,
                                  points = a.MaxPoints,
                                  editLink = GetLink(a.ToAssignedItem(), ContentViewMode.Assign),
                                  openLink = GetLink(a.ToAssignedItem(), ContentViewMode.Preview),
                                  type = a.Type,
                                  adjustedGroups = a.AssignmentSettings.DueDate != GetOriginalDueDate(a, items) ? GetAdjustedGroups(a, groups) : string.Empty
                              };

            result = new JsonDataContractResult(assignments.OrderBy(o => o.start).ThenBy(o => o.title).ToArray());

            return result;
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult View(string type, string guid)
        {
            return RedirectToAction("ViewAll", new { type = type });
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ViewAll(Models.Widget widget)
        {
            ViewData["type"] = Request.QueryString["type"];
            return View();
        }

        public ActionResult Summary(Models.Widget widget)
        {
            throw new NotImplementedException();
        }

        #region Private

        private DateTime GetOriginalDueDate(BizDC.ContentItem item, List<BizDC.ContentItem> items)
        {
            DateTime result = item.AssignmentSettings.DueDate;

            if (item.ActualEntityid != Context.CourseId)
            {
                var originalItem = items.SingleOrDefault(i => i.Id.Equals(item.Id) && i.ActualEntityid.Equals(Context.CourseId));

                if (originalItem != null)
                {
                    result = originalItem.AssignmentSettings.DueDate;
                }
                else
                {
                    result = DateTime.MinValue;
                }
            }

            return result;
        }

        private string GetAdjustedGroups(BizDC.ContentItem item, List<BizDC.Group> groups)
        {
            var result = string.Empty;

            if (!groups.IsNullOrEmpty())
            {
                var names = groups.Where(g => g.Id.ToString().Equals(item.ActualEntityid));

                if (!names.IsNullOrEmpty())
                {
                    result = string.Join(", ", names.Select(o => o.Name).ToArray());
                }
            }

            return result;
        }

        private string GetLink(AssignedItem assignment, ContentViewMode viewMode)
        {
            HttpContextBase currentContext = new HttpContextWrapper(System.Web.HttpContext.Current);
            RequestContext requestContext = this.ControllerContext.RequestContext;
            RouteData routeData = RouteTable.Routes.GetRouteData(currentContext);
            Dictionary<string, Object> htmlAttr = new Dictionary<string, object>();

            var result = string.Empty;
            var title = string.Empty;

            switch (viewMode)
            {
                case ContentViewMode.Preview:
                    title = "open";
                    break;
                case ContentViewMode.Edit:
                    title = "edit";
                    break;
                case ContentViewMode.Assign:
                    title = "edit";
                    break;
            }

            if (Context.Course.CourseType == CourseType.FACEPLATE.ToString())
            {
                htmlAttr.Add("class", "faceplatefne-assignmentwidget"); //fne-link loadFullFne 

                //result = HtmlHelper.GenerateLink(this.ControllerContext.RequestContext, System.Web.Routing.RouteTable.Routes, title, "Assignment", "DisplayItem", "ContentWidget", new RouteValueDictionary(new { id = assignment.Id, ), htmlAttr);
            result = Url.GetComponentLink(title, "item", assignment.Id, new {
                mode = viewMode, includeDiscussion = false, renderFNE = true, isBeingEdited = viewMode == ContentViewMode.Edit }, htmlAttr);
        }
            else
            {
                if (!string.IsNullOrEmpty(assignment.SubType) && assignment.SubType.ToLowerInvariant() == "eportfolio")
                {
                    routeData.Values["folderid"] = assignment.Id;

                    result = HtmlHelper.GenerateLink(this.ControllerContext.RequestContext, System.Web.Routing.RouteTable.Routes, title, "MyEportfolios", "DisplayItem", "ContentWidget", new RouteValueDictionary(new { id = assignment.Id, mode = viewMode, includeDiscussion = false }), htmlAttr);
                }
                else if (!string.IsNullOrEmpty(assignment.Type) && assignment.Type.ToLowerInvariant() == "rssfeed")
                {
                    routeData.Values["id"] = assignment.Id;

                    htmlAttr.Add("class", "fne-link");

                    result = HtmlHelper.GenerateLink(requestContext, System.Web.Routing.RouteTable.Routes, title, string.Empty, "ShowRssPopup", "RSSFeed", routeData.Values, htmlAttr);
                }
                else
                {
                    routeData.Values["assignmentID"] = assignment.Id;

                    htmlAttr.Add("class", "assignmentLink");

                    result = HtmlHelper.GenerateLink(requestContext, System.Web.Routing.RouteTable.Routes, title, string.Empty, "Index", "AssignmentCenter", routeData.Values, htmlAttr);
                }
            }

            return result;
        }

        // both calendar view and list view are configured to show 6 months worth of data
        // calendar always shows full calendar months for each month in the interval
        // list view starts with the current week and shows the remaining weeks for the first month, followed by five full calendar months.
        private DateTime GetNextIntervalMonth(DateTime d)
        {
            int showMonths = 6;
            return new DateTime(d.Year, d.Month, 1).AddMonths(showMonths);
        }
        // calculate number of days between date provided up to the end of the interal
        private int GetDays(DateTime d) {
            var nextIntervalMonth = GetNextIntervalMonth(d);
            int days = (int)nextIntervalMonth.Subtract(d).TotalDays + 1;
            return days;
        }

        private List<BizDC.ContentItem> GetCalendarModel(string entityId, DateTime startDate, DateTime endDate)
        {
            var refDate = DateTime.Now.GetCourseDateTime();
            var items = new List<BizDC.ContentItem>();

            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                items = AssignmentCenterHelper.GetDueAssignmentsForInstructor(entityId, startDate, endDate).ToList();
            }
            else
            {
                if (startDate < refDate && endDate > refDate)
                {
                    items = AssignmentCenterHelper.GetDueAssignmentsForStudent(-1 * refDate.Subtract(startDate).Days, true, true).ToList();
                    items.AddRange(AssignmentCenterHelper.GetDueAssignmentsForStudent(endDate.Subtract(refDate).Days, true, true).ToList());
                }
                else
                {
                    var numberOfDays = 0;

                    if (refDate > endDate)
                    {
                        numberOfDays = -1 * refDate.Subtract(startDate).Days;
                    }
                    else
                    {
                        numberOfDays = endDate.Subtract(refDate).Days;
                    }

                    items = AssignmentCenterHelper.GetDueAssignmentsForStudent(numberOfDays, true, true).ToList();
                }
            }

            return items;
        }

        private AssignmentWidget GetListModel(string entityId)
        {
            var result = new AssignmentWidget(DateTime.Now.GetCourseDateTime());
            var items = new List<BizDC.ContentItem>();
            var subtitleList = new List<KeyValuePair<string, string>>();
            if (entityId.IsNullOrEmpty())
            {
                entityId = Context.EntityId;
            }

            // How many days till next inteval?
            int numberOfDays = GetDays(result.ReferenceDate);

            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                items = AssignmentCenterHelper.GetDueAssignmentsForInstructor(entityId, numberOfDays).ToList();
            }
            else
            {
                items = AssignmentCenterHelper.GetDueAssignmentsForStudent(numberOfDays, true, true).ToList();
            }

            foreach (var item in items)
            {
                var ci = item.ToContentItem(ContentActions);
                subtitleList.Add(new KeyValuePair<string, string>(item.Id, ci.GetFriendlyItemContentType()));
            }
         
            var all = items.Map(m => m.ToAssignedItem()).ToList();

            result.Groups = new List<AssignedItemGroup>() { GetThisWeek(all, result), GetNextWeek(all, result) };
            result.Groups.AddRange(GetRestWeeks(all, result));            
            result.Groups = (from g in result.Groups
                             where g != null
                             select g).ToList();
           
            var lastWeek = result.Groups.Max(o => DateTime.Parse(o.Title.Replace("week of ", "").Replace(" (this week)", "").Replace(" (next week)", "")));

            var addedWeeeks = result.Groups.Select(a => a.Title).ToList();

           // The code above generated the collection of weeks up to the week of last assignment
           // Here we are just filling out the weeks up to the next 6 calendar months
           // This logic should be refactored and potentially merged into GetRestWeek()

            // when does the next interval begin?
            var nextInterval = GetNextIntervalMonth(result.ReferenceDate);

            // stepping forward one week - which month are we in?
            var nextWeek = lastWeek.AddDays(7); 
            var nextMonth = new DateTime(lastWeek.Year, lastWeek.Month, 1).AddMonths(1);

            while (nextMonth <= nextInterval)
            {
                lastWeek = lastWeek.AddDays(7);
                var weekTitle = string.Format("week of {0}", lastWeek.ToString("MMM d"));

                // nextMonth is calculated to control the while loop
                nextWeek = lastWeek.AddDays(7);
                nextMonth = new DateTime(nextWeek.Year, nextWeek.Month, 1).AddMonths(1);
                
                //If this group already exists, do not add it to the list.
                if (!addedWeeeks.IsNullOrEmpty() && addedWeeeks.Contains(weekTitle))
                    continue;

                result.Groups.Add(new AssignedItemGroup(null)
                {
                    Title = weekTitle,
                    Type = lastWeek.ToString("MMMM yyyy"),
                    DefaultDisplay = "There are no assignments due this week.",
                    FirstDefaultDate = string.Empty,
                    CountOfAssignments = 0,
                });
            }

            result.All = all.OrderBy(a => a.DueDate);

            foreach (var group in result.Groups)
            {
                if (!group.Assignments.IsNullOrEmpty())
                {                    
                    foreach (var item in group.Assignments)
                    {                        
                        var type = subtitleList.Where(o => o.Key.Equals(item.Id)).SingleOrDefault();

                        if (type.Value != null)
                        {
                            item.FriendlyNameSourceType = type.Value;
                            result.HasData = true;
                        }
                    }
                }
            }

            return result;
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

            AssignedItemGroup thisWeek = null;

            if (all != null && all.Count() > 0)
            {
                var twa = AssignmentsForRange(all,
                            model.ReferenceDate,
                            model.ReferenceDate.EndOfWeek(DayOfWeek.Sunday).EndOfDay()
                    ).OrderBy(o => o.StartDate).ThenBy(o => o.Title).ToList();

                if (twa.FirstOrDefault(o => o.DueDate.ToShortDateString().Equals(DateTime.Now.ToShortDateString())) == null)
                {
                    twa.Insert(0, new AssignedItem() { Id = "", DueDate = DateTime.Now });
                }

                if (!twa.IsNullOrEmpty())
                {
                    twa = twa.OrderBy(a => a.DueDate).ToList();

                    if (!twa.IsNullOrEmpty())
                    {
                        foreach (AssignedItem assignment in twa)
                        {
                            TimeSpan ts = assignment.DueDate.Subtract(model.ReferenceDate);

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
                            else
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
                        }

                        firstDefaultDate = string.Format("the first is due {0}", twa.First().DueDateDisplay);
                    }
                }
                else
                {
                    twa = new List<AssignedItem>();
                }

                if (twa.Where(o => o.DueDate.ToShortDateString() == model.ReferenceDate.ToShortDateString()).FirstOrDefault() == null)
                {
                    twa.Add(new AssignedItem() { DueDate = model.ReferenceDate, DueDateDisplay = "There are no assignments due today." });
                    twa.OrderBy(o => o.DueDate).ThenBy(o => o.Title);
                }

                thisWeek = new AssignedItemGroup(
                    twa
                )
                {
                    Title = string.Format("week of {0} (this week)", model.ReferenceDate.StartOfWeek(DayOfWeek.Sunday).ToString("MMM d")),
                    Type = model.ReferenceDate.ToString("MMMM yyyy"),
                    DefaultDisplay = "There are no assignments due this week.",
                    FirstDefaultDate = firstDefaultDate,
                    CountOfAssignments = twa.Count(),
                };
            }            

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
                foreach (AssignedItem assignment in assignments)
                {
                    assignment.DueDateDisplay = assignment.DueDate.ToString("h:mm tt");
                }

                firstDefaultDate = string.Format("the first is due {0}", assignments.First().DueDateDisplay);
            }
            else
            {
                assignments = new List<AssignedItem>();
            }

            var nextWeek = new AssignedItemGroup(assignments.OrderBy(o => o.DueDate).ThenBy(o => o.Title))
            {
                Title = string.Format("week of {0} (next week)", start.StartOfWeek(DayOfWeek.Sunday).ToString("MMM d")),
                Type = start.ToString("MMMM yyyy"),
                DefaultDisplay = "There are no assignments due next week.",
                FirstDefaultDate = firstDefaultDate,
                CountOfAssignments = assignments.Count()
            };

            return nextWeek;
        }

        private List<AssignedItemGroup> GetRestWeeks(IEnumerable<AssignedItem> all, AssignmentWidget model)
        {
            List<AssignedItemGroup> result = new List<AssignedItemGroup>();

            var firstDefaultDate = String.Empty;
            var start = model.ReferenceDate.AddDays(14.0).StartOfWeek(DayOfWeek.Sunday).StartOfDay();
            var end = DateTime.MaxValue.EndOfDay();

            var assignments = AssignmentsForRange(all, start, end);
            if (!assignments.IsNullOrEmpty())
            {
                assignments = assignments.OrderBy(o => o.DueDate).ThenBy(o => o.Title).ToList();
            }

            if (!assignments.IsNullOrEmpty())
            {
                foreach (AssignedItem assignment in assignments)
                {
                    assignment.DueDateDisplay = assignment.DueDate.ToString("h:mm tt");
                }

                firstDefaultDate = string.Format("the first is due {0}", assignments.First().DueDateDisplay);
            }
            else
            {
                assignments = new List<AssignedItem>();
            }

          
            var defaultDays = GetNextIntervalMonth(start).AddDays(-1);
    
            var lastAssignment = assignments.OrderBy(o => o.DueDate).LastOrDefault();

            var weeks = new List<DateTime>();

            if (lastAssignment != null)
            {
                var lastShowDate = lastAssignment.DueDate;

                if (defaultDays > lastShowDate)
                {
                    lastShowDate = defaultDays;
                }

                for (var date = start; date <= lastShowDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        weeks.Add(date);
                    }
                }
            }

            foreach (var week in weeks)
            {
                result.Add(new AssignedItemGroup(AssignmentsForRange(assignments, week.StartOfWeek(DayOfWeek.Sunday), week.EndOfWeek(DayOfWeek.Sunday)))
                {
                    Title = string.Format("week of {0}", week.StartOfWeek(DayOfWeek.Sunday).ToString("MMM d")),
                    Type = week.StartOfWeek(DayOfWeek.Sunday).ToString("MMMM yyyy"),
                    DefaultDisplay = "There are no assignments due this week.",
                    FirstDefaultDate = firstDefaultDate,
                    CountOfAssignments = assignments.Count()
                });
            }

            return result;
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

        #endregion
    }
}
