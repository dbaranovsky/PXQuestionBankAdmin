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
                    return validationResults.All(x => x.IsValidated);
                }
                return false;
            }
        }
    }
}
