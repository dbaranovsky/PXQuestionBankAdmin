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
            var sortingField = string.Format("{0}{1}/{2}", ElStrings.ProductCourseSection, currentCourseId,
                sortCriterion.ColumnName);
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

        public Question CreateQuestion(string courseId, Question question)
        {
            throw new System.NotImplementedException();
        }

        public Question GetQuestion(string repositoryCourseId, string questionId)
        {
            return Mapper.Map<Question>(GetAgilixQuestion(repositoryCourseId, questionId));
        }

        private Bfw.Agilix.DataContracts.Question GetAgilixQuestion(string repositoryCourseId,
            string questionId)
        {
            return GetAgilixQuestions(repositoryCourseId, new List<string>() {questionId}).FirstOrDefault();
        }

        private IEnumerable<Bfw.Agilix.DataContracts.Question> GetAgilixQuestions(string repositoryCourseId,
            IEnumerable<string> questionIds)
        {
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

        private bool UpdateQuestionSequence(string productCourseId, string courseId, string questionId, int newSequenceValue)
        {
            throw new System.NotImplementedException();
        }

        public Question UpdateQuestion(Question question)
        {
            var agilixQuestion = GetAgilixQuestion(question.EntityId, question.Id);
            Mapper.Map(question, agilixQuestion);
            var cmd = new PutQuestions();
            cmd.Add(Mapper.Map<Bfw.Agilix.DataContracts.Question>(agilixQuestion));
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return question;
        }

        public bool UpdateQuestionField(string productCourseId, string repositoryCourseId, string questionId, string fieldName, string fieldValue)
        {
            if (fieldName.Equals(MetadataFieldNames.Sequence))
            {
                return UpdateQuestionSequence(productCourseId, repositoryCourseId, questionId, int.Parse(fieldValue));
            }
            var question = GetQuestion(repositoryCourseId, questionId);
            if (question != null)
            {
                var productCourseSection = question.ProductCourseSections.FirstOrDefault(s => s.ProductCourseId == productCourseId);
                if (productCourseSection != null)
                {
                    if (productCourseSection.ProductCourseValues != null && productCourseSection.ProductCourseValues.ContainsKey(fieldName))
                    {
                        productCourseSection.ProductCourseValues[fieldName] = new List<string>() { fieldValue };
                    }
                    UpdateQuestion(question);
                }
            }
            return true;
        }

        public bool UpdateSharedQuestionField(string repositoryCourseId, string questionId, string fieldName, string fieldValue)
        {
            var question = GetQuestion(repositoryCourseId, questionId);
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
                            var fieldQuery = string.Join(" OR ",
                                filterFieldDescriptor.Values.Select(v =>
                                        string.Format("{0}/{1}:\"{2}\"", productCourseSection, filterFieldDescriptor.Field, v)));
                           
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

        //public void GetQuestions()
        //{
        //    var questionId = "02E31F06ADB849895CFE3E26F2420258";
        //    var entityid = "39768";
        //    var cmd = new GetQuestions()
        //    {
        //        SearchParameters = new QuestionSearch()
        //        {
        //            EntityId = entityid,
        //            QuestionIds = new List<string>() { questionId }
        //        }
        //    };

        //    businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
        //    var x = cmd.Questions;

        //    var cmd1 = new PutQuestions();
        //    cmd1.Add(x);
        //    businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd1);
        //}
    //    private readonly IContext businessContext;

    //    /// <summary>
    //    /// Limitation of the Search command
    //    /// </summary>
    //    private const int SearchCommandMaxRows = 25;

    //    private readonly Dictionary<string, string> availableQuestionTypes;

    //    private const string QueryParameters = "(dlap_class:question)";


    //    public QuestionCommands(IContext businessContext)
    //    {
    //        this.businessContext = businessContext;
    //        this.availableQuestionTypes = ConfigurationHelper.GetQuestionTypes();
    //    }

    //    public void SaveQuestions(IList<Question> questions)
    //    {
    //        var cmd = new PutQuestions();
    //        cmd.Add(Mapper.Map<IList<Bfw.Agilix.DataContracts.Question>>(questions));
    //        businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
    //    }

    //    /// <summary>
    //    /// Get question for specify query
    //    /// </summary>
    //    /// <param name="query">search query</param>
    //    /// <param name="page">current page</param>
    //    /// <param name="questionPerPage">question per page</param>
    //    /// <returns>questions</returns>
    //    public PagedCollection<Question> GetQuestionList(string courseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
    //    {
    //        var searchCommand = new Search()
    //        {
    //            SearchParameters = new SolrSearchParameters()
    //            {
    //                EntityId = ConfigurationHelper.GetDisciplineCourseId(),
    //                Query = QueryParameters,
    //                Rows = questionPerPage,
    //                Start = ((page - 1) * questionPerPage),
    //            }
    //        };

    //        var resultDocs = ExecuteSearchCommand(searchCommand);

    //        return PareseDocuments(resultDocs);
    //    }


    //    private IEnumerable<XDocument> ExecuteSearchCommand(Search searchCommand)
    //    {
    //        var resultsDocs = new List<XDocument>();

    //        if (searchCommand.SearchParameters.Rows > SearchCommandMaxRows)
    //        {
    //            ExecuteSearchCommandWithServerSidePaging(searchCommand, resultsDocs);
    //        }
    //        else
    //        {
    //            ExecuteAndAppendResult(searchCommand, resultsDocs);
    //        }

    //        return resultsDocs;
    //    }


    //    private void ExecuteSearchCommandWithServerSidePaging(Search searchCommand, List<XDocument> resultsDocs)
    //    {
    //        int numRows = searchCommand.SearchParameters.Rows;
    //        searchCommand.SearchParameters.Rows = SearchCommandMaxRows;

    //        while (numRows > 0)
    //        {
    //            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(searchCommand);
    //            resultsDocs.Add(searchCommand.SearchResults);

    //            searchCommand.SearchParameters.Start += SearchCommandMaxRows;
    //            numRows -= SearchCommandMaxRows;
    //        }
    //    }


    //    private void ExecuteAndAppendResult(Search searchCommand, List<XDocument> resultsDocs)
    //    {
    //        businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(searchCommand);
    //        resultsDocs.Add(searchCommand.SearchResults);
    //    }


    //    private QuestionList PareseDocuments(IEnumerable<XDocument> documents)
    //    {
    //        var questionList = new QuestionList();

    //        foreach (var xDocument in documents)
    //        {
    //            SerachCommandXmlParserHelper.PareseResultXDocument(xDocument, questionList, availableQuestionTypes);
    //        }

    //        return questionList;
    //    }
    }
}
