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
    public static class AssetLinkMapper
    {
        /// <summary>
        /// Maps a generic ContentItem from the business layer to a AssetLink contentitem.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static AssetLink ToAssetLink(this BizDC.ContentItem biz)
        {
            var model = new AssetLink();

            model.ToBaseContentItem(biz);
            if (biz != null)
            {
                model.Url = biz.Href;
            }

            return model;
        }
    }
}
