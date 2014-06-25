namespace Macmillan.PXQBA.Business.Models
{
    public class BulkOperationResult
    {
        public bool IsSuccess { get; set; }

        public int DraftSkipped { get; set; }

        public int PermissionSkipped { get; set; }
    }
}
