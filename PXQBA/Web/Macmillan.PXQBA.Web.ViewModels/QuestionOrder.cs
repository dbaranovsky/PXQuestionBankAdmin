namespace Macmillan.PXQBA.Web.ViewModels
{
    /// <summary>
    /// Represents questions sorting order
    /// </summary>
    public class QuestionOrder 
    {
        /// <summary>
        /// Order type
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// Field that is used to sort by
        /// </summary>
        public string OrderField { get; set; }
    }
}
