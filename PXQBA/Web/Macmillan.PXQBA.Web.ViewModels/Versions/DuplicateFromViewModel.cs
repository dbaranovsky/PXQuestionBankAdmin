namespace Macmillan.PXQBA.Web.ViewModels.Versions
{
    /// <summary>
    /// Question duplicate from
    /// </summary>
    public class DuplicateFromViewModel
    {
        /// <summary>
        /// Id of the question duplicate from
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Product course the question belongs to
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Chapter the question belongs to
        /// </summary>
        public string Chapter { get; set; }


        /// <summary>
        /// Bank the question belongs to
        /// </summary>
        public string Bank { get; set; }
    }
}