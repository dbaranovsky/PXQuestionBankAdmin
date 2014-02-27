using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters to filter grades by.
    /// </summary>
    public class GradeListSearch
    {
        /// <summary>
        /// Enrollment id of the enrollment grades belong to.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Id of the item the grades belong to.
        /// </summary>
        public string ItemId { get; set; }
    }
}
