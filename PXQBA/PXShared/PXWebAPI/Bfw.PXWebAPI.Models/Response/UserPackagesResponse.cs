using System.Collections.Generic;
using System.Runtime.Serialization;
using Bfw.PXWebAPI.Models.CoreServices;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// UserPackagesResponse
	/// </summary>
	[DataContract]
	public class UserPackagesResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public List<PackageSiteInfo> results { get; set; }
	}
}
