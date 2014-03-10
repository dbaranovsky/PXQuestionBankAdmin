using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfw.Agilix.DataContracts;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IQuestionListManagementService
    {
        /// <summary>
        /// Retrieves questions list
        /// </summary>
        /// <returns></returns>
        IEnumerable<Question> GetQuestionList();
    }
}
