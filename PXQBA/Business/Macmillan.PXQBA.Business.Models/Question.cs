using System;
using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question model
    /// </summary>
    public class Question
    {
        /// <summary>
        /// Question id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Question preview
        /// </summary>
        public string Preview { get; set; }

        /// <summary>
        /// Question status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Question entity id (repository course id)
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Question quiz id (item id)
        /// </summary>
        public string QuizId { get; set; }

        private QuestionMetadataSection defaultSection;

        /// <summary>
        /// Section with default metadata values
        /// </summary>
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

        /// <summary>
        /// List of sections for product course local values
        /// </summary>
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

        /// <summary>
        /// Question body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Question interaction type
        /// </summary>
        public string InteractionType { get; set; }

        /// <summary>
        /// Question interaction data
        /// </summary>
        public string InteractionData { get; set; }

        /// <summary>
        /// Question custom url (for custom questions)
        /// </summary>
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

        /// <summary>
        /// Id of the question current question is created as a draft from
        /// </summary>
        public string DraftFrom { get; set; }

        /// <summary>
        /// Question version number 
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Modified date
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Modified by
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Id of the question current question was duplicated from
        /// </summary>
        public string DuplicateFrom { get; set; }

        /// <summary>
        /// Id of the shared question current question was duplicated from
        /// </summary>
        public string DuplicateFromShared { get; set; }

        /// <summary>
        /// Version number current question version was restored from
        /// </summary>
        public string RestoredFromVersion { get; set; }

        /// <summary>
        /// Indicates if question version is published from draft
        /// </summary>
        public bool IsPublishedFromDraft { get; set; }

        /// <summary>
        /// Question score
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Indicates if current version of the question is initial version of draft
        /// </summary>
        public bool IsDraftInitialVersion { get; set; }

        /// <summary>
        /// General feedback
        /// </summary>
        public string GeneralFeedback { get; set; }
    }
}
