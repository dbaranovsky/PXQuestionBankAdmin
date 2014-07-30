using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels.Filter;

namespace Macmillan.PXQBA.Web.ViewModels
{
    /// <summary>
    /// Represent collection of question for react.js controls
    /// </summary>
    public class QuestionListDataResponse
    {
        /// <summary>
        /// Filtration
        /// </summary>
        public IEnumerable<FilterFieldDescriptor> Filter { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// List of questions
        /// </summary>
        public IEnumerable<QuestionMetadata> QuestionList { get; set; }

        /// <summary>
        /// Total pages for current query
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Field Ordering
        /// </summary>
        public QuestionOrder Order { get; set; }

        /// <summary>
        /// List of columns to display
        /// </summary>
        public IEnumerable<QuestionFieldViewModel> Columns { get; set; }

        /// <summary>
        /// List of all available for display columns
        /// </summary>
        public IEnumerable<QuestionFieldViewModel> AllAvailableColumns { get; set; }

        /// <summary>
        /// Question card layout filled with values
        /// </summary>
        public string QuestionCardLayout { get; set; }

        /// <summary>
        /// Product course name
        /// </summary>
        public string ProductTitle { get; set; }

        #region Capabilities
      
        /// <summary>
        /// Indicates if current user can view question list
        /// </summary>
        public bool CanViewQuestionList { get; set; }

        /// <summary>
        /// Indicates if current user can preview questions
        /// </summary>
        public bool CanPreviewQuestion { get; set; }

        /// <summary>
        /// Indicates if current user can create questions
        /// </summary>
        public bool CanCreateQuestion { get; set; }

        /// <summary>
        /// Indicates if current user can duplicate questions
        /// </summary>
        public bool CanDuplicateQuestion { get; set; }

        /// <summary>
        /// Indicates if current user can flag questions
        /// </summary>
        public bool CanFlagQuestion { get; set; }

        /// <summary>
        /// Indicates if current user can unflag questions
        /// </summary>
        public bool CanUnflagQuestion { get; set; }

        /// <summary>
        /// Indicates if current user can add notes to questions
        /// </summary>
        public bool CanAddNotesQuestion { get; set; }

        /// <summary>
        /// Indicates if user can remove notes from question
        /// </summary>
        public bool CanRemoveNotesQuestion { get; set; }

        /// <summary>
        /// Indicates if current user can share questions
        /// </summary>
        public bool CanShareQuestion { get; set; }

        /// <summary>
        /// Indicates if current user can view question history
        /// </summary>
        public bool CanViewHistory { get; set; }

        /// <summary>
        /// Indicates if current user can create question drafts
        /// </summary>
        public bool CanCreateNewDraft { get; set; }

        /// <summary>
        /// Indicates if current user can publish draft
        /// </summary>
        public bool CanPublishDraft { get; set; }

        /// <summary>
        /// Indicates if current user can change question status from Available to In progress
        /// </summary>
        public bool CanChangeFromAvailibleToInProgress { get; set; }

        /// <summary>
        /// Indicates if current user can change question status from Available to Deleted
        /// </summary>
        public bool CanChangeFromAvailibleToDeleted { get; set; }

        /// <summary>
        /// Indicates if current user can change question status from Deleted to In progress
        /// </summary>
        public bool CanChangeFromDeletedToInProgress { get; set; }

        /// <summary>
        /// Indicates if current user can change question status from Deleted to Available
        /// </summary>
        public bool CanChangeFromDeletedToAvailible { get; set; }

        /// <summary>
        /// Indicates if current user can change question status from In Progress to Available
        /// </summary>
        public bool CanChangeFromInProgressToAvailible { get; set; }

        /// <summary>
        /// Indicates if current user can change question status from In Progress to Deleted
        /// </summary>
        public bool CanChangeFromInProgressToDeleted { get; set; }

        #endregion


    }
}