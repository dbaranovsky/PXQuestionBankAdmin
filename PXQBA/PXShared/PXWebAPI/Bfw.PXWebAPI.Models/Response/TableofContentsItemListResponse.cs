using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// TableofContentsItemListResponse
	/// </summary>
	[DataContract]
	public class TableofContentsItemListResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public List<TableofContentsItem> results { get; set; }
	}
}
