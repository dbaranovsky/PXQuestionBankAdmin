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
    public static class FeaturedContentItem
    {
        /// <summary>
        /// Maps a FeaturedItem business object to a FeaturedContentItem model.
        /// </summary>
        /// <param name="biz">The FeaturedItem business object.</param>
        /// <returns>
        /// FeaturedContentItem model.
        /// </returns>
        public static Models.FeaturedContentItem ToFeaturedContentItem(this Biz.DataContracts.ContentItem biz)
        {
            var model = new Models.FeaturedContentItem();

            if (null != biz)
            {
                string image = biz.Properties.ContainsKey("image") ? biz.Properties["image"].As<string>() : "#";
                string title = biz.Properties.ContainsKey("title") ? biz.Properties["title"].As<string>() : string.Empty;
                string id = biz.Properties.ContainsKey("targetid") ? biz.Properties["targetid"].As<string>() : string.Empty;

                model.Id = id;
                model.ImageUrl = new Uri(image, UriKind.RelativeOrAbsolute);
                model.Title = title;
            }

            return model;
        }
    }
}
