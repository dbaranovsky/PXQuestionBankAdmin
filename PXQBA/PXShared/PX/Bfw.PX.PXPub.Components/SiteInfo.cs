namespace Bfw.PX.PXPub.Components
{
    /// <summary>
    /// Stores information about the current site.
    /// </summary>
    public class SiteInfo
    {
        /// <summary>
        /// A private variable to store the Site Decription.
        /// </summary>
        private string siteDescription;

        /// <summary>
        /// A private variable to store the baseURL
        /// </summary>
        private string baseURL;

        /// <summary>
        /// A private variable to store the CourseId
        /// </summary>
        private string agilixCourseID;

        /// <summary>
        /// A private variable to store the Site Id
        /// </summary>
        private string siteID;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteInfo"/> class.
        /// </summary>
        public SiteInfo()
        {
        }

        /// <summary>
        /// Gets or sets the site description.
        /// </summary>
        /// <value>
        /// The site description.
        /// </value>
        public string SiteDescription
        {
            get { return siteDescription; }
            set { siteDescription = value; }
        }


        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public string ID
        {
            get { return siteID; }
            set { siteID = value; }
        }

        /// <summary>
        /// Gets or sets the agilix ID.
        /// </summary>
        /// <value>
        /// The agilix ID.
        /// </value>
        public string AgilixID
        {
            get { return agilixCourseID; }
            set { agilixCourseID = value; }
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string URL
        {
            get { return baseURL; }
            set { baseURL = value; }
        }
    }
}
