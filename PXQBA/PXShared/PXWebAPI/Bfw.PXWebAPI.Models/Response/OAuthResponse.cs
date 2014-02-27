using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// OAuthResponse
	/// </summary>
	[DataContract]
	public class OAuthResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public string results { get; set; }
	}
}
