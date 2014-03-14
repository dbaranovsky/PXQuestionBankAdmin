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
   
            //var searchParameters = new QuestionSearch
            //                       {
            //                           //Hardcode
            //                           EntityId = "71836"
            //                       };

            //Test question previews
            var searchParameters = new QuestionSearch
            {
                //Hardcode
                EntityId = "6710",
                QuestionIds = new List<string>()
                                                     {
                                                         "452EF88D20AB277580BA749FA74C0062",
                                                         "D7B4955C3EE64257848718FC9A19AB5F",
                                                         "2a82e2b8-5d4e-4dd7-945d-ea7754b0d5f2",
                                                         "A86E04F5461E480AAB0BA803385D1F66",
                                                         "A4193E519BA842B1AAE062238221AC98",
                                                         "B640D1855BD54E888FC34C3E84AD04EA",
                                                         "4189DEBA08154FC9A15487E46BA2A110",
                                                         "932CA591958A4D10ACC2409DCB6F1574",
                                                         "452EF88D20AB277580BA749FA74C006C",
                                                         "48AABB190433463986C9E819261A67FA",
                                                         "AE72CF0E8816455B90FD356AF38517B4",
                                                         "452EF88D20AB277580BA749FA74C0072"

                                                     
                                                     }
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
