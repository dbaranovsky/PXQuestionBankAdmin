using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
	/// <summary>
	/// settings for the assign tab used to assign the content items
	/// </summary>
	public class AssignTabSettings
	{
		/// <summary>
		/// Shows 'Make Gradable' checkbox in the assign tab
		/// </summary>
		[DataMember]
		public Boolean ShowMakeGradeable { get; set; }

		/// <summary>
		///  shows "Allow Late Submission" checkbox
		/// </summary>
		[DataMember]
		public Boolean ShowAllowLateSubmissions { get; set; }

		public Boolean ShowScheduleReminder { get; set; }

		/// <summary>
		/// shows a checkbox that allows the user to make the system send a reminder
		/// </summary>
		[DataMember]
		public Boolean ShowSendReminder { get; set; }

		[DataMember]
		public Boolean ShowSubContentCreation { get; set; }

		/// <summary>
		/// Shows the other assingments scheduled for the same day
		/// </summary>
		[DataMember]
		public Boolean ShowAssignedSameDay { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public Boolean ShowGradebookCategory { get; set; }


		[DataMember]
		public Boolean ShowPointsPossible { get; set; }

		/// <summary>
		///  shows the dropdown "Include score in gradebook"
		/// </summary>
		[DataMember]
		public Boolean ShowIncludeScore { get; set; }

		[DataMember]
		public Boolean ShowCalculationType { get; set; }

		/// <summary>
		/// show/hide Show Mark As Complete
		/// </summary>
		[DataMember]
		public Boolean ShowMarkAsComplete { get; set; }

        /// <summary>
        /// Toggle showing of the completion category
        /// </summary>
        [DataMember]
        public Boolean ShowCompletionCategory { get; set; }

	}
}
