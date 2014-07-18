using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Service to handle operations with questions
    /// </summary>
    public interface IQuestionManagementService
    {
        /// <summary>
        /// Retrieves questions list
        /// </summary>
        /// <returns></returns>
        PagedCollection<Question> GetQuestionList(Course course, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);

        /// <summary>
        /// Retrieves questions for comparing courses
        /// </summary>
        /// <returns></returns>
        PagedCollection<ComparedQuestion> GetComparedQuestionList(string questionRepositoryCourseId,
            string firstCourseId, string secondCourseId, int startingRecordNumber, int recordCount);

        /// <summary>
        /// Create a new question
        /// </summary>
        /// <param name="question"></param>
        /// <returns>The updated object that was persisted</returns>
        Question CreateQuestion(Course course, string questionType, string bank, string chapter);

        /// <summary>
        /// Returns question by its ID
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Question GetQuestion(Course course, string questionId, string version = null);

        /// <summary>
        /// Creates template for new question based on existing one. 
        /// The question is not persisted at this point. Use CreateQuestion to persist the question after modification
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Question DuplicateQuestion(Course course, string questionId, string version = null);

        /// <summary>
        /// Update existing question metafields
        /// </summary>
        /// <param name="temporaryQuestion"></param>
        /// <returns></returns>
        Question UpdateQuestion(Course course, string sourceQuestionId, Question temporaryQuestion);

        bool UpdateQuestionField(Course course, string questionId, string fieldName, string fieldValue, IEnumerable<Capability> userCapabilities);
        bool UpdateSharedQuestionField(Course course, string questionId, string fieldName, IEnumerable<string> fieldValues);

        BulkOperationResult BulklUpdateQuestionField(Course course, string[] questionId, string fieldName, string fieldValue, IEnumerable<Capability> userCapabilities);

        Question CreateTemporaryQuestion(Course course, string questionId);

        bool RemoveFromTitle(string[] questionsId, Course currentCourse);

        bool PublishToTitle(string[] questionsId, int courseIdToPublish, string bank, string chapter, Course currentCourse);

        IEnumerable<Question> GetVersionHistory(Course currentCourse, string questionId);

        Question GetTemporaryQuestionVersion(Course currentCourse, string questionId, string version);
        bool PublishDraftToOriginal(Course currentCourse, string draftQuestionId);
        Question CreateDraft(Course course, string questionId, string version = null);
        Question RestoreQuestionVersion(Course course, string questionId, string version);
        
        void RemoveRelatedQuestionTempResources(string questionIdToEdit, Course questionRepositoryCourseId);

        ValidationResult ValidateFile(string fileName, byte[] file);

        void ImportFile(int id, string courseId);

        bool ImportQuestions(Course sourceCourse, string[] questionsIds, Course targetCourse);
    }

}