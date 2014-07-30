namespace Macmillan.PXQBA.Web.ViewModels.TiteList
{
    /// <summary>
    /// Response on adding site builder course into QBA
    /// </summary>
    public class AddSiteBuilderRepositoryResponse
    {
        /// <summary>
        /// Indicates if error occurred
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Added course id
        /// </summary>
        public string CourseId { get; set; }
    }
}
