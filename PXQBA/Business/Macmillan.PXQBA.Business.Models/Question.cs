using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    public class Question
    {
        public string Id { get; set; }

        public string Preview { get; set; }

        public string Status { get; set; }

        public string EntityId { get; set; }
        public string QuizId { get; set; }

        public string QuestionIdDuplicateFrom { get; set; }

        private Dictionary<string, List<string>> defaultValues;

        public Dictionary<string, List<string>> DefaultValues
        {
            get
            {
                if (defaultValues == null)
                {
                    defaultValues = new Dictionary<string, List<string>>();
                }
                return defaultValues;
            }
            set
            {
                defaultValues = value;
            }
        }

        private List<ProductCourseSection> productCourseSections;

        public List<ProductCourseSection> ProductCourseSections
        {
            get
            {
                if (productCourseSections == null)
                {
                    productCourseSections = new List<ProductCourseSection>();
                }
                return productCourseSections;
            }
            set
            {
                productCourseSections = value;
            }
        }

        public string Body { get; set; }
        public string InteractionType { get; set; }
        public string InteractionData { get; set; }
        public string Answer { get; set; }

        public string CustomUrl { get; set; }
    }
}
