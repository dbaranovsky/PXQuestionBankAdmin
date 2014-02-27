using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// /// Represents a GroupSet business object.
    /// </summary>
    public class GroupSet
    {
        /// <summary>
        /// Gets or sets the name of the group set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the group set.
        /// </summary>
        public int Id { get; set; }
    }
}
