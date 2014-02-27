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
    public static class QuickLinkMapper
    {

        /// <summary>
        /// Mapping the QuickLink to ContentItem.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this QuickLink model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "CustomActivity";
                biz.Properties["bfw_type"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.Type };
                biz.Properties["LinkedItemId"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.LinkedItemId };
                biz.Properties["LinkUrl"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.LinkUrl };
            }

            return biz;
        }

        /// <summary>
        /// Mapping the ContentItem to a QuickLink.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static QuickLink ToQuickLink(this BizDC.ContentItem biz, BizSC.IContentActions content)
        {
            var model = new QuickLink();
            model.ToBaseContentItem(biz);

            model.LinkedItemId = biz.Properties.ContainsKey("LinkedItemId") ? biz.Properties["LinkedItemId"].Value.ToString() : "";
            model.LinkUrl = biz.Properties.ContainsKey("LinkUrl") ? biz.Properties["LinkUrl"].Value.ToString() : "";
            model.Type = biz.Properties.ContainsKey("bfw_type") ? biz.Properties["bfw_type"].Value.ToString() : "";

            return model;
        }

    }
}
