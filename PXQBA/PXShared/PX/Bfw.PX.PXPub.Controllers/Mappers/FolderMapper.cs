using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class FolderMapper
    {
        /// <summary>
        /// Maps a generic ContentItem from the business layer to a Document contentitem.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static Folder ToFolder(this BizDC.ContentItem biz)
        {
            var theme  = string.Empty;
            if (biz.Properties.Keys.Contains("SelectedTheme"))
            {
                theme = biz.Properties["SelectedTheme"].Value.ToString();
            }
            var bannerImage = string.Empty;
            if (biz.Properties.Keys.Contains("BannerImage"))
            {
                bannerImage = biz.Properties["BannerImage"].Value.ToString();
            }
            var studentDescription = string.Empty;
            if (biz.Properties.Keys.Contains("StudentDescription"))
            {
                studentDescription = biz.Properties["StudentDescription"].Value.ToString();
            }

            return new Folder
            {
                Id = biz.Id,
                Title = biz.Title,
                ParentId = biz.ParentId,
                Sequence = biz.Sequence,
                Url = biz.Href,
                Description = studentDescription,
                Theme = theme,
                Image = bannerImage,
            };
        }

        /// <summary>
        /// Maps a Folder to a generic ContentItem from a Folder Model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this Folder model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "Folder";
                biz.Href = string.Format("Templates/Data/{0}/index.html", biz.Id);
            }

            return biz;
        }

        /// <summary>
        /// For a folder, load its child items.
        /// </summary>
        /// <param name="folder">The folder to act upon.</param>
        /// <param name="content">An implementation of the IContentActions interface.</param>
        /// <param name="entityId">The entity to which the folder belongs.</param>
        public static void LoadChildren(this Folder folder, BizSC.IContentActions content, string entityId)
        {
            folder.Folders = new List<TocItem>();
            folder.Items = new List<TocItem>();
            var bizChildren = content.ListChildren(entityId, folder.Id);
            foreach (var bizChild in bizChildren)
            {
                if (!bizChild.Hidden)
                {
                    var i = bizChild.ToContentItem(content, false);

                    if (i is Folder)
                    {
                        folder.Folders.Add(new TocItem(bizChild.Title, bizChild.Id, bizChild.Type, "", null));
                    }
                    else
                    {
                        folder.Items.Add(new TocItem(bizChild.Title, bizChild.Id, bizChild.Type, "", null));
                    }
                }
            }
        }
    }
}
