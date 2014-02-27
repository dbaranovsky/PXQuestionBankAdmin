using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters for getting questions from DLAP
    /// </summary>
    [DataContract]
    public class QuestionSearch
    {
        /// <summary>
        /// Ids of the questions to find.
        /// </summary>
        public IEnumerable<String> QuestionIds { get; set; }

        /// <summary>
        /// Id of the entity in which the questions exist.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// XPath syntax based query where / is assumed to be the data element of the
        /// question.
        /// </summary>
        public string Query { get; set; }
    }
}
