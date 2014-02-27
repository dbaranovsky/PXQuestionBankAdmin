using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PXWebAPI.Mappers
{
	/// <summary>
	/// SearchQueryMapper
	/// </summary>
	public static class SearchQueryMapper
	{
		/// <summary>
		/// Converts to a Biz Search Query from a Search Query Model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public static Bfw.PX.Biz.DataContracts.SearchQuery ToSearchQuery(this Bfw.PXWebAPI.Models.SearchQuery model)
		{
			var biz = new Bfw.PX.Biz.DataContracts.SearchQuery
						{
							IncludeFields = model.IncludeFields,
							MetaIncludeFields = model.MetaIncludeFields,
							ContentTypes = model.ContentTypes,
							ExactPhrase = model.ExactPhrase,
							ExactQuery = model.ExactQuery,
							ExcludeWords = model.ExcludeWords,
							IncludeWords =
								MappingHelper.Translate(model.IncludeWords, new[] { '~', '!', '^', '(', ')', '{', '}', '[', ']', ':' }, ""),
							ClassType = model.ClassType,
							EntityId = model.EntityId
						};


			var catList = new List<string>();

			if (!String.IsNullOrEmpty(model.MetaCategories))
			{
				catList = model.MetaCategories.Split(',').ToList();
			}

			biz.MetaCategories = catList;
			biz.Start = model.Start;
			biz.Rows = model.Rows;

			biz.IsFaceted = model.IsFaceted;
			if (model.IsFaceted && model.FacetedQuery != null)
			{
				biz.FacetedQuery = new Bfw.PX.Biz.DataContracts.FacetedSearchQuery()
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
        public static PX.PXPub.Models.SearchQuery ToSearchQuery(this SearchQuery biz)
		{
            var model = new PX.PXPub.Models.SearchQuery
			{
				IncludeFields = biz.IncludeFields,
				MetaIncludeFields = biz.MetaIncludeFields,
				ContentTypes = biz.ContentTypes,
				ExactPhrase = biz.ExactPhrase,
				ExcludeWords = biz.ExcludeWords,
				IncludeWords = biz.IncludeWords,
				ClassType = biz.ClassType,
				MetaCategories = String.Join(",", biz.MetaCategories),
				Start = biz.Start,
				Rows = biz.Rows
			};

			return model;
		}
	}

}
