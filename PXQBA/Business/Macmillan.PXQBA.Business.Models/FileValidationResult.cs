using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Result of parsed file validation
    /// </summary>
   public class FileValidationResult
    {
       /// <summary>
       /// Parsed file id
       /// </summary>
       public long Id { get; set; }

       /// <summary>
       /// File name
       /// </summary>
       public string FileName { get; set; }

       /// <summary>
       /// Parsed file validation errors
       /// </summary>
       public IEnumerable<string> ValidationErrors { get; set; }

       /// <summary>
       /// Indicates if file was validated
       /// </summary>
       public bool IsValidated
       {
           get
           {
               return ValidationErrors == null || !ValidationErrors.Any();
           }
       }

       /// <summary>
       /// Number of questions parsed
       /// </summary>
       public int QuestionParsed { get; set; }

       /// <summary>
       /// Number of questions skipped while parsing
       /// </summary>
       public int QuestionSkipped { get; set; }
    }
}
