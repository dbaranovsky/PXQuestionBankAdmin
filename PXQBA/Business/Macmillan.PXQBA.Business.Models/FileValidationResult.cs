using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Models
{
   public class FileValidationResult
    {
       public int Id { get; set; }

       public string FileName { get; set; }

       public IEnumerable<string> ValidationErrors { get; set; }

       public bool IsValidated
       {
           get
           {
               return ValidationErrors == null || !ValidationErrors.Any();
           }
       }
    }
}
