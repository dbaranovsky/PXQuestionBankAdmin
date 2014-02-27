using System.Linq;
using Bfw.Common.Collections;
using Bfw.PXWebAPI.Models;

namespace Bfw.PXWebAPI.Mappers
{
	/// <summary>
	/// SearchResultsMapper
	/// </summary>
	public static class SearchResultsMapper
	{

		/// <summary>
		/// Converts the ContentItem to a link collection and loads its child links.
		/// </summary>
		/// <param name="biz">The biz.</param>
		/// <returns></returns>
        public static PX.PXPub.Models.SearchResults ToSearchResults1(this Bfw.PX.Biz.DataContracts.SearchResultSet biz)
		{
            var model = new PX.PXPub.Models.SearchResults();

			MapSearchResults(biz, model);

			return model;
		}


		/// <summary>
		/// ToFacetedSearchResults
		/// </summary>
		/// <param name="biz"></param>
		/// <returns></returns>
        public static PX.PXPub.Models.FacetedSearchResults ToApiFacetedSearchResults(this PX.Biz.DataContracts.SearchResultSet biz)
		{
            var model = new PX.PXPub.Models.FacetedSearchResults();

			MapSearchResults(biz, model);

            model.FacetFields = biz.FacetFields.Select(f => new PX.PXPub.Models.FacetField()
			{
				FieldName = f.FieldName,
				FieldValues =
                    f.FieldValues.Select(fv => new PX.PXPub.Models.FacetValue()
					{
						Value = fv.Value,
						Count = fv.Count
					}).ToList()
			}).ToList();
			return model;


		}


        private static void MapSearchResults(Bfw.PX.Biz.DataContracts.SearchResultSet biz, PX.PXPub.Models.SearchResults model)
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
