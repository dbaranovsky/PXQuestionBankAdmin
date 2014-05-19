using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IQuestionCommands
    {
        PagedCollection<Question> GetQuestionList(string questionRepositoryCourseid, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);
        Question CreateQuestion(string courseId, Question question);
        Question GetQuestion(string repositoryCourseId, string questionId);

        Question UpdateQuestion(Question question);

        bool UpdateQuestionField(string repositoryCourseId, string questionId, string fieldName, string value);
        bool UpdateSharedQuestionField(string repositoryCourseId, string questionId, string fieldName, string fieldValue);
    }
}
