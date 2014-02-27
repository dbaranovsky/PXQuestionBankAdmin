using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;


namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public class QuizHelper: IQuizHelper
    {
        public int DefaultHtmlQuizPoints { get { return 100; } }

        public List<BizDC.Question> UpdateQuizFromQuizQuestions(QuizQuestions quizQuestions, IContentActions contentActions, BizSC.IQuestionActions questionActions, BizSC.IBusinessContext context)
        {
            if (quizQuestions.Questions.IsNullOrEmpty())
            {
                var emptyQuestion = new List<BizDC.Question>();;
                //If quizQuestions.Questions is empty, remove all questions from the quiz.
                questionActions.UpdateQuestionList(context.EntityId, quizQuestions.QuizId, emptyQuestion, true, quizQuestions.MainQuizId);

                return emptyQuestion;
            }
            var courseQuestions = CopyQuestionsToCourse(quizQuestions.Questions, questionActions, context);

            //iterate through all banks
            for (int index = 0; index < quizQuestions.Questions.Count; index++)
            {
                if (quizQuestions.Questions[index].IsBank)
                {
                    var bankQuestion = quizQuestions.Questions[index];
                    var bank =
                        contentActions.GetContent(context.EntityId, bankQuestion.QuestionId)
                                      .ToQuiz(contentActions, questionActions, false);

                    if (bank != null)
                    {
                        // If this is a question bank that is just being added to this quiz,
                        // then we really want to save a copy of it.  (And make sure it does not
                        // exist in the TOC any more).
                        if (bankQuestion.IsNew)
                        {
                            bank.Id = context.NewItemId();
                            bank.ParentId = quizQuestions.QuizId;
                            bank.Categories = new List<TocCategory>();
                            bank.DefaultCategoryParentId = "PX_QUESTION_BANKS";
                            courseQuestions[index].Id = bank.Id;
                            contentActions.StoreContent(bank.ToContentItem(context.EntityId));

                            var bankQuestions = bank.Questions.Map(q => q.ToQuestion()).ToList();
                            questionActions.UpdateQuestionList(context.EntityId, bank.Id, bankQuestions, true);
                        }

                        var courseQuestion = new BizDC.Question()
                        {
                            Id = bank.Id,
                            EntityId = context.EntityId,
                            BankUse = bankQuestion.UseCount,
                            //if we dont have questions from the client, use question count from the server
                            BankCount = bankQuestion.BankQuestions.IsNullOrEmpty() ? bank.Questions.Count : bankQuestion.BankQuestions.Count,
                            InteractionType = BizDC.InteractionType.Bank,
                            Body = bank.Title
                            //TODO: points
                        };

                        if (index < courseQuestions.Count)
                        {
                            courseQuestions.Insert(index, courseQuestion);
                        }
                        else
                        {
                            courseQuestions.Add(courseQuestion);
                        }
                        //update child bank questions for banks that are open (NOTE: THIS CAN BE SLOW)
                        if (bankQuestion.BankQuestions.Count > 0)
                        {

                            var bankQuestions = CopyQuestionsToCourse(bankQuestion.BankQuestions, questionActions, context);
                            courseQuestion.Questions = bankQuestions; //add the questions to the model so they get rendered to the client
                            questionActions.UpdateQuestionList(context.EntityId, bank.Id, bankQuestions, true, quizQuestions.MainQuizId);
                        }
                        else if (bankQuestion.IsEmpty)
                        {
                            questionActions.UpdateQuestionList(context.EntityId, bank.Id, new List<BizDC.Question>(), true, quizQuestions.MainQuizId);
                        }
                    }
                }
            }

            questionActions.UpdateQuestionList(context.EntityId, quizQuestions.QuizId, courseQuestions, true, quizQuestions.MainQuizId);
            return courseQuestions;
        }

        private List<BizDC.Question> CopyQuestionsToCourse(List<QuizQuestion> quizQuestions, IQuestionActions questionActions, IBusinessContext context)
        {
            var courseQuestions = questionActions.GetQuestions(context.EntityId,
                                                              quizQuestions.Select(q => q.QuestionId)
                                                                           .Where(id => !id.IsNullOrEmpty())
                ).ToList();

            var questionsToBeCopied = quizQuestions.Where(question => courseQuestions.All(q => q.Id != question.QuestionId)
                                                                      && !question.EntityId.IsNullOrEmpty());
            //find questions that don't exist in the course and that have an entity id


            //Call to copy all the questions which are still not copied to current entityid.
            if (!questionsToBeCopied.IsNullOrEmpty())
            {
                var questionEntityString = String.Join(",", questionsToBeCopied.Map(q => q.QuestionId + "|" + q.EntityId));
                //copy the questions
                CopyQuestions(new QuizQuestions()
                {
                    QuestionIds = questionEntityString
                }, questionActions, context);
                //now update the questions because the questions are copied. 
                //make sure to update only the questions which are just copied.
                var newquestions = questionActions.GetQuestions(context.EntityId,
                                                               questionsToBeCopied.Select(qloc => qloc.QuestionId));
                //insert copies in correct order
                foreach (var newquestion in newquestions)
                {
                    var index = quizQuestions.FindIndex(q => q.QuestionId == newquestion.Id);
                    if (index < courseQuestions.Count)
                    {
                        courseQuestions.Insert(index, newquestion);
                    }
                    else
                    {
                        courseQuestions.Add(newquestion);
                    }
                }

            }
            return courseQuestions;
        }
        private void CopyQuestions(QuizQuestions quizQuestions, IQuestionActions questionActions, IBusinessContext context)
        {
            if (!(quizQuestions == null || quizQuestions.QuestionIds.IsNullOrEmpty()))
            {
                var questionLocs = (String.IsNullOrEmpty(quizQuestions.QuestionIds) ? new string[0] : quizQuestions.QuestionIds.Split(',')).Map(text => new QuestionLoc(text));
                var entityMap = new Dictionary<string, List<QuestionLoc>>();
                foreach (var questionLoc in questionLocs)
                {
                    // every question needs to have an entityid, use the current course id if one is not specified
                    if (questionLoc.EntityId == "null")
                        questionLoc.EntityId = context.CourseId;

                    if (!(entityMap.ContainsKey(questionLoc.EntityId)))
                    {
                        entityMap.Add(questionLoc.EntityId, new List<QuestionLoc>());
                    }
                    entityMap[questionLoc.EntityId].Add(questionLoc);

                }
                var copiedQuestions = new List<BizDC.Question>();
                foreach (var entityId in entityMap.Keys)
                {
                    var questionIds = entityMap[entityId].Map(ql => ql.QuestionId);
                    copiedQuestions.AddRange(questionActions.GetQuestions(entityId, questionIds));
                }
                foreach (var question in copiedQuestions)
                {
                    //default setting of points to 1 (PLATX:-6449)
                    if (question.Points == 0)
                    {
                        question.Points = 1;
                    }
                    if (question.SearchableMetaData == null)
                    {
                        question.SearchableMetaData = new Dictionary<string, string>();
                    }

                    if (question.EntityId != context.EntityId)
                    {
                        question.EntityId = context.EntityId;
                        if (question.SearchableMetaData.Keys.Contains(BizDC.QuestionMetaDataFields.PublisherSupplied.GetDescription()))
                        {
                            question.SearchableMetaData[BizDC.QuestionMetaDataFields.PublisherSupplied.GetDescription()] = "true";
                        }
                        else
                        {
                            question.SearchableMetaData.Add(BizDC.QuestionMetaDataFields.PublisherSupplied.GetDescription(), "true");
                        }

                    }
                    if (question.SearchableMetaData.Keys.Contains(BizDC.QuestionMetaDataFields.TotalUsed.GetDescription()))
                    {
                        question.SearchableMetaData[BizDC.QuestionMetaDataFields.TotalUsed.GetDescription()]
                            = (Convert.ToInt32(question.SearchableMetaData[BizDC.QuestionMetaDataFields.TotalUsed.GetDescription()]) + 1).ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        question.SearchableMetaData.Add(BizDC.QuestionMetaDataFields.TotalUsed.GetDescription(), "1");
                    }
                }
                //only store if the question belongs to another entity 
                // Else we don't need to store again as it is already there.
                if (!(copiedQuestions.IsNullOrEmpty()))
                {
                    questionActions.StoreQuestions(copiedQuestions);
                }
            }
        }
        /// <summary>
        /// Used to convert from a string like "questionId|entityId" to an object with properties set to these things.
        /// </summary>
        private class QuestionLoc
        {
            public string QuestionId { get; set; }
            public string EntityId { get; set; }

            public QuestionLoc(string text)
            {
                var parts = text.Split('|');
                if (parts.Length > 0)
                    QuestionId = parts[0];
                if (parts.Length > 1)
                    EntityId = parts[1];
            }
        }

        public List<BizDC.Question> UpdateQuizFromQuizItem(Quiz item, IContentActions contentActions, IQuestionActions questionActions, IBusinessContext context)
        {
            var quizId = item.Id;
            var quizQuestions = new QuizQuestions()
            {
                QuizId = quizId,
                MainQuizId = quizId
            };
            quizQuestions.Questions = item.Questions.Map(q => new QuizQuestion()
            {
                EntityId = q.EntityId,
                QuestionId = q.Id,
                IsBank = q.Type == "BANK",
                MainQuizId = quizId,
                QuizId = quizId,
                UseCount = q.BankUse
            }).ToList();
            return this.UpdateQuizFromQuizQuestions(quizQuestions, contentActions, questionActions, context);
        }

        /// <summary>
        /// Returns true if it could successfully make the quiz gradable (parent = PX_MANIFEST, IsAssignable = true, Points > 0)
        /// </summary>
        /// <param name="itemId">If of the item to make gradable</param>
        /// <param name="contentActions"></param>
        /// <param name="context"></param>
        /// <returns>True if the item was made gradable. False otherwise</returns>
        public bool MakeQuizGradable(string itemId, IContentActions contentActions, IBusinessContext context)
        {
            var item = contentActions.GetContent(context.EntityId, itemId);
            if (item == null || item.AssignmentSettings == null)
            {
                return false;
            }

            item.AssignmentSettings.IsAssignable = true;
            item.ParentId = contentActions.GradableParentId;

            //Don't set the weight if it has a value other than 0 (don't want to override something explicitely set
            if (item.AssignmentSettings.Points == 0)
            {
                var defaultPoints = item.DefaultPoints == 0 ? DefaultHtmlQuizPoints : item.DefaultPoints;
                item.AssignmentSettings.Points = defaultPoints;
            }

            try
            {
                contentActions.StoreContent(item);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
