using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.Versions
{
    /// <summary>
    /// Question version history model
    /// </summary>
    public  class QuestionHistoryViewModel
    {
        /// <summary>
        /// List of question versions
        /// </summary>
       public  IEnumerable<QuestionVersionViewModel> Versions { get; set; } 
    }
}
