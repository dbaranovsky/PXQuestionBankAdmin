// -----------------------------------------------------------------------
// <copyright file="RelatedContentMapper.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class RelatedContentMapper
    {
        /// <summary>
        ///  /// Convert to DC content.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Biz.DataContracts.ContentItem ToContentItem(this RelatedContent model)
        {
            var biz = new Biz.DataContracts.ContentItem()
            {
                Id = model.Id,
                ResourceEntityId = model.EntityId,
                Type = "Resource",
            };
            if (!model.RelatedContents.IsNullOrEmpty())
            {
                biz.RelatedContents = model.RelatedContents.Map(i => i.ToDCRelatedContent()).ToList();
            }
            return biz;
        }

        /// <summary>
        ///  /// Convert to DC related content.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Biz.DataContracts.RelatedContent ToDCRelatedContent(this ContentItem model)
        {
            var biz = new Biz.DataContracts.RelatedContent()
            {
                Id = model.Id,
                Sequence = model.Sequence,
                Type = model.Type
            };

            return biz;
        }

        /// <summary>
        /// Convert to related content.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Biz.DataContracts.RelatedContent ToRelatedContent(this RelatedContent model)
        {
            var biz = new Biz.DataContracts.RelatedContent()
            {
                Id = model.Id,
                Sequence = model.Sequence,
                Type = model.Type
            };

            return biz;
        }

        /// <summary>
        /// Toes the content of the related.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <param name="loadRelatedContents">if set to <c>true</c> [load related contents].</param>
        /// <returns></returns>
        public static RelatedContent ToRelatedContent(this BizDC.ContentItem biz, BizSC.IContentActions content, bool loadRelatedContents)
        {
            if (biz == null) return null;
            var model = new RelatedContent();

            model.Id = biz.Id;
            model.Title = biz.Title;
            if (loadRelatedContents && !biz.RelatedContents.IsNullOrEmpty())
            {
                var context = ServiceLocator.Current.GetInstance<BizSC.IBusinessContext>();
                model.RelatedContents = content.GetItems(context.EntityId, biz.RelatedContents.Map(i => i.Id).ToList()).Map(b => b.ToContentItem(content, false)).ToList();
            }

            return model;
        }

    }
}
