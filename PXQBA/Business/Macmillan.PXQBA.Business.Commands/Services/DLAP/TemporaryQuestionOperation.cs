using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Common.Helpers;


namespace Macmillan.PXQBA.Business.Commands.Services.DLAP
{
    public class TemporaryQuestionOperation : ITemporaryQuestionOperation
    {
        private readonly IContext businessContext;

        private const string ItemIdTemplate = "QBA_temp_quiz_{0}";

        private const string QuestionIdTemplate = "QBA_temp_question_{0}";

        private const string TemporaryCourseId = "200117";

        public TemporaryQuestionOperation(IContext businessContext)
        {
            this.businessContext = businessContext;
        }

        public Models.Question CopyQuestionToTemporaryCourse(string sourceProductCourseId, string questionIdToCopy)
        {

            var questionToCopy = CopyQuestionToCourse(sourceProductCourseId, questionIdToCopy, TemporaryCourseId, GetTemporaryQuestionId());
            AddQuestionToTemporaryQuiz(questionToCopy);
            return Mapper.Map<Models.Question>(questionToCopy);
        }

        //TODO: move to QuestionCommands when it's live
        public string GetQuizIdForQuestion(string questionId, string entityId)
        {
            var getItem = new GetItems()
            {
                SearchParameters = new ItemSearch
                {
                    EntityId = entityId,
                    Query = string.Format("/Questions/question@id='{0}'",questionId)
                }
            };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(getItem);
            return getItem.Items.Any() ? getItem.Items.First().Id : string.Empty;
        }

        public Models.Question CopyQuestionToSourceCourse(string sourceProductCourseId, string sourceQuestionId)
        {
            var question = CopyQuestionToCourse(TemporaryCourseId, GetTemporaryQuestionId(), sourceProductCourseId, sourceQuestionId);
            return Mapper.Map<Models.Question>(question);
        }

        private Question CopyQuestionToCourse(string sourceProductCourseId, string sourceQuestionId, string destinationProductCourseId, string destinationQuestionId)
        {
            var questionToCopy = GetQuestion(sourceProductCourseId, sourceQuestionId);
            if (questionToCopy == null)
            {
                throw new NullReferenceException("There is no such question in the course");
            }
            questionToCopy.EntityId = destinationProductCourseId;
            questionToCopy.Id = destinationQuestionId;
            var cmd = new PutQuestions();
            cmd.Add(questionToCopy);
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return questionToCopy;
        }

        private void AddQuestionToTemporaryQuiz(Question questionToCopy)
        {
            var item = GetTemporaryQuiz();
            var data = item.Root.Element("data");
            var hrefElement = data.Element("href");
            
            if (hrefElement == null)
            {
                hrefElement = new XElement("href");
                data.Add(hrefElement);
            }
            hrefElement.Value = "Templates/Data/AHWDG/index.html";
            var questionsElement = data.Element("questions");
            if (questionsElement == null)
            {
                questionsElement = new XElement("questions");
                data.Add(questionsElement);
            }
            questionsElement.RemoveAll();
            var questionElement = new XElement("question");
            questionElement.Add(new XAttribute("entityid", questionToCopy.EntityId));
            questionElement.Add(new XAttribute("id", questionToCopy.Id));
            questionsElement.Add(questionElement);
            HtmlXmlHelper.SwitchAttributeName(item.Root, "id", "itemid", GetTemporaryQuizId());
            HtmlXmlHelper.SwitchAttributeName(item.Root, "actualentityid", "entityid", TemporaryCourseId);
            var cmd = new PutRawItem()
            {
                ItemDoc = item
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
        }

        private XDocument GetTemporaryQuiz()
        {
            var itemXml = GetExistingTemporaryQuiz();
            if (itemXml == null)
            {
                var item = CreateNewTemporaryQuiz();
                if (item == null)
                {
                    throw new Exception("Temporary quiz creation failed");
                }
                itemXml = GetTemporaryQuiz();
            }
            return itemXml;
        }

        private Question GetQuestion(string productCourseId, string questionId)
        {
            var cmd = new GetQuestions();
            cmd.SearchParameters = new QuestionSearch
            {
                EntityId = productCourseId,
                QuestionIds = new List<string> {questionId}
            };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.Questions.FirstOrDefault();
        }

        private XDocument GetExistingTemporaryQuiz()
        {
            var getItem = new GetItems()
            {
                SearchParameters = new ItemSearch
                {
                    EntityId = TemporaryCourseId,
                    ItemId = GetTemporaryQuizId()
                }
            };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(getItem);
            if (!getItem.Items.Any())
            {
                return null;
            }
            var cmd = new GetRawItem()
            {
                EntityId = TemporaryCourseId,
                ItemId = GetTemporaryQuizId()
            };
           
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.ItemDocument;
        }

        private Item CreateNewTemporaryQuiz()
        {
            var cmd = new PutItems();
            cmd.Add(CreateTemporaryItem());
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.Items.FirstOrDefault();
        }

        private Item CreateTemporaryItem()
        {
            return new Item
            {
                EntityId = TemporaryCourseId,
                Id = GetTemporaryQuizId(),
                Type = DlapItemType.Assessment
            };
        }

        private string GetTemporaryQuizId()
        {
            return String.Format(ItemIdTemplate, businessContext.CurrentUser.Id);
        }

        private string GetTemporaryQuestionId()
        {
            return String.Format(QuestionIdTemplate, businessContext.CurrentUser.Id);
        }
    }
}