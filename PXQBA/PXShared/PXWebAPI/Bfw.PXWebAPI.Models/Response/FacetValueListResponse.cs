using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// FacetValueListResponse
	/// </summary>
	[DataContract]
	public class FacetValueListResponse : Response
	{
		/// <summary>
		/// FacetedSearchResults
		/// </summary>
		[DataMember]
        public List<PX.PXPub.Models.FacetValue> results { get; set; }
	}
}