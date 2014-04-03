using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IQuestionCommands
    {
        void SaveQuestions(IList<Question> questions);

        PagedCollection<Question> GetQuestionList(string courseId, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);
        Question CreateQuestion(Question question);
        Question GetQuestion(string questionId);

        bool UpdateQuestionField(string questionId, string fieldName, string value);
    }
}
