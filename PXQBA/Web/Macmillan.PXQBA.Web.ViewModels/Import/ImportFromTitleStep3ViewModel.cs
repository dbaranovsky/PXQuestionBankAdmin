namespace Macmillan.PXQBA.Web.ViewModels.Import
{
    /// <summary>
    /// View model for selecting destination title to import to
    /// </summary>
    public class ImportFromTitleStep3ViewModel
    {
        /// <summary>
        /// Course id to import to
        /// </summary>
        public string CourseId { get; set; }


        /// <summary>
        /// Key for getting questions to import from the session
        /// </summary>
        public string Key { get; set; }
    }
}
