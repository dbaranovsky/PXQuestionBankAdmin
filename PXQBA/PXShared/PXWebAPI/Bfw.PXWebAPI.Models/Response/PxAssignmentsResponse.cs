using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// Px Assignments Response
	/// </summary>
	[DataContract]
	public class PxAssignmentsResponse : Response
	{
		/// <summary>
		/// List of Bfw.PX.PXPub.Models.CalendarAssignment
		/// </summary>
		[DataMember]
		public List<Bfw.PX.PXPub.Models.CalendarAssignment> results { get; set; }
	}
}



