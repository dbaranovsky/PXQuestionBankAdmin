using System.Runtime.Serialization;
using Bfw.PXWebAPI.Models.DTO;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// CourseResponse
	/// </summary>
	[DataContract]
	public class CourseResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public CourseDto results { get; set; }
	}
}
