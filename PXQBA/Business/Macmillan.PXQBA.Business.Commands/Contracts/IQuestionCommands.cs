using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IQuestionCommands
    {
        void SaveQuestions(IList<Question> questions);

        QuestionList GetQuestionList(string query, int page, int questionPerPage);
    }
}
