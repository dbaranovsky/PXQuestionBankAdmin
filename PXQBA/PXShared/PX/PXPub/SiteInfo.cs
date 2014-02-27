
namespace PXPub
{
    public class SiteInfo
    {
        private string siteDescription;
        private string baseURL;
        private string agilixCourseID;
        private string siteID;

        public SiteInfo()
        {
        }

        public string SiteDescription
        {
            get { return siteDescription; }
            set { siteDescription = value; }
        }
    
        public string ID
        {
            get { return siteID; }
            set { siteID = value; }
        }

        public string AgilixID
        {
            get { return agilixCourseID; }
            set { agilixCourseID = value; }
        }

        public string URL
        {
            get { return baseURL; }
            set { baseURL = value; }
        }
    }
}
