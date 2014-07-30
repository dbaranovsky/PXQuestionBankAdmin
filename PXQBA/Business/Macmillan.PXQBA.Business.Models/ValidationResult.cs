using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Parsing result
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// List of file parsing results
        /// </summary>
        public IEnumerable<FileValidationResult> FileValidationResults { get; set; }

        /// <summary>
        /// Indicates if files are validated
        /// </summary>
        public bool IsValidated
        {
            get
            {
                if (FileValidationResults != null && FileValidationResults.Any())
                {
                    return FileValidationResults.All(x => x.IsValidated);
                }
                return false;
            }
        }
    }
}
