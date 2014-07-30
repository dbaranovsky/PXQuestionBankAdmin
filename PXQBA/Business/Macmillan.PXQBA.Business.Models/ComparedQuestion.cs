namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Information about question compared between 2 titles
    /// </summary>
    public class ComparedQuestion
    {
        /// <summary>
        /// What course compared question belongs to
        /// </summary>
        public CompareLocationType CompareLocationResult { get; set; }

        /// <summary>
        /// Question in comparison
        /// </summary>
        public Question Question { get; set; }
    }
}
