using System.Web;
using Macmillan.PXQBA.Web.ViewModels;

namespace Macmillan.PXQBA.Web.Helpers
{
    public class QuestionHelper
    {
        private const string QuestionViewModelParamName = "question_viewmodel_to_edit_";


        public static void SetQuestionViewModelToEdit(QuestionViewModel questionViewModel)
        {
            HttpContext.Current.Session[QuestionViewModelParamName + questionViewModel.RealQuestionId.ToUpper()] =
                questionViewModel;
        }

        public static QuestionViewModel GetQuestionViewModelToEdit(string questionId)
        {
            var questionViewModel =
                HttpContext.Current.Session[QuestionViewModelParamName + questionId.ToUpper()] as QuestionViewModel;
            return questionViewModel;
        }
    }
}