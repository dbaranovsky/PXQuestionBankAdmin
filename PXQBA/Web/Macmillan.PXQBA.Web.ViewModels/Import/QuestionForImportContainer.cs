namespace Macmillan.PXQBA.Web.ViewModels.Import
{
    /// <summary>
    /// Selected for import questions
    /// </summary>
    public class QuestionForImportContainer
    {
        /// <summary>
        /// Source course id to import questions from
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// The list of question ids to import
        /// </summary>
        public string[] QuestionsId { get; set; }
    }
}
