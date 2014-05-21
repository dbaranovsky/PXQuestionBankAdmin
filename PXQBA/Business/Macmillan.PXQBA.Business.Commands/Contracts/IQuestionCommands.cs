using System.Collections.Generic;
using Macmillan.PXQBA.Business.Commands.DataContracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IQuestionCommands
    {
        PagedCollection<Question> GetQuestionList(string questionRepositoryCourseid, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);
        Question CreateQuestion(Question question);
        Question GetQuestion(string repositoryCourseId, string questionId);

        Dictionary<string, int> GetQuestionCountByChapters(string questionRepositoryCourseId, string currentCourseId);
        Question UpdateQuestion(Question question);

        bool UpdateQuestionField(string productCourseId, string repositoryCourseId, string questionId, string fieldName, string value);
        bool UpdateSharedQuestionField(string repositoryCourseId, string questionId, string fieldName, string fieldValue);

        string GetQuizIdForQuestion(string questionId, string entityId);

        bool RemoveFromTitle(string[] questionsId, string questionRepositoryCourseId, string currentCourseId);
        bool PublishToTitle(string[] questionsId, int courseId, string bank, string chapter);
    }
}
