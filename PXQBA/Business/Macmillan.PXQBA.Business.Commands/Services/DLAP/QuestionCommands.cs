using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
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

        private readonly IProductCourseOperation productCourseOperation;

        public QuestionCommands(IContext businessContext, IProductCourseOperation productCourseOperation)
        {
            this.businessContext = businessContext;
            this.productCourseOperation = productCourseOperation;
        }

        private const int SearchCommandMaxRows = 25;

        /// <summary>
        /// Get question for specify query
        /// </summary>
        /// <returns>questions</returns>
        public PagedCollection<Question> GetQuestionList(string questionRepositoryCourseId, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            var filterCopy = MakeFilterCopy(filter);
            if (string.IsNullOrEmpty(sortCriterion.ColumnName))
            {
                sortCriterion.ColumnName = MetadataFieldNames.Sequence;
                sortCriterion.SortType = SortType.Asc;
            }
            var questionsSortedBySequence = GetSortedAndFilteredBySequenceSolrResults(questionRepositoryCourseId, currentCourseId, filterCopy);
            var searchResults = GetSortedAndFilteredSolrResults(questionRepositoryCourseId, currentCourseId, filterCopy, sortCriterion);

            var questions = PreparedQuestionPage(questionRepositoryCourseId, searchResults, startingRecordNumber, recordCount);
            SetCorrectSequenceDisplayValue(currentCourseId, questions, questionsSortedBySequence);
            var result = new PagedCollection<Question>
            {
                TotalItems = searchResults.Count(r => string.IsNullOrEmpty(r.DraftFrom)),
                CollectionPage = questions
            };
            return result;
        }

        private IEnumerable<Question> PreparedQuestionPage(string questionRepositoryCourseId, IEnumerable<QuestionSearchResult> searchResults, int startingRecordNumber, int recordCount)
        {
            var nonDraftResults = searchResults.Where(r => string.IsNullOrEmpty(r.DraftFrom)).Skip(startingRecordNumber).Take(recordCount);
            var questions = CreateChildren(questionRepositoryCourseId, searchResults, nonDraftResults);
            return questions;
        }

        private IEnumerable<Question> CreateChildren(string questionRepositoryCourseId, IEnumerable<QuestionSearchResult> searchResults, IEnumerable<QuestionSearchResult> parents)
        {
            var nonDraftQuestions = new List<Question>();

            foreach (var questionSearchResult in parents) 
            {
                var parentAgilix = GetAgilixQuestion(questionRepositoryCourseId, questionSearchResult.QuestionId);
                if (parentAgilix != null)
                {
                    var parent = Mapper.Map<Question>(parentAgilix);
                    nonDraftQuestions.Add(parent);

                    var drafts = searchResults.Where(r => r.DraftFrom == questionSearchResult.QuestionId);
                    if (drafts.Any())
                    {
                        nonDraftQuestions.AddRange(
                            CreateChildren(questionRepositoryCourseId, searchResults, drafts)
                                .OrderBy(q => q.ModifiedDate));
                    }
                }
            }
            return nonDraftQuestions;
        }

        private IEnumerable<FilterFieldDescriptor> MakeFilterCopy(IEnumerable<FilterFieldDescriptor> filter)
        {
            var seqFilter = filter.FirstOrDefault(item => item.Field == MetadataFieldNames.Sequence);
            if (seqFilter != null && seqFilter.Values.Any())
            {
                var filterCopy = filter.Select(filterFieldDescriptor => filterFieldDescriptor.Clone()).ToList();
                return filterCopy;
            }
            return filter;
        }

        private IEnumerable<QuestionSearchResult> GetSortedAndFilteredBySequenceSolrResults(string questionRepositoryCourseId, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter)
        {
            var sorted = GetSolrResultsSortedBySequence(questionRepositoryCourseId, currentCourseId).ToList();
            SetIndexToSearchResults(sorted);
            var seqFilter = filter.FirstOrDefault(item => item.Field == MetadataFieldNames.Sequence);
            if (seqFilter != null && seqFilter.Values.Any())
            {
                sorted.RemoveAll(s => !seqFilter.Values.Contains(s.Index));
                seqFilter.Values = sorted.Select(s => s.SortingField);
            }
            return sorted;
        }

        private void SetIndexToSearchResults(IList<QuestionSearchResult> sorted)
        {
            for (int i = 0; i < sorted.Count(); i ++)
            {
                sorted[i].Index = (i + 1).ToString();
            }
        }

        private void SetCorrectSequenceDisplayValue(string productCourseId, IEnumerable<Question> questions, IEnumerable<QuestionSearchResult> questionsSortedBySequence)
        {
            foreach (var question in questions)
            {
                var section = question.ProductCourseSections.FirstOrDefault(s => s.ProductCourseId == productCourseId);
                if (section != null)
                {
                    var sequenceDisplayValue = string.Empty;
                    var first = questionsSortedBySequence.FirstOrDefault(q => q.QuestionId == question.Id);
                    if (first != null)
                    {
                        decimal seq;
                        if (decimal.TryParse(first.SortingField, out seq))
                        {
                            sequenceDisplayValue = first.Index;
                        }
                    }
                    section.Sequence = sequenceDisplayValue;
                }
            }
        }

        private IEnumerable<QuestionSearchResult> GetSortedAndFilteredSolrResults(string questionRepositoryCourseId, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion)
        {
            var searchResults = GetSearchResults(questionRepositoryCourseId, currentCourseId, filter, sortCriterion);
            return SortSearchResults(searchResults, sortCriterion);
        }

        private IEnumerable<QuestionSearchResult> GetSolrResultsSortedBySequence(string questionRepositoryCourseId, string currentCourseId)
        {
             var filter = new List<FilterFieldDescriptor>
                         {
                             new FilterFieldDescriptor
                             {
                                 Field = MetadataFieldNames.ProductCourse,
                                 Values =
                                     new List<string>
                                     {
                                         currentCourseId
                                     }
                             }
                         };
            var sortCriterion = new SortCriterion
                               {
                                   ColumnName = MetadataFieldNames.Sequence,
                                   SortType = SortType.Asc
                               };
            return GetSortedAndFilteredSolrResults(questionRepositoryCourseId, currentCourseId, filter, sortCriterion);
        }

        private IEnumerable<QuestionSearchResult> GetSearchResults(string questionRepositoryCourseId, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion)
        {
            var query = BuildQueryString(filter);
            var sortingField = sortCriterion.ColumnName == ElStrings.QuestionStatus || sortCriterion.ColumnName == MetadataFieldNames.DlapType
                ? sortCriterion.ColumnName
                : string.Format("{0}{1}/{2}", ElStrings.ProductCourseSection, currentCourseId, sortCriterion.ColumnName);
            return PerformSearch(questionRepositoryCourseId, query, sortingField);
        }

        private IEnumerable<QuestionSearchResult> PerformSearch(string questionRepositoryCourseId, string query, string sortingField)
        {
            var results = new List<XElement>();
            IEnumerable<XElement> docElements = new List<XElement>();
            var i = 0;

            do
            {
                var searchCommand = new Search()
                {
                    SearchParameters = new SolrSearchParameters()
                    {
                        Fields = string.Format("{0}|{1}", MetadataFieldNames.DraftFrom, sortingField),
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
              //while (i <= 1);
            

            var searchResults = results.Select(doc => QuestionDataXmlParser.ToSearchResultEntity(doc, sortingField));
            return searchResults;
        }

        private IEnumerable<QuestionSearchResult> SortSearchResults(IEnumerable<QuestionSearchResult> searchResults, SortCriterion sortCriterion)
        {
            if (sortCriterion != null && sortCriterion.SortType != SortType.None)
            {
                if (sortCriterion.ColumnName == MetadataFieldNames.Sequence)
                {
                    decimal seq;
                    var questionsWithNonDecimalSequence = searchResults.Where(r => !decimal.TryParse(r.SortingField, out seq)).ToList();
                    if (sortCriterion.IsAsc)
                    {
                        var sorted = searchResults.Where(r => decimal.TryParse(r.SortingField, out seq))
                            .OrderBy(r => decimal.Parse(r.SortingField))
                            .ToList();
                        sorted.AddRange(questionsWithNonDecimalSequence);
                        return sorted;
                    }
                    questionsWithNonDecimalSequence.AddRange(searchResults.Where(r => decimal.TryParse(r.SortingField, out seq)).OrderByDescending(r => decimal.Parse(r.SortingField)));
                    return questionsWithNonDecimalSequence;
                }
                if (sortCriterion.ColumnName == MetadataFieldNames.DlapType)
                {
                    return sortCriterion.IsAsc
                        ? searchResults.OrderBy(r => QuestionTypeHelper.GetDisplayName(r.SortingField))
                        : searchResults.OrderByDescending(r => QuestionTypeHelper.GetDisplayName(r.SortingField));
                }
                return sortCriterion.IsAsc
                    ? searchResults.OrderBy(r => r.SortingField)
                    : searchResults.OrderByDescending(r => r.SortingField);
            }
            return searchResults;
        }

        public Question CreateQuestion(string productCourseId, Question question)
        {
            question.ModifiedBy = businessContext.CurrentUser.Id;
            CheckIfSequenceIsSet(productCourseId, question);
            ExecutePutQuestion(Mapper.Map<Bfw.Agilix.DataContracts.Question>(question));
            ExecuteSolrUpdateTask();
            return question;
        }

        public void ExecuteSolrUpdateTask()
        {
            var taskId = ConfigurationHelper.GetSolrUpdateTaskId();
            if (taskId.HasValue)
            {
                var cmd = new RunTask();
                cmd.SearchParameters = new TaskSearch()
                                       {
                                           TaskId = taskId.Value.ToString()
                                       };
                businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            }
        }

        private void CheckIfSequenceIsSet(string productCourseId, Question question)
        {
            var section = question.ProductCourseSections.First(s => s.ProductCourseId == productCourseId);
            if (!string.IsNullOrEmpty(section.Sequence))
            {
                section.Sequence = GetNewSequenceValue(question.EntityId, productCourseId);
            }
        }

        private string GetNewSequenceValue(string repositoryCourseId, string courseId)
        {
            decimal seq;
            var questionsWithDecimalSequence = GetSolrResultsSortedBySequence(repositoryCourseId, courseId).Where(q => decimal.TryParse(q.SortingField, out seq));
            return QuestionSequenceHelper.GetNewLastValue(questionsWithDecimalSequence);
        }

        public Question GetQuestion(string repositoryCourseId, string questionId, string version = null)
        {
            return Mapper.Map<Question>(GetAgilixQuestion(repositoryCourseId, questionId, version));
        }

        public IEnumerable<Question> GetQuestions(string repositoryCourseId, string[] questionsId)
        {
            return Mapper.Map<IEnumerable<Question>>(GetAgilixQuestions(repositoryCourseId, questionsId));
        }

        public Dictionary<string, int> GetQuestionCountByChapters(string questionRepositoryCourseId, string currentCourseId, IEnumerable<string> chapterNames)
        {
            var sortCriterion = new SortCriterion()
                                          {
                                              ColumnName = MetadataFieldNames.Chapter,
                                              SortType = SortType.Asc
                                          };
            var courseFilterFieldDescriptor = new FilterFieldDescriptor()
                                        {
                                            Field = MetadataFieldNames.ProductCourse,
                                            Values = new[] {currentCourseId}
                                        };
            var chaptersFilterFieldDescriptor = new FilterFieldDescriptor()
            {
                Field = MetadataFieldNames.Chapter,
                Values = chapterNames
            };
            var searchResults = GetSearchResults(questionRepositoryCourseId,
                                                 currentCourseId,
                                                 new List<FilterFieldDescriptor>() { courseFilterFieldDescriptor, chaptersFilterFieldDescriptor },
                                                 sortCriterion);

            return searchResults.GroupBy(x => x.SortingField).ToDictionary(groupItem => groupItem.Key, groupItem => groupItem.Count());
        }

        public Bfw.Agilix.DataContracts.Question GetAgilixQuestion(string repositoryCourseId,
            string questionId, string version = null)
        {
            if (string.IsNullOrEmpty(version))
            {
                return GetAgilixQuestions(repositoryCourseId, new List<string>() { questionId }).FirstOrDefault();
            }
            return GetSpecificVersion(repositoryCourseId, questionId, version);
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
            var questions = GetSolrResultsSortedBySequence(repositoryCourseId, productCourseId).ToList();
            var updated = QuestionSequenceHelper.UpdateSequence(questions.ToList(), questionId, newSequenceValue);
            foreach (var questionSearchResult in updated)
            {
                UpdateQuestionFieldForce(productCourseId, repositoryCourseId, questionSearchResult.QuestionId, MetadataFieldNames.Sequence, questionSearchResult.SortingField);                
            }
            return true;
        }

        public Question UpdateQuestion(Question question)
        {
            question.ModifiedBy = businessContext.CurrentUser.Id;
            var agilixQuestion = GetAgilixQuestion(question.EntityId, question.Id);
            Mapper.Map(question, agilixQuestion);
            ExecutePutQuestion(agilixQuestion);
            ExecuteSolrUpdateTask();
            return question;
        }

        public Question UpdateQuestionInTempQuiz(Question question)
        {
            question.ModifiedBy = businessContext.CurrentUser.Id;
            var agilixQuestion = GetAgilixQuestion(question.EntityId, question.Id);
            Mapper.Map(question, agilixQuestion);
            ExecutePutQuestion(agilixQuestion);
            return question;
        }

        public bool UpdateQuestions(IEnumerable<Question> questions, string repositoryCourseId)
        {
            var agilixQuestions = GetAgilixQuestions(repositoryCourseId, questions.Select(q => q.Id));
            Mapper.Map(questions, agilixQuestions);
            ExecutePutQuestions(agilixQuestions);
            ExecuteSolrUpdateTask();
            return true;
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
            UpdateQuestionFieldForce(productCourseId, repositoryCourseId, questionId, fieldName, fieldValue);
            return true;
        }

      

        private void UpdateStaticField(QuestionMetadataSection section, string fieldName, string fieldValue)
        {
            if (MetadataFieldNames.DlapTitle == fieldName)
            {
                section.Title = fieldValue;
            }
            else if (MetadataFieldNames.Chapter == fieldName)
            {
                section.Chapter = fieldValue;
            }
            else if (MetadataFieldNames.Bank == fieldName)
            {
                section.Bank = fieldValue;
            }
            else if (MetadataFieldNames.Sequence == fieldName)
            {
                section.Sequence = fieldValue;
            }
        }     
        public bool BulklUpdateQuestionField(string productCourseId, string repositoryCourseId, string[] questionId, string fieldName, string fieldValue)
        {
            if (fieldName.Equals(MetadataFieldNames.QuestionStatus))
            {
                return UpdateQuestionsStatuses(repositoryCourseId, questionId, fieldValue);
            }

            return true;
        }
       
        public bool UpdateSharedQuestionField(string repositoryCourseId, string questionId, string fieldName, IEnumerable<string> fieldValues)
        {
            var temporaryRepositoryCourseId = ConfigurationHelper.GetTemporaryCourseId();
            var question = GetQuestion(temporaryRepositoryCourseId, questionId);
            if (question != null)
            {
                if (MetadataFieldNames.GetStaticFieldNames().Contains(fieldName))
                {
                    UpdateSharedStaticField(question, fieldName, fieldValues.FirstOrDefault());
                    
                }
                else if (question.DefaultSection != null && question.DefaultSection.DynamicValues.ContainsKey(fieldName))
                {
                    var initialDefaultValues = question.DefaultSection.DynamicValues[fieldName];
                    foreach (var section in question.ProductCourseSections.Where(s => !s.DynamicValues.ContainsKey(fieldName) || AreEqualValues(s.DynamicValues[fieldName].ToList(), initialDefaultValues)))
                    {
                        var course = productCourseOperation.GetProductCourse(section.ProductCourseId);
                        var fieldDescriptor = course.FieldDescriptors.FirstOrDefault(f => f.Name == fieldName);
                        if (fieldDescriptor != null)
                        {
                            var intersectionValues = fieldDescriptor.CourseMetadataFieldValues.Any() ? fieldValues.Intersect(fieldDescriptor.CourseMetadataFieldValues.Select(v => v.Text)) : fieldValues;
                            section.DynamicValues[fieldName] = intersectionValues.ToList();
                        }
                    }

                    initialDefaultValues = fieldValues.ToList();
                }
                UpdateQuestion(question);
            }
            return true;
        }

        public bool RemoveFromTitle(string[] questionsId, string questionRepositoryCourseId, string currentCourseId)
        {
            var agilixQuestions = GetAgilixQuestions(questionRepositoryCourseId, questionsId);

            foreach (var question in agilixQuestions)
            {
                question.MetadataElements.Remove(ElStrings.ProductCourseSection + currentCourseId);
            }

            ExecutePutQuestions(agilixQuestions);
            ExecuteSolrUpdateTask();
            return true;
        }


        public IEnumerable<Question> GetVersionHistory(string questionRepositoryCourseId, string questionId)
        {
            var versions = Mapper.Map<IEnumerable<Question>>(GetAgilixQuestionsAsAdmin(questionRepositoryCourseId, new List<string>() { questionId }, true));
            return versions.ToList().OrderByDescending(v => v.Version);
        }

        public void DeleteQuestion(string repositoryCourseId, string questionId)
        {
            var questionToDelete = new XElement("question",
                   new XAttribute("entityid", repositoryCourseId),
                   new XAttribute("questionid", questionId)
               );

            var deleteCmd = new DeleteQuestions()
            {
                Questions = new List<XElement>()
                    {
                        new XElement(questionToDelete)
                    }
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(deleteCmd);
        }

        public IEnumerable<Question> GetQuestionDrafts(string questionRepositoryCourseId, Question question)
        {
            var query = string.Format("(dlap_class:question) AND (draftfrom:{0})", question.Id);
            var results = PerformSearch(questionRepositoryCourseId, query, string.Empty);
            return GetQuestions(questionRepositoryCourseId, results.Select(r => r.QuestionId).ToArray());
        }


        private Bfw.Agilix.DataContracts.Question GetSpecificVersion(string repositoryCourseId, string questionId, string version)
        {
            return
                GetAgilixQuestionsAsAdmin(repositoryCourseId, new List<string> { questionId }, true)
                    .FirstOrDefault(x => x.QuestionVersion == version);
        }

        private IEnumerable<Bfw.Agilix.DataContracts.Question> GetAgilixQuestions(string repositoryCourseId,
            IEnumerable<string> questionIds)
        {
            if (!questionIds.Any(q => !string.IsNullOrEmpty(q)))
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

        private void UpdateSharedStaticField(Question question, string fieldName, string fieldValue)
        {
            var sections = new List<QuestionMetadataSection>();
            if (MetadataFieldNames.DlapTitle == fieldName)
            {
                sections = question.ProductCourseSections.Where(s => s.Title == question.DefaultSection.Title).ToList();
            }
            else if (MetadataFieldNames.Chapter == fieldName)
            {
                sections = question.ProductCourseSections.Where(s => s.Title == question.DefaultSection.Chapter).ToList();
            }
            else if (MetadataFieldNames.Bank == fieldName)
            {
                sections = question.ProductCourseSections.Where(s => s.Title == question.DefaultSection.Bank).ToList();
            }
            foreach (var section in sections)
            {
                var course = productCourseOperation.GetProductCourse(section.ProductCourseId);
                var fieldDescriptor = course.FieldDescriptors.FirstOrDefault(f => f.Name == fieldName);
                if (fieldDescriptor != null)
                {
                    if (!fieldDescriptor.CourseMetadataFieldValues.Any() || fieldDescriptor.CourseMetadataFieldValues.Select(v => v.Text).Contains(fieldValue))
                    {
                        UpdateStaticField(section, fieldName, fieldValue);
                    }
                }
            }
            UpdateStaticField(question.DefaultSection, fieldName, fieldValue);
        }

        private bool AreEqualValues(IList<string> values, IList<string> comparedValues)
        {
            if (values.Count != comparedValues.Count)
            {
                return false;
            }

            return !values.Where((t, i) => t != comparedValues[i]).Any();
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
                            var values = filterFieldDescriptor.Values;
                            var fieldFormat = "{0}/{1}:\"{2}\"";
                            var fieldQuery = string.Empty;
                            if (filterFieldDescriptor.Field == ElStrings.QuestionStatus ||
                                filterFieldDescriptor.Field == MetadataFieldNames.DlapType)
                            {
                                fieldFormat = "{1}:\"{2}\"";
                            }
                            if (filterFieldDescriptor.Field == MetadataFieldNames.DlapType)
                            {
                                values = filterFieldDescriptor.Values.Select(v => v == QuestionTypeHelper.GraphType || v == QuestionTypeHelper.HTSType ? "custom": v);
                            }
                            if (filterFieldDescriptor.Field == MetadataFieldNames.Flag)
                            {
                                var flagQuery = new StringBuilder();
                                if (filterFieldDescriptor.Values.Contains(((int) QuestionFlag.Flagged).ToString()))
                                {
                                    flagQuery.Append(string.Format(fieldFormat, productCourseSection, filterFieldDescriptor.Field, ((int) QuestionFlag.Flagged)));
                                }
                                if (filterFieldDescriptor.Values.Contains(((int) QuestionFlag.NotFlagged).ToString()))
                                {
                                    if (!string.IsNullOrEmpty(flagQuery.ToString()))
                                    {
                                        flagQuery.Clear();
                                    }
                                    else
                                    {
                                        flagQuery.Append(" NOT (");
                                        flagQuery.Append(string.Format(fieldFormat, productCourseSection, filterFieldDescriptor.Field, ((int)QuestionFlag.Flagged)));
                                        flagQuery.Append(")");
                                    }
                                }
                                fieldQuery = flagQuery.ToString();
                            }
                            else
                            {
                                fieldQuery = string.Join(" OR ",
                                    values.Select(v =>
                                        string.Format(fieldFormat, productCourseSection, filterFieldDescriptor.Field, v)));
                            }
                            if (!string.IsNullOrEmpty(fieldQuery))
                            {
                                if (filterFieldDescriptor.Field == MetadataFieldNames.Flag)
                                {
                                    query.Append(string.Format(" AND {0}", fieldQuery));
                                }
                                else
                                {
                                    query.Append(string.Format(" AND ({0})", fieldQuery));
                                }
                            }
                        }
                    }
                }
            }
            return query.ToString();
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

            ExecuteSolrUpdateTask();

            return true;
        }


        private void ExecutePutQuestion(Bfw.Agilix.DataContracts.Question  question)
        {
            ExecutePutQuestions(new List<Bfw.Agilix.DataContracts.Question> {question});
        }

        private void ExecutePutQuestions(IEnumerable<Bfw.Agilix.DataContracts.Question> questions)
        {
            if (questions.Any())
            {
                var cmd = new PutQuestions();
                cmd.Add(questions);
                businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            }
        }


        private IEnumerable<Bfw.Agilix.DataContracts.Question> GetAgilixQuestionsAsAdmin(string repositoryCourseId,
            IEnumerable<string> questionIds, bool allVersions = false)
        {
            if (!questionIds.Any(q => !string.IsNullOrEmpty(q)))
            {
                return new List<Bfw.Agilix.DataContracts.Question>();
            }
            var cmd = new GetQuestionsAdmin()
            {
                SearchParameters = new QuestionAdminSearch()
                {
                    EntityId = repositoryCourseId,
                    QuestionIds = questionIds,
                    version = allVersions
                }
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.Questions;
        }

        private void UpdateQuestionFieldForce(string productCourseId, string repositoryCourseId, string questionId, string fieldName,
          string fieldValue)
        {
            var question = GetQuestion(repositoryCourseId, questionId);
            if (question != null)
            {
                var productCourseSection = question.ProductCourseSections.FirstOrDefault(s => s.ProductCourseId == productCourseId);
                if (productCourseSection != null)
                {
                    if (MetadataFieldNames.GetStaticFieldNames().Contains(fieldName))
                    {
                        UpdateStaticField(productCourseSection, fieldName, fieldValue);
                    }
                    else if (productCourseSection.DynamicValues != null)
                    {
                        if (productCourseSection.DynamicValues.ContainsKey(fieldName))
                        {
                            productCourseSection.DynamicValues[fieldName] = new List<string>() { fieldValue };
                        }
                        else
                        {
                            productCourseSection.DynamicValues.Add(fieldName, new List<string>() { fieldValue });
                        }
                    }
                    UpdateQuestion(question);
                }
            }
        }

    }
}
