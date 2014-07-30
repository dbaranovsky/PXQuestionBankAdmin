using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Collection of objects per page
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedCollection<T>
    {
        /// <summary>
        /// List of objects for one page
        /// </summary>
        public IEnumerable<T> CollectionPage { get; set; }

        /// <summary>
        /// Total number of objects
        /// </summary>
        public int TotalItems { get; set; }
    }
}
