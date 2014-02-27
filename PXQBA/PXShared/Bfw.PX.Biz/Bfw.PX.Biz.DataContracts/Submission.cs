using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents the submission of an item that may or may not have been graded yet.
    /// </summary>
    public class Submission
    {
        /// <summary>
        /// Gets or sets the date the submission was made.
        /// </summary>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// Gets or sets the submission's grade.
        /// </summary>
        public Grade Grade { get; set; }

        /// <summary>
        /// List of submission actions done on the item
        /// </summary>
        public IList<SubmissionAction> Actions { get; set; }

        /// <summary>
        /// Body of the submission, usually its regular string to html string.
        /// </summary>
        [DataMember]
        public string Body { get; set; }

        /// <summary>
        /// This property should be used inorder to 
        /// store XML document with or without submission.
        /// </summary>
        [DataMember]
        public XDocument Data { get; set; }

        /// <summary>
        /// This property should be used inorder to 
        /// store XML document with or without submission.
        /// </summary>
        [DataMember]
        public Stream StreamData { get; set; }

        /// <summary>
        /// Item ID to which this submission belongs.
        /// </summary>
        [DataMember]
        public string ItemId { get; set; }

        [DataMember]
        public string ItemName { get; set; }

        /// <summary>
        /// Notes for the submission.
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        /// Submitted file for the submission.
        /// </summary>
        [DataMember]
        public string SubmittedFileName { get; set; }

        /// <summary>
        /// Enrollment ID of the user.
        /// </summary>
        [DataMember]
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Type of submission "attempt|assignment|homework".
        /// </summary>
        [DataMember]
        public SubmissionType SubmissionType { get; set; }

        /// <summary>
        /// Student's first name.
        /// </summary>
        [DataMember]
        public string StudentFirstName { get; set; }

        /// <summary>
        /// Student's last name.
        /// </summary>
        [DataMember]
        public string StudentLastName { get; set; }

        /// <summary>
        /// Student's full name.
        /// </summary>
        [DataMember]
        public string StudentFullName { get; set; }

        /// <summary>
        /// The version that was submitted.
        /// </summary>
        [DataMember]
        public int Version { get; set; }

        /// <summary>
        /// Status of the submission before and after instructor grades it.
        /// </summary>
        [DataMember]
        public SubmissionStatus SubmissionStatus { get; set; }

        /// <summary>
        /// Set of attempts for each question in a homework.
        /// </summary>
        public IDictionary<string, IList<QuestionAttempt>> QuestionAttempts { get; set; }

        /// <summary>
        /// Gets or sets the submission attempt.
        /// </summary>
        /// <value>
        /// The submission attempt.
        /// </value>
        public IDictionary<string, SubmissionAttempt> SubmissionAttempts { get; set; }
    }

    public class QuestionAttempt
    {
        /// <summary>
        /// ID of the question that was attempted.
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// Answer the user submitted for this attempt.
        /// </summary>
        public string AttemptAnswer { get; set; }

        /// <summary>
        /// points attributed for this question
        /// </summary>
        /// <value>
        /// The calculated points.
        /// </value>
        public string PointsComputed { get; set; }

        /// <summary>
        /// points available for this question
        /// </summary>
        /// <value>
        /// The available points.
        /// </value>
        public string PointsPossible { get; set; }

        /// <summary>
        /// ID of the attempt
        /// </summary>
        /// <value>
        /// The attempt id.
        /// </value>
        public string PartId { get; set; }

        /// <summary>
        /// Version of the attempt
        /// </summary>
        /// <value>
        /// The attempt version.
        /// </value>
        public string AttemptVersion { get; set; }
    }
}
