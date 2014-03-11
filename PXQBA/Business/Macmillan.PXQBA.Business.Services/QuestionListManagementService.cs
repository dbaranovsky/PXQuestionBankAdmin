using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Macmillan.PXQBA.Business.Contracts;
using Bfw.Agilix.DataContracts;

namespace Macmillan.PXQBA.Business.Services
{
    /// <summary>
    /// Represents the service that handles operations with question lists
    /// </summary>
    public class QuestionListManagementService : IQuestionListManagementService
    {
        private readonly IContext businessContext;

        public QuestionListManagementService(IContext businessContext)
        {
            this.businessContext = businessContext;
        }

        /// <summary>
        /// Retrieves questions list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Question> GetQuestionList()
        {
            var quizSearch = new ItemSearch()
                {
                    EntityId = "71836",
                    Query = "/bfw_subtype='QUIZ' AND (/parent='LOR_statsportal__bps6e__master_Chapter__3')"
                };
            var batch = new Batch();
            var getItemsCommand = new GetItems()
            {
                SearchParameters = quizSearch
            };
            batch.Add(getItemsCommand);
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(getItemsCommand);
            var items = getItemsCommand.Items;
   
            var searchParameters = new QuestionSearch
                                   {
                                       //Hardcode
                                       EntityId = "71836"
                                   };
            var getQuestionsCommand = new GetQuestions
                      {
                          SearchParameters = searchParameters
                      };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(getQuestionsCommand);

            return getQuestionsCommand.Questions;
        }
    }
}
