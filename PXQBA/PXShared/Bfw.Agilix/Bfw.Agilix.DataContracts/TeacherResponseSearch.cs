using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters used to filter TeacherResponses.
    /// </summary>
    public class TeacherResponseSearch
    {
        /// <summary>
        /// Id of the enrollment to find responses for.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Id of the item to find responses for.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Version to find responses for.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Collection of responses to look for.
        /// </summary>
       public IEnumerable<string> ResponseIds { get; set; }

      
    }
}

