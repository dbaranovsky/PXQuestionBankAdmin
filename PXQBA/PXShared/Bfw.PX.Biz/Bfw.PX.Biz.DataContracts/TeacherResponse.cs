using System;
using System.Collections.Generic;
using System.IO;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a teacher's response to a student's submission.
    /// </summary>
    public class TeacherResponse
    {
        /// <summary>
        /// Gets or sets the type of the teacher response.
        /// </summary>
        public TeacherResponseType TeacherResponseType { get; set; }

        /// <summary>
        /// A bitwise OR mask that indicates which GradeStatus bits are being set or cleared in status.
        /// </summary>
        public GradeStatus Mask { get; set; }

        /// <summary>
        /// Points achieved as determined by the teacher.
        /// </summary>
        public double PointsAssigned { get; set; }

        /// <summary>
        /// Gets or sets the points computed.
        /// </summary>
        public double PointsComputed { get; set; }

        /// <summary>
        /// Gets or sets the points possible.
        /// </summary>
        public double PointsPossible { get; set; }

        /// <summary>
        /// Version of the Submission that this response applies to.
        /// </summary>
        public int ScoredVersion { get; set; }

        /// <summary>
        /// Version of the Submission that this response applies to (for specific questions on a quiz).
        /// </summary>
        public string AttemptVersion { get; set; }

        /// <summary>
        /// Bitwise OR of GradeStatus values to set. Mask indicates which status bits to set or clear.
        /// </summary>
        public GradeStatus Status { get; set; }

        /// <summary>
        /// Specifies the date that a student completed an assignment.
        /// </summary>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// Identifies the object that this response applies to.
        /// </summary>
        public string ForeignId { get; set; }

        /// <summary>
        /// Gets or sets the student enrollment ID.
        /// </summary>        
        public string StudentEnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the teacher comment.
        /// </summary>        
        public string TeacherComment { get; set; }

        /// <summary>
        /// Memorystream.
        /// </summary>
        public MemoryStream ResourceStream { get; set; }

        /// <summary>
        /// Teacher Attachments.
        /// </summary>
        public List<Attachment> TeacherAttachments { get; set; }

        /// <summary>
        /// Collection of teacher responses.
        /// </summary>
        public List<TeacherResponse> Responses { get; set; }
    }
}