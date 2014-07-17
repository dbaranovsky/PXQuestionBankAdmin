using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Models
{
    public class ValidationResult
    {
        public IEnumerable<FileValidationResult> FileValidationResults { get; set; }

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
