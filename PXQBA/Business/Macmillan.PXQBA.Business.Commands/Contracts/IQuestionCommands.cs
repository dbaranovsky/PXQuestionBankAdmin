using System.Collections.Generic;
using Macmillan.PXQBA.Business.Commands.DataContracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IQuestionCommands
    {
        PagedCollection<Question> GetQuestionList(string questionRepositoryCourseid, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);
        Question CreateQuestion(string productCourseId, Question question);
        Question GetQuestion(string repositoryCourseId, string questionId, string version = null);

        Bfw.Agilix.DataContracts.Question GetAgilixQuestion(string repositoryCourseId, string questionIdm,
            string version = null);

        Dictionary<string, int> GetQuestionCountByChapters(string questionRepositoryCourseId, string currentCourseId, IEnumerable<string> chapterNames);
        Question UpdateQuestion(Question question);
        bool UpdateQuestionField(string productCourseId, string repositoryCourseId, string questionId, string fieldName, string value, IEnumerable<Capability> userCapabilities);

        BulkOperationResult BulklUpdateQuestionField(string productCourseId, string repositoryCourseId, string[] questionId, string fieldName, string value, IEnumerable<Capability> userCapabilities);
        bool UpdateSharedQuestionField(string repositoryCourseId, string questionId, string fieldName, IEnumerable<string> fieldValues);

        string GetQuizIdForQuestion(string questionId, string entityId);

        bool RemoveFromTitle(string[] questionsId, string questionRepositoryCourseId, string currentCourseId);

        IEnumerable<Question> GetQuestions(string repositoryCourseId, string[] questionsId);

        bool UpdateQuestions(IEnumerable<Question> questions, string repositoryCourseId);

        void ExecuteSolrUpdateTask();
        IEnumerable<Question> GetVersionHistory(string questionRepositoryCourseId, string questionId);
        void DeleteQuestion(string repositoryCourseId, string questionId);
        IEnumerable<Question> GetQuestionDrafts(string questionRepositoryCourseId, Question question);

        void ExecutePutQuestion(Bfw.Agilix.DataContracts.Question question);

        IEnumerable<QuestionFacetedSearchResult> GetFacetedResults(string questionRepositoryCourseId, string currentCourseId, string facetedField);
    }
}
