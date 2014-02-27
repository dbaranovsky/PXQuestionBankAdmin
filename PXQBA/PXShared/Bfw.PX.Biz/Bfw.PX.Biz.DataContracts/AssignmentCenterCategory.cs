using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// A category in the AssignmentCenter
    /// </summary>
    public class AssignmentCenterCategory
    {
        /// <summary>
        /// The Id of the category.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Start date of the category, if assigned.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the category, if assigned.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
