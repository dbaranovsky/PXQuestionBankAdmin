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
            var searchParameters = new QuestionSearch
                                   {
                                       //Hardcode
                                       EntityId = "34724"
                                   };
            var cmd = new GetQuestions
                      {
                          SearchParameters = searchParameters
                      };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            return cmd.Questions;
        }
    }
}
