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
    public static class TocCategoryMapper
    {
        /// <summary>
        /// Convert to a toc category.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Biz.DataContracts.TocCategory ToTocCategory(this TocCategory model)
        {
            var biz = new Biz.DataContracts.TocCategory()
            {
                Id = model.Id,
                Text = model.Text,
                ItemParentId = model.ItemParentId,
                Sequence = model.Sequence
            };

            return biz;
        }
        /// <summary>
        /// Convert to a Toc category from a biz TocCategory.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="active">The active.</param>
        /// <returns></returns>
        public static TocCategory ToTocCategory(this Biz.DataContracts.TocCategory biz, string active)
        {
            if (active == null)
                active = string.Empty;

            var model = new TocCategory()
            {
                Id = biz.Id,
                Text = biz.Text,
                Active = biz.Id.ToLowerInvariant() == active.ToLowerInvariant(),
                ItemParentId = biz.ItemParentId,
                Sequence = biz.Sequence
            };

            return model;
        }

        /// <summary>
        /// Convert to a Toc category from a Biz TocCategory.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static TocCategory ToTocCategory(this Biz.DataContracts.TocCategory biz)
        {
            return ToTocCategory(biz, string.Empty);
        }

    }
}
