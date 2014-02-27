using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class RssFeedMapper
    {
        /// <summary>
        /// Convert to a BizDC.ContentItem from an RssFeed Model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this RssFeed model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "RssFeed";
                biz.Href = model.RssUrl;
            }

            return biz;
        }



        /// <summary>
        /// Convert to a BizDC.ContentItem from an RssLink Model.
        /// </summary>
        //
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this RssLink model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "CustomActivity";
                biz.ParentId = "PX_MANIFEST";
                biz.Subtype = "RSSLink";
                biz.Href = model.Link;
                biz.Title = model.LinkTitle;
                biz.Description = model.LinkDescription;
                biz.Properties["bfw_RssUrl"] = new BizDC.PropertyValue { Type = BizDC.PropertyType.String, Value = model.RssUrl };
                biz.Properties["bfw_RssArticlePubDate"] = new BizDC.PropertyValue { Type = BizDC.PropertyType.String, Value = model.PubDate };
            }
            return biz;
        }


        /// <summary>
        /// Maps a generic ContentItem from the business layer to a RSSLink contentitem.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static RssLink ToRssLink(this BizDC.ContentItem biz)
        {  
            var model = new RssLink
            {
                Id = biz.Id,
                Title = biz.Title,
                ParentId = biz.ParentId,
                Sequence = biz.Sequence,
                Link = biz.Href,
                Description = biz.Description,
                RssUrl = (biz.Properties.ContainsKey("bfw_RssUrl")? biz.Properties["bfw_RssUrl"].Value.ToString(): ""),
                PubDate = (biz.Properties.ContainsKey("bfw_RssArticlePubDate")? biz.Properties["bfw_RssArticlePubDate"].Value.ToString(): ""),
                DefaultPoints = biz.DefaultPoints,

            };
            model.Categories = biz.Categories.Map(cat => cat.ToTocCategory());


            return model;
        }


        /// <summary>
        /// Maps a generic ContentItem from the business layer to a RSSFeed contentitem.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static RssFeed ToRssFeed(this BizDC.ContentItem biz)
        {
            var model = new RssFeed
            {
                Id = biz.Id,
                Title = biz.Title,
                ParentId = biz.ParentId,
                Sequence = biz.Sequence,
                RssUrl = biz.Href,
                
            };
            model.DefaultPoints = biz.DefaultPoints;
            model.RssFeeds = model.GetFeeds(model.RssUrl);

            return model;
        }



        /// <summary>
        /// Maps a generic RSSFeed from the business layer to a RSSFeed.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static RssLink ToRssLink(this BizDC.RSSFeed biz)
        {
            var model = new RssLink
            {
                RssUrl = biz.RssUrl,
                Link = biz.Link,
                LinkDescription = biz.LinkDescription,
                Author = biz.Author,
                PubDate = biz.PubDate,
                PubDateCalculated = biz.PubDateCalculated,
                LinkTitle = biz.LinkTitle,
                Source = biz.Source,
                IsArchived = biz.IsArchived,
                ArchivedItemId = biz.ArchivedItemId,
                IsAssigned = biz.IsAssigned,
                AssignedDate = biz.AssignedDate,
                FeedCounter = biz.FeedCounter
            };
            return model;
        }

    }
}
