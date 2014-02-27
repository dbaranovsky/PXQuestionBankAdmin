using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// ContentItem List Response
	/// </summary>
	[DataContract]
	public class ContentItemListResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public List<ContentItem> results { get; set; }
	}
}
