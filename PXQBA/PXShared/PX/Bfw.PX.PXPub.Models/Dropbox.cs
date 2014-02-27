namespace Bfw.PX.PXPub.Models {

    public class Dropbox : Assignment {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dropbox"/> class.
        /// </summary>
        public Dropbox() {
            SubType = "Dropbox";
        }

        /// <summary>
        /// Submited Doc Title.
        /// </summary>
        public string SubmitedDocTitle { get; set; }

        /// <summary>
        /// Submited Doc Date.
        /// </summary>
        public string SubmitedDocDate { get; set; }

        /// <summary>
        /// Submited Doc Comment.
        /// </summary>
        public string SubmitedDocComment { get; set; }

        /// <summary>
        /// Submited Doc File Name.
        /// </summary>
        public string StudentSubmittedFilename { get; set; }

        /// <summary>
        /// Submited Doc File Size.
        /// </summary>
        public string StudentSubmittedFileSize { get; set; }

        /// <summary>
        /// Submited Doc File Edit url.
        /// </summary>
        public string StudentSubmittedFileEditUrl { get; set; }

        /// <summary>
        /// Submited Status.
        /// </summary>
        public string StudentSubmitStatus { get; set; }

        /// <summary>
        /// Allow resubmission.
        /// </summary>
        public bool AllowReSubmission { get; set; }

        /// <summary>
        /// Points achieved as determined by the teacher.
        /// </summary>
        public double PointsAssigned { get; set; }

        /// <summary>
        /// Gets or sets the points possible.
        /// </summary>
        public double PointsPossible { get; set; }

        /// <summary>
        /// Teacher Comment.
        /// </summary>
        public string TeacherComment { get; set; }

        /// <summary>
        /// Teacher Attachment File Size.
        /// </summary>
        public string TeacherAttachmentFileSize { get; set; }

        /// <summary>
        /// Is Allow Submission.
        /// </summary>
        public bool IsAllowSubmission { get; set; }
    }
}
