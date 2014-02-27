using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// FacetedSearchResultsResponse
	/// </summary>
	[DataContract]
	public class SearchResultDocListResponse : Response
	{
		/// <summary>
		/// FacetedSearchResults
		/// </summary>
		[DataMember]
		public List<SearchResultDoc> results { get; set; }
	}
}