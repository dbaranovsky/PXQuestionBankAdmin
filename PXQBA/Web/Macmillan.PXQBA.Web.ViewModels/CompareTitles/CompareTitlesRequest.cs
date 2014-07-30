namespace Macmillan.PXQBA.Web.ViewModels.CompareTitles
{
    /// <summary>
    /// Request that comes from product courses comparison list
    /// </summary>
    public class CompareTitlesRequest
    {
        /// <summary>
        /// First course to compare
        /// </summary>
        public string FirstCourse { get; set; }

        /// <summary>
        /// Second course to compare
        /// </summary>
        public string SecondCourse { get; set; }

        /// <summary>
        /// Page number requested
        /// </summary>
        public int Page { get; set; }
    }
}
