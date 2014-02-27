using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a collection of links to offsite resources
    /// </summary>
    public class RssLink : ContentItem
    {
        /// <summary>
        /// RSS Feed Address or URL.
        /// </summary>
        /// <value>
        /// The RSS URL.
        /// </value>
        [RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?", ErrorMessage = "The specified URL is not a valid RSS feed.")]
        [Required(ErrorMessage = "You must specify a url")]
        [System.ComponentModel.DisplayName("RSS Feed URL")]
        public string RssUrl { get; set; }

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
        /// The href link in the rssfeed
        /// </summary>
        /// <value>
        /// The link.
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
        /// The author of the site
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public string Author { get; set; }

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


        /// <summary>
        /// Initializes an empty description 
        /// </summary>
        public RssLink()
        {
            Type = "Rsslink";
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RSSArticle"/> class.
        /// 
        /// </summary>
        public RssLink(string RSSFeedUrl, string ArticleLink, string ArticleTitle, string ArticleDescription, string ArticlePubDate)
        {
            Type = "CustomActivity";
            RssUrl = RSSFeedUrl;
            Link = ArticleLink;
            LinkTitle = ArticleTitle;
            LinkDescription = ArticleDescription;
            PubDate = ArticlePubDate;
        }

    }
}
