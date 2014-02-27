using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides ability to list all or specific RSS Feeds on the current entity
    /// </summary>
    public interface IRSSFeedActions
    {
        /// <summary>
        /// Lists all Announcements made on the current entity
        /// </summary>
        /// <returns>list of all RSS Feed for the current entity</returns>
        List<RSSFeed> ListRssFeeds(string widgetId, int currentPageIndex, out string title, out int totalArchivedArticles, out string templateId, out string feedDescription, out string imageURL, int retrieval);

        /// <summary>
        /// Lists all RSS Archived Articles
        /// <param name="widgetId"></param>
        /// <param name="rssFeedTitle"></param>
        /// </summary>
        /// <returns>
        /// Lists all RSS Archived Articles
        /// </returns>        
        List<RSSFeed> ListRssArchivedArticles(string widgetId, out string title);


        /// <summary>
        /// Returns a list of RSSFeeds for a given URL
        /// </summary>
        /// <param name="feedURL"></param>
        /// <param name="retrievalLimit"></param>
        /// <param name="currentPageIndex"></param>
        /// <returns></returns>
        List<RSSFeed> GetRssFeeds(string feedURL, int retrievalLimit, int currentPageIndex);

        /// <summary>
        /// Get RSS widget Url
        /// </summary>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        string GetWidgetRSSUrl(string widgetId);

        /// <summary>
        /// Validate RSS URL
        /// </summary>
        /// <param name="rssURL"></param>
        /// <returns></returns>
        bool ValidateRssURL(string rssURL);
    }
}
