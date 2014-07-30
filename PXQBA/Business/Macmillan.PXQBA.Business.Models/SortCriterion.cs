namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Sorting type
    /// </summary>
    public enum SortType
    {
        None = 0,
        Asc,
        Desc
    }
    
    /// <summary>
    /// Criterion to sort the list
    /// </summary>
    public class SortCriterion
    {
        /// <summary>
        /// Column name to sort by
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Sorting type to use
        /// </summary>
        public SortType SortType { get; set; }

        /// <summary>
        /// Indicates if sorting is ascending
        /// </summary>
        public bool IsAsc { get { return SortType == SortType.Asc; } }
    }
}
