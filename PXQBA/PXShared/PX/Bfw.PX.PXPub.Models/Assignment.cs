using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace Bfw.PX.PXPub.Models
{
    public class Assignment : ContentItem
    {
        #region Properties
        [Range(0, 500, ErrorMessage = "Possible Points must be greater than zero")]
        public double PossibleScore { get; set; }

        // Subtype of the assignment
        /// <summary>
        /// Gets or sets the type of the sub.
        /// </summary>
        /// <value>
        /// The type of the sub.
        /// </value>
        public string SubType { get; set; }

        /// <summary>
        /// Gets or sets the assigned score.
        /// </summary>
        /// <value>
        /// The assigned score.
        /// </value>
        public double AssignedScore { get; set; }

        /// <summary>
        /// Represents the status of assignment
        /// </summary>
        /// <value>
        /// The assignment status.
        /// </value>
        public AssignmentStatus AssignmentStatus { get; set; }

        /// <summary>
        /// The type of drop box
        /// </summary>
        /// <value>
        /// The type of the drop box.
        /// </value>
        public DropboxType DropBoxType { get; set; }

        /// <summary>
        /// The grading category of the item
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category { get; set; }

        /// <summary>
        /// Completion Trigger
        /// </summary>
        /// <value>
        /// The completion trigger.
        /// </value>
        public CompletionTrigger CompletionTrigger { get; set; }

        /// <summary>
        /// Completion Trigger
        /// </summary>
        /// <value>
        /// The completion trigger.
        /// </value>
        public string SubmissionGradeAction { get; set; }

        /// <summary>
        /// List of documents in the collection
        /// </summary>
        /// <value>
        /// The document collection.
        /// </value>
        public DocumentCollection DocumentCollection { get; set; }

        /// <summary>
        /// List of links in the collection
        /// </summary>
        /// <value>
        /// The link collection.
        /// </value>
        public LinkCollection LinkCollection { get; set; }

        /// <summary>
        /// Toc Item items for Adding internal links
        /// </summary>
        /// <value>
        /// The toc item.
        /// </value>
        public IEnumerable<TocItem> TocItem { get; set; }        

        /// <summary>
        /// Gets or sets the submission.
        /// </summary>
        /// <value>
        /// The submission.
        /// </value>
        public Submission Submission { get; set; }
        
        /// <summary>
        /// IsConvertedToStudentFolder
        /// </summary>
        public bool IsConvertedToStudentFolder { get; set; }

        /// <summary>
        /// whether an assignment has late submission allowed
        /// </summary>
        public bool IsAllowLateSubmission { get; set; }

        /// <summary>
        /// Allow grace period for late submission
        /// </summary>
        public bool IsAllowLateGracePeriod { get; set; }

        /// <summary>
        /// Due Date after grace period.
        /// </summary>
        public DateTime DueDateAfterGracePeriod { get; set; }

        /// <summary>
        /// grace period duration
        /// </summary>
        public long LateGraceDuration { get; set; }

        /// <summary>
        /// grace period duration represented in (minutes/hours)
        /// </summary>
        public string LateGraceDurationType { get; set; }

        /// <summary>
        /// Gets or sets the learning objectives
        /// </summary>
        public List<Bfw.PX.Biz.DataContracts.LearningObjective> LearningObjectives { get; set; }

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="Assignment"/> class.
        /// </summary>
        public Assignment()
        {
            Type = "Assignment";
            TrackMinutesSpent = false;
            DueDate = DateTime.MinValue;
            DropBoxType = DropboxType.None;
            DocumentCollection = new DocumentCollection();
            LinkCollection = new LinkCollection();
            Policies = new List<string>();
            ExtendedProperties = new Hashtable();
        }



    }
}