using System;
using System.Linq;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class AssessmentSettingsMapper
    {
        private static int AutoMultiplier
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LearningCurveDefaultTargetMultiplier"]);

            }
        }
        /// <summary>
        /// Maps QuizSettings to a generic ContentItem
        /// </summary>
        /// <param name="model">The quiz settings</param>
        /// <param name="item">The content item</param>
        public static void MapTo(this AssessmentSettings model, BizDC.ContentItem item)
        {
            if (!string.IsNullOrEmpty(item.Subtype) && item.Subtype.ToLowerInvariant() != "learningcurveactivity")
            {
                item.AssessmentSettings.SubmissionGradeAction = (BizDC.SubmissionGradeAction)model.SubmissionGradeAction;
                item.AssessmentSettings.GradeRule = (BizDC.GradeRule)model.GradeRule;
                item.AssessmentSettings.TimeLimit = model.TimeLimit;
                item.AssessmentSettings.QuestionDelivery = (BizDC.QuestionDelivery)model.QuestionDelivery;
                item.AssessmentSettings.AllowSaveAndContinue = model.AllowSaveAndContinue;
                item.AssessmentSettings.AutoSubmitAssessments = model.AutoSubmitAssessments;
                item.AssessmentSettings.RandomizeQuestionOrder = model.RandomizeQuestionOrder;
                item.AssessmentSettings.RandomizeAnswerOrder = model.RandomizeAnswerOrder;
                item.AssessmentSettings.AllowViewHints = model.AllowViewHints;
                item.AssessmentSettings.PercentSubstractHint = model.HintSubstractPercentage;
                item.AssessmentSettings.ShowScoreAfter = (model.ShowScoreAfter == null) ? BizDC.ReviewSetting.Never : (BizDC.ReviewSetting)model.ShowScoreAfter;
                item.AssessmentSettings.ShowQuestionsAnswers = (model.ShowQuestionsAnswers == null) ? BizDC.ReviewSetting.Never : (BizDC.ReviewSetting)model.ShowQuestionsAnswers;
                item.AssessmentSettings.ShowRightWrong = (model.ShowRightWrong == null) ? BizDC.ReviewSetting.Never : (BizDC.ReviewSetting)model.ShowRightWrong;
                item.AssessmentSettings.ShowAnswers = (model.ShowAnswers == null) ? BizDC.ReviewSetting.Never : (BizDC.ReviewSetting)model.ShowAnswers;
                item.AssessmentSettings.ShowFeedbackAndRemarks = (model.ShowFeedbackAndRemarks == null) ? BizDC.ReviewSetting.Never : (BizDC.ReviewSetting)model.ShowFeedbackAndRemarks;
                item.AssessmentSettings.ShowSolutions = (model.ShowSolutions == null) ? BizDC.ReviewSetting.Never : (BizDC.ReviewSetting)model.ShowSolutions;
                
            }
            if (model.NumberOfAttempts != null && model.NumberOfAttempts.Attempts != null)
            {
                item.AssessmentSettings.AttemptLimit = model.NumberOfAttempts.Attempts.GetValueOrDefault();
            }
            item.AssessmentSettings.StudentsCanEmailInstructors = model.StudentsCanEmailInstructors;
            item.AssessmentSettings.AutoCalibrateDifficulty = model.AutoCalibrateDifficulty;
            var poolCount = (from c in item.QuizQuestions where c.Type == "2" select c).Count();
            item.AssessmentSettings.LearningCurveTargetScore = model.AutoTargetScore ? (poolCount * AutoMultiplier).ToString() : model.LearningCurveTargetScore;
            item.AssessmentSettings.AutoTargetScore = model.AutoTargetScore;
        }

        /// <summary>
        /// Maps generic ContentItem to QuizSettings
        /// </summary>
        /// <param name="model">The quiz settings</param>
        /// <param name="item">The content item</param>
        public static void MapFrom(this AssessmentSettings model, BizDC.ContentItem item)
        {
            model.AssessmentType = item.Type.Equals("homework", StringComparison.CurrentCultureIgnoreCase) ? AssessmentType.Homework :
                item.Subtype.Equals("htmlquiz", StringComparison.CurrentCultureIgnoreCase) || item.Subtype.Equals("epage", StringComparison.CurrentCultureIgnoreCase) ? AssessmentType.HtmlQuiz : AssessmentType.Quiz;
            if (item.Subtype.ToLowerInvariant() == "learningcurveactivity")
            {
                model.AssessmentType = AssessmentType.LearningCurve;
                model.AutoCalibrateDifficulty = item.AssessmentSettings.AutoCalibrateDifficulty;
                model.LearningCurveTargetScore = string.IsNullOrEmpty(item.AssessmentSettings.LearningCurveTargetScore) ? "0" : item.AssessmentSettings.LearningCurveTargetScore;
                model.AutoTargetScore = item.AssessmentSettings.AutoTargetScore;
            }

            model.NumberOfAttempts = new NumberOfAttempts { Attempts = item.AssessmentSettings.AttemptLimit };
            model.SubmissionGradeAction = (SubmissionGradeAction)item.AssessmentSettings.SubmissionGradeAction;
            model.TimeLimit = item.AssessmentSettings.TimeLimit;
            model.QuestionDelivery = (QuestionDelivery)item.AssessmentSettings.QuestionDelivery;
            model.AllowSaveAndContinue = item.AssessmentSettings.AllowSaveAndContinue;
            model.AutoSubmitAssessments = item.AssessmentSettings.AutoSubmitAssessments;
            model.RandomizeQuestionOrder = item.AssessmentSettings.RandomizeQuestionOrder;
            model.RandomizeAnswerOrder = item.AssessmentSettings.RandomizeAnswerOrder;
            model.AllowViewHints = item.AssessmentSettings.AllowViewHints;
            model.HintSubstractPercentage = item.AssessmentSettings.PercentSubstractHint;

            model.ShowScoreAfter = (ReviewSetting)item.AssessmentSettings.ShowScoreAfter;
            model.ShowQuestionsAnswers = (ReviewSetting)item.AssessmentSettings.ShowQuestionsAnswers;
            model.ShowRightWrong = (ReviewSetting)item.AssessmentSettings.ShowRightWrong;
            model.ShowAnswers = (ReviewSetting)item.AssessmentSettings.ShowAnswers;
            model.ShowFeedbackAndRemarks = (ReviewSetting)item.AssessmentSettings.ShowFeedbackAndRemarks;
            model.ShowSolutions = (ReviewSetting)item.AssessmentSettings.ShowSolutions;
            model.AssessmentId = item.Id;
            model.GradeRule = (Models.GradeRule)item.AssessmentSettings.GradeRule;

            model.StudentsCanEmailInstructors = item.AssessmentSettings.StudentsCanEmailInstructors;
        }
    }
}
