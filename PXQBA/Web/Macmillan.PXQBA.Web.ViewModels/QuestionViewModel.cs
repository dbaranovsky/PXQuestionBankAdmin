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

        public QuestionType Type { get; set; }
        public string Preview { get; set; }
      
        /// <summary>
        /// Question particular version
        /// </summary>
        public string Version { get; set; }

        public bool IsDuplicateOfSharedQuestion
        {
            get
            {
                return !string.IsNullOrEmpty(QuestionIdDuplicateFrom);
            }
        }

        public string QuestionIdDuplicateFrom{ get; set; }

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
        private Dictionary<string, IEnumerable<string>> localValues;
        public Dictionary<string, IEnumerable<string>> LocalValues
        {
            get
            {
                if (localValues == null)
                {
                    localValues = new Dictionary<string, IEnumerable<string>>();
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
