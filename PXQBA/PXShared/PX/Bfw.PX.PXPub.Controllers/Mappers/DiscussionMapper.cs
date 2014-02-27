using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class DiscussionMapper
    {

        /// <summary>
        /// Convert to a BizDC.ContentItem from a Discussion Model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this Discussion model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "Discussion";
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
                sw.Write(System.Web.HttpUtility.HtmlDecode(model.Description));
                sw.Flush();

                biz.Resources = new List<BizDC.Resource>() { rez };
            }

            return biz;
        }

        /// <summary>
        /// Maps a generic ContentItem from the business layer to a Discussion contentitem.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static Discussion ToDiscussion(this BizDC.ContentItem biz, BizSC.IContentActions content, bool loadAttachments = false)
        {
            var model = new Discussion();
            model.ToBaseContentItem(biz);

            // Load attachments
            if (loadAttachments)
            {
                var context = ServiceLocator.Current.GetInstance<BizSC.IBusinessContext>();

                var docCollection = content.GetContent(context.CourseId, biz.Id + "_DOCS", true);
                if (docCollection != null)
                {
                    model.DocumentCollection = (DocumentCollection)docCollection.ToContentItem(content);
                }
            }

          

            return model;
        }
    }
}
