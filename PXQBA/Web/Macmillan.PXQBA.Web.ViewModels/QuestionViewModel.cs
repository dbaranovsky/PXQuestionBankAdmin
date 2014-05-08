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

        public QuestionStaticMetadata LocalMetadata { get; set; }

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
        public QuestionStaticMetadata SharedMetadata { get; set; }

    }
}
