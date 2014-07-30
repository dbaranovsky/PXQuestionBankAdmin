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
        /// <summary>
        /// List of filters that should be applied
        /// </summary>
        public IEnumerable<FilterFieldDescriptor> Filter { get; set; }

        /// <summary>
        /// Page number requested
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Order type
        /// </summary>
        public SortType OrderType { get; set; }

        /// <summary>
        /// Order field
        /// </summary>
        public string OrderField { get; set; }

        /// <summary>
        /// List of columns to display
        /// </summary>
        public IEnumerable<string> Columns { get; set; }
    }
}
