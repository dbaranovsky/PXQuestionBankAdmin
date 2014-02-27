using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class SearchResultsMapper
    {

        /// <summary>
        /// Converts the ContentItem to a link collection and loads its child links.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static SearchResults ToSearchResults(this BizDC.SearchResultSet biz)
        {
            var model = new SearchResults();

            MapSearchResults(biz, model);

            return model;
        }


        public static FacetedSearchResults ToFacetedSearchResults(this BizDC.SearchResultSet biz)
        {
            var model = new FacetedSearchResults();
            
            MapSearchResults(biz, model);

            model.FacetFields = biz.FacetFields.Select(f => new Bfw.PX.PXPub.Models.FacetField()
                                            {
                                                FieldName = f.FieldName,
                                                FieldValues =
                                                    f.FieldValues.Select(fv => new Bfw.PX.PXPub.Models.FacetValue()
                                                                                   {
                                                                                       Value = fv.Value,
                                                                                       Count = fv.Count
                                                                                   }).ToList()
                                            }).ToList();
            return model;


        }


        private static void MapSearchResults(SearchResultSet biz, SearchResults model)
        {
            model.itemid = biz.itemid;
            model.maxScore = biz.maxScore;
            model.numFound = biz.numFound;
            model.start = biz.start;
            model.time = biz.time;
            model.doc_class = biz.doc_class;
            model.Query = biz.Query.ToSearchQuery();
            model.metaValue = biz.metaValue;

            if (!biz.docs.IsNullOrEmpty())
            {
                model.docs = biz.docs.Map(d => d.ToSearchResultDoc()).ToList();
            }
        }
    }
}
