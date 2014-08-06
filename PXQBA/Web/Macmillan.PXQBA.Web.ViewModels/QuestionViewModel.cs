using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels.Versions;

namespace Macmillan.PXQBA.Web.ViewModels
{
    /// <summary>
    /// Question model
    /// </summary>
    public class QuestionViewModel
    {
        /// <summary>
        /// Question id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Question id from dlap
        /// </summary>
        public string RealQuestionId { get; set; }

        private QuestionMetadataSection defaultSection;

        /// <summary>
        /// Question default section
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

        /// <summary>
        /// Question status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Question preview
        /// </summary>
        public string Preview { get; set; }
      
        /// <summary>
        /// Model for shared question current question was duplicated from
        /// </summary>
        public SharedQuestionDuplicateFromViewModel SharedQuestionDuplicateFrom { get; set; }

        /// <summary>
        /// Indicates if question is shared
        /// </summary>
        public bool IsShared
        {
            get
            {
                return ProductCourses != null && ProductCourses.Count() > 1;
            }
        }

        /// <summary>
        /// List of product courses question belongs to
        /// </summary>
        public IEnumerable<string> ProductCourses
        {
            get; set;
        }

        /// <summary>
        /// Repository course id
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Quiz id question belongs to
        /// </summary>
        public string QuizId { get; set; }

        /// <summary>
        /// Action player url
        /// </summary>
        public string ActionPlayerUrl { get; set; }

        /// <summary>
        /// Editor url
        /// </summary>
        public string EditorUrl { get; set; }

        private QuestionMetadataSection localSection;

        /// <summary>
        /// Local metadata section
        /// </summary>
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

        /// <summary>
        /// Question type
        /// </summary>
        public string QuestionType { get; set; }

        /// <summary>
        /// Graph editor html
        /// </summary>
        public string GraphEditorHtml { get; set; }

        /// <summary>
        /// Interation data
        /// </summary>
        public string InteractionData { get; set; }

        /// <summary>
        /// Parent product course id
        /// </summary>
        public string ParentProductCourseId 
        {
            get
            {
                if (string.IsNullOrEmpty(LocalSection.ParentProductCourseId))
                {
                    return LocalSection.ProductCourseId;
                }
                return LocalSection.ParentProductCourseId;
            }
        }
        /// <summary>
        /// Original question id if current question is draft
        /// </summary>
        public string DraftFrom { get; set; }

        /// <summary>
        /// Indicates if current question is draft
        /// </summary>
        public bool IsDraft { get { return !String.IsNullOrEmpty(DraftFrom); } }

        /// <summary>
        /// Question score
        /// </summary>
        public double Score { get; set; }


        #region Capabilities

        /// <summary>
        /// Indicates if current user has capability to test question
        /// </summary>
        public bool CanTestQuestion { get; set; }

        /// <summary>
        /// Indicates if current user has capability to test question version
        /// </summary>
        public bool CanTestQuestionVersion { get; set; }

        /// <summary>
        /// Indicates if current user has capability to override metadata
        /// </summary>
        public bool CanOverrideMetadata { get; set; }

        /// <summary>
        /// Indicates if current user has capability to restore metadata
        /// </summary>
        public bool CanRestoreMetadata { get; set; }

        /// <summary>
        /// Indicates if current user has capability to edit question
        /// </summary>
        public bool CanEditQuestion { get; set; }

        /// <summary>
        /// Indicates if current user has capability to edit shared question content
        /// </summary>
        public bool CanEditSharedQuestionContent { get; set; }

        /// <summary>
        /// Indicates if current user has capability to edit shared question metadata
        /// </summary>
        public bool CanEditSharedQuestionMetadata{ get; set; }

        /// <summary>
        /// Indicates if current user has capability to publish draft
        /// </summary>
        public bool CanPublishDraft { get; set; }

        /// <summary>
        /// Indicates if current user has capability to restore version
        /// </summary>
        public bool CanRestoreVersion { get; set; }

        /// <summary>
        /// Indicates if current user has capability to create draft from version
        /// </summary>
        public bool CanCreateDraftFromVersion { get; set; }

        /// <summary>
        /// Indicates if current user has capability to view history
        /// </summary>
        public bool CanViewHistory { get; set; }

        /// <summary>
        /// Indicates if current user has capability to add notes to question
        /// </summary>
        public bool CanAddNotesQuestion { get; set; }

        /// <summary>
        /// Indicates if current user has capability to remove notes from question
        /// </summary>
        public bool CanRemoveNotesQuestion { get; set; }
        #endregion



    }
}
