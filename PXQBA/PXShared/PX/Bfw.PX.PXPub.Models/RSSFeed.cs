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
    public class RssFeed : ContentItem
    {
        /// <summary>
        /// RSS Feed Address or URL.
        /// </summary>
        /// <value>
        /// The RSS URL.
        /// </value>
        //[RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?", ErrorMessage = "The specified URL is not a valid RSS feed.")]
        //[Required(ErrorMessage = "You must specify a url")]
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
        /// Gets or sets the RSS feeds.
        /// </summary>
        /// <value>
        /// The RSS feeds.
        /// </value>
        public IEnumerable<RssFeed> RssFeeds { get; set; }

        /// <summary>
        /// Initializes an empty description and link list
        /// </summary>
        public RssFeed()
        {
            Type = "RssFeed";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSSArticle"/> class.
        /// 
        /// </summary>
        public RssFeed(string RSSFeedUrl, string ArticleLink, string ArticleTitle, string ArticleDescription, string ArticlePubDate)
        {
            Type = "CustomActivity";
            RssUrl = RSSFeedUrl;
            Link = ArticleLink;
            LinkTitle = ArticleTitle;
            LinkDescription = ArticleDescription;
            PubDate = ArticlePubDate;
        }


        /// <summary>
        /// Gets the feeds.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public IEnumerable<RssFeed> GetFeeds(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    var xmlTree = XDocument.Load(url);
                    var itemsXml = xmlTree.Descendants("item");
                    Description = xmlTree.Element("rss") == null ? "" : xmlTree.Element("rss").Element("channel").Element("description").Value;
                    var rssFeeds = new List<RssFeed>();

                    foreach (var item in itemsXml)
                    {
                        rssFeeds.Add(new RssFeed()
                        {
                            PubDate = item.Element("pubDate") == null ? "" : DateTime.Parse(item.Element("pubDate").Value.Substring(0, 16)).ToShortDateString(),
                            LinkTitle = item.Element("title") == null ? "" : item.Element("title").Value,
                            Link = item.Element("link") == null ? "" : item.Element("link").Value,
                            LinkDescription = item.Element("description") == null ? "" : StripHTML(item.Element("description").Value),
                            Author = item.Element("author") == null ? "" : item.Element("author").Value
                        });
                    }

                    RssFeeds = rssFeeds;
                    return rssFeeds;

                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Strips the HTML.
        /// </summary>
        /// <param name="htmlString">The HTML string.</param>
        /// <returns></returns>
        private string StripHTML(string htmlString)
        {
            string pattern = @"<(.|\n)*?>";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }
    }
}
