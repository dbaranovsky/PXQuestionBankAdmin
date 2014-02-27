using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Wrapper class for a collection of questions.
    /// </summary>
    public class QuestionResultSet
    {
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// List collection of question items.
        /// </summary>
        public IList<Question> Questions { get; set; }
    }
}
