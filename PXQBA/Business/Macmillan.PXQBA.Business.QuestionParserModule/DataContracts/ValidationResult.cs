using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.QuestionParserModule.DataContracts
{
    public class ValidationResult
    {
        private IList<FileValidationResult> fileValidationResults;

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
