namespace Macmillan.PXQBA.Business.Models
{
    public enum SortType
    {
        None = 0,
        Asc,
        Desc
    }
    
    public class SortCriterion
    {
        public string ColumnName { get; set; }
        public SortType SortType { get; set; }

        public bool IsAsc { get { return SortType == SortType.Asc; } }
    }
}
