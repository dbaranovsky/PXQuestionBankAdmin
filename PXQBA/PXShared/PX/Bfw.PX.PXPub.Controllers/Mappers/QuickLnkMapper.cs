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
    public static class QuickLnkMapper
   {

        /// <summary>
        /// Mapping the ContentItem to a simple quick link.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static QuickLnk ToQuickLnk(this BizDC.ContentItem biz, BizSC.IContentActions content)
        {
            var model = new QuickLnk();            

            model.LinkedItemId = biz.Properties.ContainsKey("LinkedItemId") ? biz.Properties["LinkedItemId"].Value.ToString() : "";
            model.LinkUrl = biz.Properties.ContainsKey("LinkUrl") ? biz.Properties["LinkUrl"].Value.ToString() : "";
            model.LinkTitle = biz.Title;


            return model;
        }

    }
}
