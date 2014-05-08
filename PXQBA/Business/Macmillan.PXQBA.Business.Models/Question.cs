using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question DTO
    /// </summary>
    public class Question
    {
        public string Id { get; set; }

        public QuestionStaticMetadata LocalMetadata { get; set; }

        public QuestionType Type { get; set; }
        public string Preview { get; set; }

        /// <summary>
        /// Question particular version
        /// </summary>
        public string Version { get; set; }

        public string QuestionIdDuplicateFrom { get; set; }

        private List<ProductCourseSection> productCourses;
        public List<ProductCourseSection> ProductCourses
        {
            get
            {
                if (productCourses == null)
                {
                    productCourses = new List<ProductCourseSection>();
                }
                return productCourses;
            }
            set
            {
                productCourses = value;
            }
        } 

        public string EntityId { get; set; }

        public string QuizId { get; set; }

        public QuestionStaticMetadata SharedMetadata { get; set; }
    }
}
