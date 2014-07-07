using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Commands.DataContracts
{
    public class QuestionSearchResult
    {
        public QuestionSearchResult()
        {
            DynamicFields = new Dictionary<string, string>();
        }

        public string QuestionId { get; set; }

        public string SortingField { get; set; }

        public string Index { get; set; }

        public string DraftFrom { get; set; }

        public Dictionary<string, string> DynamicFields { get; set; } 
    }
}
