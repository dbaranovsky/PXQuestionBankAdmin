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
    public static class LinkMapper
    {

        /// <summary>
        /// Converts a CustomActivity content item to a Link content item.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static Link ToLink(this BizDC.ContentItem biz)
        {
            var model = new Link();
            model.ToBaseContentItem(biz);

            model.Url = biz.Href;
            model.Sequence = biz.Sequence;
            model.Hidden = biz.Hidden;
            model.ExtendedLinkType = biz.Properties.ContainsKey("bfw_extendedlinktype") ? biz.Properties["bfw_extendedlinktype"].Value.ToString() : "";
            return model;
        }

        /// <summary>
        /// Maps a Link to a generic ContentItem.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this Link model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "CustomActivity";
                biz.Subtype = "Link";
                biz.Hidden = model.Hidden;
                biz.Href = model.Url;

                biz.Properties["bfw_extendedlinktype"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.ExtendedLinkType };
            }

            return biz;
        }

    }
}
