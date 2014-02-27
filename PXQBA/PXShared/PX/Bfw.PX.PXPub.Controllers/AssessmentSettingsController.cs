using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class AssessmentSettingsController : Controller
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        public AssessmentSettingsController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions)
        {
            Context = context;
            ContentActions = contentActions;
        }

        /// <summary>
        /// Displays the quiz settings component
        /// </summary>
        /// <param name="itemId">Id of the item</param>
        /// <returns>The quiz settings</returns>
        public PartialViewResult Index(string itemId, string entityId)
        {
            // Create a view model
            var model = new Models.AssessmentSettings();
            if (entityId == "ManageGroups")
                entityId = "EntireClass";

            // If we were not specified an entity id (e.g., a group id), then
            // just load the settings for the entire class.
            model.EntityIdIsCourseId = String.IsNullOrEmpty(entityId) || entityId.ToLower() == "null" || entityId.Equals("EntireClass", StringComparison.CurrentCultureIgnoreCase);
            if (model.EntityIdIsCourseId)
            {
                entityId = Context.CourseId;
            }
            model.EntityId = entityId;

            // Load the quiz settings
            BizDC.ContentItem item = ContentActions.GetContent(entityId, itemId);

            // Populate the view model
            model.MapFrom(item);
            if (item.ParentId == "PX_TEMPLATES")
            {
                ViewData["IsTemplate"] = true;
            }
            else
            {
                ViewData["IsTemplate"] = false;
            }

            ViewData["enableLearningCurveQuiz"] = Context.Course.EnableLearningCurveQuiz;
            ViewData["enableHomeworkQuiz"] = Context.Course.EnableHomeworkQuiz;

            // Render the quiz settings
            return PartialView(model);
        }

        /// <summary>
        /// Saves the quiz settings
        /// </summary>
        /// <param name="itemId">The item Id</param>
        /// <param name="settings">The quiz settings</param>
        /// <returns>Success</returns>
        [HttpPost]
        public JsonResult Save(Models.AssessmentSettings settings)
        {
            string result = string.Empty;
            string returnMessage = string.Empty;

            if (settings.AllowViewHints == true && settings.HintSubstractPercentage < 0)
            {
                result = "FAIL";
                returnMessage = "Hint subtract value can't be less than zero";
            }
            else
            {
                if (Request.Params["AssessmentSettings.ShowScoreAfter"] != null)
                {
                    // Bind assessment settings because of the prefix problem
                    settings.ShowScoreAfter = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowScoreAfter"]);
                    settings.ShowQuestionsAnswers = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowQuestionsAnswers"]);
                    settings.ShowRightWrong = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowRightWrong"]);
                    settings.ShowAnswers = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowAnswers"]);
                    settings.ShowFeedbackAndRemarks = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowFeedbackAndRemarks"]);
                    settings.ShowSolutions = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowSolutions"]);
                }

                if (String.IsNullOrEmpty(settings.EntityId) || settings.EntityId.ToLower() == "null" || settings.EntityId.Equals("EntireClass", StringComparison.CurrentCultureIgnoreCase))
                {
                    settings.EntityId = Context.CourseId;
                }
                // Load the quiz settings
                BizDC.ContentItem item = ContentActions.GetContent(settings.EntityId, settings.AssessmentId);

                // Map new settings
                settings.MapTo(item);

                // Store back the new settings
                ContentActions.StoreContent(item, settings.EntityId);

                // Tell the UI everything was saved
                result = "SUCCESS";
                returnMessage = "Your settings have been saved";
            }

            return Json(new { Result = result, ReturnMessage = returnMessage });
        }

        private static Dictionary<AssessmentType, string> typeMap = new Dictionary<AssessmentType, string>()
        {
            { AssessmentType.Homework, "Homework" },
            { AssessmentType.Quiz, "Assessment" },
            { AssessmentType.LearningCurve, "Assessment" }
        };

        [HttpPost]
        public JsonResult ChangeType(string itemId, AssessmentType newType)
        {

            var result = new JsonResult();

            var item = ContentActions.GetContent(Context.CourseId, itemId);
            item.Type = typeMap[newType];
            if (newType == AssessmentType.LearningCurve)
            {
                item.Subtype = "LearningCurveActivity";
            }
            else
            {
                item.Subtype = "";
            }

            ContentActions.StoreContent(item);

            result.Data = Json(new { success = true });
            return result;
        }

        public ActionResult SearchClassRoster()
        {
            return View();
        }
    }
}