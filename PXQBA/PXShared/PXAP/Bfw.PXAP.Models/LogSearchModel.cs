using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PXAP.Models
{
    public class LogSearchModel
    {

        public LogSearchModel()
        {
            SeverityOptions = new List<SelectListItem>();
            CategoryOptions = new List<SelectListItem>();
            SourceOptions = new List<SelectListItem>();
        }        

        public IEnumerable<LogModel> Logs { get; set; }

        public string Severity { get; set; }

        public string Category { get; set; }

        public string Source { get; set; }

        /// <summary>
        /// options for serverity dropdown on the Log Search screen
        /// </summary>
        public List<SelectListItem> SeverityOptions
        {
            get;
            set;
        }

        /// <summary>
        /// options for category dropdown on the Log Search screen
        /// </summary>
        public List<SelectListItem> CategoryOptions
        {
            get;
            set;
        }

        //List<SelectListItem> srcOptions = new List<SelectListItem>();

        /// <summary>
        /// options for source dropdown on the Log Search screen
        /// </summary>
        public List<SelectListItem> SourceOptions
        {
            get;
            set;
        }
                
        //[Required(ErrorMessage = "Is required")]
        //[DataType(DataType.Date, ErrorMessage = "This is not a date!")]          
        [Display(Name = "Start Date")]        
        public DateTime? StartDate { get; set; }
        
        //[Required(ErrorMessage="Is required")]
        //[DataType(DataType.Date, ErrorMessage = "This is not a date!")]        
        [Display(Name = "End Date")]        
        public DateTime? EndDate { get; set; }
        
        [RegularExpression("^[A-Za-z0-9\\.\\s%_\\[\\]\\^-]+$", ErrorMessage = "Not Valid")]
        public string Message { get; set; }
        
    }
}
