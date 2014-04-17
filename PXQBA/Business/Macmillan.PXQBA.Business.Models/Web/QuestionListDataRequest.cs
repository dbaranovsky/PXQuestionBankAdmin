using System.Collections;
using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models.Web
{
    /// <summary>
    //  Represent request for question data by react.js controls
    /// </summary>
    public class QuestionListDataRequest
    {
        public IEnumerable<FilterFieldDescriptor> Filter { get; set; }

        public int PageNumber { get; set; }

        public SortType OrderType { get; set; }

        public string OrderField { get; set; }

        public IEnumerable<string> Columns { get; set; }
    }
}
