using System.Web.Mvc;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
	public class RSSFeedController : Controller
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
		protected ContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
		/// Initializes a new instance of the <see cref="RSSFeedController"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="contActions">The cont actions.</param>
		/// <param name="helper">The helper.</param>
		/// <param name="pageActions">The Page Actions</param>
		/// <param name="assignmentActions">The Assignment Actions</param>
		/// <param name="enrollmentActions">The enrollment actions.</param>
		/// <param name="gradeActions">The grade actions.</param>
		public RSSFeedController( BizSC.IBusinessContext context,
								BizSC.IContentActions contActions,
								ContentHelper helper,
                                AssignmentCenterHelper assignmentCenterHelper
)
		{
			Context = context;
			ContentActions = contActions;
			ContentHelper = helper;
            AssignmentCenterHelper = assignmentCenterHelper;
		}

		/// <summary>
		/// Attempts to save the Html Document, if behavior is Cancel then nothing is saved and
		/// the action is redirected to CreateAndAssign.
		/// </summary>
		/// <param name="content">The content.<see cref="RssFeed"/></param>
		/// <param name="behavior">The behavior.</param>
		/// <returns></returns>
		public ActionResult SaveRssFeed( RssFeed content, string behavior, Assign assign, string toc = "syllabusfilter")
		{
			ActionResult result = null;
			ContentView model = null;

			if (behavior == "Save & Open")
			{
				behavior = "Save";
			}

			switch (behavior.ToLowerInvariant())
			{
				case "cancel":
					var idToLoad = string.IsNullOrEmpty(content.Id) ? content.ParentId : content.Id;
					model = ContentHelper.LoadContentView(idToLoad, ContentViewMode.Preview, false, toc);
					result = View("DisplayItem", model);
					break;

				case "save":

					if (ModelState.IsValid)
					{
						ContentHelper.StoreRssFeed(content, Context.EntityId);
                        if (assign != null && assign.DueDate.Year > System.DateTime.MinValue.Year)
                        {
                            assign.Id = content.Id;
                            AssignmentCenterHelper.AssignItem(assign, toc);
                        }
                        
						model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, false, toc);
						result = View("DisplayItem", model);
					}
					else
					{
						content.EnvironmentUrl = Context.EnvironmentUrl;
						content.CourseInfo = Context.Course.ToCourse();
						content.EnrollmentId = Context.EnrollmentId;
						content.Status = string.IsNullOrEmpty(content.Id) ? ContentStatus.New : ContentStatus.Existing;
						result = View("CreateContent", content);
					}
					break;

				default:
					result = View();
					break;
			}

			return result;
		}

		/// <summary>
		/// Views the RSS feed.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="contentId">The content id.</param>
		/// <returns></returns>
		public ActionResult ViewRssFeed( RssFeed content, string contentId, string toc = "syllabusfilter")
		{
			var model = ContentHelper.LoadContentView(contentId, ContentViewMode.Preview, toc);
			var result = View("DisplayItem", model);
			return result;
		}

		/// <summary>
		/// Show Popup
		/// </summary>
		/// <param name="id"></param>
		/// <param name="eId"></param>
		/// <param name="initialindex"></param>
		/// <returns></returns>
		public ActionResult ShowRssPopup( string id, string eId, string initialindex )
		{
			if (string.IsNullOrEmpty(eId))
			{
				eId = Context.EnrollmentId;
			}
			ViewData["id"] = id;
			ViewData["eId"] = eId;
			ViewData["initialindex"] = initialindex;
			return View("~/Views/Shared/DisplayTemplates/RssPartials/RssFeedPopup.ascx");
		}
	}
}