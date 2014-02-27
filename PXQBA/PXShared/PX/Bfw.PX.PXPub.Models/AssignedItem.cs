using System;
using System.Collections.Generic;

namespace Bfw.PX.PXPub.Models
{
	/// <summary>
	/// Store information to represent an assignment
	/// </summary>
	public class AssignedItem
	{
		/// <summary>
		/// Assignment ID
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		public string Id { get; set; }

		/// <summary>
		/// The title of the assignment
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		public string Title { get; set; }

        /// <summary>
        /// The subtitle of the assignment
        /// </summary>
        /// <value>
        /// The subtitle.
        /// </value>
        public string SubTitle { get; set; }

		/// <summary>
		/// Startdate of an item
		/// </summary>
		/// <value>
		/// The start date.
		/// </value>
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Date the assignment is due
		/// </summary>
		/// <value>
		/// The due date.
		/// </value>
		public DateTime DueDate { get; set; }

		/// <summary>
		/// The score the user has obtained
		/// </summary>
		/// <value>
		/// The score.
		/// </value>
		public Score Score { get; set; }

		/// <summary>
		/// Date the assignment was last submitted, or null if it has not been submitted
		/// </summary>
		/// <value>
		/// The submitted date.
		/// </value>
		public DateTime? SubmittedDate { get; set; }

		/// <summary>
		/// Category the assignment falls under
		/// </summary>
		/// <value>
		/// The category.
		/// </value>
		public string Category { get; set; }

		/// <summary>
		/// Type of the parent Object
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public string Type { get; set; }

		/// <summary>
		/// Sub type of the parent object
		/// </summary>
		/// <value>
		/// The type of the sub.
		/// </value>
		public string SubType { get; set; }

        /// <summary>
        /// Type of the content
        /// </summary>
        public string ContentType { get; set; }

		/// <summary>
		/// Gets or sets the lesson id.
		/// </summary>
		/// <value>
		/// The lesson id.
		/// </value>
		public string lessonId { get; set; }

		/// <summary>
		/// Gets or sets the rubric id.
		/// </summary>
		/// <value>
		/// The rubric id.
		/// </value>
		public string rubricId { get; set; }

		/// <summary>
		/// Gets or sets the is multipart lesson relateds
		/// </summary>
		/// <value>
		/// The is multipart.
		/// </value>
		public string isMultipart { get; set; }

		/// <summary>
		/// Gets or sets the syllabus filter, this has been moved to Categories collection
		/// </summary>
		/// <value>
		/// The syllabus filter.
		/// </value>
		public string SyllabusFilter { get; set; }

		/// <summary>
		/// Gets or sets the completion trigger.
		/// </summary>
		/// <value>
		/// The completion trigger.
		/// </value>
		public CompletionTrigger CompletionTrigger { get; set; }

        /// <summary>
        /// in case of CompletionTrigger == 0 (Minutes) 
        /// </summary>
        public int TimeToComplete { get; set; }

		// get set value for include gradebook score dropdown index
		public int IncludeGbbScoreTrigger { get; set; }

		/// <summary>
		/// populate enum in dropdown
		/// </summary>
        public SubmissionGradeAction SubmissionGradeAction { get; set; }

        /// <summary>
        /// Grade rule
        /// </summary>
        public GradeRule GradeRule { get; set; }

        /// <summary>
        /// whether an assignment is gradeable or not
        /// </summary>
        public bool IsGradeable { get; set; }

        /// <summary>
        /// Gets or sets the maximum points available for this item.
        /// </summary>
        /// <value>
        /// The maximum points.
        /// </value>
        public Double? MaxPoints { get; set; }

        /// <summary>
        /// minimum score a student must achieve to pass
        /// </summary>
        public Double? PassingScore { get; set; }

        /// <summary>
        /// set checkbox value 
        /// </summary>
        public bool IsMarkAsCompleteChecked { get; set; }

        /// <summary>
        /// whether an assignment has late submission allowed
        /// </summary>
        public bool IsAllowLateSubmission { get; set; }

        /// <summary>
        /// whether to highlight the late submission
        /// </summary>
        public bool IsHighlightLateSubmission { get; set; }

        /// <summary>
        /// Allow grace period for late submission
        /// </summary>
        public bool IsAllowLateGracePeriod { get; set; }

        /// <summary>
        /// grace period duration
        /// </summary>
        public long LateGraceDuration { get; set; }

        /// <summary>
        /// grace period duration represented in (minutes/hours)
        /// </summary>
        public string LateGraceDurationType { get; set; }

        /// <summary>
        /// whether an email reminder is turned on for the assignment
        /// </summary>
        public bool IsSendReminder { get; set; }

        /// <summary>
        /// Return whether the item is locked for edit for the current course type
        /// </summary>
        public bool IsItemLocked { get; set; }

        /// <summary>
        /// details of the reminder email if the send reminder is set to true
        /// </summary>
        public ReminderEmail ReminderEmail { get; set; }

        /// <summary>
        /// A flag which controls the visibility of Assign button
        /// </summary>
        public bool IsContentCreateAssign { get; set; }

        /// <summary>
        /// A flag which marks the assigned item to be viewed in a read-only mode
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Where or not to allow extra credit
        /// </summary>
        public bool IsAllowExtraCredit { get; set; }

        /// <summary>
        /// Gets or sets the Assignment Center Filter Section, Used for the showing a drop down list on the Assign Page.
        /// </summary>
        /// <value>
        /// The syllabus.
        /// </value>
        public AssignmentCenterFilterSection Syllabus { get; set; }

        /// <summary>
        /// Gets or sets the grade book weights.
        /// </summary>
        /// <value>
        /// The grade book weights.
        /// </value>
        public GradeBookWeights GradeBookWeights { get; set; }

        /// <summary>
        /// Gets or sets the type of the source.
        /// </summary>
        /// <value>
        /// The type of the source.
        /// </value>
        public string SourceType { get; set; }

        /// <summary>
        /// Returns the friendly name for the source type
        /// </summary>
        public string FriendlyNameSourceType { get; set; }

        /// <summary>
        /// type of the course like eportfolio, eportfolio dashboard
        /// </summary>
        public string CourseType { get; set; }

        /// <summary>
        /// Course time zone
        /// </summary>
        public string CourseTimeZone { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        public string Sequence { get; set; }

        /// <summary>
        /// Gets or sets the request type whether its Assign or Unassign
        /// </summary>
        /// <value>
        /// Assign or Unassign
        /// </value>
        public string RequestType { get; set; }

        /// <summary>
        /// Contains the Sort Index for assignments with no due date
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// Gets or sets the selected rubric path.
        /// </summary>
        /// <value>
        /// The selected rubric path.
        /// </value>
        public String RubricPath { get; set; }
        /// <summary>
        /// Gets or sets the selected rubric path.
        /// </summary>
        /// <value>
        /// The selected rubric path.
        /// </value>
        public String Instructions { get; set; }

        /// <summary>
        /// Gets or sets the important flag for the Assignment
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// Due date display for the Assignment Widget
        /// </summary>
        public string DueDateDisplay { get; set; }

        /// <summary>
        /// settings for the assign tab used to assign the content items
        /// </summary>
        public AssignTabSettings AssignTabSettings { get; set; }

        /// <summary>
        /// List of allowed content items which can be created from the Assign tab
        /// </summary>        
        public List<RelatedTemplate> RelatedTemplates { get; set; }

        /// <summary>
        /// Is show the extra credit option
        /// </summary>
        public bool IsShowExtraCreditOption { get; set; }

        /// <summary>
        /// list of available grade actions
        /// </summary>
        public List<SubmissionGradeAction> AvailableGradeActions { get; set; }

        /// <summary>
        /// list of available completion triggers
        /// </summary>
        public List<CompletionTrigger> AvailableCompletionTriggers { get; set; }

        /// <summary>
        /// Default completion trigger
        /// </summary>
        public CompletionTrigger DefaultCompletionCriterion { get; set; }

        /// <summary>
        /// Default grade action
        /// </summary>
        public SubmissionGradeAction DefaultGradeAction { get; set; }

        /// <summary>
        /// When true, this item sets its own score in the gradebook.
        /// </summary>
        public bool Sco { get; set; }

        /// <summary>
        /// Gets or sets the custom fields
        /// </summary>
        /// <value>
        /// The custom fields.
        /// </value>
        public IDictionary<string, string> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is removable or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it is removable; otherwise, <c>false</c>.
        /// </value>
        public bool IsRemovable { get; set; }

        #region Containers

        public List<Container> Containers { get; set; }

        public List<Container> SubContainerIds { get; set; }

        #endregion

        #region Assignment Unit

        /// <summary>
        /// Gets or sets the assignment units
        /// </summary>
        /// <value>
        /// The assignment units.
        /// </value>
        public List<AssignmentUnit> AssignmentUnits { get; set; }

        #endregion

        /// <summary>
	    /// list of available SumbissionGradeAction enum values
	    //        /// ==============================
	    //non-submittable items (ebook page, HTML page, doc collection, etc.)

	    //Calculation Type:
	    //Full credit on completion

	    //Item is complete when student:
	    //Views the activity
	    //Views the activity for a specified amount of time [shows text box for minutes if selected]

	    //==============================
	    //auto-graded items (quiz/homework/learningcurve/ARGA)

	    //Calculation Type:
	    //Use score earned
	    //Full credit on completion

	    //Item is complete when student:
	    //Makes a submission
	    //Acheives a passing score [shows a text box for passing score]

	    //==============================
	    //Discussion board

	    //Calculation Type:
	    //Manually graded
	    //Full credit on completion

	    //Item is complete when student:
	    //Views the activity
	    //Submits a post

	    //==============================
	    //Dropbox / Writing Assignment

	    //Calculation Type:
	    //Manually graded
	    //Full credit on completion

	    //Item is complete when student:
	    //Makes a submission
	    /// </summary>
	    public IEnumerable<KeyValuePair<string, object>> GetAvailableSubmissionGradeAction()
	    {
            var actions = new List<KeyValuePair<string, object>>();
            if (AvailableGradeActions == null || AvailableGradeActions.Count == 0)
            {
                // First check if the item sets its own score (Sco)
                if (this.Sco || this.Type.ToLowerInvariant() == "assessment" ||
                    this.Type.ToLowerInvariant() == "customactivity" || this.Type.ToLowerInvariant() == "homework")
                {
                    //If this is not LearningCurve, allow 'Use score earned'
                    if (string.IsNullOrEmpty(ContentType) || !ContentType.Contains("LearningCurve"))
                    {

                        actions.Add(
                            new KeyValuePair<string, object>(
                                Enum.GetName(typeof (SubmissionGradeAction), SubmissionGradeAction.Default),
                                "Use score earned"));
                    }
                    actions.Add(
                        new KeyValuePair<string, object>(
                            Enum.GetName(typeof(SubmissionGradeAction), SubmissionGradeAction.Full_Credit),
                            "Full credit on completion"));
                }
                else if (this.Type.ToLowerInvariant() == "assignment")
                {
                    if (this.SubType != null && (this.SubType.ToLowerInvariant() == "reflectionassignment" ||
                        this.SubType.ToLowerInvariant() == "eportfolio"))
                    {
                        actions.Add(
                       new KeyValuePair<string, object>(
                           Enum.GetName(typeof(SubmissionGradeAction), SubmissionGradeAction.Manual),
                           "Manually Graded"));
                    }
                    else
                    {
                        actions.Add(
                            new KeyValuePair<string, object>(
                                Enum.GetName(typeof(SubmissionGradeAction), SubmissionGradeAction.Full_Credit),
                                "Full credit on completion"));

                        actions.Add(
                        new KeyValuePair<string, object>(
                            Enum.GetName(typeof(SubmissionGradeAction), SubmissionGradeAction.Manual),
                            "Manually Graded"));
                    }
                }
                else if (this.SourceType.ToLowerInvariant() == "dropbox" ||
                    this.Type.ToLowerInvariant() == "discussion")
                {
                    actions.Add(
                    new KeyValuePair<string, object>(
                        Enum.GetName(typeof(SubmissionGradeAction), SubmissionGradeAction.Manual),
                        "Manually Graded"));

                    actions.Add(
                    new KeyValuePair<string, object>(
                        Enum.GetName(typeof(SubmissionGradeAction), SubmissionGradeAction.Full_Credit),
                        "Full credit on completion"));
                }
                else
                {
                    actions.Add(
                new KeyValuePair<string, object>(
                    Enum.GetName(typeof(SubmissionGradeAction), SubmissionGradeAction.Full_Credit),
                    "Full credit on completion"));
                }
            }
            else
            {
                foreach (var action in AvailableGradeActions)
                {
                    switch (action)
                    {
                        case SubmissionGradeAction.Default:
                            actions.Add(
                                new KeyValuePair<string, object>(SubmissionGradeAction.Default.ToString(), "Use score earned"));
                            break;
                        case SubmissionGradeAction.Manual:
                            actions.Add(
                                new KeyValuePair<string, object>(SubmissionGradeAction.Manual.ToString(), "Manually Graded"));
                            break;
                        case SubmissionGradeAction.Full_Credit:
                            actions.Add(
                                new KeyValuePair<string, object>(SubmissionGradeAction.Full_Credit.ToString(), "Full credit on completion"));
                            break;
                        default:
                            break;
                    }
                }
            }
            return actions;
	    }

	    public IEnumerable<KeyValuePair<string, object>> AvailableSubmissionGradeAction
	    {
	        get { return GetAvailableSubmissionGradeAction(); }
	    }

 		/// <summary>
		/// Initializes a new instance of the <see cref="AssignedItem"/> class.
		/// </summary>
		public AssignedItem()
		{
			Syllabus = new AssignmentCenterFilterSection();
            RelatedTemplates = new List<RelatedTemplate>();
			ReminderEmail = new ReminderEmail();
		}
	}
}
