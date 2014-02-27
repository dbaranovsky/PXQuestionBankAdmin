using System.Runtime.Serialization;
using Bfw.PXWebAPI.Models.DTO;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// UserEnrollmentDetailResponse
	/// </summary>
	[DataContract]
	public class UserEnrollmentDetailResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public UserEnrollmentDto results { get; set; }
	}
}
