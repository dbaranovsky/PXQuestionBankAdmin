namespace Macmillan.PXQBA.Web.ViewModels.Versions
{
    /// <summary>
    /// Model that stores info about version restored from
    /// </summary>
    public class RestoredFromVersionViewModel
    {
        /// <summary>
        /// Version number
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Version created date
        /// </summary>
        public string VersionDate { get; set; }

        /// <summary>
        /// Version author
        /// </summary>
        public string VersionAuthor { get; set; }
    }
}