using System.Linq;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;


namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class LearningCurveActivityMapper
    {
        public static LearningCurveActivity ToLearningCurveActivity(this BizDC.ContentItem biz, BizSC.IContentActions content, BizSC.IQuestionActions questionActions, bool isLoadQuestions, bool isLoadTopics, bool isLoadRelatedContent = false)
        {
            if (biz == null) return null;
            var model = new LearningCurveActivity();


            model.Id = biz.Id;
            model.MaxPoints = biz.MaxPoints;
            model.Title = biz.Title;
            model.ParentId = biz.ParentId;
            model.Url = biz.Href;
            model.Sequence = biz.Sequence;
            //model.QuizType = (QuizType)Enum.Parse(typeof(QuizType), biz.Type);
            // TODO: There will be a better way to do this, need to follow up with Sam Hathaway about it.
            model.IsLC = biz.CustomExamType == "LearningCurve";
            model.Description = biz.Description;
            model.DefaultPoints = biz.DefaultPoints;
            model.CustomFields = biz.CustomFields;
            //set the due date of the quiz.
            //model.DueDate = biz.AssessmentSettings.DueDate;

            // Set the policy text according to the assessment settings
            model.PolicyDescription = AssignmentHelper.PolicyDescriptionFromSettings(biz.AssessmentSettings, model.GetQuizTypeName(model.QuizType));

            var context = ServiceLocator.Current.GetInstance<BizSC.IBusinessContext>();
            if (isLoadQuestions)
            {
                questionActions = questionActions ?? ServiceLocator.Current.GetInstance<BizSC.IQuestionActions>();
                var items = questionActions.GetQuestions(context.EntityId, biz, null, null);
                model.Questions = items.Questions.Map(q => { return !isLoadRelatedContent ? q.ToQuestion() : q.ToQuestion(); }).ToList();

                if (isLoadTopics)
                {
                    foreach (var q in model.Questions)
                    {
                        if (q.Type == "BANK")
                        {
                            var pool = content.GetContent(context.EntityId, q.Id).ToQuiz(content, null, true, true);
                            pool.RelatedContent = q.RelatedContent;
                            model.Topics.Add(pool);
                        }
                    }
                }
            }

            if (biz.AssessmentSettings != null)
            {
                model.AttemptLimit = biz.AssessmentSettings.AttemptLimit;
                model.AllowSaveAndContinue = biz.AssessmentSettings.AllowSaveAndContinue;
                model.TimeLimit = biz.AssessmentSettings.TimeLimit;
                model.SubmissionGradeAction = biz.AssessmentSettings.SubmissionGradeAction.ToString();
                model.AutoCalibrateDifficulty = biz.AssessmentSettings.AutoCalibrateDifficulty;
                model.TargetScore = biz.AssessmentSettings.LearningCurveTargetScore;
                model.AutoTargetScore = biz.AssessmentSettings.AutoTargetScore;
            }

            model.WhoopsRight = biz.GetValueFromProperty("bfw_whoopsright");
            model.WhoopsWrong = biz.GetValueFromProperty("bfw_whoopswrong");
            model.EbookReferenceDescription = biz.GetValueFromProperty("bfw_EbookReferenceDescription");
            
            if (context.Course != null) model.CourseInfo = context.Course.ToCourse();

            return model;
        }
    }
}
