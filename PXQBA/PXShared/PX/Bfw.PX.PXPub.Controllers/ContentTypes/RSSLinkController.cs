using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class RSSLinkController : Controller
    {
        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Access to an IAssignmnetActions implementation.
        /// </summary>
        protected BizSC.IAssignmentActions AssignmentActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the grade actions.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }

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
        /// Initializes a new instance of the <see cref="RSSFeedController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="helper">The helper.</param>
        /// <param name="pageActions">The Page Actions</param>
        /// <param name="assignmentActions">The Assignment Actions</param>
        /// <param name="enrollmentActions">The enrollment actions.</param>
        /// <param name="gradeActions">The grade actions.</param>
        public RSSLinkController(BizSC.IBusinessContext context, 
                                BizSC.IContentActions contActions, 
                                ContentHelper helper, 
                                BizSC.IPageActions pageActions, 
                                BizSC.IAssignmentActions assignmentActions, 
                                BizSC.IEnrollmentActions enrollmentActions,
                                BizSC.IGradeActions gradeActions)
        {
            Context = context;
            ContentActions = contActions;
            ContentHelper = helper;
            PageActions = pageActions;
            AssignmentActions = assignmentActions;
            EnrollmentActions = enrollmentActions;
            GradeActions = gradeActions;
        }

        /// <summary>
        /// Attempts to save the RSS Article
        /// </summary>
        /// <param name="RSSFeedUrl"></param>
        /// <param name="ArticleLink"></param>
        /// <param name="ArticleTitle"></param>
        /// <param name="ArticleDescription"></param>
        /// <param name="ArticlePubDate"></param>
        /// <returns></returns>
        public ActionResult ArchiveRssFeed(string RSSFeedUrl, string ArticleLink, string ArticleTitle,
            string ArticleDescription, string ArticlePubDate, string toc = "syllabusfilter")
        {
            Models.RssLink article = new RssLink(RSSFeedUrl, ArticleLink, ArticleTitle, ArticleDescription, ArticlePubDate);
            ContentHelper.StoreRssLink(article, Context.EntityId, toc);
            return Json(new { Result = "SUCCESS", RSSArticleId = article.Id });
        }

        public ActionResult SaveRssFeed(string RSSFeedUrl, string ArticleLink, string ArticleTitle, string ArticleDescription, string ArticlePubDate, string parentId, string sequence ="")
        {
            var id = SaveRSSLink(RSSFeedUrl, ArticleLink, ArticleTitle, ArticleDescription, ArticlePubDate, parentId, sequence);
            return Json(new { Result = "SUCCESS", RSSArticleId = id });
        }

        /// <summary>
        /// Attempts to Remove the RSS Article
        /// </summary>
        /// <param name="rssArticleId"></param>
        /// <param name="IsConfirm"></param>
        /// <returns></returns>
        public ActionResult UnArchiveRssFeed(string rssArticleId,bool IsConfirm)
        {
            string msg = string.Empty;
            
            var submissions = GradeActions.GetStudentsSubmissionInfo(Context.EntityId, rssArticleId, EnrollmentActions);
            int submissionsCount = 0;
            foreach (var submission in submissions)
            {
                submissionsCount++;
            }

            if ((submissionsCount == 0) || (IsConfirm))
            {
                var archivedItem = ContentActions.GetContent(Context.CourseId, rssArticleId).ToAssignedItem();
                AssignmentActions.Unassign(archivedItem.ToContentItem(Context.EntityId, ContentActions));
                archivedItem.StartDate = DateTime.MinValue;
                archivedItem.DueDate = DateTime.MinValue;
                ContentHelper.RemoveRssLink(rssArticleId, Context.EntityId);
                msg = "ItemDeleted";
            }
            else
            {
                msg = "ShowConfirmMessage";
            }

            return Json(new { Result = "SUCCESS", ControllerResponse = msg });
        }

        /// <summary>
        /// Assign Article
        /// </summary>
        /// <param name="rssArticleId"></param>
        /// <param name="assignDate"></param>
        /// <returns></returns>
        public ActionResult AssignRssFeed(string RSSFeedUrl, string ArticleLink, string ArticleTitle, string ArticleDescription,
            string ArticlePubDate, string RssArticleId, string RssAssignDate, string toc = "syllabusfilter")
        {
            DateTime rssDtTime = new DateTime();
            if (RssAssignDate != "")
            {
                rssDtTime = Convert.ToDateTime(RssAssignDate);
            }

            if (RssArticleId == string.Empty)
            {
                Models.RssLink article = new RssLink(RSSFeedUrl, ArticleLink, ArticleTitle, ArticleDescription, ArticlePubDate);
                ContentHelper.StoreRssLink(article, Context.EntityId, toc);
                RssArticleId = article.Id;
            }

            BizDC.AssignedItem assignedItem = new BizDC.AssignedItem();
            assignedItem.DueDate = rssDtTime;
            assignedItem.StartDate = rssDtTime;
            assignedItem.Id = RssArticleId;
            assignedItem.SyllabusFilter = "PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY";

            AssignmentActions.Assign(assignedItem);
            return Json(new { Result = "SUCCESS", RSSArticleId = RssArticleId, DueDate = rssDtTime.ToString(), IsAssigned = "true" });
        }


        /// <summary>
        /// Unassign Article
        /// </summary>
        /// <param name="rssArticleId"></param>
        /// <returns></returns>
        public ActionResult UnAssignRssFeed(string rssArticleId)
        {
            var archivedItem = ContentActions.GetContent(Context.CourseId, rssArticleId).ToAssignedItem();
            AssignmentActions.Unassign(archivedItem.ToContentItem(Context.EntityId, ContentActions));
            return Json(new { Result = "SUCCESS"});
        }
        
        /// <summary>
        /// Views the RSS feed.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="contentId">The content id.</param>
        /// <returns></returns>
        public ActionResult ViewRssLink(RssFeed content, string contentId, string toc = "syllabusfilter")
        {
            var model = ContentHelper.LoadContentView(contentId, ContentViewMode.Preview, toc);
            var result = View("DisplayItem", model);
            return result;
        }

        private string SaveRSSLink(string RSSFeedUrl, string ArticleLink, string ArticleTitle, string ArticleDescription, string ArticlePubDate, string parentId, string sequence = "")
        {
            Link content = new Link();
            content.Url = ArticleLink;
            content.Title = ArticleTitle;
            content.ParentId = parentId;

            ContentHelper.StoreLink(content, Context.EntityId);

            return content.Id;
        }
    }
}