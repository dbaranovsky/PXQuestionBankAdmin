using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Commands.DataContracts
{
    public class QuestionDynamicSearchResult
    {
        public QuestionDynamicSearchResult()
        {
            Values = new Dictionary<string, string>();
        }

        public string QuestionId { get; set; }

        public Dictionary<string, string> Values { get; set; } 
    }
}
