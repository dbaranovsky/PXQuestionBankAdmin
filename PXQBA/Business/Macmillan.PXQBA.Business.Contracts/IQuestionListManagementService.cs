using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Models;
using QuestionVm = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IQuestionListManagementService
    {
        /// <summary>
        /// Retrieves questions list
        /// </summary>
        /// <returns></returns>
        QuestionList GetQuestionList(string query, int page, int questionPerPage);
    }
}
