using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Macmillan.PXQBA.Business.Contracts;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using QuestionVm = Macmillan.PXQBA.Business.Models.Question;


namespace Macmillan.PXQBA.Business.Services
{
    /// <summary>
    /// Represents the service that handles operations with question lists
    /// </summary>
    public class QuestionListManagementService : IQuestionListManagementService
    {
        private readonly IContext businessContext;

        /// <summary>
        /// Limitation of the Search command
        /// </summary>
        private const int SearchCommandMaxRows = 25;

        private readonly Dictionary<string, string> availableQuestionTypes;

        private const string EntityId = "6710";
        private const string QueryParameters = "(dlap_class:question)";
        
        public QuestionListManagementService(IContext businessContext)
        {
            this.businessContext = businessContext;
            this.availableQuestionTypes = ConfigurationHelper.GetQuestionTypes();
        }

        /// <summary>
        /// Get question for specify query
        /// </summary>
        /// <param name="query">search query</param>
        /// <param name="page">current page</param>
        /// <param name="questionPerPage">question per page</param>
        /// <returns>questions</returns>
        public QuestionList GetQuestionList(string query, int page, int questionPerPage)
        {
            var searchCommand = new Search()
            {
                SearchParameters = new SolrSearchParameters()
                {
                    EntityId = EntityId,
                    Query = QueryParameters,
                    Rows = questionPerPage,
                    Start = ((page - 1) * questionPerPage),
                }
            };

            var resultDocs = ExecuteSearchCommand(searchCommand);

            return PareseDocuments(resultDocs); 
        }


        private IEnumerable<XDocument> ExecuteSearchCommand(Search searchCommand)
        {
            var resultsDocs = new List<XDocument>();

            if (searchCommand.SearchParameters.Rows > SearchCommandMaxRows)
            {
                ExecuteSearchCommandWithServerSidePaging(searchCommand, resultsDocs);
            }
            else
            {
                ExecuteAndAppendResult(searchCommand, resultsDocs);
            }

            return resultsDocs;
        }


        private void ExecuteSearchCommandWithServerSidePaging(Search searchCommand, List<XDocument> resultsDocs)
        {
            int numRows = searchCommand.SearchParameters.Rows;
            searchCommand.SearchParameters.Rows = SearchCommandMaxRows;

            while (numRows > 0)
            {
                businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(searchCommand);
                resultsDocs.Add(searchCommand.SearchResults);

                searchCommand.SearchParameters.Start += SearchCommandMaxRows;
                numRows -= SearchCommandMaxRows;
            }
        }


        private void ExecuteAndAppendResult(Search searchCommand, List<XDocument> resultsDocs)
        {
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(searchCommand);
            resultsDocs.Add(searchCommand.SearchResults);
        }


        private QuestionList PareseDocuments(IEnumerable<XDocument> documents)
        {
            var questionList = new QuestionList();

            foreach (var xDocument in documents)
            {
                SerachCommandXmlParserHelper.PareseResultXDocument(xDocument, questionList, availableQuestionTypes);
            }

            return questionList;
        }
    }
}
