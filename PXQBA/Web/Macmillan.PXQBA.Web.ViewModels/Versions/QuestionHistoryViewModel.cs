using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.Versions
{
    public  class QuestionHistoryViewModel
    {
       public  IEnumerable<QuestionVersionViewModel> Versions { get; set; } 
    }
}
