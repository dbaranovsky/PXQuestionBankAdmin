using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public class ProductCourseSection
    {
        public string ProductCourseId { get; set; }

        private Dictionary<string, List<string>> productCourseValues;

        public Dictionary<string, List<string>> ProductCourseValues
        {
            get
            {
                if (productCourseValues == null)
                {
                    productCourseValues = new Dictionary<string, List<string>>();
                }
                return productCourseValues;
            }
            set
            {
                productCourseValues = value;
            }
        }
    }
}