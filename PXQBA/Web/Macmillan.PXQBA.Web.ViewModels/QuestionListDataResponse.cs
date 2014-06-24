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

        public IEnumerable<QuestionFieldViewModel> Columns { get; set; }

        public IEnumerable<QuestionFieldViewModel> AllAvailableColumns { get; set; }

        public string QuestionCardLayout { get; set; }

        public string ProductTitle { get; set; }

        #region Capabilities
      
        public bool CanViewQuestionList { get; set; }
        public bool CanPreviewQuestion { get; set; }
        public bool CanCreateQuestion { get; set; }
        public bool CanDuplicateQuestion { get; set; }

        public bool CanFlagQuestion { get; set; }
        public bool CanUnflagQuestion { get; set; }

        public bool CanAddNotesQuestion { get; set; }

        public bool CanRemoveNotesQuestion { get; set; }

        public bool CanShareQuestion { get; set; }

        public bool CanViewHistory { get; set; }

        public bool CanCreateNewDraft { get; set; }

        public bool CanPublishDraft { get; set; }

        public bool CanChangeFromAvailibleToInProgress { get; set; }
        public bool CanChangeFromAvailibleToDeleted { get; set; }
        public bool CanChangeFromDeletedToInProgress { get; set; }
        public bool CanChangeFromDeletedToAvailible { get; set; }
        public bool CanChangeFromInProgressToAvailible { get; set; }
        public bool CanChangeFromInProgressToDeleted { get; set; }

        #endregion


    }
}