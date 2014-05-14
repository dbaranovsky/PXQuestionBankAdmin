using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
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

        private const int SearchCommandMaxRows = 10;

        /// <summary>
        /// Get question for specify query
        /// </summary>
        /// <param name="query">search query</param>
        /// <param name="page">current page</param>
        /// <param name="questionPerPage">question per page</param>
        /// <returns>questions</returns>
        public PagedCollection<Question> GetQuestionList(string courseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            var i = 0;
            Search searchCommand;
            var query = BuildQueryString(filter);
            do
            {
                searchCommand = new Search()
                {
                    SearchParameters = new SolrSearchParameters()
                    {
                        EntityId = courseId,
                        Query = query,
                        Rows = SearchCommandMaxRows,
                        Start = (i * SearchCommandMaxRows),
                    }
                };
                i++;
                businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(searchCommand);
            } while (searchCommand.SearchResults.Element("results").Element("result").Elements("doc").Count() == SearchCommandMaxRows);
            return null;
        }

        public Question CreateQuestion(string courseId, Question question)
        {
            throw new System.NotImplementedException();
        }

        public Question GetQuestion(string questionId)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateQuestionSequence(string courseId, string questionId, int newSequenceValue)
        {
            throw new System.NotImplementedException();
        }

        public Question UpdateQuestion(Question question)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateQuestionField(string questionId, string fieldName, string value)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateSharedQuestionField(string questionId, string fieldName, string fieldValue)
        {
            throw new System.NotImplementedException();
        }

        private string BuildQueryString(IEnumerable<FilterFieldDescriptor> filter)
        {
            var query = new StringBuilder("dlap_class:question");
            if (filter != null)
            {
                var productCourseFilterField = filter.First(field => field.Field == MetadataFieldNames.ProductCourse);
                if (productCourseFilterField != null)
                {
                    var productCourseId = productCourseFilterField.Values.First();
                    if (productCourseId != null)
                    {
                        var productCourseSection = string.Format("product-course-id-{0}", productCourseId);
                        foreach (var filterFieldDescriptor in filter)
                        {
                            foreach (var value in filterFieldDescriptor.Values)
                            {
                                query.Append(string.Format(" AND {0}/{1}:{2}", productCourseSection,
                                    filterFieldDescriptor.Field, value));
                            }
                        }
                    }
                }
            }
            return query.ToString();
        }

        public void GetQuestions()
        {
            var questionId = "B8B35A1E8D1A4A70A2E622727A135D4A";
            var entityid = "71836";
            var cmd = new GetQuestions()
            {
                SearchParameters = new QuestionSearch()
                {
                    EntityId = entityid,
                    QuestionIds = new List<string>() { questionId }
                }
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            var x = cmd.Questions;

            var cmd1 = new PutQuestions();
            cmd1.Add(x);
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd1);
        }
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
