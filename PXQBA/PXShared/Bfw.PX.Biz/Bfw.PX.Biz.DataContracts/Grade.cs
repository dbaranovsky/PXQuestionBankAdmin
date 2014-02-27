using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
    /// <summary>
    /// Represents a Grade business object (see http://192.168.78.60/Docs/Schema/Grades)    
    /// </summary>
    [Serializable]
    [DataContract]
    public class Grade
    {
        [DataMember]
        public string EnrollmentId { get; set; }

        [DataMember]
        public string EnrollmentName { get; set; }
        /// <summary>
        /// Gets or sets the ID of the item ( content or enrollment) associated with the grade
        /// </summary>
        [DataMember]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the name of the item (content or enrollment) associated with the grade
        /// </summary>
        [DataMember]
        public string ItemName { get; set; }

        [DataMember]
        public ContentItem GradedItem { get; set; }

        /// <summary>
        /// The number of points achieved for the course according to the grade calculator.
        /// </summary>
        [DataMember]
        public double Achieved { get; set; }

        /// <summary>
        /// The number of points possible for the course according to the grade calculator.
        /// </summary>
        [DataMember]
        public double Possible { get; set; }

        /// <summary>
        /// The letter grade achieved for the course according to the grade calculator.
        /// </summary>
        [DataMember]
        public string Letter { get; set; }

        /// <summary>
        /// The number of actual points achieved for this item without any curving rules applied. This attribute is included only if it differs from achieved.
        /// </summary>
        [DataMember]
        public double RawAchieved { get; set; }

        /// <summary>
        /// The number of actual points possible for this item without any curving rules applied. This attribute is included only if it differs from possible.
        /// </summary>
        [DataMember]
        public double RawPossible { get; set; }

        /// <summary>
        /// The number of attempts made on this item.
        /// </summary>
        [DataMember]
        public int Attempts { get; set; }

        /// <summary>
        /// The date the item was last scored.
        /// </summary>
        [DataMember]
        public DateTime? ScoredDate { get; set; }

        /// <summary>
        /// Version of the Submission that this response applies to.
        /// </summary>
        [DataMember]
        public int ScoredVersion { get; set; }

        /// <summary>
        /// Version of the Submission.
        /// </summary>
        [DataMember]
        public int SubmittedVersion { get; set; }

        /// <summary>
        /// The date of the last submission.
        /// </summary>
        [DataMember]
        public DateTime? SubmittedDate { get; set; }

        /// <summary>
        /// Status of the grade
        /// </summary>
        [DataMember]
        public GradeStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public GradeRule Rule { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public IEnumerable<SubmissionLog> Submissions { get; set; }
    }

    public class SubmissionLog
    {
        public int AttemptNo { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public double RawAchieved { get; set; }

        public double RawPossible { get; set; }

        public double Achieved { get; set; }

        public double Possible { get; set; }

        public double Grade { get; set; }
    }
}
