using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Commands.DataContracts
{
    /// <summary>
    /// Represents the result returned by SOLR Search command
    /// </summary>
    public class QuestionSearchResult
    {
        public QuestionSearchResult()
        {
            DynamicFields = new Dictionary<string, string>();
        }

        /// <summary>
        /// Question id
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// Field name that is used for sorting by
        /// </summary>
        public string SortingField { get; set; }

        /// <summary>
        /// Id of the question current question was created as draft from
        /// </summary>
        public string DraftFrom { get; set; }

        /// <summary>
        /// Any fields that a requested in SOLR Search command
        /// </summary>
        public Dictionary<string, string> DynamicFields { get; set; } 
    }
}
