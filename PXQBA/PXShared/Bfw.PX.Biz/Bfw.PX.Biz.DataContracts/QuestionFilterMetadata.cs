using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
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
