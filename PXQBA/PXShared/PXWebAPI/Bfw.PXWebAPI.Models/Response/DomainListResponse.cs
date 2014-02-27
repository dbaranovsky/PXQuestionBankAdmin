using System.Collections.Generic;
using System.Runtime.Serialization;
using Bfw.PXWebAPI.Models.DTO;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// DomainListResponse
	/// </summary>
	[DataContract]
	public class DomainListResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public List<DomainDto> results { get; set; }
	}
}
