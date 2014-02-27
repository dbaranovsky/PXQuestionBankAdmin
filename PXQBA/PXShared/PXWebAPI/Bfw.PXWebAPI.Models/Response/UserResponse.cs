using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{

	/// <summary>
	/// UserResponse
	/// </summary>
	[DataContract]
	public class UserResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public User results { get; set; }
	}

}
