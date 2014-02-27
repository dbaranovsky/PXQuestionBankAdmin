using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// The RSS that was made on a Course.
    /// </summary>
    public class RSSFeed
    {
        /// <summary>
        /// RSS URL of the RSS.
        /// </summary>
        /// <value>
        /// The URL
        /// </value>
        public string RssUrl { get; set; }

        /// <summary>
        /// Link URL of the RSS Feed.
        /// </summary>
        /// <value>
        /// The Link URL
        /// </value>
        public string Link { get; set; }

        /// <summary>
        /// Thr Description of the site
        /// </summary>
        /// <value>
        /// The link description.
        /// </value>
        public string LinkDescription { get; set; }

        /// <summary>
        /// Author of the RSS.
        /// </summary>
        /// <value>
        /// The Author
        /// </value>
        public string Author { get; set; }

        /// <summary>
        /// Publication date of an Rss link.
        /// </summary>
        /// <value>
        /// The pub date.
        /// </value>
        public string PubDate { get; set; }

        /// <summary>
        /// Calculated Publication date of an Rss link.
        /// Here Calculated means: How old is any RSS Feed
        /// </summary>
        /// <value>
        /// The pub date.
        /// </value>
        public string PubDateCalculated { get; set; }

        /// <summary>
        /// The title of RSS Link.
        /// </summary>
        /// <value>
        /// The link title.
        /// </value>
        public string LinkTitle { get; set; }


        /// <summary>
        /// The Source of RSS Link.
        /// </summary>
        /// <value>
        /// The Source.
        /// </value>
        public string Source { get; set; }

        /// <summary>
        /// Is it already Archived.
        /// </summary>
        /// <value>
        /// True or False
        /// </value>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Archived Item Id.
        /// </summary>
        /// <value>
        /// Archived Item Id
        /// </value>
        public string ArchivedItemId { get; set; }

        /// <summary>
        /// Is it Assigned.
        /// </summary>
        /// <value>
        /// True or False
        /// </value>
        public bool IsAssigned { get; set; }

        /// <summary>
        /// Archived Item Id.
        /// </summary>
        /// <value>
        /// Archived Item Id
        /// </value>
        public DateTime AssignedDate { get; set; }

        /// <summary>
        /// Feed Counter.
        /// </summary>
        /// <value>
        /// Feed Counter
        /// </value>
        public int FeedCounter { get; set; }


    }
}

