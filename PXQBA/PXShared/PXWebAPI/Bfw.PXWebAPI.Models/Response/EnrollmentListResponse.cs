using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// EnrollmentListResponse
	/// </summary>
	[DataContract]
	public class EnrollmentListResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public IList<Enrollment> results { get; set; }
	}


}
