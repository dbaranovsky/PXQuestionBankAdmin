using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.Import
{
    /// <summary>
    /// View model for parsed files
    /// </summary>
    public class ImportFromFileViewModel
    {
        /// <summary>
        /// Id of the parsed file stored in database
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// Indicates if file was imported
        /// </summary>
        public bool IsImported { get; set; }

        /// <summary>
        /// Indicates if there are no valid questions to import
        /// </summary>
        public bool IsNothingToImport { get; set; }

        /// <summary>
        /// Parsing error message
        /// </summary>
        public string ParsingErrorMessage { get; set; }

        /// <summary>
        /// Number of questions that were successfully parsed
        /// </summary>
        public int QuestionToImport { get; set; }

        /// <summary>
        /// Number of questions that were skipped
        /// </summary>
        public int QuestionSkipped { get; set; }
    }
}
