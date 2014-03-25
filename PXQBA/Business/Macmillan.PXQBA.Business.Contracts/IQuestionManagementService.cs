using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IQuestionManagementService
    {
        /// <summary>
        /// Creates new question
        /// </summary>
        /// <param name="titleId">Title id</param>
        /// <param name="type">Type of question</param>
        /// <param name="bank">Question bank</param>
        /// <param name="chapter">Chapter</param>
        void AddNewQuestion(string titleId, SelectedItem type, SelectedItem bank, SelectedItem chapter);
    }
}