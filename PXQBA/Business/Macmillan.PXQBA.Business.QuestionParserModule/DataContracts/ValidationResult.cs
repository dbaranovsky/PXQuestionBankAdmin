using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.QuestionParserModule.DataContracts
{
    /// <summary>
    /// Parsing result
    /// </summary>
    public class ValidationResult
    {
        private IList<FileValidationResult> fileValidationResults;

        /// <summary>
        /// List of file parsing results
        /// </summary>
        public IList<FileValidationResult> FileValidationResults
        {
            get
            {
                if (fileValidationResults == null)
                {
                    fileValidationResults = new List<FileValidationResult>();
                }
                return fileValidationResults;
            }
            set
            {
                fileValidationResults = value;
            }
        }
    }
}
