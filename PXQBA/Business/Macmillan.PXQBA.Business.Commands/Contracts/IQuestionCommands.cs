using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IQuestionCommands
    {
        PagedCollection<Question> GetQuestionList(string courseId, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);
        Question CreateQuestion(string courseId, Question question);
        Question GetQuestion(string questionId);

        void UpdateQuestionSequence(string courseId, string questionId, int newSequenceValue);

        Question UpdateQuestion(Question question);

        bool UpdateQuestionField(string questionId, string fieldName, string value);
    }
}
