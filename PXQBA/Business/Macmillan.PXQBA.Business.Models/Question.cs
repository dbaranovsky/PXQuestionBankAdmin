using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question DTO
    /// </summary>
    //public class Question
    //{
    //    public string Id { get; set; }

    //    public QuestionStaticMetadata LocalMetadata { get; set; }

    //    public QuestionType Type { get; set; }
    //    public string Preview { get; set; }

    //    /// <summary>
    //    /// Question particular version
    //    /// </summary>
    //    public string Version { get; set; }

    //    public string QuestionIdDuplicateFrom { get; set; }

    //    private List<ProductCourseSection> productCourses;
    //    public List<ProductCourseSection> ProductCourses
    //    {
    //        get
    //        {
    //            if (productCourses == null)
    //            {
    //                productCourses = new List<ProductCourseSection>();
    //            }
    //            return productCourses;
    //        }
    //        set
    //        {
    //            productCourses = value;
    //        }
    //    } 

    //    public string EntityId { get; set; }

    //    public string QuizId { get; set; }

    //    public QuestionStaticMetadata SharedMetadata { get; set; }
    //}

    public class Question
    {
        public string Id { get; set; }

        public string Preview { get; set; }

        public string EntityId { get; set; }

        public string QuestionIdDuplicateFrom { get; set; }

        public QuestionType Type { get; set; }

        private Dictionary<string, IList<string>> defaultValues;

        public Dictionary<string, IList<string>> DefaultValues
        {
            get
            {
                if (defaultValues == null)
                {
                    defaultValues = new Dictionary<string, IList<string>>();
                }
                return defaultValues;
            }
            set
            {
                defaultValues = value;
            }
        }

        private IList<ProductCourseSectionNew> productCourseSections;

        public IList<ProductCourseSectionNew> ProductCourseSections
        {
            get
            {
                if (productCourseSections == null)
                {
                    productCourseSections = new List<ProductCourseSectionNew>();
                }
                return productCourseSections;
            }
            set
            {
                productCourseSections = value;
            }
        }

    }

    public class ProductCourseSectionNew
    {
        public string ProductCourseId { get; set; }

        private Dictionary<string, IEnumerable<string>> productCourseValues;

        public Dictionary<string, IEnumerable<string>> ProductCourseValues
        {
            get
            {
                if (productCourseValues == null)
                {
                    productCourseValues = new Dictionary<string, IEnumerable<string>>();
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
