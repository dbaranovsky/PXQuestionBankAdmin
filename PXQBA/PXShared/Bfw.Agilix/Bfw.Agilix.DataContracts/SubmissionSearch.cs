namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters used to filter submissions.
    /// </summary>
    public class SubmissionSearch
    {
        /// <summary>
        /// Id of the enrollment to find submissions for.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Id of the item to find submissions for.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Type of submission package to return.
        /// </summary>
        public string PackageType { get; set; }

        /// <summary>
        /// Path of the submission file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Version of the submission.
        /// </summary>
        public int Version { get; set; }
    }
}
