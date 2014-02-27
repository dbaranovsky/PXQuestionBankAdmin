using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents an item updated in the system.
    /// </summary>
    public class ItemUpdate
    {
        /// <summary>
        /// updated assignment's course id
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// updated assignment's student enrollment id
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Updated item id which may even be a folder student has created 
        /// </summary>
        public string ItemId { get; set; }
    }
}
