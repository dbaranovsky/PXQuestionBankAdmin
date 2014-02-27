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
    public static class DocumentCollectionMapper
    {
        /// <summary>
        /// Maps a generic ContentItem from the business layer to a Document contentitem.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static DocumentCollection ToDocumentCollection(this BizDC.ContentItem biz, BizSC.IContentActions content, bool loadDocuments = false)
        {
            var model = new DocumentCollection();

            model.Id = biz.Id;
            model.Title = biz.Title;
            model.ParentId = biz.ParentId;
            model.Sequence = biz.Sequence;
            if (!biz.Resources.IsNullOrEmpty())
            {
                var desc = biz.Resources.FirstOrDefault().GetStream();
                using (var sw = new System.IO.StreamReader(desc))
                {
                    model.Description = sw.ReadToEnd();
                }
            }

            var context = content.Context;
            if (loadDocuments)
            {
                var docs = content.ListChildren(context.CourseId, biz.Id);

                if (!docs.IsNullOrEmpty())
                {
                    var items = docs.Map(d => d.ToDocument(content)).ToList();
                    items.RemoveAll(i => i.FileName == "" && i.Size == 0 && i.ContentType != null);
                    model.Documents = items;
                }
            }
            if (!biz.Properties.IsNullOrEmpty())
                {
                    if (biz.Properties.ContainsKey("FileName"))
                    {
                        model.ExtendedProperties.Add("FileName", biz.Properties["FileName"].Value);
                    }
                    if (biz.Properties.ContainsKey("CreationDate"))
                    {
                        model.ExtendedProperties.Add("CreationDate", biz.Properties["CreationDate"].Value);
                    }
                    if (biz.Properties.ContainsKey("FileSize"))
                    {
                        model.ExtendedProperties.Add("FileSize", biz.Properties["FileSize"].Value);
                    }
                    if (biz.Properties.ContainsKey("DocId"))
                    {
                        model.ExtendedProperties.Add("DocId", biz.Properties["DocId"].Value);
                    }
                }
            
            List<TocCategory> modelCats = new List<TocCategory>();
            if (biz.Categories != null)
            {
                foreach (Bfw.PX.Biz.DataContracts.TocCategory cat in biz.Categories)
                {
                    modelCats.Add(cat.ToTocCategory());
                }
            }
            model.Categories = modelCats;
            return model;
        }

        /// <summary>
        /// Convert to a BizDC.ContentItem from a Document Collection Model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static Biz.DataContracts.ContentItem ToContentItem(this DocumentCollection model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "Resource";
                biz.Subtype = "DocumentCollection";
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

                biz.Resources = new List<Biz.DataContracts.Resource>() { rez };
            }

            return biz;
        }
    }
}
