
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// BoolResponse
	/// </summary>
	[DataContract]
	public class BoolResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public bool results { get; set; }
	}
}
