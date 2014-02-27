

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// FacetedSearchResultsResponse
	/// </summary>
	[DataContract]
	public class ApiSearchResultDocListResponse : Response
	{
		/// <summary>
		/// FacetedSearchResults
		/// </summary>
		[DataMember]
		public List<Bfw.PX.PXPub.Models.SearchResultDoc> results { get; set; }
	}
}