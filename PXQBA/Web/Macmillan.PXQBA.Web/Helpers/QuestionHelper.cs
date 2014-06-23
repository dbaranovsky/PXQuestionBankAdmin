using System.Web;
using Macmillan.PXQBA.Web.ViewModels;

namespace Macmillan.PXQBA.Web.Helpers
{
    public class QuestionHelper
    {
        private const string QuestionIdParamName = "question_id_to_edit";
        private const string QuestionViewModelParamName = "question_viewmodel_to_edit";

        public static string QuestionIdToEdit
        {
            get { return HttpContext.Current.Session[QuestionIdParamName].ToString(); }
            set { HttpContext.Current.Session[QuestionIdParamName] = value; }
        }

        public static QuestionViewModel QuestionViewModelToEdit
        {
            get { return HttpContext.Current.Session[QuestionViewModelParamName] as  QuestionViewModel; }
            set { HttpContext.Current.Session[QuestionViewModelParamName] = value; }
        }
    }
}