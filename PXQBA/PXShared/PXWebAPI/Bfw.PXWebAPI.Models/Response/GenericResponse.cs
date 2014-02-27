
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// GenericResponse
	/// </summary>
	[DataContract]
	public class GenericResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public virtual object results { get; set; }
	}
}
