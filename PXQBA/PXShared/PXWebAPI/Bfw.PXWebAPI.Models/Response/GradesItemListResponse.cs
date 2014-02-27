using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// GradesItemListResponse
	/// </summary>
	[DataContract]
	public class GradesItemListResponse : Response
	{

		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public List<GradesItem> results { get; set; }
	}
}
