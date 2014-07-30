using System.Collections.Generic;

namespace Macmillan.PXQBA.Web.ViewModels.TiteList
{
    /// <summary>
    /// List of available product courses
    /// </summary>
    public class ProductCourseListDataResponse
    {
        /// <summary>
        /// Product courses list
        /// </summary>
        public IEnumerable<ProductCourseViewModel> Titles { get; set; }
    }
}
