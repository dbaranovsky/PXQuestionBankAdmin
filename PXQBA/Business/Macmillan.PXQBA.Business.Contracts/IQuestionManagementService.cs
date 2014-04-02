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
        
        //void SaveQuestion(Question question);
        //void SaveQuestions(IList<Question> questions);

        //void CreateQuestion(string questionType);
        bool UpdateQuestionField(string questionId, string fieldName, string fieldValue);
    }
}