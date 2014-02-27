using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// UserListResponse
	/// </summary>
	[DataContract]
	public class UserListResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public IList<User> results { get; set; }
	}
}
