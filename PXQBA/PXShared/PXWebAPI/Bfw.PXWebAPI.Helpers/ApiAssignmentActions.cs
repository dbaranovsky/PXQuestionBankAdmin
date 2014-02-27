using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;

namespace Bfw.PXWebAPI.Helpers
{

	/// <summary>
	/// IApiAssignmentActions interface
	/// </summary>
	public interface IApiAssignmentActions
	{
		/// <summary>
		/// Get Assignments
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		List<CalendarAssignment> GetAssignments(string id);

	}

	/// <summary>
	/// Assignment Actions
	/// </summary>
	public class ApiAssignmentActions : IApiAssignmentActions
	{

		#region Properties

		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }
        protected ICacheProvider CacheProvider { get; set; }
		protected IContentActions PxContentActions { get; set; }
		protected ICourseActions PxCourseActions { get; set; }
		protected INoteActions PxNoteActions { get; set; }
		protected IDocumentConverter PxDocumentConverter { get; set; }
		protected IDomainActions PxDomainActions { get; set; }
		protected IGradeActions PxGradeActions { get; set; }
		protected IEnrollmentActions PxEnrollmentActions { get; set; }
        protected IItemQueryActions PxItemQueryActions { get; set; }

		#endregion



		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiAssignmentActions"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
		/// <param name="context"> </param>
        public ApiAssignmentActions(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context, ICacheProvider cacheProvider)
		{
			SessionManager = sessionManager;
			Context = context;
		    CacheProvider = cacheProvider;

			PxDocumentConverter = new AsposeDocumentConverter();
            var databaseManager = new Bfw.Common.Database.DatabaseManager();
			PxContentActions = new ContentActions(context, sessionManager, PxDocumentConverter, databaseManager, PxItemQueryActions);
			PxNoteActions = new NoteActions(context, sessionManager);
			PxDomainActions = new DomainActions(context, sessionManager, cacheProvider);
			PxCourseActions = new CourseActions(Context, SessionManager, PxNoteActions, PxContentActions, PxDomainActions);
			PxGradeActions = new GradeActions(Context, sessionManager, PxDocumentConverter, PxContentActions, PxEnrollmentActions);
		}

	    

	    #endregion


		/// <summary>
		/// Get Assignments
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public List<CalendarAssignment> GetAssignments(string id)
		{
			var lastDay = DateTime.Now.ToString();
			var maxDate = DateTime.Parse(lastDay).AddMonths(2);

			var model = GetCalendarModel();
			var assignments = from a in model.All
							  where a.DueDate < maxDate
							  select new CalendarAssignment
							  {
								  id = a.Id,
								  start =
									  new Bfw.Common.DateTimeWithZone(a.DueDate,
																	  TimeZoneInfo.FindSystemTimeZoneById(
																		  Context.Course.CourseTimeZone), false).UniversalTime,
								  title = a.Title,
								  points = a.MaxPoints.HasValue ? a.MaxPoints.Value : 0,
								  editLink = GetLink(a, ContentViewMode.Assign),
								  openLink = GetLink(a, ContentViewMode.Preview),
								  type = a.Type
							  };

			var result = assignments.OrderBy(o => o.start).ThenBy(o => o.title).ToList();

			return result;
		}


		#region Private

		private string GetLink(AssignedItem assignment, ContentViewMode viewMode)
		{
			HttpContextBase currentContext = new HttpContextWrapper(System.Web.HttpContext.Current);

			var routeData = RouteTable.Routes.GetRouteData(currentContext);

			var result = String.Empty;

			if (routeData != null)
			{
				var requestContext = new RequestContext(currentContext, routeData);

				var htmlAttr = new Dictionary<string, object>();

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

					var urlHelper = new UrlHelper(requestContext);

					result = urlHelper.GetComponentLink(title, "item", assignment.Id, new
					{
						mode = viewMode,
						includeDiscussion = false,
						renderFNE = true,
						isBeingEdited =
					viewMode == ContentViewMode.Edit
					}, htmlAttr);
				}
				else
				{
					if (!string.IsNullOrEmpty(assignment.SubType) && assignment.SubType.ToLowerInvariant() == "eportfolio")
					{
						routeData.Values["folderid"] = assignment.Id;

						result = HtmlHelper.GenerateLink(requestContext, System.Web.Routing.RouteTable.Routes,
														 title, "MyEportfolios", "DisplayItem", "ContentWidget",
														 new RouteValueDictionary(
															new { id = assignment.Id, mode = viewMode, includeDiscussion = false }), htmlAttr);
					}
					else if (!string.IsNullOrEmpty(assignment.Type) && assignment.Type.ToLowerInvariant() == "rssfeed")
					{
						routeData.Values["id"] = assignment.Id;

						htmlAttr.Add("class", "fne-link");

						result = HtmlHelper.GenerateLink(requestContext, System.Web.Routing.RouteTable.Routes, title, string.Empty,
														 "ShowRssPopup", "RSSFeed", routeData.Values, htmlAttr);
					}
					else
					{
						routeData.Values["assignmentID"] = assignment.Id;

						htmlAttr.Add("class", "assignmentLink");

						result = HtmlHelper.GenerateLink(requestContext, System.Web.Routing.RouteTable.Routes, title, string.Empty,
														 "Index", "AssignmentCenter", routeData.Values, htmlAttr);
					}
				}
			}

			return result;
		}

		private AssignmentWidget GetCalendarModel()
		{
			var result = new AssignmentWidget(DateTime.Now.GetCourseDateTime());

			//var rootFolder = ConfigurationManager.AppSettings["assignmentCenterRootFolder"];
			var entityId = string.IsNullOrEmpty(Context.EnrollmentId) ? Context.EntityId : Context.EnrollmentId;

			var items = PxContentActions.ListContentWithDueDates(entityId, string.Empty);

			var subtitleList = new List<KeyValuePair<string, string>>();
			var permissionedItems = new List<Bfw.PX.Biz.DataContracts.ContentItem>();
			var accessLevel = Context.AccessLevel.ToString().ToLowerInvariant();

			var submissions = new List<Bfw.PX.Biz.DataContracts.Grade>();
			if (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
			{
				if (Context.EnrollmentId != null)
				{
					submissions = PxGradeActions.GetGradesByEnrollment(Context.EnrollmentId, items.Select(o => o.Id).ToList()).ToList();
				}
			}

			foreach (var item in items)
			{
				if (item.Subtype != null && item.Subtype.ToLowerInvariant() == "pxunit")
				{
					continue;
				}

				var ci = item.ToContentItem(PxContentActions);
				subtitleList.Add(new KeyValuePair<string, string>(item.Id, ci.GetFriendlyItemContentType()));

				//if the student has submitted an assignment then it is no longer available in the assignment widget
				if (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
				{
					if (submissions.Any(o => o.GradedItem.Id.Equals(item.Id) && !o.SubmittedDate.Equals(DateTime.MinValue)))
					{
						continue;
					}
				}

				if (ci.Visibility.Descendants(accessLevel).Count() != 0)
				{
					permissionedItems.Add(item);
				}
			}

			var all = permissionedItems.Map(m => m.ToAssignedItem());

			result.Groups = new List<AssignedItemGroup> { GetThisWeek(all, result), GetNextWeek(all, result) };
			result.Groups.AddRange(GetRestWeeks(all, result));
			result.Groups = ( from g in result.Groups
							  where g != null
							  select g ).ToList();

			var lastWeek = result.Groups.Max(o => DateTime.Parse(o.Title.Replace("week of ", "").Replace(" (this week)", "").Replace(" (next week)", "")));

			while (lastWeek < DateTime.Today.AddMonths(5))
			{
				lastWeek = lastWeek.AddDays(7);

				result.Groups.Add(new AssignedItemGroup(null)
				{
					Title = string.Format("week of {0}", lastWeek.ToString("MMM d")),
					Type = lastWeek.ToString("MMMM yyyy"),
					DefaultDisplay = "There are no assignments due this week.",
					FirstDefaultDate = string.Empty,
					CountOfAssignments = 0,
				});
			}

			result.All = all.OrderBy(a => a.DueDate);

			foreach (var group in result.Groups)
			{
				if (@group.Assignments.IsNullOrEmpty()) continue;
				foreach (var item in @group.Assignments)
				{
					var type = subtitleList.SingleOrDefault(o => o.Key.Equals(item.Id));

					if (type.Value == null) continue;
					item.FriendlyNameSourceType = type.Value;
					result.HasData = true;
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
		private static AssignedItemGroup GetThisWeek(IEnumerable<AssignedItem> all, AssignmentWidget model)
		{
			var firstDefaultDate = String.Empty;

			AssignedItemGroup thisWeek = null;

			if (all != null && all.Any())
			{
				var twa = AssignmentsForRange(all,
							model.ReferenceDate,
							model.ReferenceDate.EndOfWeek(DayOfWeek.Sunday).EndOfDay()
					).OrderBy(o => o.StartDate).ThenBy(o => o.Title).ToList();

				if (twa.FirstOrDefault(o => o.DueDate.ToShortDateString().Equals(DateTime.Now.ToShortDateString())) == null)
				{
					twa.Insert(0, new AssignedItem { DueDate = DateTime.Now });
				}

				if (!twa.IsNullOrEmpty())
				{
					twa = twa.OrderBy(a => a.DueDate).ToList();

					if (!twa.IsNullOrEmpty())
					{
						foreach (var assignment in twa)
						{
							var ts = assignment.DueDate.Subtract(model.ReferenceDate);

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
								assignment.DueDateDisplay = assignment.DueDate.ToString("MM/dd/yy") == DateTime.Today.AddDays(1).ToString("MM/dd/yy") ? string.Format("Tomorrow, {0}", assignment.DueDate.ToString("h:mm tt")) : assignment.DueDate.ToString("dddd, h:mm tt");
							}
						}

						firstDefaultDate = string.Format("the first is due {0}", twa.First().DueDateDisplay);
					}
				}
				else
				{
					twa = new List<AssignedItem>();
				}

				if (twa.FirstOrDefault(o => o.DueDate.ToShortDateString() == model.ReferenceDate.ToShortDateString()) == null)
				{
					twa.Add(new AssignedItem { DueDate = model.ReferenceDate, DueDateDisplay = "There are no assignments due today." });
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
		private static AssignedItemGroup GetNextWeek(IEnumerable<AssignedItem> all, AssignmentWidget model)
		{
			var firstDefaultDate = String.Empty;
			var start = model.ReferenceDate.AddDays(7.0).StartOfWeek(DayOfWeek.Sunday).StartOfDay();
			var end = model.ReferenceDate.AddDays(7.0).EndOfWeek(DayOfWeek.Sunday).EndOfDay();

			var assignments = AssignmentsForRange(all, start, end);
			if (!assignments.IsNullOrEmpty())
			{
				assignments = assignments.OrderBy(o => o.StartDate).ThenBy(o => o.Title).ToList();

				foreach (var assignment in assignments)
				{
					assignment.DueDateDisplay = assignment.DueDate.ToString("h:mm tt");
				}

				firstDefaultDate = string.Format("the first is due {0}", assignments.First().DueDateDisplay);
			}
			else
			{
				assignments = new List<AssignedItem>();
			}

			var nextWeek = new AssignedItemGroup(assignments.OrderBy(o => o.StartDate).ThenBy(o => o.Title))
			{
				Title = string.Format("week of {0} (next week)", start.StartOfWeek(DayOfWeek.Sunday).ToString("MMM d")),
				Type = start.ToString("MMMM yyyy"),
				DefaultDisplay = "There are no assignments due next week.",
				FirstDefaultDate = firstDefaultDate,
				CountOfAssignments = assignments.Count()
			};

			return nextWeek;
		}

		private static IEnumerable<AssignedItemGroup> GetRestWeeks(IEnumerable<AssignedItem> all, AssignmentWidget model)
		{
			var firstDefaultDate = String.Empty;
			var start = model.ReferenceDate.AddDays(14.0).StartOfWeek(DayOfWeek.Sunday).StartOfDay();
			var end = DateTime.MaxValue.EndOfDay();
            var weeks = new List<DateTime>(); // return

			var assignments = AssignmentsForRange(all, start, end);
			if (!assignments.IsNullOrEmpty())
			{
				assignments = assignments.OrderBy(o => o.StartDate).ThenBy(o => o.Title).ToList();

				foreach (var assignment in assignments)
				{
					assignment.DueDateDisplay = assignment.DueDate.ToString("h:mm tt");
				}

				firstDefaultDate = string.Format("the first is due {0}", assignments.First().DueDateDisplay);
			}
			else
			{
				assignments = new List<AssignedItem>();
			}

            if (assignments.Count() > 0)
            {
			    var defaultDays = start.AddDays(7 * 22);

                // Find last valid due date to show, sbd
                var lastShowDate = defaultDays;
			    var assignmentsOrderedByDueDateDesc = assignments.OrderBy(o => o.DueDate).Reverse().ToList();
                foreach (var assignment in assignmentsOrderedByDueDateDesc)
                {
                    if (assignment.DueDate.Year < DateTime.MaxValue.Year) // 9999
                    {
                        lastShowDate = assignment.DueDate;
                        break;
                    }
                }

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
            } // if assignments

			return weeks.Select(week => new AssignedItemGroup(AssignmentsForRange(assignments, week.StartOfWeek(DayOfWeek.Sunday), week.EndOfWeek(DayOfWeek.Sunday)))
			{
				Title = string.Format("week of {0}", week.StartOfWeek(DayOfWeek.Sunday).ToString("MMM d")),
				Type = week.StartOfWeek(DayOfWeek.Sunday).ToString("MMMM yyyy"),
				DefaultDisplay = "There are no assignments due this week.",
				FirstDefaultDate = firstDefaultDate,
				CountOfAssignments = assignments.Count()
			}).ToList();
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
