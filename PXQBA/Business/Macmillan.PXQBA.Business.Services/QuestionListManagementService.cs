using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Macmillan.PXQBA.Business.Commands.Contracts;
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
        private readonly IQuestionCommands questionCommands;
        
        public QuestionListManagementService(IQuestionCommands questionCommands)
        {
            this.questionCommands = questionCommands;
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
            return questionCommands.GetQuestionList(query, page, questionPerPage);
        }
    }
}
