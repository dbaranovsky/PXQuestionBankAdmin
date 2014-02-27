using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// EnrollmentResponse
	/// </summary>
	[DataContract]
	public class EnrollmentResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public Enrollment results { get; set; }
	}

}
