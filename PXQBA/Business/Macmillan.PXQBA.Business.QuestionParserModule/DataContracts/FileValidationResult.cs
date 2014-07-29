using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.QuestionParserModule.DataContracts
{
    /// <summary>
    /// File validation and parsing result
    /// </summary>
   public class FileValidationResult
    {
       /// <summary>
       /// Name of the file that was parsed
       /// </summary>
       public string FileName { get; set; }

       private IList<string> validationErrors;

       /// <summary>
       /// Validation and parsing errors
       /// </summary>
       public IList<string> ValidationErrors
       {
           get
           {
               if (validationErrors == null)
               {
                   validationErrors = new List<string>();
               }
               return validationErrors;
           }
           set
           {
               validationErrors = value;
           }
       }

       private IList<ParsedQuestion> questions;

       /// <summary>
       /// List of parsed questions
       /// </summary>
       public IList<ParsedQuestion> Questions
       {
           get
           {
               if (questions == null)
               {
                   questions = new List<ParsedQuestion>();
               }
               return questions;
           }
           set
           {
               questions = value;
           }
       }


       private IList<ParsedResource> resources;

       /// <summary>
       /// List of resources that questions have reference to
       /// </summary>
       public IList<ParsedResource> Resources
       {
           get
           {
               if (resources == null)
               {
                   resources = new List<ParsedResource>();
               }
               return resources;
           }
           set
           {
               resources = value;
           }
       }
    }
}
