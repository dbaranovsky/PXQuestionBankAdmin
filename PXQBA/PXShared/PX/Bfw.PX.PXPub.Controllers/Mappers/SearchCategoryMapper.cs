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
    public static class SearchCategoryMapper
    {
        /// <summary>
        /// Converts to a Search Category from a Biz Search Category.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static SearchCategory ToSearchCategory(this BizDC.SearchCategory biz)
        {
            return new SearchCategory
                       {
                           Title = biz.Title,
                           AssociatedItems = String.Join(",", biz.AssociatedItems),
                           IsSearchable = biz.IsSearchable
                       };
        }
    }
}
