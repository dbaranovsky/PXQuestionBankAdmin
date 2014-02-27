using System.Runtime.Serialization;
using Bfw.PXWebAPI.Models.DTO;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// DomainResponse
	/// </summary>
	[DataContract]
	public class DomainResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public DomainDto results { get; set; }
	}
}
