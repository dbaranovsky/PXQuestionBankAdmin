using System.Web;
using Macmillan.PXQBA.Web.ViewModels;

namespace Macmillan.PXQBA.Web.Helpers
{
    /// <summary>
    /// Question helper
    /// </summary>
    public class QuestionHelper
    {
        private const string QuestionViewModelParamName = "question_viewmodel_to_edit_";

        /// <summary>
        /// Saves currently edited question to session
        /// </summary>
        /// <param name="questionViewModel">Question to save</param>
        public static void SetQuestionViewModelToEdit(QuestionViewModel questionViewModel)
        {
            HttpContext.Current.Session[QuestionViewModelParamName + questionViewModel.RealQuestionId.ToUpper()] =
                questionViewModel;
        }

        /// <summary>
        /// Gets currently edited question from session
        /// </summary>
        /// <param name="questionId">Question id to retrieve from session</param>
        /// <returns>Question</returns>
        public static QuestionViewModel GetQuestionViewModelToEdit(string questionId)
        {
            var questionViewModel =
                HttpContext.Current.Session[QuestionViewModelParamName + questionId.ToUpper()] as QuestionViewModel;
            return questionViewModel;
        }
    }
}