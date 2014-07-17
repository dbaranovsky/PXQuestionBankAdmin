namespace Macmillan.PXQBA.Web.ViewModels.Import
{
    public class ImportQuestionsToTitleResult
    {
        public bool IsError { get; set; }

        public string ErrorMessage { get; set; }

        public int QuestionImportedCount { get; set; }
    }
}
