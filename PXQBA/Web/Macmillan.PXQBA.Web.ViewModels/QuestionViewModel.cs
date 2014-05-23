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

        public bool IsDuplicateOfSharedQuestion
        {
            get
            {
                return LocalValues.ContainsKey(MetadataFieldNames.QuestionIdDuplicateFrom) &&
                       !string.IsNullOrEmpty(LocalValues[MetadataFieldNames.QuestionIdDuplicateFrom].FirstOrDefault());
            }
        }

        public bool IsShared
        {
            get
            {
                return ProductCourses != null && ProductCourses.Count() > 1;
            }
        }

        public string SharedWith
        {
            get
            {
                return string.Join(", ", ProductCourses);
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

    }
}
