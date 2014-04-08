using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Models.Web;

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
        PagedCollection<Question> GetQuestionList(Course course, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);

        /// <summary>
        /// Create a new question
        /// </summary>
        /// <param name="course"></param>
        /// <param name="question"></param>
        /// <returns>The updated object that was persisted</returns>
        Question CreateQuestion(Course course, Question question);

        /// <summary>
        /// Returns question by its ID
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Question GetQuestion(string questionId);

        /// <summary>
        /// Creates template for new question based on existing one. 
        /// The question is not persisted at this point. Use CreateQuestion to persist the question after modification
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Question GetDuplicateQuestionTemplate(string questionId);

        /// <summary>
        /// Builds template for new question populating fields with defaults
        /// </summary>
        /// <returns></returns>
        Question GetNewQuestionTemplate();

        /// <summary>
        /// Changes question sequence number in scope of same bank questions.
        /// As sequence is defined in scope of cource, the cource id needs to be passed as well
        /// </summary>
        /// <param name="course"></param>
        /// <param name="questionId"></param>
        /// <param name="newSequenceValue"></param>
        void UpdateQuestionSequence(Course course, string questionId, int newSequenceValue);

        /// <summary>
        /// Returns list of question types available for specified course
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        IEnumerable<QuestionType> GetQuestionTypesForCourse(Course course);
        
        bool UpdateQuestionField(string questionId, string fieldName, string fieldValue);
    }
}