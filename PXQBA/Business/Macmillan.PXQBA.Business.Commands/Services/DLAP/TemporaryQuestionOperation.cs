using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Common.Helpers;


namespace Macmillan.PXQBA.Business.Commands.Services.DLAP
{
    public class TemporaryQuestionOperation : ITemporaryQuestionOperation
    {
        private readonly IContext businessContext;

        private IQuestionCommands questionCommands;

        private const string ItemIdTemplate = "QBA_temp_quiz_{0}";

        private const string QuestionIdTemplate = "QBA_temp_question_{0}";

        private const string ItemIdVersionTemplate = "QBA_temp_ver_quiz_{0}";

        private const string QuestionIdVersionTemplate = "QBA_temp_ver_question_{0}";

        private delegate string GetTemporaryQuizIdDelegate();

        private delegate string GetTemporaryQuestionIdDelegate();

        private GetTemporaryQuestionIdDelegate GetTemporaryQuestionId;

        private GetTemporaryQuizIdDelegate GetTemporaryQuizId;

        private readonly string temporaryCourseId = ConfigurationHelper.GetTemporaryCourseId();

        public TemporaryQuestionOperation(IContext businessContext, IQuestionCommands questionCommands)
        {
            this.businessContext = businessContext;
            this.questionCommands = questionCommands;
            GetTemporaryQuestionId = GetTemporaryQuestionIdForQuestion;
            GetTemporaryQuizId = GetTemporaryQuizIdForQuestion;
        }

        public Models.Question CreateQuestion(string productCourseId, Models.Question question)
        {
            question.ModifiedBy = businessContext.CurrentUser.Id;
            questionCommands.SetSequence(productCourseId, question);
            var agilixQuestion = Mapper.Map<Question>(question);
            var questionCreated = CopyQuestionToCourse(temporaryCourseId, GetTemporaryQuestionId(), agilixQuestion);
            var result = UpdateQuestionQuiz(questionCreated);
            result.Id = Guid.NewGuid().ToString();
            return result;
        }

        public Models.Question CopyQuestionToTemporaryCourse(string sourceProductCourseId, string questionIdToCopy, string version = null)
        {
            if (!string.IsNullOrEmpty(version))
            {
                GetTemporaryQuestionId = GetTemporaryQuestionIdForVersion;
                GetTemporaryQuizId = GetTemporaryQuizIdForVersion;
            }

            var questionToCopy = CopyQuestionToCourse(sourceProductCourseId, questionIdToCopy, temporaryCourseId, GetTemporaryQuestionId(), version);
            CopyResources(sourceProductCourseId, temporaryCourseId, QuestionHelper.GetQuestionRelatedResources(questionToCopy.QuestionXml));

            return UpdateQuestionQuiz(questionToCopy);
        }

        private Models.Question UpdateQuestionQuiz(Question questionToCopy)
        {
            AddQuestionToTemporaryQuiz(questionToCopy);
            var question = Mapper.Map<Models.Question>(questionToCopy);
            question.QuizId = GetTemporaryQuizId();
            return question;
        }

        public Models.Question CopyQuestionToSourceCourse(string sourceProductCourseId, string sourceQuestionId)
        {
            var question = CopyQuestionToCourse(temporaryCourseId, GetTemporaryQuestionId(), sourceProductCourseId, sourceQuestionId);
            CopyResources(temporaryCourseId, sourceProductCourseId,  QuestionHelper.GetQuestionRelatedResources(question.QuestionXml));
            //questionCommands.DeleteQuestion(temporaryCourseId, GetTemporaryQuestionId());
            return Mapper.Map<Models.Question>(question);
        }

        private void CopyResources(string from, string to, IEnumerable<string> resourcesPath)
        {
            if (!resourcesPath.Any())
            {
                return;
            }
            foreach (var copyResoursecCmd in resourcesPath.Select(resourcePath => new CopyResources
                                                                                  {
                                                                                      DestEntityId = to,
                                                                                      SourceEntityId = @from,
                                                                                      SourcePath = ConfigurationHelper.GetBrainhoneyCourseImageFolder() + resourcePath
                                                                                  }))
            {
                businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(copyResoursecCmd);
            }
        }

        public void RemoveResources(string itemId, List<string> questionRelatedResources)
        {
            if (!questionRelatedResources.Any())
            {
                return;
            }

            var cmd = new DeleteResources
                      {
                          ResourcesToDelete =
                              questionRelatedResources.Select(relatedResource => new Resource
                                                                                 {
                                                                                     EntityId = itemId,
                                                                                     Url = ConfigurationHelper.GetBrainhoneyCourseImageFolder() + 
                                                                                            relatedResource
                                                                                 }).ToList()
                      };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
        }

        private Question CopyQuestionToCourse(string sourceProductCourseId, string sourceQuestionId, string destinationProductCourseId, string destinationQuestionId,string version = null)
        {
            var questionToCopy = questionCommands.GetAgilixQuestion(sourceProductCourseId, sourceQuestionId, version);
            if (questionToCopy == null)
            {
                return questionCommands.GetAgilixQuestion(destinationProductCourseId, destinationQuestionId);
            }
            return CopyQuestionToCourse(destinationProductCourseId, destinationQuestionId, questionToCopy);
        }

        private Question CopyQuestionToCourse(string destinationProductCourseId, string destinationQuestionId, Question questionToCopy)
        {
            questionToCopy.EntityId = destinationProductCourseId;
            questionToCopy.Id = destinationQuestionId;
            questionCommands.ExecutePutQuestion(questionToCopy);
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
            HtmlXmlHelper.SwitchAttributeName(item.Root, "actualentityid", "entityid", temporaryCourseId);
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


        private XDocument GetExistingTemporaryQuiz()
        {
            var getItem = new GetItems()
            {
                SearchParameters = new ItemSearch
                {
                    EntityId = temporaryCourseId,
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
                EntityId = temporaryCourseId,
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
                EntityId = temporaryCourseId,
                Id = GetTemporaryQuizId(),
                Type = DlapItemType.Assessment
            };
        }

        private string GetTemporaryQuizIdForQuestion()
        {
            return String.Format(ItemIdTemplate, businessContext.CurrentUser.Id);
        }

        private string GetTemporaryQuestionIdForQuestion()
        {
            return String.Format(QuestionIdTemplate, businessContext.CurrentUser.Id);
        }

        private string GetTemporaryQuestionIdForVersion()
        {
            return String.Format(QuestionIdVersionTemplate, businessContext.CurrentUser.Id);
        }

        private string GetTemporaryQuizIdForVersion()
        {
            return String.Format(ItemIdVersionTemplate, businessContext.CurrentUser.Id);
        }
    }
}