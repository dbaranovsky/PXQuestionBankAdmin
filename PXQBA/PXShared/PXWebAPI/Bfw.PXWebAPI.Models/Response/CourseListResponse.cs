using System.Collections.Generic;
using System.Runtime.Serialization;
using Bfw.PXWebAPI.Models.DTO;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// CourseListResponse
	/// </summary>
	[DataContract]
	public class CourseListResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public List<CourseDto> results { get; set; }
	}
}
