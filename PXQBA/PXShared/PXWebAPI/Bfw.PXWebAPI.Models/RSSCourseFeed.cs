using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// The following XML should be used to tell what RSS feeds are available to a particular course: 
    /// <bfw_rss_feeds> 
    ///   <feed name="scientific american" url="http://www.si.com" /> 
    ///   <feed name="foo" url="http://www.bar.com" /> 
    ///</bfw_rss_feeds> 
    /// </summary>
    public class RSSCourseFeed
    {
        /// <summary>
        /// Name of the RSS feed
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URL for the RSS feed
        /// </summary>
        public string Url { get; set; }
    }
}
