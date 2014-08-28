using System;
using System.Web;
using Macmillan.PXQBA.Web.ViewModels.Import;

namespace Macmillan.PXQBA.Web.Helpers
{
    /// <summary>
    /// Represents helper of question importing mechanism
    /// </summary>
    public static class ImportQuestionsHelper
    {
        private const string QuestionsForImportParamName = "questions_for_import_";

        /// <summary>
        /// Saves a container with selected question to import
        /// </summary>
        /// <param name="contaner">Container with questions</param>
        /// <returns>Session key the container was created for</returns>
        public static string SaveImportContainer(QuestionForImportContainer contaner)
        {
            string key = Guid.NewGuid().ToString().ToUpper();
            HttpContext.Current.Session[QuestionsForImportParamName + key] = contaner;
            return key;
        }

        /// <summary>
        /// Gets a container with selected questions to import
        /// </summary>
        /// <param name="key">Questions container session key</param>
        /// <returns>Questions container</returns>
        public static QuestionForImportContainer GetImportContainer(string key)
        {
            return HttpContext.Current.Session[QuestionsForImportParamName+key] as QuestionForImportContainer;
        }
    }
}