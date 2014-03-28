using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Service to handle operations with question
    /// </summary>
    public interface IQuestionManagementService
    {
        void SaveQuestion(Question question);

        void SaveQuestions(IList<Question> questions);

        void CreateQuestion(string questionType);
    }
}