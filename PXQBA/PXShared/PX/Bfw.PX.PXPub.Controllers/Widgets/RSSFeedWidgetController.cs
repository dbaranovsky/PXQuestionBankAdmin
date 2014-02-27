
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using System.Text.RegularExpressions;

using Bfw.Common.Collections;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provides all actions necessary to support the RSS Feed Widget.
    /// </summary>
    [PerfTraceFilter]
    public class RSSFeedWidgetController : Controller, IPXWidget
    {

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// The rss feed actions.
        /// </summary>
        /// <value>
        /// The rss feed actions.
        /// </value>
        protected BizSC.IRSSFeedActions RSSFeedActions { get; set; }

        /// <summary>
        /// Depends on the IBusinessContext and IRssFeedActions interfaces.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rssfeedActions">The rss feed actions.</param>
        /// <param name="pageActions">The Page actions.</param>
        public RSSFeedWidgetController(BizSC.IBusinessContext context, BizSC.IRSSFeedActions rssfeedActions, BizSC.IPageActions pageActions)
        {
            Context = context;
            RSSFeedActions = rssfeedActions;
            PageActions = pageActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Get RSS widget Url
        /// </summary>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        protected RSSFeedWidget GetRSSWidgetUrl(string widgetId)
        {
            string rssUrl = RSSFeedActions.GetWidgetRSSUrl(widgetId);
            var model = new RSSFeedWidget();

            if (!rssUrl.IsNullOrEmpty())
            {
                model.RSSUrl = rssUrl;
            }
            return model;
        }

        /// <summary>
        /// Get the RSS Feed Widget Model for any given WidgetId
        /// </summary>
        /// <param name="widgetId">WidgetId</param>
        /// <returns>RSSFeedWidget</returns>
        protected RSSFeedWidget GetRSSFeeds(string widgetId, int currentPageIndex, out string templateId, out string feedDescription, out string imageURL, int retrievalLimit = 5)
        {
            string title = string.Empty;
            templateId = string.Empty;
            feedDescription = string.Empty;
            imageURL = string.Empty;

            int totalArchivedArticles = 0;
            var biz = RSSFeedActions.ListRssFeeds(widgetId, currentPageIndex, out title, out totalArchivedArticles, out templateId, out feedDescription, out imageURL, retrievalLimit);
            var model = new RSSFeedWidget();

            if (!biz.IsNullOrEmpty())
            {
                model.Title = title;
                model.TotalArchivedArticles = totalArchivedArticles;
                model.RSSFeeds = biz.Map(b => b.ToRssLink()).ToList();
            }

            return model;
        }

        /// <summary>
        /// Get all the RSS Archived Articles for any given WidgetId
        /// </summary>
        /// <param name="widgetId">WidgetId</param>
        /// <returns>RSSFeedWidget</returns>
        protected RSSFeedWidget GetArchivedRSSFeeds(string widgetId)
        {
            string title = string.Empty;
            var biz = RSSFeedActions.ListRssArchivedArticles(widgetId, out title);
            var model = new RSSFeedWidget();

            if (!biz.IsNullOrEmpty())
            {
                model.Title = title;
                model.RSSFeeds = biz.Map(b => b.ToRssLink()).ToList();
                model.TotalArchivedArticles = model.RSSFeeds.Count();
            }

            return model;
        }

        /// <summary>
        /// Gets the list of all RSS Feed for the current entity and renders them in a view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult View(Bfw.PX.PXPub.Models.Widget model)
        {
            string widgetId = model.Id;
            string templateId = string.Empty;
            string feedDescription = string.Empty;
            string imageURL = string.Empty;

            ViewData.Model = GetRSSFeeds(widgetId, 0, out templateId, out feedDescription, out imageURL);
            ViewData["feedDescription"] = feedDescription;
            imageURL.Replace("\n", "");
            ViewData["imageURL"] = imageURL;

            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                ViewData["InstructorAccessLevel"] = true;
            }
            else
            {
                ViewData["InstructorAccessLevel"] = false;
            }

            return View("Summary");
        }

        /// <summary>
        /// Gets the list of all RSS Feed for the current entity and renders them in a view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult CompactView(Bfw.PX.PXPub.Models.Widget model)
        {
            string widgetId = model.Id;
            string templateId = string.Empty;
            string feedDescription = string.Empty;
            string imageURL = string.Empty;

            ViewData.Model = GetRSSFeeds(widgetId, 0, out templateId, out feedDescription, out imageURL);
            ViewData["feedDescription"] = feedDescription;
            ViewData["imageURL"] = imageURL;

            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                ViewData["InstructorAccessLevel"] = true;
            }
            else
            {
                ViewData["InstructorAccessLevel"] = false;
            }
            return View("CompactSummary");
        }

        /// <summary>
        /// Gets the list of all RSS Feed for the current entity and renders them in a view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult CompactViewFPStart(Bfw.PX.PXPub.Models.Widget model)
        {
            var rss = new RSSFeedWidget();

            if (rss.Title.IsNullOrEmpty())
            {
                rss.Title = model.Title;
                ViewData.Model = rss;
            }

            ViewData["feedDescription"] = string.Empty;
            ViewData["imageURL"] = string.Empty;

            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                ViewData["InstructorAccessLevel"] = true;
            }
            else
            {
                ViewData["InstructorAccessLevel"] = false;
            }

            return View("CompactSummaryFPStart");
        }

        /// <summary>
        /// Gets the list of all RSS Feed for the current entity and renders them in a view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult CompactViewFacePlate(Bfw.PX.PXPub.Models.Widget model)
        {
            ViewData["loadOnReady"] = "true";
            ViewData["Id"] = model.Id;
            if (model.Properties.ContainsKey("bfw_feed_items_to_show"))
            {
                int retrievalLimit = Int32.Parse(model.Properties["bfw_feed_items_to_show"].Value.ToString());
                ViewData["retrievalLimit"] = retrievalLimit;
                ViewData["scrollingRestricted"] = "true";
            }
            else
            {
                ViewData["retrievalLimit"] = 5;
            }
            return View("CompactSummaryFacePlate");
        }

        public ActionResult CompactViewFacePlateLoad(Bfw.PX.PXPub.Models.Widget model, int retrievalLimit = 5, string scrollingRestricted = "false")
        {
            string widgetId = model.Id;
            string templateId = string.Empty;
            string feedDescription = string.Empty;
            string imageURL = string.Empty;
            
            ViewData.Model = GetRSSFeeds(widgetId, 0, out templateId, out feedDescription, out imageURL, retrievalLimit);
            ViewData["feedDescription"] = feedDescription;
            ViewData["imageURL"] = imageURL;
            if (scrollingRestricted.ToLower().Equals("true"))
            {
                ViewData["retrievalCap"] = retrievalLimit;
            }

            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                ViewData["InstructorAccessLevel"] = true;
            }
            else
            {
                ViewData["InstructorAccessLevel"] = false;
            }

            if (Context.Course.CourseType.ToLower().Equals("faceplate"))
            {
                ViewData["FaceplateView"] = true;
            }

            return View("CompactSummaryFacePlate");
        }

        /// <summary>
        /// To get the arguments which is required before add a widget
        /// </summary>
        /// <returns></returns>
        public ActionResult OnBeforeAdd(Bfw.PX.PXPub.Models.Widget model)
        {
            if (model != null)
            {
                string widgetId = model.Id;
                ViewData.Model = GetRSSWidgetUrl(widgetId);
            }
            return View("AddEdit");
        }

        /// <summary>
        /// Add Custom RSS Widget under PX_MANIFEST
        /// </summary>
        /// <param name="RssFeedUrl"></param>
        /// <param name="WidgetZoneID"></param>
        /// <param name="WidgetTemplateID"></param>
        /// <returns></returns>
        public ActionResult AddRSSWidget(string rssFeedUrl, string pageName, string WidgetZoneID, string WidgetTemplateID, string WidgetID, string PrevSeq, string NextSeq)
        {
            //// pending: validation
            string errorMsg = string.Empty;
            string result = string.Empty;
            string widgetId = string.Empty;
            string mode = string.Empty;
            string feedUrl = rssFeedUrl;

            if (string.IsNullOrEmpty(feedUrl))
            {
                result = "Fail";
                errorMsg = "You must specify a URL";
            }
            else if (!IsValidUrl(feedUrl))
            {
                result = "Fail";
                errorMsg = "The specified URL is not a valid RSS feed";
            }
            else
            {
                bool rssURLExist = this.RSSFeedActions.ValidateRssURL(feedUrl);
                if (!rssURLExist)
                {
                    result = "Fail";
                    errorMsg = "Please enter a valid URL.";
                }
                else
                {
                    var properties = new Dictionary<string, BizDC.PropertyValue>();
                    properties["bfw_feedurl"] = new BizDC.PropertyValue()
                    {
                        Type = BizDC.PropertyType.String,
                        Value = feedUrl
                    };


                    if (WidgetID.IsNullOrEmpty())
                    {
                        mode = "ADD";
                        var newWidget = PageActions.AddWidget(pageName, WidgetZoneID, WidgetTemplateID, PrevSeq, NextSeq, "", properties);
                        if (newWidget != null)
                        {
                            result = "Success";
                            widgetId = newWidget.Id;
                        }
                        else
                        {
                            result = "Fail";
                        }
                    }
                    else
                    {
                        result = "Success";
                        mode = "EDIT";
                        var oldWidget = PageActions.UpdateWidget(pageName, WidgetID, null, properties);
                    }
                }
            }
            return Json(new { Result = result, Mode = mode, OldWidgetID = WidgetID, ErrorMes = errorMsg, ZoneId = WidgetZoneID, WidgetTemplateID = WidgetTemplateID, WidgetId = widgetId });
        }

        /// <summary>
        /// Validate RSS Feed URL 
        /// </summary>
        /// <param name="urlToValidate"></param>
        /// <returns>bool</returns>
        protected bool IsValidUrl(string urlToValidate)
        {
            try
            {
                Uri myUri = new Uri(urlToValidate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the list of all RSS Feeds for the current entity and renders them in a view.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View("Summary");
        }

        /// <summary>
        /// Gets the list of all RSS Feeds for the current entity and renders them in a view.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public ActionResult ViewAllArchived(string widgetId)
        {
            ViewData.Model = GetArchivedRSSFeeds(widgetId);
            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                ViewData["InstructorAccessLevel"] = true;
            }
            else
            {
                ViewData["InstructorAccessLevel"] = false;
            }
            ViewData["WidgetId"] = widgetId;
            return View("ViewAllArchived");
        }

        /// <summary>
        /// Gets the list of all RSS Feeds for the current entity and renders them in a view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            return View("Summary");
        }

        /// <summary>
        /// Gets the partial list of RSS Feed for the current entity and renders them in a view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult PartialListView(string widgetId, int currentPageIndex)
        {
            string templateId = string.Empty;
            string viewName = string.Empty;
            string feedDescription = string.Empty;
            string imageURL = string.Empty;

            RSSFeedWidget rssFeedWidget = GetRSSFeeds(widgetId, currentPageIndex, out templateId, out feedDescription, out imageURL);

            if (rssFeedWidget.RSSFeeds != null)
            {
                ViewData.Model = rssFeedWidget;
                if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
                {
                    ViewData["InstructorAccessLevel"] = true;
                }
                else
                {
                    ViewData["InstructorAccessLevel"] = false;
                }
                if (templateId == "PX_Custom_RSS_Feed" || templateId == "PX_ScientificAmerican_RSS_Feed")
                {
                    viewName = "SummaryPartial";
                }
                else if (templateId == "PX_Custom_RSS_Feed_Compact" || templateId == "PX_ScientificAmerican_RSS_Feed_Compact")
                {
                    viewName = "CompactSummaryPartial";
                }
                else if (templateId == "PX_FacePlate_ScientificAmerican_RSS_Feed_Compact" || templateId == "PX_FacePlate_ScientificAmerican_RSS_Feed_Compact" ||  templateId == "PX_FacePlate_SA_RSS_Feed_Widget" || templateId == "PX_FacePlate_TE_RSS_Feed_Widget")
                {
                    viewName = "CompactSummaryFacePlatePartial";
                }
                else if (templateId == "PX_Custom_RSS_Feed_FP_Start")
                {
                    viewName = "CompactSummaryFPStartPartial";
                }
                
                return View(viewName);
            }
            else
            {
                return Json(new { Result = "Success", Message = "NoMoreRSSFeeds" });
            }
        }

        #endregion
    }
}
