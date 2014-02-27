
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// FacetedSearchResultsResponse
	/// </summary>
	[DataContract]
	public class FacetedSearchResultsResponse : Response
	{
		/// <summary>
		/// FacetedSearchResults
		/// </summary>
		[DataMember]
        public PX.PXPub.Models.FacetedSearchResults results { get; set; }
	}
}
