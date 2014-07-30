namespace Macmillan.PXQBA.Web.ViewModels.Import
{
    /// <summary>
    /// Result of the import operation
    /// </summary>
    public class ImportQuestionsToTitleResult
    {
        /// <summary>
        /// Indicates if import failed
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Error message if any
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Number of imported questions if success
        /// </summary>
        public int QuestionImportedCount { get; set; }
    }
}
