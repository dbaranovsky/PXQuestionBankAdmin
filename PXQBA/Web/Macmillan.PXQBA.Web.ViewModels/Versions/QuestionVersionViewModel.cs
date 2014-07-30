using System;
using System.Security;

namespace Macmillan.PXQBA.Web.ViewModels.Versions
{
    /// <summary>
    /// Single question version model
    /// </summary>
    public class QuestionVersionViewModel
    {
        /// <summary>
        /// Question id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Version number
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Version was modified/created
        /// </summary>
        public string  ModifiedDate { get; set; }

        /// <summary>
        /// Version was modified/created by
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// If current version was duplicated then reference to the original question is stored
        /// </summary>
        public DuplicateFromViewModel DuplicateFrom { get; set; }

        /// <summary>
        /// If current version was restored from another version, then note about this is stored
        /// </summary>
        public RestoredFromVersionViewModel RestoredFromVersion { get; set; }

        /// <summary>
        /// Indicates if current version was published from draft
        /// </summary>
        public bool IsPublishedFromDraft { get; set; }

        /// <summary>
        /// Question preview
        /// </summary>
        public string QuestionPreview { get; set; }

        /// <summary>
        /// Indicates if current version is draft initial version
        /// </summary>
        public bool IsDraftInitialVersion { get; set; }

    }
}
