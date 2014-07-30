namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Result of bulk operation
    /// </summary>
    public class BulkOperationResult
    {
        /// <summary>
        /// Indicates if bulk operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Number of draft questions skipped
        /// </summary>
        public int DraftSkipped { get; set; }

        /// <summary>
        /// Number of questions skipped because no permissions to change status
        /// </summary>
        public int PermissionStatusSkipped { get; set; }

        /// <summary>
        /// Number of question skipped because lack of permissions
        /// </summary>
        public int PermissionSkipped { get; set; }
    }
}
