using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Bfw.Common;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
	/// <summary>
	/// Contains all settings necessary to represent an assignment.
	/// </summary>
	[DataContract]
	public class AssignmentSettings
	{
		/// <summary>
		/// Date the assignment is due, DateTime.Min represents no due date.
		/// </summary>
        private DateTime _startDate;

        [DataMember]
        public DateTime StartDate 
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                if (StartDateTZ != null)
                {
                    StartDateTZ.LocalTime = value;
                }
            }
        }

        public DateTimeWithZone StartDateTZ { get; set; }

		/// <summary>
		/// Date the assignment is due, DateTime.Min represents no due date.
		/// </summary>
        private DateTime _dueDate;

        [DataMember]
        public DateTime DueDate
        {
            get
            {
                return _dueDate;
            }
            set
            {
                _dueDate = value;
                if (DueDateTZ != null)
                {
                    DueDateTZ.LocalTime = value;
                }
            }
        }

        public DateTimeWithZone DueDateTZ { get; set; }

            /// <summary>
        /// The date and time when students can see their score for this item
        /// </summary>
        private DateTime _gradeReleaseDate;

        [DataMember]
        public DateTime GradeReleaseDate
        {
            get
            {
                return _gradeReleaseDate;
            }
            set
            {
                _gradeReleaseDate = value;
            }
        }

		/// <summary>
		/// True if item is assigned [even with no due date].
		/// </summary>
		[DataMember]
		public bool meta_bfw_Assigned { get; set; }

		/// <summary>
		/// Number of points the assignment is worth.
		/// </summary>
		[DataMember]
		public double Points { get; set; }
	   
		/// <summary>
		/// File Path of Rubric if the grading option is chosen as Rubric.
		/// </summary>
		[DataMember]
		public string Rubric { get; set; }

		/// <summary>
		/// Category to which the assignment belongs.
		/// </summary>
		[DataMember]
		public string Category { get; set; }

        /// <summary>
        /// Order in which this item should be displayed relative to other items within assigned category.
        /// </summary>
        [DataMember]
        public string CategorySequence { get; set; }

		/// <summary>
		/// True if assigning is allowed.
		/// </summary>
		[DataMember]
		public bool IsAssignable { get; set; }

		/// <summary>
		/// The type of drop box for this assignment, if it has one.
		/// </summary>
		/// <value>
		/// Any value of <see cref="DropBoxType" />.
		/// </value>
		public DropBoxType DropBoxType { get; set; }

		/// <summary>
		/// True if assigment can be submitted beyond due date.
		/// </summary>
		[DataMember]
		public bool AllowLateSubmission { get; set; }

		/// <summary>
		/// A <see cref="CompletionTrigger" /> value that controls when the item is marked as completed.
		/// </summary>
		/// <value>
		/// Any value of <see cref="CompletionTrigger" />.
		/// </value>
		[DataMember]
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
		/// include score in gradebook
		/// </summary>
		[DataMember]
		public int IncludeGbbScoreTrigger { get; set; }

		/// <summary>
		/// Scored Attempt
		/// </summary>
		[DataMember]
        public SubmissionGradeAction SubmissionGradeAction { get; set; }

        /// <summary>
        /// GradeRulet
        /// </summary>
        [DataMember]
        public GradeRule GradeRule { get; set; }

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
        /// Is allow extra credit
        /// </summary>
        [DataMember]
        public bool IsAllowExtraCredit { get; set; }

	}
}
