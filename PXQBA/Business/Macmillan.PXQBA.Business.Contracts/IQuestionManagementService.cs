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
        PagedCollection<Question> GetQuestionList(string courseId, SortCriterion sortCriterion, int startingRecordNumber, int recordCount);

        /// <summary>
        /// Create a new question
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="question"></param>
        /// <returns>The updated object that was persisted</returns>
        Question CreateQuestion(string courseId, Question question);

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
        
        bool UpdateQuestionField(string questionId, string fieldName, string fieldValue);
    }
}