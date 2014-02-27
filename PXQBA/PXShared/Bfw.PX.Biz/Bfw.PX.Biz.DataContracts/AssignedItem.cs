using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
	/// <summary>
	/// Represents an item that has been assigned to the user.
	/// </summary>    
	[DataContract]
	public class AssignedItem
	{
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[DataMember]
		public string Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        public string SubTitle { get; set; }

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		[DataMember]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the start date.
		/// </summary>
		/// <value>
		/// The start date.
		/// </value>
		[DataMember]
		public DateTime? StartDate { get; set; }

		/// <summary>
		/// Gets or sets the due date.
		/// </summary>
		/// <value>
		/// The due date.
		/// </value>
		[DataMember]
		public DateTime? DueDate { get; set; }

		/// <summary>
		/// Gets or sets the maximum points available for this item.
		/// </summary>
		/// <value>
		/// The maximum points.
		/// </value>
		[DataMember]
		public Double? MaxPoints { get; set; }

		/// <summary>
		/// Category the assignment falls under.
		/// </summary>
		/// <value>
		/// The category.
		/// </value>
		public string Category { get; set; }

		/// <summary>
		/// Assignment center syllabus filter id.
		/// </summary>
		/// <value>
		/// The syllabus filter id.
		/// </value>
		public string SyllabusFilter { get; set; }

		/// <summary>
		/// A <see cref="CompletionTrigger" /> value that controls when the item is marked as completed.
		/// </summary>
		/// <value>
		/// Any value of <see cref="CompletionTrigger" />.
		/// </value>
		public CompletionTrigger CompletionTrigger { get; set; }

        /// <summary>
        /// in case of CompletionTrigger == 0 (Minutes) 
        /// </summary>
        public int TimeToComplete { get; set; }

        /// <summary>
        /// minimum score a student must achieve to pass
        /// </summary>
        public Double? PassingScore { get; set; }

		/// <summary>
		/// The order in which this item should be displayed relative to other items with the same parent.
		/// </summary>
		public string Sequence { get; set; }

		/// <summary>
		/// Gets or sets the associated rubric id.
		/// </summary>
		/// <value>
		/// The rubric id.
		/// </value>
		[DataMember]
		public string RubricPath { get; set; }

		/// <summary>
		/// Get or sets the important flag of an Assignment
		/// </summary>
		[DataMember]
		public bool IsImportant { get; set; }

		/// <summary>
		/// whether an assignment is gradeable or not
		/// </summary>
		[DataMember]
		public bool IsGradeable { get; set; }

		/// <summary>
		/// set checkbox value
		/// </summary>
		[DataMember]
		public bool IsMarkAsCompleteChecked { get; set; }

		/// <summary>
		/// value of IncludeScoreInGBB dropdown 
		/// </summary>
		[DataMember]
		public int IncludeGbbScoreTrigger { get; set; }

		/// <summary>
		/// whether an assignment has late submission allowed
		/// </summary>
		[DataMember]
		public bool IsAllowLateSubmission { get; set; }

        /// <summary>
        /// whether to highlight the late submission
        /// </summary>
        [DataMember]
        public bool IsHighlightLateSubmission { get; set; }

        /// <summary>
        /// Allow grace period for late submission
        /// </summary>
        [DataMember]
        public bool IsAllowLateGracePeriod { get; set; }

        /// <summary>
        /// Is allow extra credit;
        /// </summary>
        [DataMember]
        public bool IsAllowExtraCredit { get; set; }

        /// <summary>
        /// grace period duration
        /// </summary>
        [DataMember]
        public long LateGraceDuration { get; set; }

        /// <summary>
        /// grace period duration represented in (minutes/hours)
        /// </summary>
        [DataMember]
        public string LateGraceDurationType { get; set; }

		/// <summary>
		/// Grade rule from dropdown
		/// </summary>
		[DataMember]
		public GradeRule GradeRule { get; set; }

		/// <summary>
		/// whether an email reminder is turned on for the assignment
		/// </summary>
		[DataMember]
		public bool IsSendReminder { get; set; }

		/// <summary>
		/// details of the reminder email if the send reminder is set to true
		/// </summary>
		[DataMember]
		public ReminderEmail ReminderEmail { get; set; }

        /// <summary>
        /// Applicable Group Id
        /// </summary>
        [DataMember]
        public string GroupId { get; set; }

        [DataMember]
        public SubmissionGradeAction SubmissionGradeAction { get; set; }


        /// <summary>
        /// Gets or sets the custom fields
        /// </summary>
        /// <value>
        /// The custom fields.
        /// </value>
        [DataMember]
        public IDictionary<string, string> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets the associated rubric id.
        /// </summary>
        /// <value>
        /// The rubric id.
        /// </value>
        [DataMember]
        public string Instructions { get; set; }
	}
}