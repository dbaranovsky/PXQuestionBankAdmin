using System.Collections.Generic;
using System.Runtime.Serialization;
using Bfw.PXWebAPI.Models.DTO;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// UserEnrollmentListResponse
	/// </summary>
	[DataContract]
	public class UserEnrollmentListResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public IList<UserEnrollmentDto> results { get; set; }
	}
}
