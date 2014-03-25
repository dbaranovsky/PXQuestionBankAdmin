using System.Collections.Generic;
using Bfw.Agilix.DataContracts;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IQuestionFilterManagementService
    {
        /// <summary>
        /// Retrieves question type list
        /// </summary>
        /// <returns></returns>
        IEnumerable<dynamic> GetQuestionTypeList();
    }
}