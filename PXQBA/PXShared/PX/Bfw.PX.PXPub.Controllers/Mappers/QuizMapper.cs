using System;
using System.Linq;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
	public static class QuizMapper
	{

		/// <summary>
		/// Maps a Quiz to a generic ContentItem.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="courseId">The course id.</param>
		/// <returns></returns>
		public static BizDC.ContentItem ToContentItem(this Quiz model, string courseId)
		{
			var biz = model.ToBaseContentItem(courseId);

			if (null != model)
			{
				biz.Type = model.QuizType.ToString();
				biz.Description = model.Description;
				// this is dumb. it's a hack around a specific agilix component thing where the content
				// will be skipped if it doesn't have an HREF
				biz.Href = "Templates/Data/dcef7cf5ba194495a7c86145b557bc15/index.html";
			}

			return biz;
		}

		/// <summary>
		/// Provides a conversion to the Quiz type.
		/// </summary>
		/// <param name="biz">The biz.</param>no 
		/// <param name="content">The content.</param>
        /// <param name="questionActions">The content.</param>
		/// <param name="isLoadQuestions">The is load questions.</param>
		/// <returns></returns>
        public static Quiz ToQuiz(this BizDC.ContentItem biz, BizSC.IContentActions content, BizSC.IQuestionActions questionActions, bool? isLoadQuestions, bool isLoadRelatedContent = false, bool loadQuestionInPool = false)
		{
			if (biz == null) return null;
			var model = new Quiz();
			model.ToBaseQuiz(biz, content, questionActions, isLoadQuestions, isLoadRelatedContent, loadQuestionInPool);
			return model;
		}

        public static void ToBaseQuiz(this Quiz quiz, BizDC.ContentItem biz, BizSC.IContentActions content, BizSC.IQuestionActions questionActions, bool? isLoadQuestions, bool isLoadRelatedContent = false, bool loadQuestionInPool = false)
		{
			var isLoadChildren = isLoadQuestions.HasValue && isLoadQuestions.Value;

			quiz.Id = biz.Id;
			quiz.MaxPoints = biz.MaxPoints;
			quiz.Title = biz.Title;
			quiz.ParentId = biz.ParentId;
			quiz.Url = biz.Href;
			quiz.Sequence = biz.Sequence;
			quiz.QuizType = (QuizType)Enum.Parse(typeof(QuizType), biz.Type);
			// TODO: There will be a better way to do this, need to follow up with Sam Hathaway about it.
			quiz.IsLC = biz.CustomExamType == "LearningCurve";
			quiz.Description = biz.Description;
			quiz.DefaultPoints = biz.DefaultPoints;
			var context = content != null && content.Context != null ? content.Context : ServiceLocator.Current.GetInstance<BizSC.IBusinessContext>();
			if (context.Course != null) 
                quiz.CourseInfo = context.Course.ToCourse();

			// Set the policy text according to the assessment settings
            quiz.PolicyDescription = AssignmentHelper.PolicyDescriptionFromSettings(context, biz.AssignmentSettings, biz.AssessmentSettings, quiz.GetQuizTypeName(quiz.QuizType));

		    if (isLoadChildren && !string.IsNullOrEmpty(context.EntityId))
		    {
                questionActions = questionActions ?? ServiceLocator.Current.GetInstance<BizSC.IQuestionActions>();
                var questions =
                    questionActions.GetQuestions(context.EntityId, biz, true, context.EntityId, null, null).Questions;
		        quiz.Questions = questions.Map(q =>
		            { return !isLoadRelatedContent ? q.ToQuestion(content, loadQuestionInPool) : q.ToQuestion(); })
		                                  .ToList();

                if (null != biz.AssessmentGroups)
                {
                    foreach (Question q in quiz.Questions)
                    {
                        foreach (var grp in biz.AssessmentGroups)
                        {
                            var agQuestionId = grp.Name.Split('_')[1];
                            if (q.Id == agQuestionId)
                            {
                                q.Attempts = grp.Attempts;
                                q.TimeLimit = grp.TimeLimit;
                                q.Score = grp.SubmissionGradeAction.ToString();
                                q.Scrambled = grp.Scrambled;
                                q.Hints = grp.Hints;
                                q.Review = grp.Review.ToString();
                                if (null != grp.ReviewSettings)
                                {
                                    q.ReviewSettings = new ReviewSettings
                                    {
                                        ShowScoreAfter = (ReviewSetting)grp.ReviewSettings.ShowScoreAfter,
                                        ShowQuestionsAnswers = (ReviewSetting)grp.ReviewSettings.ShowQuestionsAnswers,
                                        ShowAnswers = (ReviewSetting)grp.ReviewSettings.ShowAnswers,
                                        ShowRightWrong = (ReviewSetting)grp.ReviewSettings.ShowRightWrong,
                                        ShowFeedbackAndRemarks = (ReviewSetting)grp.ReviewSettings.ShowFeedbackAndRemarks,
                                        ShowSolutions = (ReviewSetting)grp.ReviewSettings.ShowSolutions
                                    };
                                }

                                break;
                            }
                        }
                    }
                }
		    }
		    else
		    {
		        quiz.Questions = biz.QuizQuestions.Map(q => new Question()
		            {
                        Id=q.QuestionId,
                        BankUse = !q.Count.IsNullOrEmpty() ? int.Parse(q.Count) : 0,
                        EntityId =  q.EntityId,
                        Type = q.Type.IsNullOrEmpty()? "1": q.Type,
                        Score = q.Score,
		            }).ToList();
		    }

			if (biz.AssessmentSettings != null)
			{
				quiz.AttemptLimit = biz.AssessmentSettings.AttemptLimit;
				quiz.AllowSaveAndContinue = biz.AssessmentSettings.AllowSaveAndContinue;
				quiz.TimeLimit = biz.AssessmentSettings.TimeLimit;
				quiz.SubmissionGradeAction = biz.AssessmentSettings.SubmissionGradeAction.ToString();
				quiz.DueDate = biz.AssessmentSettings.DueDate;
			    quiz.ShowReviewScreen = (biz.AssessmentSettings.ShowQuestionsAnswers == BizDC.ReviewSetting.Each);
			}
		    quiz.ShowGrade = true;
            if (!biz.Properties.IsNullOrEmpty() && biz.Properties.ContainsKey("bfw_IncludeGbbScoreTrigger"))
		    {
		        var includeGbbScoreTrigger = biz.Properties["bfw_IncludeGbbScoreTrigger"].Value.ToString();
                if (includeGbbScoreTrigger == "2" && quiz.DueDate > DateTime.Now)
		        {
		            quiz.ShowGrade = false;
		        }
		    }
		}
	}
}
