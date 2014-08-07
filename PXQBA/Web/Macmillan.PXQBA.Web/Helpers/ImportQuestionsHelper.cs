using System;
using System.Web;
using Macmillan.PXQBA.Web.ViewModels.Import;

namespace Macmillan.PXQBA.Web.Helpers
{
    public static class ImportQuestionsHelper
    {
        private const string QuestionsForImportParamName = "questions_for_import_";

        public static string SaveImportContainer(QuestionForImportContainer contaner)
        {
            string key = Guid.NewGuid().ToString().ToUpper();
            HttpContext.Current.Session[QuestionsForImportParamName + key] = contaner;
            return key;
        }

        public static QuestionForImportContainer GetImportContainer(string key)
        {
            return HttpContext.Current.Session[QuestionsForImportParamName+key] as QuestionForImportContainer;
        }
    }
}