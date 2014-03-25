using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.Services
{
    public class QuestionFilterManagementService: IQuestionFilterManagementService
    {
        /// <summary>
        /// Gets the list of all question types from config
        /// </summary>
        /// <returns>Question types list</returns>
        public IEnumerable<dynamic> GetQuestionTypeList()
        {
            return ConfigurationHelper.GetQuestionTypes().Select(item => new { item.Key, item.Value });
        }
    }
}