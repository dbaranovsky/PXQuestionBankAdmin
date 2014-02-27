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
    /// Represents the dashboard settings.
    /// </summary>
    public class QuestionFilterMetadata
    {
        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// filterable
        /// </summary>
        public bool Filterable { get; set; }
        /// <summary>
        /// searchterm
        /// </summary>
        public string Searchterm { get; set; }
        /// <summary>
        /// friendlyname
        /// </summary>
        public string Friendlyname { get; set; }
    }


     
}
