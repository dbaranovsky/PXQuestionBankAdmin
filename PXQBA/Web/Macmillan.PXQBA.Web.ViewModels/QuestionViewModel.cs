using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.ViewModels
{
    /// <summary>
    /// Question DTO
    /// </summary>
    public class QuestionViewModel
    {
        public string Id { get; set; }

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

        public string Status { get; set; }

        public string Preview { get; set; }
      
        /// <summary>
        /// Question particular version
        /// </summary>
        public string Version { get; set; }

        public SharedQuestionDuplicateFromViewModel SharedQuestionDuplicateFrom { get; set; }
        public bool IsShared
        {
            get
            {
                return ProductCourses != null && ProductCourses.Count() > 1;
            }
        }

        public IEnumerable<string> ProductCourses
        {
            get; set;
        }

        public string EntityId { get; set; }

        public string QuizId { get; set; }
        public string ActionPlayerUrl { get; set; }
        public string EditorUrl { get; set; }
        private Dictionary<string, List<string>> localValues;
        public Dictionary<string, List<string>> LocalValues
        {
            get
            {
                if (localValues == null)
                {
                    localValues = new Dictionary<string, List<string>>();
                }
                return localValues;
            }
            set
            {
                localValues = value;
            }
        }

        public string QuestionType { get; set; }

        public string GraphEditorHtml { get; set; }

        public string InteractionData { get; set; }

        public string ParentProductCourseId 
        {
            get
            {
                if (LocalValues.ContainsKey(MetadataFieldNames.ParentProductCourseId))
                {
                    return LocalValues[MetadataFieldNames.ParentProductCourseId].First();
                }
                return localValues[MetadataFieldNames.ProductCourse].First();
            }
        }
    }
}
