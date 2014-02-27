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
    public static class HtmlDocumentMapper
    {
        /// <summary>
        /// Provides a conversion to the HtmlDocument type.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static HtmlDocument ToHtmlDocument(this BizDC.ContentItem biz)
        {
            var model = new HtmlDocument();

            model.ToBaseContentItem(biz);
            model.DefaultPoints = biz.DefaultPoints;
            model.ApplicableEnrollmentId = biz.CourseId;

            if (!biz.Resources.IsNullOrEmpty())
            {
                var desc = biz.Resources.FirstOrDefault().GetStream();
                using (var sw = new System.IO.StreamReader(desc))
                {
                    model.Body = sw.ReadToEnd();
                }
            }

            //image paths in html docs are "../../../../BrainHoney/Resource/[dicspId]....", we have to remove all ../ and just have / so that path can be resolved by domain
            if (!string.IsNullOrEmpty(model.Body))
            {
                model.Body = model.Body.Replace("../BrainHoney/Resource", "/BrainHoney/Resource");
                model.Body = model.Body.Replace("../", string.Empty);
            }

            return model;
        }

        
        /// <summary>
        /// Maps an HtmlDocument to a generic ContentItem.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this HtmlDocument model, string courseId)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            var biz = model.ToBaseContentItem(courseId);

            biz.Type = "Resource";
            biz.Subtype = "HtmlDocument";
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
            sw.Write(System.Web.HttpUtility.HtmlDecode(model.Body));
            sw.Flush();

            biz.Resources = new List<BizDC.Resource>() {rez};

            return biz;
        }
    }
}
