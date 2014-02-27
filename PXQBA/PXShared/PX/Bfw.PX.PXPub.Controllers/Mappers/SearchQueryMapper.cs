using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using SearchQuery = Bfw.PX.PXPub.Models.SearchQuery;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class SearchQueryMapper
    {
        /// <summary>
        /// Converts to a Biz Search Query from a Search Query Model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static BizDC.SearchQuery ToSearchQuery(this SearchQuery model)
        {
            var biz = new BizDC.SearchQuery();

            biz.IncludeFields = model.IncludeFields;
            biz.MetaIncludeFields = model.MetaIncludeFields;
            biz.ContentTypes = model.ContentTypes;
            biz.ExactPhrase = model.ExactPhrase;
            biz.ExactQuery = model.ExactQuery;
            biz.ExcludeWords = model.ExcludeWords;
            biz.IncludeWords = Helper.Translate(model.IncludeWords, new char[] { '~', '!', '^', '(', ')', '{', '}', '[', ']', ':' }, "");
            biz.ClassType = model.ClassType;
            biz.EntityId = model.EntityId;

            List<string> catList = new List<string>();

            if (!String.IsNullOrEmpty(model.MetaCategories))
            {
                catList = model.MetaCategories.Split(',').ToList();
            }

            biz.MetaCategories = catList;
            biz.Start = model.Start;
            biz.Rows = model.Rows;

            biz.IsFaceted = model.IsFaceted;
            if(model.IsFaceted && model.FacetedQuery != null)
            {
                biz.FacetedQuery = new FacetedSearchQuery()
                                       {
                                           Fields = model.FacetedQuery.Fields,
                                           Limit = model.FacetedQuery.Limit,
                                           MinCount = model.FacetedQuery.MinCount
                                       };
            }
            
            return biz;
        }

        /// <summary>
        /// Converts to a Search Query from a Biz Content Search Query.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static SearchQuery ToSearchQuery(this BizDC.SearchQuery biz)
        {
            var model = new SearchQuery();

            if (biz != null)
            {
                model = new SearchQuery
                                {
                                    IncludeFields = biz.IncludeFields,
                                    MetaIncludeFields = biz.MetaIncludeFields,
                                    ContentTypes = biz.ContentTypes,
                                    ExactPhrase = biz.ExactPhrase,
                                    ExcludeWords = biz.ExcludeWords,
                                    IncludeWords = biz.IncludeWords,
                                    ClassType = biz.ClassType,
                                    MetaCategories = biz.MetaCategories != null ? String.Join(",", biz.MetaCategories) : null,
                                    Start = biz.Start,
                                    Rows = biz.Rows
                                };
            }

            return model;
        }
    }
}
