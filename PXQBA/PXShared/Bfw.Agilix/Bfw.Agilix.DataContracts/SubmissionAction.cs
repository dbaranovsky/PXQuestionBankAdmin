using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    public class SubmissionAction
    {
        /// <summary>
        /// The student's location, such as IP address, where the student performed the action
        /// </summary>
        public String Location { get; set; }

        /// <summary>
        /// Type of the action
        /// start|save|resume|submit
        /// </summary>
        public SubmissionActionType Type { get; set; }

        /// <summary>
        /// The date and time the student performed the action
        /// </summary>
        public DateTime Date { get; set; }
    }
}
