using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{    
    /// <summary>
    /// Parameters for filtering gradebook weights.
    /// </summary>
    [DataContract]
    public class GradeBookWeightSearch
    {
        /// <summary>
        /// Id of the entity the gradebook weights belong to.
        /// </summary>
        public string EntityId { get; set; }
    }
}
