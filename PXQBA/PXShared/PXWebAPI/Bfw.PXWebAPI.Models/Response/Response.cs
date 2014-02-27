using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{

	/// <summary>
	/// Response
	/// </summary>
	[DataContract]
	public class Response
	{
		/// <summary>
		/// status_code
		/// </summary>
		[DataMember]
		public int status_code { get; set; }
		/// <summary>
		/// error_message
		/// </summary>
		[DataMember]
		public string error_message { get; set; }
	}
}
