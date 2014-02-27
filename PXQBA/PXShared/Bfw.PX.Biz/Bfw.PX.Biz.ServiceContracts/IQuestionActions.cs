using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.PX.Biz.DataContracts;
using LearningCurveQuestionSettings = Bfw.PX.Biz.DataContracts.LearningCurveQuestionSettings;
using Question = Bfw.PX.Biz.DataContracts.Question;
using QuestionAnalysis = Bfw.PX.Biz.DataContracts.QuestionAnalysis;
using Resource = Bfw.PX.Biz.DataContracts.Resource;

namespace Bfw.PX.Biz.ServiceContracts
{
	/// <summary>
	/// Provides methods to retrieve, store, and otherwise manipulate content.
	/// </summary>
	public interface IQuestionActions
	{
        /// <summary>
        /// Provides access to the BusinessContext that the service is running under.
        /// </summary>
        IBusinessContext Context { get; }

        string CQScriptString { get; }
        /// <summary>
        /// Finds Question Course Id from Course
        /// </summary>
        /// <param name="entityId">Entity Id of Course</param>
        /// <returns>Returns Question Course Id</returns>
        string GetQuestionRepositoryCourse(string entityId);

        /// <summary>
        /// Deletes question items with the specified ID from the course/entity.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="questionId">The question item ID.</param>
        void DeleteInvalidQuizQuestion(string entityId, string questionId);

        /// <summary>
        /// Gets a question by its ID.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="questionId">The question ID.</param>
        /// <returns></returns>
        Question GetQuestion(string entityId, string questionId);

        /// <summary>
        /// Gets the questions for an assessment.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <returns></returns>
        QuestionResultSet GetQuestions(string entityId, ContentItem item, string startIndex, string lastIndex);

        /// <summary>
        /// Gets the questions for an assessment.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <returns></returns>
        QuestionResultSet GetQuestions(string entityId, string itemId, string startIndex, string lastIndex);

        /// <summary>
        /// Gets the questions for an assessment.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="item">The item </param>
        /// <param name="ignoreBanks">Ignore Banks</param>
        /// <param name="questionEntityId">Question entity id</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The last index.</param>
        /// <returns></returns>
        QuestionResultSet GetQuestions(string entityId, ContentItem item, bool ignoreBanks, string questionEntityId,
                                       string start, string end);

        /// <summary>
        /// Gets a list of questions by their IDs.
        /// </summary>
        IEnumerable<Question> GetQuestions(string entityId, IEnumerable<string> questionIds);

        string GetCustomQuestionPreview(string customUrl, string questionData, string questionId);

        string GetHTSQuestionPreview(string sURL, string sXml, string questionId);

        string GetGraphQuestionPreview(string sURL, string sXml, string questionId);

        /// <summary>
        /// Get the question analysis by entity ID.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <returns></returns>
        IList<QuestionAnalysis> GetQuestionAnalysis(string entityId, string itemId);

	    /// <summary>
	    /// Update the list of question ids associated with a quiz
	    /// </summary>
	    /// <param name="entityId">The entity ID.</param>
	    /// <param name="itemId">The item ID.</param>
	    /// <param name="questions">The questions.</param>
	    void UpdateQuestionList(string entityId, string itemId, IList<Question> questions, bool savePrevPoints, string mainQuizId = "");

        /// <summary>
        /// Append a list of questions to the existing question list.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="questions">The questions.</param>
        void AppendQuestionList(string entityId, string itemId, IList<Question> questions, string mainQuizId = "");

        /// <summary>
        /// Add a question pool to the QuestionList.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="question">The question.</param>
        void AddQuestionToQuestionList(string entityId, string parentId, string itemId, Question question);

	    /// <summary>
	    /// Edit a question pool in the Question list.
	    /// </summary>
	    /// <param name="entityId">The entity ID.</param>
	    /// <param name="parentId">The parent ID.</param>
	    /// <param name="itemId">The item ID.</param>
	    /// <param name="poolCount">The pool count.</param>
	    /// <param name="points">Number of points in the pool</param>
	    void EditQuestionList(string entityId, string parentId, string itemId, string poolCount, int? points);

        /// <summary>
        /// Stores the specified question
        /// </summary>
        /// <param name="question">The question.</param>
        void StoreQuestion(Question question);

        /// <summary>
        /// Stores the collection of questions.
        /// </summary>
        /// <param name="question">The question.</param>
        void StoreQuestions(IList<Question> question);

        /// <summary>
        /// This method is used for Live Preview of Graph or any other custom Question
        /// </summary>
        /// <param name="customXML">Custom XML data / Interaction Data content</param>
        /// <param name="customURL">FMA_GRAPH or HTS (In future we might have any other custom type question)</param>
        /// <param name="entityId">This is current course id or parent course id from where we want to Show Live Preview</param>
        /// <returns>Returns Question id, that will be used by ShowQuiz (in QuizController)</returns>
        string StoreQuizPreview(string customXML, string customURL, string entityId);

        /// <summary>
        /// This method is used to create a copy of question in current Context.CourseId from different course id.
        /// </summary>
        /// <param name="sourceEntityId">Source EntityId From where question will be found</param>
        /// <param name="destinationEntityId">Destination Entity Id where question will be copied</param>
        /// <param name="sourceQuestionId">Question Id within source entity id that will be copied over to Context.CourseId</param>
        /// <param name="mockQuestionId">Id of mock question to be created. It can be GUId or some constant id, if intended to overwrite.</param>
        /// <param name="mockQuizId">Id of mock quiz. It can be new GUID value or some constant id, if same quiz has to be used again and again</param>
        /// <param name="changedQuestionId">This is the changed Question Id based on question type and Custom URL (HTS / FMA_GRAPH) if exists</param>
        /// <returns>Question id of the copied question which should be same as mockQuestionId</returns>
        string StoreMockQuiz(string sourceEntityId, string destinationEntityId, string sourceQuestionId, string mockQuestionId, string mockQuizId, out string changedQuestionId);

        /// <summary>
        /// Updates all settings set on the settings object for the specified quiz and question.
        /// </summary>
        /// <param name="entityId">Id of the entity the quiz exists in.</param>
        /// <param name="quizId">Id of the quiz the question exists in.</param>
        /// <param name="questionId">Id of the question in the quiz that is being updated.</param>
        /// <param name="settings">New value of all settings being updated for the question.</param>
        void UpdateQuestionSettings(string entityId, QuestionSettings settings);

        /// <summary>
        /// Updates the learning curve question settings.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="settings">The settings.</param>
        void UpdateLearningCurveQuestionSettings(string entityId, LearningCurveQuestionSettings settings);

        

        /// <summary>
        /// Deletes the questions.
        /// </summary>
        /// <param name="entityId">The entityId.</param>
        /// <param name="questionIds">The question ids.</param>
        void DeleteQuestions(string entityId, IEnumerable<string> questionIds);


        /// <summary>
        /// Removes the question from cache.
        /// </summary>
        /// <param name="questions">The questions.</param>
        void RemoveQuestionsFromCache(List<Biz.DataContracts.Question> questions);

        /// <summary>
        /// Gets a list of questions by their IDs .
        /// </summary>
        /// <param name="entityId">Id of the entity in which the questions exist.</param>
        /// <param name="questionIds">Ids of the questions to get</param>
        /// <param name="searchQuestionsFilter"> </param>
        /// <param name="count"> </param>
        /// <returns></returns>
        IEnumerable<DataContracts.Question> GetQuestions(string entityId, IEnumerable<string> questionIds, string searchQuestionsFilter, int? count);

        ///<summary>
        ///
        /// Get a list of Questions 
        /// </summary>
        IEnumerable<Question> GetQuestionsList(string entityId, string questionId, bool version = false);

        /// <summary>
        /// GetQuizzesForSelectedChapters
        /// </summary>
        /// <param name="entityId"> </param>
        /// <param name="selectedChaptersList"> </param>
        /// "cmd=getitemlist&entityid=65131&query=/bfw_subtype='QUIZ' 
        /// AND (/parent='chapterId' OR /parent='chapterId')"
        /// <returns></returns>
        List<ContentItem> GetQuizzesForSelectedChapters(string entityId, IEnumerable<string> selectedChaptersList);

        List<ContentItem> GetCourseChapters(string entityId);

        /// <summary>
        /// Get Course Quizzes
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        List<ContentItem> GetCourseQuizzes(string entityId);
	}
}