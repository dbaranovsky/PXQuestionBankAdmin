using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// UserEnrollmentResponse
	/// </summary>
	[DataContract]
	public class UserEnrollmentResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public UserEnrollment results { get; set; }
	}
}
