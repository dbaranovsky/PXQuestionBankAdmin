using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// Simple DictionaryResponse
	/// </summary>
	[DataContract]
	public class DictionaryResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public Dictionary<string, string> results { get; set; }
	}


	/// <summary>
	/// Simple DictionaryResponse
	/// </summary>
	[DataContract]
	public class TupleDictionaryResponse : Response
	{
		/// <summary>
		/// results
		/// </summary>
		[DataMember]
		public Dictionary<string, Tuple<string, bool>> results { get; set; }
	}


}
