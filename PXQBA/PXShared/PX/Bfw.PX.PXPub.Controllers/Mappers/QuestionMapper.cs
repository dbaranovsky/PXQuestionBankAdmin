using System.Collections.Generic;
using System.Linq;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class QuestionMapper
    {

        /// <summary>
        /// Map a question model to a question business object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static BizDC.Question ToQuestion(this Question model)
        {
            var biz = new BizDC.Question
                          {
                              Id = model.Id,
                              EntityId = model.EntityId,
                              Title = model.Title,
                              Body = model.Text,
                              CustomUrl = model.CustomUrl,
                              InteractionType = QuizHelpers.InteractionTypeForDescription(model.Type),
                              Points = model.Points,
                              BankCount = model.BankCount,
                              InteractionData = model.InteractionData,
                              Interaction = TypeConversion.ConvertType<QuestionInteraction, BizDC.QuestionInteraction>(model.Interaction, null),
                              BankUse = model.BankUse,
                              AssignedQuizes = model.AssignedQuizes.Map(c => c.ToBaseContentItem(model.EntityId)).ToList(),
                              QuestionMetaData = model.QuestionMetaData,
                              AssessmentGroups = model.AssessmentGroups,
                              SearchableMetaData = model.SearchableMetaData,
                              QuestionStatus = model.QuestionStatus
                          };

            if (model.eBookChapter != null)
            {

                biz.eBookChapter = model.eBookChapter;

            }
            if (!model.Choices.IsNullOrEmpty())
            {
                biz.Choices = model.Choices.Map(c => c.ToQuestionChoice()).ToList();
            }

            if (!model.LearningCurveQuestionSettings.IsNullOrEmpty())
            {
                biz.LearningCurveQuestionSettings = model.LearningCurveQuestionSettings.Map(c => c.ToLearningCurveQuestionSettings()).ToList();
            }

            biz.AnswerList = model.AnswerList;
            return biz;
        }

        /// <summary>
        /// Map a question business object to a question model. It returns related content if existing.
        /// </summary>
        /// <param name="biz"></param>
        /// <param name="contentActions"></param>
        /// <returns></returns>
        public static Question ToQuestion(this BizDC.Question biz)
        {
            var question = new Question
                            {
                                Id = biz.Id,
                                Title = biz.Title,
                                GeneralFeedback = biz.GeneralFeedback,
                                EntityId = biz.EntityId,
                                Text = biz.Body,
                                CustomUrl = biz.CustomUrl,
                                Type = QuizHelpers.DescriptionForInteractionType(biz.InteractionType),
                                Points = biz.Points,
                                BankCount = biz.BankCount,
                                BankUse = biz.BankUse == -1 ? biz.BankCount : biz.BankUse,
                                InteractionData = biz.InteractionData,
                                Interaction = TypeConversion.ConvertType<BizDC.QuestionInteraction, QuestionInteraction>(biz.Interaction, null),
                                QuestionXml = biz.QuestionXml,
                                IsHts = biz.InteractionType == Bfw.PX.Biz.DataContracts.InteractionType.Custom && biz.CustomUrl == "HTS",
                                AssignedQuizes = biz.AssignedQuizes.Map(q => q.ToContentItem(null)).ToList(),   //Content actions should be null 
                                                                                                                //so we aren't unintentinally executing DLAP calls here
                                QuestionMetaData = biz.QuestionMetaData,
                                SearchableMetaData = biz.SearchableMetaData,
                                AssessmentGroups = biz.AssessmentGroups,
                                AdminFlag = biz.AdminFlag,
                                AssignedChapter = biz.AssignedChapter == null ? "":biz.AssignedChapter,
                                UsedIn = biz.UsedIn,
                                ExcerciseNo = biz.ExcerciseNo,
                                Difficulty = biz.Difficulty,
                                CongnitiveLevel = biz.CongnitiveLevel,
                                BloomsDomain = biz.BloomsDomain,
                                Guidance = biz.Guidance,
                                FreeResponseQuestion = biz.FreeResponseQuestion,
                                QuestionBank = biz.UsedIn,
                                QuestionStatus = biz.QuestionStatus,
                                Questions = biz.Questions.IsNullOrEmpty() ? null : biz.Questions.Map(q => q.ToQuestion()).ToList()
                            };

            question.EbookSectionText = biz.EbookSectionText;
            question.eBookChapter = biz.eBookChapter;
            question.QuestionBank = biz.QuestionBank;
            question.QuestionBankText = biz.QuestionBankText;
            

            if (biz.SelectedLearningObjectives != null)
            {
                question.SelectedLearningObjectives = biz.SelectedLearningObjectives;

            }
            if (biz.SelectedSuggestedUse != null)
            {
                question.SelectedSuggestedUse = biz.SelectedSuggestedUse;

            }

            if (question.eBookChapter == null)
            {

                question.eBookChapter = biz.eBookChapter;
            }
            if (!biz.Choices.IsNullOrEmpty())
            {
                question.Choices = biz.Choices.Map(c => c.ToQuestionChoice()).ToList();
            }

            if (biz.Analysis != null)
            {
                question.Analysis = biz.Analysis.ToQuestionAnalysis();
            }

            if (biz.LearningCurveQuestionSettings != null)
            {
                question.LearningCurveQuestionSettings = biz.LearningCurveQuestionSettings.Map(c => c.ToLearningCurveQuestionSettings()).ToList();
            }

            question.AnswerList = biz.AnswerList;
            
                //var ci = contentActions.GetContent(contentActions.Context.EntityId, "RELATED_CONTENT_" + biz.Id);
                //if (ci != null)
                //{
                //    question.RelatedContent = ci.ToRelatedContent(contentActions, true);
                //}
            
            return question;
        }

        /// <summary>
        /// Map a question business object to a question model.
        /// </summary>
        /// <param name="biz">The biz</param>
        /// <param name="loadQuestionInPool">If true, then also load all the question inside this question pool</param>
        /// <returns></returns>
        public static Question ToQuestion(this BizDC.Question biz, BizSC.IContentActions contentActions, bool loadQuestionInPool)
        {
            var question = biz.ToQuestion();

            if (loadQuestionInPool)
            {
                var quiz = contentActions.GetContent(contentActions.Context.EntityId, biz.Id).ToQuiz(contentActions, null, true, false, true);
                if (biz.InteractionType == BizDC.InteractionType.Bank && !quiz.Questions.IsNullOrEmpty())
                {
                    if (question.Questions.IsNullOrEmpty())
                    {
                        question.Questions = new List<Question>();
                    }
                    foreach (Question q in quiz.Questions)
                    {
                        question.Questions.Add(q);
                    }

                }
            }
            return question;
        }
    }
}
