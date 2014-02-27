using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXAP.Models
{
   public class LogModel
    {
       public int LogID { get; set; }
       public string Severity { get; set; }
        public string Source { get; set; }
        public DateTime? Time { get; set; }
        public string CategoryName { get; set; }
        public string Message { get; set; }
    }
}
