
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// ApiCourseResponse
	/// </summary>
	[DataContract]
	public class ApiCourseResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public Bfw.PX.PXPub.Models.Course results { get; set; }

	}
}


