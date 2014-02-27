using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class FeaturedWidgetMapper
    {
        /// <summary>
        /// Provides a conversion to Feature Content Widget.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static FeaturedWidget ToFeatureWidget(this BizDC.ContentItem biz, BizSC.IContentActions content)
        {
            var model = new FeaturedWidget();

            model.Title = biz.Title;

            if (biz.Properties.ContainsKey("content-type"))
            {
                model.ContentType = biz.Properties["content-type"].Value.ToString();
            }

            if (!biz.Resources.IsNullOrEmpty())
            {
                var descStream = biz.Resources.FirstOrDefault().GetStream();
                using (var sw = new StreamReader(descStream))
                {
                    model.Description = HttpUtility.HtmlDecode(sw.ReadToEnd());
                }
            }

            return model;
        }
    }
}
