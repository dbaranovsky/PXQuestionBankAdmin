using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question metadata
    /// </summary>
    public class QuestionMetadata
    {
        public QuestionMetadata()
        {
            Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// List of all the metadata fields
        /// </summary>
        public Dictionary<string, string> Data { get; set; } 

        /// <summary>
        /// Indicates if current user has capability to edit questions
        /// </summary>
        public bool CanEditQuestion { get; set; }

        /// <summary>
        /// Indicates if current user has capability to create drafts from Available to Instructors questions
        /// </summary>
        public bool CanCreateDraftFromAvailableQuestion { get; set; }

        /// <summary>
        /// Indicates if current user has capability to change draft question status
        /// </summary>
        public bool CanChangeDraftStatus { get; set; }
    }
}
