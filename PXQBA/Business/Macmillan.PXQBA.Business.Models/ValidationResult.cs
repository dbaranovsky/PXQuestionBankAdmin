using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Models
{
    public class ValidationResult
    {
        public IEnumerable<FileValidationResult> validationResults { get; set; }

        public bool IsValidated
        {
            get
            {
                if (validationResults != null && validationResults.Any())
                {
                    if (validationResults.Any(x => !x.IsValidated))
                    {
                        return false;
                    }
                }
                return false;
            }
        }
    }
}
