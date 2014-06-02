using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels.Versions;

namespace Macmillan.PXQBA.Web.ViewModels
{
    /// <summary>
    /// Question DTO
    /// </summary>
    public class QuestionViewModel
    {

        public string Id { get; set; }

        private QuestionMetadataSection defaultSection;
        public QuestionMetadataSection DefaultSection
        {
            get
            {
                if (defaultSection == null)
                {
                    defaultSection = new QuestionMetadataSection();
                }
                return defaultSection;
            }
            set
            {
                defaultSection = value;
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
        private QuestionMetadataSection localSection;
        public QuestionMetadataSection LocalSection
        {
            get
            {
                if (localSection == null)
                {
                    localSection = new QuestionMetadataSection();
                }
                return localSection;
            }
            set
            {
                localSection = value;
            }
        }

        public string QuestionType { get; set; }

        public string GraphEditorHtml { get; set; }

        public string InteractionData { get; set; }

        public string ParentProductCourseId 
        {
            get
            {
                return "70295";
                if (string.IsNullOrEmpty(LocalSection.ParentProductCourseId))
                {
                    return LocalSection.ProductCourseId;
                }
                return LocalSection.ParentProductCourseId;
            }
        }
        public bool IsDraft { get; set; }        
        public IEnumerable<QuestionVersionViewModel> Versions { get; set; } 

    }
}
