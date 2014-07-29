using System.Collections.Generic;
using Macmillan.PXQBA.Business.Commands.DataContracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    /// <summary>
    /// Represents the list of operations with questions
    /// </summary>
    public interface IQuestionCommands
    {
        /// <summary>
        /// Loads question list using following steps:
        ///     1. If no sort criterion is set or is set by Sequence, then we set it to sort by Bank
        ///     2. SOLR search command is run with the filter applied
        ///     3. Appropriate page from questions is chosen
        ///     4. For all the banks from questions from point 3 questions are loaded and sorted by sequence. 
        ///        Thus we can calculate display sequence for question from point 3.
        ///     5. If initial sorting was none or by bank or by sequence, sort questions from point 3 by sequence per bank.
        /// </summary>
        /// <param name="questionRepositoryCourseid">Repository course id</param>
        /// <param name="currentCourseId">Current product course id</param>
        /// <param name="filter">Filter applied to the question list</param>
        /// <param name="sortCriterion">Sort criterion applied to question list</param>
        /// <param name="startingRecordNumber">Starting record number - necessary for paging</param>
        /// <param name="recordCount">Records count to return - page size</param>
        /// <returns>Paged question list</returns>
        PagedCollection<Question> GetQuestionList(string questionRepositoryCourseid, string currentCourseId, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);

        /// <summary>
        /// Gets list of questions compared for 2 chosen product courses
        /// </summary>
        /// <param name="questionRepositoryCourseId">Repository course id</param>
        /// <param name="firstCourseId">First course id to compare</param>
        /// <param name="secondCourseId">Second course id to compare</param>
        /// <param name="startingRecordNumber">Starting record number - necessary for paging</param>
        /// <param name="recordCount">Records count to return - page size</param>
        /// <returns>Paged comparison question list</returns>
        PagedCollection<ComparedQuestion> GetComparedQuestionList(string questionRepositoryCourseId, string firstCourseId, string secondCourseId,
            int startingRecordNumber, int recordCount);

        /// <summary>
        /// Create new question in dlap and set sequence
        /// </summary>
        /// <param name="productCourseId">Course id</param>
        /// <param name="question">Question to create</param>
        /// <returns>Created question</returns>
        Question CreateQuestion(string productCourseId, Question question);

        /// <summary>
        /// Sets sequence for question
        /// </summary>
        /// <param name="productCourseId">Course id</param>
        /// <param name="question">Question</param>
        void SetSequence(string productCourseId, Question question);

        /// <summary>
        /// Gets question from dlap
        /// </summary>
        /// <param name="repositoryCourseId">Repository course id</param>
        /// <param name="questionId">Question id</param>
        /// <param name="version">Question version number</param>
        /// <returns>Question version</returns>
        Question GetQuestion(string repositoryCourseId, string questionId, string version = null);

        /// <summary>
        /// Gets Agilix question from dlap
        /// </summary>
        /// <param name="repositoryCourseId">Course id</param>
        /// <param name="questionIdm">Question id</param>
        /// <param name="version">Version number</param>
        /// <returns>Question version</returns>
        Bfw.Agilix.DataContracts.Question GetAgilixQuestion(string repositoryCourseId, string questionIdm,
            string version = null);

        /// <summary>
        /// Gets question count per course chapter
        /// </summary>
        /// <param name="questionRepositoryCourseId">Repository course id</param>
        /// <param name="currentCourseId">Current course id</param>
        /// <param name="chapterNames">Chapter names</param>
        /// <returns>Questions count per chapter</returns>
        Dictionary<string, int> GetQuestionCountByChapters(string questionRepositoryCourseId, string currentCourseId, IEnumerable<string> chapterNames);
        
        /// <summary>
        /// Updates question in dlap
        /// </summary>
        /// <param name="question">Question to update</param>
        /// <param name="courseId">Course id</param>
        /// <returns>Updated question</returns>
        Question UpdateQuestion(Question question, string courseId = null);

        /// <summary>
        /// Updates question field
        /// </summary>
        /// <param name="productCourseId">Course id</param>
        /// <param name="repositoryCourseId">Repository course id</param>
        /// <param name="questionId">Question id</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">New field value</param>
        /// <param name="userCapabilities">User capabilities</param>
        /// <returns>If success</returns>
        bool UpdateQuestionField(string productCourseId, string repositoryCourseId, string questionId, string fieldName, string value, IEnumerable<Capability> userCapabilities);

        /// <summary>
        /// Updates field in bulk questions
        /// </summary>
        /// <param name="productCourseId">Course id</param>
        /// <param name="repositoryCourseId">repository course id</param>
        /// <param name="questionId">Question id</param>
        /// <param name="fieldName">Field name to update</param>
        /// <param name="value">New field value</param>
        /// <param name="userCapabilities">User capabilities</param>
        /// <returns>Bulk update result</returns>
        BulkOperationResult BulkUpdateQuestionField(string productCourseId, string repositoryCourseId, string[] questionId, string fieldName, string value, IEnumerable<Capability> userCapabilities);

        /// <summary>
        /// Updates qeustion shared fieldRe
        /// </summary>
        /// <param name="repositoryCourseId">Repository course id</param>
        /// <param name="questionId">Question id</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="fieldValues">New field values</param>
        /// <returns>If success</returns>
        bool UpdateSharedQuestionField(string repositoryCourseId, string questionId, string fieldName, IEnumerable<string> fieldValues);

        /// <summary>
        /// Gets quiz id question belongs to
        /// </summary>
        /// <param name="questionId">Question id</param>
        /// <param name="entityId">Repository course id</param>
        /// <returns>Quiz id</returns>
        string GetQuizIdForQuestion(string questionId, string entityId);

        /// <summary>
        /// Removes questions from product course (unpublishing)
        /// </summary>
        /// <param name="questionsId">Ids of questions'</param>
        /// <param name="questionRepositoryCourseId">Repository course id</param>
        /// <param name="currentCourseId">Current course id</param>
        /// <returns>If success</returns>
        bool RemoveFromTitle(string[] questionsId, string questionRepositoryCourseId, string currentCourseId);

        /// <summary>
        /// Gets questions from dlap
        /// </summary>
        /// <param name="repositoryCourseId">Repository course id</param>
        /// <param name="questionsId">Questions' ids</param>
        /// <returns>Question list</returns>
        IEnumerable<Question> GetQuestions(string repositoryCourseId, string[] questionsId);

        /// <summary>
        /// Updates questions
        /// </summary>
        /// <param name="questions">Questions to update</param>
        /// <param name="repositoryCourseId">Repository course id</param>
        /// <param name="courseId">Course id</param>
        /// <returns></returns>
        bool UpdateQuestions(IEnumerable<Question> questions, string repositoryCourseId, string courseId = null);

        /// <summary>
        /// Execute solr update task in order to re-index question data in SOLR
        /// </summary>
        void ExecuteSolrUpdateTask();

        /// <summary>
        /// Gets version history for the question
        /// </summary>
        /// <param name="questionRepositoryCourseId">Repository course id</param>
        /// <param name="questionId">Question id</param>
        /// <returns>Version history</returns>
        IEnumerable<Question> GetVersionHistory(string questionRepositoryCourseId, string questionId);

        /// <summary>
        /// Totally deletes the questions
        /// </summary>
        /// <param name="repositoryCourseId">Repository course id</param>
        /// <param name="questionId">Question id</param>
        void DeleteQuestion(string repositoryCourseId, string questionId);

        /// <summary>
        /// Loads drafts for question
        /// </summary>
        /// <param name="questionRepositoryCourseId">Repository course id</param>
        /// <param name="question">Question</param>
        /// <returns>Drafts list</returns>
        IEnumerable<Question> GetQuestionDrafts(string questionRepositoryCourseId, Question question);

        /// <summary>
        /// Runs PutQuestions command in dlap
        /// </summary>
        /// <param name="question">Question to save</param>
        /// <param name="courseId">Course id</param>
        void ExecutePutQuestion(Bfw.Agilix.DataContracts.Question question, string courseId = null);

        /// <summary>
        /// Gets faceted search results
        /// </summary>
        /// <param name="questionRepositoryCourseId">Repository course id</param>
        /// <param name="currentCourseId">Current course id</param>
        /// <param name="facetedField">Faceted field</param>
        /// <returns>List of results after faceted search</returns>
        IEnumerable<QuestionFacetedSearchResult> GetFacetedResults(string questionRepositoryCourseId, string currentCourseId, string facetedField);

        /// <summary>
        /// Creates questions and set sequence for each question
        /// </summary>
        /// <param name="productCourseId"></param>
        /// <param name="questions"></param>
        void CreateQuestions(string productCourseId, IEnumerable<Question> questions);
    }
}
