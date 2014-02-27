using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Contains all data necessary to render the RSSFeedWidget
    /// </summary>
    public class RSSFeedWidget
    {
        /// <summary>
        /// Title of the RSS Feed
        /// </summary>
        /// <value>
        /// The RSS Feed Title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// RSS Url
        /// </summary>
        /// <value>
        /// The RSS Url.
        /// </value>
        public string RSSUrl { get; set; }

        /// <summary>
        /// Total Archived Articles
        /// </summary>
        /// <value>
        /// Total Count of Archived Articles
        /// </value>
        public int TotalArchivedArticles { get; set; }

        /// <summary>
        /// List of RSS Feeds to render
        /// </summary>
        /// <value>
        /// The RSS Feeds.
        /// </value>
        public List<RssLink> RSSFeeds { get; set; }
    }
}
