using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters for filtering enrollment activity.
    /// </summary>
    public class EnrollmentActivitySearch
    {
        /// <summary>
        /// Id of the enrollment to filter activity of.
        /// </summary>
        public string EnrollmentId { get; set; }
    }
}
