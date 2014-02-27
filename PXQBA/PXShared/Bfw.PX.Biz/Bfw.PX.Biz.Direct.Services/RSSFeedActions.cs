using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.PX.Biz.DataContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.Agilix.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Logging;



using CodeForce.Utilities;
using System.Xml.XPath;
using System.Xml;
using System.Net;
using System.IO;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implementation of the IAnnouncementActions interface
    /// </summary>
    public class RSSFeedActions : IRSSFeedActions
    {
        #region Properties

        /// <summary>
        /// Current Business Context
        /// </summary>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// The ISessionManager implementation to use for communicating with Dlap
        /// </summary>
        private ISessionManager SessionManager { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a default RSSFeedActions, which depends on IBusinessContext
        /// and IRSSFeedService implementations.
        /// </summary>
        /// <param name="context">The IBusinessContext implementation.</param>
        /// <param name="sessionManager">The session manager.</param>
        public RSSFeedActions(IBusinessContext context, ISessionManager sessionManager)
        {
            Context = context;
            SessionManager = sessionManager;
        }

        #endregion

        #region IRSSFeedActions Members


        /// <summary>
        /// Lists all RSS Archived Articles
        /// <param name="widgetId"></param>
        /// <param name="rssFeedTitle"></param>
        /// </summary>
        /// <returns>
        /// Lists all RSS Archived Articles
        /// </returns>
        public List<BizDC.RSSFeed> ListRssArchivedArticles(string widgetId, out string rssFeedTitle)
        {
            rssFeedTitle = string.Empty;
            List<BizDC.RSSFeed> rssFeeds = new List<BizDC.RSSFeed>();
            DataContracts.ContentItem item = null;
            int counter = 0;

            using (Context.Tracer.DoTrace("RSSFeedActions.ListRssArchivedArticles(widgetId={0})", widgetId))
            {
                GetItems cmdGetWidgetFeeds;
                GetItems cmdGetArchivedFeeds;

                cmdGetWidgetFeeds = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        ItemId = widgetId
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmdGetWidgetFeeds);
                item = cmdGetWidgetFeeds.Items[0].ToContentItem(Context);
                string feedURL = item.Properties["bfw_feedurl"].Value.ToString();

                cmdGetArchivedFeeds = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        //Query = string.Format(@"/bfw_type='{0}'AND/parent='{1}'AND/bfw_properties/bfw_property='{2}'", "RSSLink", "PX_MANIFEST", System.Web.HttpUtility.UrlEncode(feedURL)) // TO DO, query is not working.
                        Query = string.Format(@"/bfw_type='{0}'AND/parent='{1}'", "RSSLink", "PX_MANIFEST")
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmdGetArchivedFeeds);

                // Find total count of Archived Articles for one Widget URL
                foreach (Item archivedItem in cmdGetArchivedFeeds.Items)
                {
                    BizDC.ContentItem ci = archivedItem.ToContentItem(Context);
                    if (ci.Properties["bfw_RssUrl"].Value.ToString() == feedURL)
                    {
                        BizDC.RSSFeed rssFeed = new BizDC.RSSFeed();
                        rssFeed.RssUrl = ci.Properties["bfw_RssUrl"].Value.ToString();
                        rssFeed.Link = ci.Href;
                        rssFeed.LinkDescription = ci.Description;
                        rssFeed.PubDate = getPubDate(DateTimeConversion.UtcRelativeAdjustCommon(Convert.ToDateTime(ci.Properties["bfw_RssArticlePubDate"].Value.ToString()), Context.Course.CourseTimeZone));
                        rssFeed.LinkTitle = ci.Title;
                        rssFeed.IsArchived = true;
                        rssFeed.ArchivedItemId = ci.Id;
                        rssFeed.IsAssigned = false;
                        rssFeed.FeedCounter = counter;
                        if (ci.AssignmentSettings != null)
                        {
                            if (ci.AssignmentSettings.meta_bfw_Assigned == true)
                            {
                                rssFeed.IsAssigned = true;
                                rssFeed.AssignedDate = ci.AssignmentSettings.DueDate;
                            }
                        }
                        rssFeeds.Add(rssFeed);
                        counter++;
                    }
                }
            }
            return rssFeeds;
        }

        /// <summary>
        /// Get RSS widget Url
        /// </summary>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public string GetWidgetRSSUrl(string widgetId)
        {
            string rssUrl = string.Empty;
            DataContracts.ContentItem item = null;
            using (Context.Tracer.DoTrace("RSSFeedActions.GetWidgetRSSUrl(widgetId={0})", widgetId))
            {
                GetItems cmdGetWidget;
                cmdGetWidget = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        ItemId = widgetId
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmdGetWidget);
                item = cmdGetWidget.Items[0].ToContentItem(Context);
                if (item.Properties.ContainsKey("bfw_feedurl"))
                {
                    rssUrl = item.Properties["bfw_feedurl"].Value.ToString();
                }
            }
            return rssUrl;
        }

        /// <summary>
        /// Lists all RSS Feeds made on the current entity
        /// <param name="widgetId"></param>
        /// <param name="rssFeedTitle"></param>
        /// <param name="totalArchivedArticles"></param>
        /// </summary>
        /// <returns>
        /// list of all RSS Feeds for the current entity
        /// </returns>
        public List<BizDC.RSSFeed> ListRssFeeds(string widgetId, int currentPageIndex, out string rssFeedTitle, out int totalArchivedArticles, out string templateId, out string feedDescription, out string imageURL, int retrievalLimit)
        {
            rssFeedTitle = string.Empty;
            totalArchivedArticles = 0;
            templateId = string.Empty;
            feedDescription = string.Empty;
            imageURL = string.Empty;
            List<BizDC.RSSFeed> rssFeeds = new List<BizDC.RSSFeed>();
            DataContracts.ContentItem item = null;

            using (Context.Tracer.DoTrace("RSSFeedActions.ListRSSFeeds(widgetId={0})", widgetId))
            {
                GetItems cmdGetWidgetFeeds;
                GetItems cmdGetArchivedFeeds;


                cmdGetWidgetFeeds = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        ItemId = widgetId
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmdGetWidgetFeeds);
                item = cmdGetWidgetFeeds.Items[0].ToContentItem(Context);
                string feedURL = "";
                if(item.Properties.ContainsKey("bfw_feedurl"))
                    feedURL = item.Properties["bfw_feedurl"].Value.ToString();
                templateId = cmdGetWidgetFeeds.Items[0].Data.Element("bfw_widget_template").Value;
                if (item.Properties.ContainsKey("bfw_feeddescription"))
                {
                    feedDescription = item.Properties["bfw_feeddescription"].Value.ToString();
                }
                if (item.Properties.ContainsKey("bfw_imageurl"))
                {
                    imageURL = item.Properties["bfw_imageurl"].Value.ToString();
                }
                cmdGetArchivedFeeds = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        //Query = string.Format(@"/bfw_type='{0}'AND/parent='{1}'AND/bfw_properties/bfw_property='{2}'", "RSSLink", "PX_MANIFEST", System.Web.HttpUtility.UrlEncode(feedURL)) // TO DO, query is not working.
                        Query = string.Format(@"/bfw_type='{0}'AND/parent='{1}'", "RSSLink", "PX_MANIFEST")
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmdGetArchivedFeeds);

                // Find total count of Archived Articles for one Widget URL
                foreach (Item archivedItem in cmdGetArchivedFeeds.Items)
                {
                    if (archivedItem.ToContentItem(Context).Properties["bfw_RssUrl"].Value.ToString() == feedURL)
                    {
                        totalArchivedArticles++;
                    }
                }



                try
                {
                    List<RssItem> lstFeedItems = getRssFeedItems(feedURL, out rssFeedTitle);


                    rssFeeds = ParseRssFeedItems(retrievalLimit, feedURL, cmdGetArchivedFeeds, lstFeedItems, currentPageIndex);
                }
                catch // If there is no feeds for any particular RSS Feed URL
                {
                    return rssFeeds;
                }
            }
            return rssFeeds;
        }
        
        /// <summary>
        /// Returns a list of RSSFeeds for a given URL
        /// </summary>
        /// <param name="feedURL"></param>
        /// <param name="retrievalLimit"></param>
        /// <param name="currentPageIndex"></param>
        /// <returns></returns>
        public List<RSSFeed> GetRssFeeds(string feedURL, int retrievalLimit, int currentPageIndex)
        {
            List<RSSFeed> rssFeeds = new List<RSSFeed>();
            try
            {
                string rssFeedTitle = string.Empty;
                List<RssItem> lstFeedItems = getRssFeedItems(feedURL, out rssFeedTitle);


                rssFeeds = ParseRssFeedItems(retrievalLimit, feedURL, null, lstFeedItems, currentPageIndex);
            }
            catch // If there is no feeds for any particular RSS Feed URL
            {
                return rssFeeds;
            }
            return rssFeeds;
        }


        //public ContentItem GetContentItemLink(RSSFeed feed, string parentId, string categoryId)
        //{
        //    ContentItem ci = new ContentItem()
        //                         {
        //                             Href = feed.Link,
        //                             Description = feed.LinkDescription,
        //                             Title = feed.LinkTitle,
        //                             Id = "",
        //                         };
        //    ci.Properties["bfw_RssUrl"] = 
        //    // Find total count of Archived Articles for one Widget URL
        //    foreach (Item archivedItem in cmdGetArchivedFeeds.Items)
        //    {
        //        BizDC.ContentItem ci = archivedItem.ToContentItem(Context);
        //        if (ci.Properties["bfw_RssUrl"].Value.ToString() == feedURL)
        //        {
        //            BizDC.RSSFeed rssFeed = new BizDC.RSSFeed();
        //            rssFeed.RssUrl = ci.Properties["bfw_RssUrl"].Value.ToString();
        //            rssFeed.Link = ci.Href;
        //            rssFeed.LinkDescription = ci.Description;
        //            rssFeed.PubDate = getPubDate(DateTimeConversion.UtcRelativeAdjustCommon(Convert.ToDateTime(ci.Properties["bfw_RssArticlePubDate"].Value.ToString()), Context.Course.CourseTimeZone));
        //            rssFeed.LinkTitle = ci.Title;
        //            rssFeed.IsArchived = true;
        //            rssFeed.ArchivedItemId = ci.Id;
        //            rssFeed.IsAssigned = false;
        //            if (ci.AssignmentSettings != null)
        //            {
        //                if (ci.AssignmentSettings.meta_bfw_Assigned == true)
        //                {
        //                    rssFeed.IsAssigned = true;
        //                    rssFeed.AssignedDate = ci.AssignmentSettings.DueDate;
        //                }
        //            }
        //            rssFeeds.Add(rssFeed);
        //            counter++;
        //        }
        //    }
        //}
        public bool ValidateRssURL(string rssURL)
        {
            bool rssExist = false;
            using (Context.Tracer.DoTrace("RSSFeedActions.ValidateRssURL(rssURL={0})", rssURL))
            {
                if (!string.IsNullOrEmpty(rssURL))
                {
                    try
                    {
                        Stream stream = ParseGarbageOut(rssURL);                        

                        var xmlTree = XDocument.Load(stream);
                        
                        var itemsXml = xmlTree.Descendants("item");
                        if (itemsXml.Count() == 0)
                        {
                            itemsXml = xmlTree.Root.Elements().Where(i => i.Name.LocalName == "entry");
                        }
                        if (itemsXml.Count() == 0)
                        {
                            itemsXml = xmlTree.Root.Descendants().Where(i => i.Name.LocalName == "item");
                        }

                        int counter = 0;
                        foreach (var item in itemsXml)
                        {
                            counter++;
                        }
                        if (counter > 0)
                        {
                            rssExist = true;
                        }
                        else
                        {
                            rssExist = false;
                        }
                    }
                    catch
                    {
                        rssExist = false;
                    }
                }
            }
            return rssExist;
        }


        #endregion

        #region private Functions

        private Stream ParseGarbageOut(string rssURL)
        {
            WebRequest request = WebRequest.Create(rssURL);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();

            var contentType = response.ContentType;

            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            response.Close();
            
            var cDataMatch = Regex.Match(content, @"\<\!\[CDATA\[(.*\<\!\[CDATA\[.*\]\]\>.*)\]\]\>");
            if (cDataMatch.Success)
            {
                for (int i = 0; i < cDataMatch.Groups.Count; i++)
                {
                    content = content.Replace(cDataMatch.Groups[i].Value, string.Empty);
                }
            }

            byte[] byteArray;

            if (contentType.Contains("utf-8"))
            {
                byteArray = Encoding.UTF8.GetBytes(content);
            }
            else
            {
                byteArray = Encoding.ASCII.GetBytes(content);
            }

            stream = new MemoryStream(byteArray);

            return stream;
        }

        /// <summary>
        /// Returns a list of RSSFeed items given a list of CodeForce RssItem
        /// </summary>
        /// <param name="retrievalLimit"></param>
        /// <param name="feedURL"></param>
        /// <param name="cmdGetArchivedFeeds"></param>
        /// <param name="lstFeedItems"></param>
        /// <param name="currentPageIndex"></param>
        /// <returns></returns>
        private List<RSSFeed> ParseRssFeedItems(int retrievalLimit, string feedURL, GetItems cmdGetArchivedFeeds,
                                      List<RssItem> lstFeedItems, int currentPageIndex)
        {
            List<RSSFeed> rssFeeds = new List<RSSFeed>();
            int startCounter = 5 * currentPageIndex;
            int counter = 0;
            int totalCounter = 0;
            foreach (RssItem entry in lstFeedItems)
            {
                if ((totalCounter >= startCounter) && (counter < retrievalLimit))
                {
                    BizDC.RSSFeed rssFeed = new BizDC.RSSFeed();
                    rssFeed.RssUrl = feedURL;
                    rssFeed.Link = entry.Link.ToString();
                    rssFeed.LinkDescription = Regex.Replace(entry.Description, @"<(.|\n)*?>", string.Empty);
                    rssFeed.PubDateCalculated =
                        getPubDate(DateTimeConversion.UtcRelativeAdjustCommon(entry.Date, Context.Course.CourseTimeZone));
                    rssFeed.PubDate =
                        DateTimeConversion.UtcRelativeAdjustCommon(entry.Date, Context.Course.CourseTimeZone).ToString();
                    rssFeed.LinkTitle = Regex.Replace(entry.Title, @"<(.|\n)*?>", string.Empty);
                    rssFeed.IsArchived = false;
                    rssFeed.IsAssigned = false;
                    rssFeed.FeedCounter = totalCounter;
                    if (cmdGetArchivedFeeds != null)
                    {
                        foreach (Item archivedItem in cmdGetArchivedFeeds.Items)
                        {
                            if (entry.Link.ToString() == archivedItem.Href)
                            {
                                if (archivedItem.ToContentItem(Context).AssignmentSettings != null)
                                {
                                    if (archivedItem.ToContentItem(Context).AssignmentSettings.meta_bfw_Assigned == true)
                                    {
                                        rssFeed.IsAssigned = true;
                                        rssFeed.AssignedDate = archivedItem.ToContentItem(Context).AssignmentSettings.DueDate;
                                    }
                                }
                                rssFeed.IsArchived = true;
                                rssFeed.ArchivedItemId = archivedItem.Id;
                                break;
                            }
                        }
                    }
                    rssFeeds.Add(rssFeed);
                    counter++;
                }
                totalCounter++;
            }
            return rssFeeds;
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


        /////////////////////////////////////////
        private List<RssItem> Parse(string url)
        {
            List<RssItem> result = null;

            result = ParseRss(url);

            if (result == null || result.Count == 0)
            {
                result = ParseAtom(url);
            }

            if (result == null || result.Count == 0)
            {
                result = ParseRdf(url);
            }

            return result;
        }

        /// <summary>
        /// Parses an RSS feed and returns a <see cref="IList&lt;Item&gt;"/>.
        /// </summary>
        private List<RssItem> ParseRss(string url)
        {
            try
            {
                var stream = ParseGarbageOut(url);

                XDocument doc = XDocument.Load(stream);

                // RSS/Channel/item
                var entries = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                              select new RssItem
                              {
                                  Date = item.Element(GetDateElementName(item)) == null ? DateTime.Now : convertDateTime(item.Elements().First(i => i.Name.LocalName == GetDateElementName(item)).Value),
                                  Title = item.Element("title") == null ? "" : item.Elements().First(i => i.Name.LocalName == "title").Value,
                                  Link = item.Element("link") == null ? "" : item.Elements().First(i => i.Name.LocalName == "link").Value,
                                  Description = item.Element("description") == null ? "" : item.Elements().First(i => i.Name.LocalName == "description").Value,
                              };

                return entries.ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Takes care of different date element names
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetDateElementName(XElement item)
        {
            string date = null;

            if (item.Element("pubDate") != null)
            {
                date = "pubDate";
            }
            else if (item.Element("pubdate") != null)
            {
                date = "pubdate";
            }

            return date;
        }

        /// <summary>
        /// Parses an Atom feed and returns a <see cref="IList&lt;Item&gt;"/>.
        /// </summary>
        private List<RssItem> ParseAtom(string url)
        {
            try
            {
                XDocument doc = XDocument.Load(url);

                var entries = from i in doc.Root.Elements("{http://www.w3.org/2005/Atom}entry")
                        select new RssItem
                        {
                            Date = i.Element("{http://www.w3.org/2005/Atom}published") == null ? DateTime.Now : convertDateTime(i.Element("{http://www.w3.org/2005/Atom}published").Value),
                            Title = i.Element("{http://www.w3.org/2005/Atom}title") == null ? "" : i.Element("{http://www.w3.org/2005/Atom}title").Value,
                            Link = i.Element("{http://www.w3.org/2005/Atom}link").Attribute("href") == null ? "" : i.Element("{http://www.w3.org/2005/Atom}link").Attribute("href").Value,
                            Description = i.Element("{http://www.w3.org/2005/Atom}description") == null ? "" : i.Element("{http://www.w3.org/2005/Atom}description").Value,
                        };

                return entries.ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parses an RDF feed and returns a <see cref="IList&lt;Item&gt;"/>.
        /// </summary>
        private List<RssItem> ParseRdf(string url)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(url);

                XmlNameTable nTable = doc.NameTable;
                XmlNamespaceManager nsManager = new XmlNamespaceManager(nTable);

                nsManager.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                nsManager.AddNamespace("item", "http://purl.org/rss/1.0/");
                nsManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                XmlNodeList XMLItems = doc.DocumentElement.SelectNodes("//rdf:RDF/item:item", nsManager);

                List<RssItem> entries = new List<RssItem>();

                foreach (XmlNode item in XMLItems)
                {
                    var entity = new RssItem()
                    {
                        Date = convertDateTime(item.SelectSingleNode("dc:date", nsManager).InnerText),
                        Title = item.SelectSingleNode("item:title", nsManager).InnerText,
                        Link = item.SelectSingleNode("item:link", nsManager).InnerText,
                        Description = item.SelectSingleNode("item:description", nsManager).InnerText
                    };

                    entries.Add(entity);
                }

                return entries;
            }
            catch
            {
                return null;
            }
        }

        private List<RssItem> getRssFeedItems(string feedURL, out string rssFeedTitle)
        {
            rssFeedTitle = string.Empty;
            List<RssItem> lstFeedItems = new List<RssItem>();

            using (Context.Tracer.DoTrace("RSSFeedActions.getRssFeedItems(feedURL={0})", feedURL))
            {
                if (!string.IsNullOrEmpty(feedURL))
                {
                    try
                    {
                        var stream = ParseGarbageOut(feedURL);

                        var xmlTree = XDocument.Load(stream);
                        var itemsXml = xmlTree.Descendants("item");

                        if (xmlTree.Element("rss") != null && xmlTree.Element("rss").Element("channel").Element("title") != null)
                        {
                            rssFeedTitle = xmlTree.Element("rss").Element("channel").Element("title").Value;
                        }

                        if (rssFeedTitle == string.Empty)
                        {
                            rssFeedTitle = xmlTree.Element("{http://www.w3.org/2005/Atom}feed") == null ? "" : xmlTree.Element("{http://www.w3.org/2005/Atom}feed").Element("{http://www.w3.org/2005/Atom}title").Value;
                        }

                        lstFeedItems = Parse(feedURL);
                    }
                    catch
                    {
                    }
                }
            }

            return lstFeedItems;
        }

        private DateTime convertDateTime(string StrDate)
        {
            DateTime ConvertedDate;
            using (Context.Tracer.DoTrace("RSSFeedActions.convertDateTime(StrDate={0})", StrDate))
            {
                try
                {
                    ConvertedDate = new DateTime();
                    int TZstart = StrDate.LastIndexOf(" ");

                    if (TZstart != -1)
                    {
                        string TZstr = StrDate.Substring(TZstart + 1); // +1 to avoid the space character
                        Hashtable ZoneTable = fillHashTimeZone();
                        string TZvalue = ZoneTable[TZstr] as string;

                        // its a timezone problem ...
                        if (TZvalue != null)
                        {
                            // replace timezone name with actual hours (AEST = +1000)
                            string NewDateStr = StrDate.Replace(TZstr, TZvalue);

                            try
                            {
                                ConvertedDate = Convert.ToDateTime(NewDateStr);

                                // success this time
                                return ConvertedDate;
                            }
                            catch (FormatException innerfex)
                            {
                                // something else wrong, we dont know what to do
                                throw innerfex;
                            }
                        }
                        else
                        {
                            return ConvertedDate;
                        }
                    }
                    else
                    {
                        ConvertedDate = Convert.ToDateTime(StrDate);
                        // just a plain date
                        return ConvertedDate;
                    }
                }
                catch (FormatException fex)
                {
                    // try finding a timezone in the date
                    int TZstart = StrDate.LastIndexOf(" ");

                    if (TZstart != -1)
                    {
                        string TZstr = StrDate.Substring(TZstart + 1); // +1 to avoid the space character
                        Hashtable ZoneTable = fillHashTimeZone();
                        string TZvalue = ZoneTable[TZstr] as string;

                        // its a timezone problem ...
                        if (TZvalue != null)
                        {
                            // replace timezone name with actual hours (AEST = +1000)
                            string NewDateStr = StrDate.Replace(TZstr, TZvalue);

                            try
                            {
                                ConvertedDate = Convert.ToDateTime(NewDateStr);

                                // success this time
                                return ConvertedDate;
                            }
                            catch (FormatException innerfex)
                            {
                                // something else wrong, we dont know what to do
                                throw innerfex;
                            }
                        }
                    }

                    throw fex;
                }
            }
        }

        private string getPubDate(DateTime feedPubDate)
        {
            string pubDate = string.Empty;
            TimeSpan ts = DateTime.Now.GetCourseDateTime(Context) - feedPubDate;
            if (ts.Days < 8 && ts.Days > 0)
            {
                if (ts.Days > 1)
                {
                    pubDate = string.Format("{0} Days ago", ts.Days);
                }
                else
                {
                    pubDate = string.Format("{0} Day ago", ts.Days);
                }
            }
            else if (ts.Days > 7 || ts.Hours < 0)
            {
                string format = "MMM d, yyyy HH:mm tt";
                pubDate = feedPubDate.ToString(format);
            }
            else if (ts.Hours > 0)
            {
                pubDate = string.Format("{0} Hours {1} Minutes ago", ts.Hours.ToString(), ts.Minutes.ToString());
            }
            else if (ts.Hours == 0)
            {
                pubDate = string.Format("{0} Minutes ago", Math.Abs(ts.Minutes).ToString());
            }

            return pubDate;
        }
        #endregion

        #region datetime utils

        private string[][] TimeZones = new string[][] {
            new string[] {"ACDT", "+1030", "Australian Central Daylight"},
            new string[] {"ACST", "+0930", "Australian Central Standard"},
            new string[] {"ADT", "-0300", "(US) Atlantic Daylight"},
            new string[] {"AEDT", "+1100", "Australian East Daylight"},
            new string[] {"AEST", "+1000", "Australian East Standard"},
            new string[] {"AHDT", "-0900", ""},
            new string[] {"AHST", "-1000", ""},
            new string[] {"AST", "-0400", "(US) Atlantic Standard"},
            new string[] {"AT", "-0200", "Azores"},
            new string[] {"AWDT", "+0900", "Australian West Daylight"},
            new string[] {"AWST", "+0800", "Australian West Standard"},
            new string[] {"BAT", "+0300", "Bhagdad"},
            new string[] {"BDST", "+0200", "British Double Summer"},
            new string[] {"BET", "-1100", "Bering Standard"},
            new string[] {"BST", "-0300", "Brazil Standard"},
            new string[] {"BT", "+0300", "Baghdad"},
            new string[] {"BZT2", "-0300", "Brazil Zone 2"},
            new string[] {"CADT", "+1030", "Central Australian Daylight"},
            new string[] {"CAST", "+0930", "Central Australian Standard"},
            new string[] {"CAT", "-1000", "Central Alaska"},
            new string[] {"CCT", "+0800", "China Coast"},
            new string[] {"CDT", "-0500", "(US) Central Daylight"},
            new string[] {"CED", "+0200", "Central European Daylight"},
            new string[] {"CET", "+0100", "Central European"},
            new string[] {"CST", "-0600", "(US) Central Standard"},
            new string[] {"CENTRAL", "-0600", "(US) Central Standard"},
            new string[] {"EAST", "+1000", "Eastern Australian Standard"},
            new string[] {"EDT", "-0400", "(US) Eastern Daylight"},
            new string[] {"EED", "+0300", "Eastern European Daylight"},
            new string[] {"EET", "+0200", "Eastern Europe"},
            new string[] {"EEST", "+0300", "Eastern Europe Summer"},
            new string[] {"EST", "-0500", "(US) Eastern Standard"},
            new string[] {"EASTERN", "-0500", "(US) Eastern Standard"},
            new string[] {"FST", "+0200", "French Summer"},
            new string[] {"FWT", "+0100", "French Winter"},
            new string[] {"GMT", "-0000", "Greenwich Mean"},
            new string[] {"GST", "+1000", "Guam Standard"},
            new string[] {"HDT", "-0900", "Hawaii Daylight"},
            new string[] {"HST", "-1000", "Hawaii Standard"},
            new string[] {"IDLE", "+1200", "Internation Date Line East"},
            new string[] {"IDLW", "-1200", "Internation Date Line West"},
            new string[] {"IST", "+0530", "Indian Standard"},
            new string[] {"IT", "+0330", "Iran"},
            new string[] {"JST", "+0900", "Japan Standard"},
            new string[] {"JT", "+0700", "Java"},
            new string[] {"MDT", "-0600", "(US) Mountain Daylight"},
            new string[] {"MED", "+0200", "Middle European Daylight"},
            new string[] {"MET", "+0100", "Middle European"},
            new string[] {"MEST", "+0200", "Middle European Summer"},
            new string[] {"MEWT", "+0100", "Middle European Winter"},
            new string[] {"MST", "-0700", "(US) Mountain Standard"},
            new string[] {"MOUNTAIN", "-0700", "(US) Mountain Standard"},
            new string[] {"MT", "+0800", "Moluccas"},
            new string[] {"NDT", "-0230", "Newfoundland Daylight"},
            new string[] {"NFT", "-0330", "Newfoundland"},
            new string[] {"NT", "-1100", "Nome"},
            new string[] {"NST", "+0630", "North Sumatra"},
            new string[] {"NZ", "+1100", "New Zealand "},
            new string[] {"NZST", "+1200", "New Zealand Standard"},
            new string[] {"NZDT", "+1300", "New Zealand Daylight "},
            new string[] {"NZT", "+1200", "New Zealand"},
            new string[] {"PDT", "-0700", "(US) Pacific Daylight"},
            new string[] {"PST", "-0800", "(US) Pacific Standard"},
            new string[] {"PACIFIC", "-0800", "(US) Pacific Standard"},
            new string[] {"ROK", "+0900", "Republic of Korea"},
            new string[] {"SAD", "+1000", "South Australia Daylight"},
            new string[] {"SAST", "+0900", "South Australia Standard"},
            new string[] {"SAT", "+0900", "South Australia Standard"},
            new string[] {"SDT", "+1000", "South Australia Daylight"},
            new string[] {"SST", "+0200", "Swedish Summer"},
            new string[] {"SWT", "+0100", "Swedish Winter"},
            new string[] {"USZ3", "+0400", "USSR Zone 3"},
            new string[] {"USZ4", "+0500", "USSR Zone 4"},
            new string[] {"USZ5", "+0600", "USSR Zone 5"},
            new string[] {"USZ6", "+0700", "USSR Zone 6"},
            new string[] {"UT", "-0000", "Universal Coordinated"},
            new string[] {"UTC", "-0000", "Universal Coordinated"},
            new string[] {"UZ10", "+1100", "USSR Zone 10"},
            new string[] {"WAT", "-0100", "West Africa"},
            new string[] {"WET", "-0000", "West European"},
            new string[] {"WST", "+0800", "West Australian Standard"},
            new string[] {"YDT", "-0800", "Yukon Daylight"},
            new string[] {"YST", "-0900", "Yukon Standard"},
            new string[] {"ZP4", "+0400", "USSR Zone 3"},
            new string[] {"ZP5", "+0500", "USSR Zone 4"},
            new string[] {"ZP6", "+0600", "USSR Zone 5"},
            new string[] {"+0000", "+0000", "Greenwich Mean"}
        };

        private Hashtable fillHashTimeZone()
        {
            Hashtable ZoneTable = new Hashtable(50);

            foreach (string[] TimeZone in TimeZones)
            {
                ZoneTable.Add(TimeZone[0], TimeZone[1]);
            }
            return ZoneTable;
        }

        #endregion




    }
}
