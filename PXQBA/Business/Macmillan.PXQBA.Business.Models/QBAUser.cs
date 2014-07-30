using System.Collections.Generic;
using System.Data.Common;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// QBA user model
    /// </summary>
    public class QBAUser
    {
        /// <summary>
        /// User id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// User full name
        /// </summary>
        public string FullName { get; set; }

        private IList<UserProductCourse> productCourses { get; set; }

        /// <summary>
        /// Product courses available in QBA
        /// </summary>
        public IList<UserProductCourse> ProductCourses
        {
            get
            {
                if (productCourses == null)
                {
                    productCourses = new List<UserProductCourse>();
                }
                return productCourses;
            }
            set
            {
                productCourses = value;
            }
        }

        /// <summary>
        /// Count of product courses current user has access to
        /// </summary>
        public int ProductCoursesCount { get; set; }
    }
}