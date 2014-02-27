using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
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
