
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
    public static class CustomWidgetMapper
    {
        /// <summary>
        /// Provides a conversion to the HtmlDocument type.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static CustomWidget ToCustomWidget(this BizDC.ContentItem biz, BizSC.IContentActions content)
        {
            var model = new CustomWidget();

            model.ToBaseContentItem(biz);

            if (!biz.Resources.IsNullOrEmpty())
            {
                var desc = biz.Resources.FirstOrDefault().GetStream();
                using (var sw = new System.IO.StreamReader(desc))
                {
                    model.WidgetContents = sw.ReadToEnd();
                }
            }

            if (!model.Description.IsNullOrEmpty()) model.WidgetContents = model.Description;

            return model;
        }
      


        /// <summary>
        /// Maps an Custom widget to a generic ContentItem.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this CustomWidget model, string courseId)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            var biz = model.ToBaseContentItem(courseId);

            biz.Type = "CustomActivity";
            biz.Subtype = "Widget";
            biz.Href = string.Format("Templates/Data/{0}/index.html", biz.Id);

            var rez = new BizDC.Resource()
            {
                ContentType = "text/html",
                Extension = "html",
                Status = BizDC.ResourceStatus.Normal,
                Url = biz.Href,
                EntityId = courseId
            };

            var sw = new System.IO.StreamWriter(rez.GetStream());
            sw.Write(System.Web.HttpUtility.HtmlDecode(model.WidgetContents));
            sw.Flush();

            biz.Resources = new List<BizDC.Resource>() { rez };

            return biz;
        }

    
    }
}
