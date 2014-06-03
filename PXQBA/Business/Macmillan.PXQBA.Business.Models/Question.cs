using System;
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

        private List<QuestionMetadataSection> productCourseSections;

        public List<QuestionMetadataSection> ProductCourseSections
        {
            get
            {
                if (productCourseSections == null)
                {
                    productCourseSections = new List<QuestionMetadataSection>();
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
        public string CustomUrl { get; set; }

        /// <summary>
        /// Choice list if this is a multiple choice question.
        /// </summary>
        public IList<QuestionChoice> Choices;

        /// <summary>
        /// Correct answer.
        /// </summary>
        public string Answer
        {
            get
            {
                string result = string.Empty;

                if (AnswerList != null && AnswerList.Count > 0)
                {
                    result = AnswerList.First();
                }

                return result;
            }
        }

        /// <summary>
        /// Answer List
        /// </summary>
        public IList<string> AnswerList;

        public string DraftFrom { get; set; }

        public string Version { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public string DuplicateFrom { get; set; }

        public string DuplicateFromShared { get; set; }
    }
}
