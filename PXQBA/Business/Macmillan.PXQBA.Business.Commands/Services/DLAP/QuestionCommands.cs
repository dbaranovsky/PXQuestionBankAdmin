﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.DataContracts;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Commands.Services.DLAP
{
    public class QuestionCommands : IQuestionCommands
    {
        private readonly IContext businessContext;

        public QuestionCommands(IContext businessContext)
        {
            this.businessContext = businessContext;
        }

        private const int SearchCommandMaxRows = 25;

        /// <summary>
        /// Get question for specify query
        /// </summary>
        /// <returns>questions</returns>
        public PagedCollection<Question> GetQuestionList(string questionRepositoryCourseId, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            var searchResults = GetSearchResults(questionRepositoryCourseId, currentCourseId, filter, sortCriterion);
            searchResults = SortSearchResults(searchResults, sortCriterion);

            var questions = GetAgilixQuestions(questionRepositoryCourseId, searchResults.Skip(startingRecordNumber).Take(recordCount).Select(r => r.QuestionId));
            var result = new PagedCollection<Question>
            {
                TotalItems = searchResults.Count(),
                CollectionPage = Mapper.Map<IEnumerable<Question>>(questions)
            };
            return result;
        }

        private IEnumerable<QuestionSearchResult> GetSearchResults(string questionRepositoryCourseId, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion)
        {
            var results = new List<XElement>();
            IEnumerable<XElement> docElements = new List<XElement>();
            var i = 0;
            var query = BuildQueryString(filter);
            var sortingField = sortCriterion.ColumnName == ElStrings.QuestionStatus
                ? sortCriterion.ColumnName
                : string.Format("{0}{1}/{2}", ElStrings.ProductCourseSection, currentCourseId, sortCriterion.ColumnName);
            do
            {
                var searchCommand = new Search()
                                    {
                                        SearchParameters = new SolrSearchParameters()
                                                           {
                                                               Fields = sortingField,
                                                               EntityId = questionRepositoryCourseId,
                                                               Query = query,
                                                               Rows = SearchCommandMaxRows,
                                                               Start = (i*SearchCommandMaxRows),
                                                           }
                                    };
                i++;
                
                businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(searchCommand);
                if (searchCommand.SearchResults.Element("results") != null)
                {
                    if (searchCommand.SearchResults.Element("results").Element("result") != null)
                    {
                        docElements = searchCommand.SearchResults.Element("results").Element("result").Elements("doc");
                    }
                }
                results.AddRange(docElements);
            } while (docElements.Count() == SearchCommandMaxRows);

            var searchResults = results.Select(doc => QuestionDataXmlParser.ToSearchResultEntity(doc, sortingField));
            return searchResults;
        }

        private IEnumerable<QuestionSearchResult> SortSearchResults(IEnumerable<QuestionSearchResult> searchResults, SortCriterion sortCriterion)
        {
            if (sortCriterion != null && sortCriterion.SortType != SortType.None)
            {
                return sortCriterion.IsAsc
                    ? searchResults.OrderBy(r => r.SortingField)
                    : searchResults.OrderByDescending(r => r.SortingField);
            }
            return searchResults;
        }

        public Question CreateQuestion(Question question)
        {
            ExecutePutQuestion(Mapper.Map<Bfw.Agilix.DataContracts.Question>(question));
            return question;
        }

        public Question GetQuestion(string repositoryCourseId, string questionId)
        {
            return Mapper.Map<Question>(GetAgilixQuestion(repositoryCourseId, questionId));
        }

        public Dictionary<string, int> GetQuestionCountByChapters(string questionRepositoryCourseId, string currentCourseId)
        {
            var sortCriterion = new SortCriterion()
                                          {
                                              ColumnName = MetadataFieldNames.Chapter,
                                              SortType = SortType.Asc
                                          };
            var filterFieldDescriptor = new FilterFieldDescriptor()
                                        {
                                            Field = MetadataFieldNames.ProductCourse,
                                            Values = new[] {currentCourseId}
                                        };
            var searchResults = GetSearchResults(questionRepositoryCourseId,
                                                 currentCourseId,
                                                 new List<FilterFieldDescriptor>() { filterFieldDescriptor },
                                                 sortCriterion);

            return searchResults.GroupBy(x => x.SortingField).ToDictionary(groupItem => groupItem.Key, groupItem => groupItem.Count());
        }

        private Bfw.Agilix.DataContracts.Question GetAgilixQuestion(string repositoryCourseId,
            string questionId)
        {
            return GetAgilixQuestions(repositoryCourseId, new List<string>() {questionId}).FirstOrDefault();
        }

        private IEnumerable<Bfw.Agilix.DataContracts.Question> GetAgilixQuestions(string repositoryCourseId,
            IEnumerable<string> questionIds)
        {
            if (!questionIds.Any())
            {
                return new List<Bfw.Agilix.DataContracts.Question>();
            }
            var cmd = new GetQuestions()
            {
                SearchParameters = new QuestionSearch()
                {
                    EntityId = repositoryCourseId,
                    QuestionIds = questionIds
                }
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.Questions;
        }

        public string GetQuizIdForQuestion(string questionId, string entityId)
        {
            var getItem = new GetItems()
            {
                SearchParameters = new ItemSearch
                {
                    EntityId = entityId,
                    Query = string.Format("/Questions/question@id='{0}'", questionId)
                }
            };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(getItem);
            return getItem.Items.Any() ? getItem.Items.First().Id : string.Empty;
        }

        private bool UpdateQuestionSequence(string productCourseId, string repositoryCourseId, string questionId, int newSequenceValue)
        {
           throw new NotImplementedException();
        }

        public Question UpdateQuestion(Question question)
        {
            var agilixQuestion = GetAgilixQuestion(question.EntityId, question.Id);
            Mapper.Map(question, agilixQuestion);
            ExecutePutQuestion(agilixQuestion);
            return question;
        }

        public bool UpdateQuestionField(string productCourseId, string repositoryCourseId, string questionId, string fieldName, string fieldValue)
        {
            if (fieldName.Equals(MetadataFieldNames.Sequence))
            {
                return UpdateQuestionSequence(productCourseId, repositoryCourseId, questionId, int.Parse(fieldValue));
            }
            if (fieldName.Equals(MetadataFieldNames.QuestionStatus))
            {
                return UpdateQuestionStatus(repositoryCourseId, questionId, fieldValue);
            }
            var question = GetQuestion(repositoryCourseId, questionId);
            if (question != null)
            {
                var productCourseSection = question.ProductCourseSections.FirstOrDefault(s => s.ProductCourseId == productCourseId);
                if (productCourseSection != null)
                {
                    if (productCourseSection.ProductCourseValues != null)
                    {
                        if (productCourseSection.ProductCourseValues.ContainsKey(fieldName))
                        {
                            productCourseSection.ProductCourseValues[fieldName] = new List<string>() {fieldValue};
                        }
                        else
                        {
                            productCourseSection.ProductCourseValues.Add(fieldName, new List<string>() { fieldValue });
                        }
                    }
                    UpdateQuestion(question);
                }
            }
            return true;
        }

        public bool BulklUpdateQuestionField(string productCourseId, string repositoryCourseId, string[] questionId, string fieldName, string fieldValue)
        {
            if (fieldName.Equals(MetadataFieldNames.QuestionStatus))
            {
                return UpdateQuestionsStatuses(repositoryCourseId, questionId, fieldValue);
            }

            return true;
        }



       
        public bool UpdateSharedQuestionField(string repositoryCourseId, string questionId, string fieldName, string fieldValue)
        {
            var temporaryRepositoryCourseId = ConfigurationHelper.GetTemporaryCourseId();
            var question = GetQuestion(temporaryRepositoryCourseId, questionId);
            if (question != null)
            {
                if (question.DefaultValues != null && question.DefaultValues.ContainsKey(fieldName))
                {
                    question.DefaultValues[fieldName] = new List<string>(){fieldValue};
                }
                UpdateQuestion(question);
            }
            return true;
        }

        private string BuildQueryString(IEnumerable<FilterFieldDescriptor> filter)
        {
            var query = new StringBuilder("(dlap_class:question)");
            if (filter != null)
            {
                var productCourseFilterField = filter.First(field => field.Field == MetadataFieldNames.ProductCourse);
                if (productCourseFilterField != null)
                {
                    var productCourseId = productCourseFilterField.Values.First();
                    if (productCourseId != null)
                    {
                        var productCourseSection = string.Format("{0}{1}", ElStrings.ProductCourseSection, productCourseId);
                        foreach (var filterFieldDescriptor in filter)
                        {
                            var fieldFormat = "{0}/{1}:\"{2}\"";
                            if (filterFieldDescriptor.Field == ElStrings.QuestionStatus ||
                                filterFieldDescriptor.Field == "dlap_q_" + ElStrings.type)
                            {
                                fieldFormat = "{1}:\"{2}\"";
                            }
                            var fieldQuery = string.Join(" OR ",
                                filterFieldDescriptor.Values.Select(v =>
                                    string.Format(fieldFormat, productCourseSection, filterFieldDescriptor.Field, v)));
                            if (!string.IsNullOrEmpty(fieldQuery))
                            {
                                query.Append(string.Format(" AND ({0})", fieldQuery));
                            }
                        }
                    }
                }
            }
            return query.ToString();
        }

        public bool RemoveFromTitle(string[] questionsId, string questionRepositoryCourseId, string currentCourseId)
        {
            var agilixQuestions = GetAgilixQuestions(questionRepositoryCourseId, questionsId);

            foreach (var question in agilixQuestions)
            {
                question.MetadataElements.Remove(ElStrings.ProductCourseSection + questionRepositoryCourseId);
            }

            ExecutePutQuestions(agilixQuestions);

            return true;
        }

        public bool PublishToTitle(string[] questionsId, int courseId, string bank, string chapter)
        {
            //TODO implement in real data
            return true;
        }


        public bool SetQuestionsStatus(string[] questionId, string status)
        {
            //TODO implement in real data
            return true;
        }


        private bool UpdateQuestionStatus(string repositoryCourseId, string questionId, string newValue)
        {
            return UpdateQuestionsStatuses(repositoryCourseId, new List<string> { questionId }, newValue);
        }


        private bool UpdateQuestionsStatuses(string repositoryCourseId, IEnumerable<string> questionId, string newValue)
        {

            var questions = GetAgilixQuestions(repositoryCourseId, questionId);
            if (questions == null)
            {
                return false;
            }

           // var newStatus = ((int)((QuestionStatus)EnumHelper.GetItemByDescription(typeof(QuestionStatus), newValue))).ToString();

            foreach (var question in questions)
            {
                question.QuestionStatus = newValue;
            }

            ExecutePutQuestions(questions);

            return true;

        }


        private void ExecutePutQuestion(Bfw.Agilix.DataContracts.Question  question)
        {
            ExecutePutQuestions(new List<Bfw.Agilix.DataContracts.Question> {question});
        }

        private void ExecutePutQuestions(IEnumerable<Bfw.Agilix.DataContracts.Question> questions)
        {
            var cmd = new PutQuestions();
            cmd.Add(questions);
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
        }

    }
}
