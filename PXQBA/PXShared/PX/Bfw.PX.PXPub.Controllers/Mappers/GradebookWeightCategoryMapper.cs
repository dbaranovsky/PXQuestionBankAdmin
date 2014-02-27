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
    public static class GradebookWeightCategoryMapper
    {
        /// <summary>
        /// Converts to a Grade Book Weight Catgory from a Biz Grade Book Weight Category.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static GradeBookWeightCategory ToGradeBookWeightCategory(this BizDC.GradeBookWeightCategory biz)
        {
            List<ContentItem> items = new List<ContentItem>();

            if (biz.Items != null)
            {
                items = (from s in biz.Items
                         select new ContentItem() { Title = s.Title, Id = s.Id, Weight = s.Weight, Percent = s.Percent, CategorySequence = s.AssignmentSettings.CategorySequence }).ToList();
            }

            var model = new GradeBookWeightCategory
            {
                Id = biz.Id,
                ItemWeightTotal = biz.ItemWeightTotal,
                Percent = biz.Percent,
                PercentWithExtraCredit = biz.PercentWithExtraCredit,
                Text = biz.Text,
                Weight = biz.Weight,
                DropLowest = biz.DropLowest,
                Sequence = biz.Sequence,
                Items = items
            };

            return model;
        }
    }
}
