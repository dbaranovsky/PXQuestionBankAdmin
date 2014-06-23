using System;
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
                if (string.IsNullOrEmpty(LocalSection.ParentProductCourseId))
                {
                    return LocalSection.ProductCourseId;
                }
                return LocalSection.ParentProductCourseId;
            }
        }
        public string DraftFrom { get; set; }

        public bool IsDraft { get { return !String.IsNullOrEmpty(DraftFrom); } }

        public double Score { get; set; }


        #region Capabilities
        public bool CanTestQuestion { get; set; }
        public bool CanOverrideMetadata { get; set; }

        public bool CanRestoreMetadata { get; set; }

        public bool CanTrySepcificVersion { get; set; }

        public bool CanEditAvailibleQuestion { get; set; }
        public bool CanEditInProgesQuestion { get; set; }
        public bool CanEditDeletedQuestion { get; set; }

        public bool CanEditSharedQuestionContent { get; set; }
        public bool CanEditSharedQuestionMetadata{ get; set; }

        public bool CanPublishDraft { get; set; }
        public bool CanRestoreVersion { get; set; }

        public bool CanCreateDraftFromVersion { get; set; }
        #endregion



    }
}
