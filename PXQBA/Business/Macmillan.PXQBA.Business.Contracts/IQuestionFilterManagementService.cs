using System.Collections.Generic;
using Bfw.Agilix.DataContracts;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Represents service that manages question filter logic
    /// </summary>
    public interface IQuestionFilterManagementService
    {
        /// <summary>
        /// Retrieves question type list
        /// </summary>
        /// <returns>Question type list</returns>
        IEnumerable<dynamic> GetQuestionTypeList();
    }
}