using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// GradesItemListResponse
	/// </summary>
	[DataContract]
	public class PxGradesItemListResponse : Response
	{

		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public List<PX.Biz.DataContracts.Grade> results { get; set; }
	}
}
