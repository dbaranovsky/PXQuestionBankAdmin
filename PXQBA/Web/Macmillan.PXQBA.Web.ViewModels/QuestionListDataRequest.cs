using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels.Filter;

namespace Macmillan.PXQBA.Web.ViewModels
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
