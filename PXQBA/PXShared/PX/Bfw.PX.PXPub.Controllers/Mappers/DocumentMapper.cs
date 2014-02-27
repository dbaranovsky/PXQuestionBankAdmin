using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.IO;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class DocumentMapper
    {
        /// <summary>
        /// Maps a Document to a generic ContentItem.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this Document model, string courseId, System.Web.HttpPostedFileBase file)
        {
            Stream inputStream = null;
            string fileName = string.Empty;
            long fileLength = 0;
            string contentType = string.Empty;

            if (file != null)
            {
                inputStream = file.InputStream;
                fileName = System.IO.Path.GetFileName(file.FileName);
                fileLength = file.InputStream.Length;
                contentType = file.ContentType;
            }

            return ToContentItem(model, courseId, inputStream, fileName, fileLength, contentType);
        }

        /// <summary>
        /// Maps a Document to a generic ContentItem.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="inputStream">The InputStream of the document.</param>
        /// <param name="fileName">The FileName of the document.</param>
        /// <param name="fileLength">The document's length.</param>
        /// <param name="contentType">The document's content type.</param>
        public static BizDC.ContentItem ToContentItem(this Document model, string courseId, Stream inputStream, string fileName, long fileLength, string contentType)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "AssetLink";
                biz.Subtype = "Document";
                biz.Hidden = true;
                biz.Href = string.Format("Assets/{0}", model.FileName);

                if (null != inputStream)
                {
                    model.FileName = fileName;
                    biz.Href = string.Format("Assets/{0}", model.FileName);
                    model.Size = fileLength;
                    model.ContentType = contentType;

                    if (model.Uploaded.Year == DateTime.MinValue.Year)
                    {
                        model.Uploaded = DateTime.Now.GetCourseDateTime();
                    }

                    var rez = new BizDC.Resource()
                    {
                        ContentType = contentType,
                        Extension = System.IO.Path.GetExtension(fileName).Replace(".", ""),
                        Status = BizDC.ResourceStatus.Normal,
                        Url = biz.Href,
                        EntityId = courseId
                    };

                    var dest = rez.GetStream();
                    inputStream.Position = 0;
                    inputStream.Copy(dest);
                    dest.Flush();

                    biz.Resources = new List<BizDC.Resource> { rez };
                }

                biz.Properties["filename"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.FileName };
                biz.Properties["uploaded"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.DateTime, Value = model.Uploaded };
                biz.Properties["filesize"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.Integer, Value = model.Size };
                biz.Properties["mimetype"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.ContentType };
            }

            return biz;
        }

        /// <summary>
        /// Maps a generic ContentItem from the business layer to a Document contentitem.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static Document ToDocument(this BizDC.ContentItem biz, BizSC.IContentActions content)
        {
            var model = new Document();

            try
            {
                model.Id = biz.Id;
                model.Title = biz.Title;
                model.ParentId = biz.ParentId;
                model.FileName = "";
                model.DefaultPoints = biz.DefaultPoints;
              
                if (biz.Properties.Keys.Contains("filename"))
                {
                    model.FileName = biz.Properties["filename"].As<string>();
                }

                if (biz.Properties.Keys.Contains("filesize"))
                {
                    model.Size = biz.Properties["filesize"].As<long>();
                }

                if (biz.Properties.Keys.Contains("uploaded"))
                {
                    model.Uploaded = biz.Properties["uploaded"].As<DateTime>();
                }

                model.ContentType = biz.Properties.ContainsKey("mimetype") ? biz.Properties["mimetype"].As<string>() : string.Empty;
                model.Sequence = biz.Sequence;
                
            }
            catch { }

            return model;
        }
    }
}
