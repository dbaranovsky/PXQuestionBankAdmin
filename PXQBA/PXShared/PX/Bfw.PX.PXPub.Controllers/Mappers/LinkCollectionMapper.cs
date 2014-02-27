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
    public static class LinkCollectionMapper
    {

        /// <summary>
        /// Converts the ContentItem to a link collection and loads its child links.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static LinkCollection ToLinkCollection(this BizDC.ContentItem biz, BizSC.IContentActions content, bool loadLinks = false)
        {
            var model = new LinkCollection();

            model.Id = biz.Id;
            model.Title = biz.Title;
            model.ParentId = biz.ParentId;
            model.Sequence = biz.Sequence;
            model.Description = biz.Description;

            var context = ServiceLocator.Current.GetInstance<BizSC.IBusinessContext>();
            if (loadLinks)
            {
                var links = content.ListChildren(context.CourseId, biz.Id);

                if (!links.IsNullOrEmpty())
                {
                    model.Links = links.Map(d => d.ToLink()).ToList();
                }
            }

            return model;
        }

        /// <summary>
        /// Convert to a BizDC.ContentItem from a LinkCollection.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this LinkCollection model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "Resource";
                biz.Subtype = "LinkCollection";
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
    }
}
