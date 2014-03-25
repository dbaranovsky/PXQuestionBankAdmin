using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Services
{
    /// <summary>
    /// Service that handles operations with question entities
    /// </summary>
    public class QuestionManagementService : IQuestionManagementService
    {
        private readonly IContext businessContext;

        public QuestionManagementService(IContext businessContext)
        {
            this.businessContext = businessContext;
        }

        /// <summary>
        /// Creates new question
        /// </summary>
        /// <param name="titleId">Title id</param>
        /// <param name="type">Type of question</param>
        /// <param name="bank">Question bank</param>
        /// <param name="chapter">Chapter</param>
        public void AddNewQuestion(string titleId, SelectedItem type, SelectedItem bank, SelectedItem chapter)
        {
            var metaData = new Dictionary<string, string>
            {
                {"createdBy", businessContext.CurrentUser.Id},
                {"userCreated", "true"},
                {"totalUsed", "1"},
                {"questionstatus", "0"}
            };
            //var question = new Models.Question()
            //{
            //    ItemId = bank.Id,
            //    EnrollmentId = businessContext.EnrollmentId,
            //    Id = Guid.NewGuid().ToString(),
            //    Type = Models.Question.QuestionTypeShortNameFromId(questionType),
            //    EntityId = mainEntityId,
            //    IsLast = true,
            //    Points = 1,
            //    IsNewQuestion = true,
            //    SearchableMetaData = metaData,
            //    Text = "",
            //    CustomUrl = ""
            //};

            //if (Models.Question.QuestionTypeShortNameFromId(questionType) == "HTS" ||
            //    Models.Question.QuestionTypeShortNameFromId(questionType) == "CUSTOM")
            //{
            //    question.Text = "Advanced Question";
            //    question.CustomUrl = "HTS";
            //    question.Type = "CUSTOM";
            //}

            //if (Models.Question.QuestionTypeShortNameFromId(questionType) == "FMA_GRAPH")
            //{
            //    question.Text = "Graphing exercise";
            //    question.CustomUrl = "FMA_GRAPH";
            //    question.Type = "CUSTOM";
            //}

            //QuestionActions.StoreQuestion(question.ToQuestion());

            //var quiz = this.UpdateQuizWithNewQuestion(quizId, question);

            //question.QuizType = quiz.QuizType;
            //question.Attempts = quiz.AttemptLimit.ToString();

            ////question.HtsPlayerUrl = Context.Domain.CustomQuestionUrls["HTS"];
            //if (Context.Domain.CustomQuestionUrls.ContainsKey("HTS"))
            //{
            //    //question.HtsPlayerUrl = "http://localhost:49676/pxPlayer.ashx";
            //    question.HtsPlayerUrl = businessContext.Domain.CustomQuestionUrls["HTS"];
            //}
        }


        private string GetDisciplineCourseId(string titleId)
        {
            string questionCourseId = string.Empty;
            var chapters = GetCourseChapters(titleId);
            var questionBanks = GetQuestionBanksForSelectedChapters(titleId, chapters.Select(item => item.Id));

            //foreach (ContentItem questionBank in questionBanks)
            //{
            //    foreach (QuizQuestion quizQuestion in questionBank.QuizQuestions)
            //    {
            //        if (quizQuestion != null && !string.IsNullOrEmpty(quizQuestion.EntityId) && quizQuestion.EntityId != entityId)
            //        {
            //            questionCourseId = quizQuestion.EntityId;
            //            break;
            //        }
            //    }

            //    if (!string.IsNullOrEmpty(questionCourseId))
            //    {
            //        break;
            //    }
            //}

            return questionCourseId;
        }

        private IEnumerable<ContentItem> GetCourseChapters(string entityId)
        {
            const string parentId = "PX_LOR";
            var SearchParameters = BuildListChildrenQuery(entityId, parentId, 1);
            var items = FindContentItems(SearchParameters);
            return items;
        }

        private ItemSearch BuildListChildrenQuery(string entityId, string parentId, int depth)
        {
            var search = new ItemSearch()
            {
                EntityId = entityId,
                ItemId = parentId,
                Depth = depth
            };
            search.Query = string.Format(@"/bfw_tocs/bfw_toc_contents@parentid='{0}'", parentId);
            return search;
        }

        private IEnumerable<ContentItem> FindContentItems(ItemSearch search)
        {
            var batch = new Batch();
            var cmd = new GetItems()
            {
                SearchParameters = search
            };
            batch.Add(cmd);
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            cmd = batch.CommandAs<GetItems>(0);

            var contentItems = new List<ContentItem>();

            if (cmd.Items != null)
            {
                contentItems = Mapper.Map<List<ContentItem>>(cmd.Items);
            }

            return contentItems;
        }

        /// <summary>
        /// GetQuestionBanksForSelectedChapters
        /// </summary>
        /// <param name="entityId"> </param>
        /// <param name="selectedChaptersList"> </param>
        /// "cmd=getitemlist&entityid=65131&query=/bfw_subtype='QUIZ' 
        /// AND (/parent='chapterId' OR /parent='chapterId')"
        /// <returns></returns>
        public List<ContentItem> GetQuestionBanksForSelectedChapters(string entityId, IEnumerable<string> selectedChaptersList)
        {
            string selectedChapters = selectedChaptersList.Select(ch => String.Format("/parent='{0}'", ch.Replace("ChapterSelectedValues=", ""))).Fold(" OR ");

            string query = string.Format("/bfw_subtype='QUIZ' AND ( {0} )", selectedChapters);

            List<ContentItem> itemsToReturn = null;

            var itemsSearchQuery = new ItemSearch()
            {
                EntityId = entityId,
                Query = query
            };

            var items = FindContentItems(itemsSearchQuery);

            if (items != null)
            {
                itemsToReturn = items.OrderBy(itm => itm.Title).Distinct().ToList();
            }

            return itemsToReturn;
        }
    }
}