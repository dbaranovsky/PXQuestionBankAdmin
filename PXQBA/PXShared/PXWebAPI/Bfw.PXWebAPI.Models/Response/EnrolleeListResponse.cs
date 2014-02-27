using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// EnrolleeListResponse
	/// </summary>
	[DataContract]
	public class EnrolleeListResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public List<Enrollee> results { get; set; }
	}
}
