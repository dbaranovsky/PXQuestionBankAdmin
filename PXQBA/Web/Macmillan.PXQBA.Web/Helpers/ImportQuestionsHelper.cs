using System.Web;
using Macmillan.PXQBA.Web.ViewModels.Import;

namespace Macmillan.PXQBA.Web.Helpers
{
    public static class ImportQuestionsHelper
    {
        private const string QuestionsForImportParamName = "questions_for_import";

        public static QuestionForImportContainer QuestionsForImport
        {
            get { return HttpContext.Current.Session[QuestionsForImportParamName] as QuestionForImportContainer; }
            set { HttpContext.Current.Session[QuestionsForImportParamName] = value; }
        }
    }
}