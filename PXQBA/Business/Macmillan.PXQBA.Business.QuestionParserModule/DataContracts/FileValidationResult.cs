using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.QuestionParserModule.DataContracts
{
   public class FileValidationResult
    {
       public string FileName { get; set; }

       private IList<string> validationErrors;

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
