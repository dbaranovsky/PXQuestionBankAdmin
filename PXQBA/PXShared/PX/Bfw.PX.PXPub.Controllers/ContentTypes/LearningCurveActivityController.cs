using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using System.Web;
using Bfw.Agilix.DataContracts;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class LearningCurveActivityController : Controller
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
        /// Access to an IContentActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Access to an IQuestionActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IQuestionActions QuestionActions { get; set; }

        /// <summary>
        /// Access to an IEnrollmentActions implementation.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Access to an IUserActions implementation.
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }

        /// <summary>
        /// Access to a content helper object.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }
        #endregion Properties

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contActions"></param>
        /// <param name="questionActions"></param>
        /// <param name="userActions"></param>
        /// <param name="enrollmentActions"></param>
        /// <param name="helper"></param>
        public LearningCurveActivityController(BizSC.IBusinessContext context, BizSC.IContentActions contActions, BizSC.IQuestionActions questionActions, BizSC.IUserActions userActions, BizSC.IEnrollmentActions enrollmentActions, ContentHelper helper)
        {
            Context = context;
            ContentActions = contActions;
            UserActions = userActions;
            EnrollmentActions = enrollmentActions;
            QuestionActions = questionActions;
            ContentHelper = helper;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Create a new learning item
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Create(LearningCurveActivity content, string toc = "syllabusfilter")
        {
            var timezone = content.CourseInfo == null ? Context.Course.CourseTimeZone : content.CourseInfo.CourseTimeZone;
            var offset = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            if(offset != null)
                content.CustomFields.Add( new KeyValuePair<string,string>("course_time_zone", offset.StandardName + "," + offset.BaseUtcOffset.TotalHours) );
            
            ContentHelper.StoreQuiz(content);
            var model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, false, false);
            ViewData["isStartActiviy"] = false;
            return View("DisplayItem", model);
        }

        /// <summary>
        /// Generate the learning curve content format needed for the player
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public ActionResult GenerateLearningCurvePlayerContent(string id, string questionId = null)  
        {
            var learningCurve = ContentActions.GetContent(Context.EntityId, id).ToLearningCurveActivity(ContentActions, QuestionActions, true, true, true);
            learningCurve.BookId = RouteData.Values["course"].ToString();
            if (!String.IsNullOrEmpty(questionId))
            {
                ModifyItemForSingleQuestionDisplay(learningCurve, questionId);
            }
            var content = new LearningCurveTransform(learningCurve).Execute();
            //content = Uri.EscapeDataString(content);
            return Content(content);
        }

        /// <summary>
        /// Update target score of an item
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="itemid"></param>
        /// <param name="targetScore"></param>
        /// <returns></returns>
        public ActionResult UpdateTargetScoreInItem(string entityId, string itemId, string targetScore) {

            if(string.IsNullOrEmpty(entityId))
                entityId = Context.EntityId;

            var learningCurve = ContentActions.GetContent(entityId, itemId);
            try
            {
                var queryStrings = HttpUtility.ParseQueryString(learningCurve.Href);
                queryStrings["st"] = targetScore;
                string url = HttpUtility.UrlDecode(queryStrings.ToString());
                learningCurve.Href = url;
                ContentActions.StoreContent(learningCurve);
            }
            catch (Exception e)
            {
                return Json( new { Status="Error", Message = e.Message });
            }

            return Json(new { Status = "Success"});
            
        }

        /// <summary>
        /// Get info of an item
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public JsonResult GetEbookInfo(string entityId, string itemId)
        {
            if (string.IsNullOrEmpty(entityId))
                entityId = Context.EntityId;

            var failJsonResponse =
                Json(
                    new
                    {
                        Status = "Fail",
                        ProductType = null == Context.Course ? string.Empty : Context.Course.CourseType
                    });

            if (itemId.IsNullOrEmpty())
                return failJsonResponse;

            var item = ContentActions.GetContent(entityId, itemId);
            
            if (null != item)
            {
                return Json(new { Status = "Success", ProductType = null == Context.Course? string.Empty : Context.Course.CourseType, Id = item.Id, Title = item.Title });
            }
            var itemSearch = new ItemSearch()
            {
                EntityId = entityId,
                Query = "/meta-angel-pageid='" + itemId + "'",
                ExcludeStudentItem = true
            };

            var angelItem = ContentActions.FindContentItems(itemSearch);
            return angelItem.IsNullOrEmpty() ? failJsonResponse : Json(new { Status = "Success", Id = angelItem.First().Id, Title = angelItem.First().Title });
        }

        /// <summary>
        /// get user email adress by enrollment id
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns>A list of email address</returns>
        public IEnumerable<string> GetUserEmailAddress(List<string> enrollmentIds)
        {
            if (enrollmentIds.IsNullOrEmpty())
                return null;
            var classEnrollments = EnrollmentActions.GetAllEntityEnrollmentsAsAdmin(Context.EntityId);
            var activityEnrollments = classEnrollments.Where(e => enrollmentIds.Contains(e.Id)).ToList();
            return activityEnrollments.IsNullOrEmpty()
                ? null
                : activityEnrollments.Where(e => e.User != null).Select(e => HttpUtility.UrlEncode(e.User.Email));
        }

        /// <summary>
        /// get user email adress by enrollment id
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns>Json object</returns>
        public JsonResult GetUserEmailAddressJsonResult(List<string> enrollmentIds)
        {
            if (enrollmentIds.IsNullOrEmpty())
                return null;
            var emails = GetUserEmailAddress(enrollmentIds);
            return emails.IsNullOrEmpty()?null : Json(emails);
        } 
        #endregion Methods
        
        #region Implementation
        private void ModifyItemForSingleQuestionDisplay(LearningCurveActivity item, string questionId)
        {
            if (item.Topics != null)
            {
                var found = false;
                for (int i = item.Topics.Count - 1; i >= 0; i--)
                {
                    var t = item.Topics[i];
                    if (!found && t.Questions != null)
                    {
                        var question = t.Questions.SingleOrDefault(q => q.Id == questionId);
                        if (question != null)
                        {
                            found = true;
                            t.Questions = new List<Models.Question>() { question };
                            continue;
                        }
                        else
                        {
                            item.Topics.Remove(t);
                        }
                    }
                    else
                    {
                        item.Topics.Remove(t);
                    }
                }
            }
        }
        #endregion Implementation
    }
}
