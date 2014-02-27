using System.Collections.Generic;
using System.Runtime.Serialization;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PXWebAPI.Models.Response
{

	/// <summary>
	/// GradeListResponse
	/// </summary>
	[DataContract]
	public class GradeListResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public IEnumerable<Grade> results { get; set; }
	}
}
