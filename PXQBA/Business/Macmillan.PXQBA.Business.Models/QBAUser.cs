using System.Collections.Generic;
using System.Data.Common;

namespace Macmillan.PXQBA.Business.Models
{
    public class QBAUser
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        private IList<UserProductCourse> productCourses { get; set; }

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

        public int ProductCoursesCount { get; set; }
    }
}