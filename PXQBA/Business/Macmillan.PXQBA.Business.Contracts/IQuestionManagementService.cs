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
        /// <param name="course">Course to create question in</param>
        /// <param name="questionType">Question type to create</param>
        /// <param name="bank">Bank to create in</param>
        /// <param name="chapter">Chapter to create in</param>
        /// <returns>The updated object that was persisted</returns>
        Question CreateQuestion(Course course, string questionType, string bank, string chapter);

        /// <summary>
        /// Returns question by its ID
        /// </summary>
        /// <param name="course">Course</param>
        /// <param name="questionId">Question id to load</param>
        /// <param name="version">Question version to load</param>
        /// <returns>Question version</returns>
        Question GetQuestion(Course course, string questionId, string version = null);

        /// <summary>
        /// Creates template for new question based on existing one. 
        /// The question is not persisted at this point. Use CreateQuestion to persist the question after modification
        /// </summary>
        /// <param name="course">Course</param>
        /// <param name="questionId">Question id to duplicate</param>
        /// <param name="version">Version duplicate from</param>
        /// <returns>Duplicated question</returns>
        Question DuplicateQuestion(Course course, string questionId, string version = null);

        /// <summary>
        /// Update existing question metafields
        /// </summary>
        /// <param name="sourceQuestionId">Source question id</param>
        /// <param name="temporaryQuestion">Temporary question</param>
        /// <param name="course">Course</param>
        /// <returns>Updated question</returns>
        Question UpdateQuestion(Course course, string sourceQuestionId, Question temporaryQuestion);

        /// <summary>
        /// Updates question field with the new value
        /// </summary>
        /// <param name="course">Course question belongs to</param>
        /// <param name="questionId">Question id to update field</param>
        /// <param name="fieldName">Field name to update</param>
        /// <param name="fieldValue">New field value</param>
        /// <param name="userCapabilities">User capabilities</param>
        /// <returns>If update was successfull</returns>
        bool UpdateQuestionField(Course course, string questionId, string fieldName, string fieldValue, IEnumerable<Capability> userCapabilities);

        /// <summary>
        /// Updates shared question field with the new value
        /// </summary>
        /// <param name="course">Course question belongs to</param>
        /// <param name="questionId">Question id to update field</param>
        /// <param name="fieldName">Field name to update</param>
        /// <param name="fieldValues">New values</param>
        /// <returns>If update was successfull</returns>
        bool UpdateSharedQuestionField(Course course, string questionId, string fieldName, IEnumerable<string> fieldValues);

        /// <summary>
        /// Updates question field for bulk questions
        /// </summary>
        /// <param name="course">Course questions belong to</param>
        /// <param name="questionId">Question id list</param>
        /// <param name="fieldName">Field to update</param>
        /// <param name="fieldValue">New field value</param>
        /// <param name="userCapabilities">User capabilities for the course</param>
        /// <returns>Update result</returns>
        BulkOperationResult BulklUpdateQuestionField(Course course, string[] questionId, string fieldName, string fieldValue, IEnumerable<Capability> userCapabilities);

        /// <summary>
        /// Creates temporary question in temp course for editing
        /// </summary>
        /// <param name="course">Course id question belongs to</param>
        /// <param name="questionId">Question id to create temp question from</param>
        /// <returns>Created temp question</returns>
        Question CreateTemporaryQuestion(Course course, string questionId);

        /// <summary>
        /// Removes questions from title (unsharing)
        /// </summary>
        /// <param name="questionsId">Question ids</param>
        /// <param name="currentCourse">Course to remove questions from</param>
        /// <returns>If success</returns>
        bool RemoveFromTitle(string[] questionsId, Course currentCourse);

        /// <summary>
        /// Publishes the list of question to another title (sharing)
        /// </summary>
        /// <param name="questionsId">Question id list to publish</param>
        /// <param name="courseIdToPublish">Destination course id</param>
        /// <param name="bank">Destination bank</param>
        /// <param name="chapter">Destination chapter</param>
        /// <param name="currentCourse">Source course</param>
        /// <returns>Operation result</returns>
        BulkOperationResult PublishToTitle(string[] questionsId, int courseIdToPublish, string bank, string chapter, Course currentCourse);

        /// <summary>
        /// Loads version history for question. Basically each version is a question. So it loads the list of questions where each question is a separate version of original question
        /// </summary>
        /// <param name="currentCourse">Course question belongs to</param>
        /// <param name="questionId">Question id</param>
        /// <returns>List of versions</returns>
        IEnumerable<Question> GetVersionHistory(Course currentCourse, string questionId);

        /// <summary>
        /// Loads temporary question version from temp course
        /// </summary>
        /// <param name="currentCourse">Current course</param>
        /// <param name="questionId">Question id</param>
        /// <param name="version">Version to load</param>
        /// <returns>Question version</returns>
        Question GetTemporaryQuestionVersion(Course currentCourse, string questionId, string version);

        /// <summary>
        /// Publish draft question to original. Basically original question id is copied to draft question id and is saved. 
        /// Question with the draft id is deleted
        /// </summary>
        /// <param name="currentCourse">Current course</param>
        /// <param name="draftQuestionId">Draft question id</param>
        /// <returns>If success</returns>
        bool PublishDraftToOriginal(Course currentCourse, string draftQuestionId);

        /// <summary>
        /// Creates draft question from question version. Latest version is used if no version specified
        /// </summary>
        /// <param name="course">Current course</param>
        /// <param name="questionId">Question id</param>
        /// <param name="version">Question version</param>
        /// <returns>Created draft</returns>
        Question CreateDraft(Course course, string questionId, string version = null);

        /// <summary>
        /// Restores question version. Basically question version is loaded and then saved back (it creates new version automatically)
        /// </summary>
        /// <param name="course">Current course</param>
        /// <param name="questionId">Question id</param>
        /// <param name="version">Question version</param>
        /// <returns>Restored question version</returns>
        Question RestoreQuestionVersion(Course course, string questionId, string version);
        
        /// <summary>
        /// Removes question related resources from temp course
        /// </summary>
        /// <param name="questionIdToEdit">Question id to remove resources for</param>
        /// <param name="questionRepositoryCourseId">Question repository id</param>
        void RemoveRelatedQuestionTempResources(string questionIdToEdit, Course questionRepositoryCourseId);

        /// <summary>
        /// Parses questions from file and validate them and saved parsed data to database
        /// </summary>
        /// <param name="fileName">File name to parse</param>
        /// <param name="file">File data</param>
        /// <returns>Result of validation</returns>
        ValidationResult ValidateFile(string fileName, byte[] file);

        /// <summary>
        /// Imports previously parsed questions from database to dlap
        /// </summary>
        /// <param name="id">Parsed file id</param>
        /// <param name="courseId">Course to import to</param>
        /// <returns>File id imported</returns>
        int ImportFile(int id, string courseId);

        /// <summary>
        /// Import questions from course to course
        /// </summary>
        /// <param name="sourceCourse">Source course</param>
        /// <param name="questionsIds">Question id list to import</param>
        /// <param name="targetCourse">Destination course</param>
        /// <returns>If success</returns>
        bool ImportQuestions(Course sourceCourse, string[] questionsIds, Course targetCourse);

        /// <summary>
        /// Loads validated and parsed file from database
        /// </summary>
        /// <param name="fileId">File id</param>
        /// <returns>Parsed file</returns>
        ParsedFile GetValidatedFile(int fileId);

        /// <summary>
        /// Deletes temporary question with quiz it is located in
        /// </summary>
        /// <param name="questionId">Original question id to delete temp question for</param>
        void DeleteTemporaryQuestionWithQuiz(string questionId);

    }

}