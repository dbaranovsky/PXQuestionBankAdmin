using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents the Question filter
    /// </summary>
    public class QuestionFilter
    {
        /// <summary>
        /// List of Filter Metadata
        /// </summary>
        public IEnumerable<QuestionFilterMetadata> FilterMetadata { get; set; }
    }
}
