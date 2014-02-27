using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Assignment reminder email structure
    /// </summary>
    public class ReminderEmail
    {
        /// <summary>
        /// Email subject 
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// email body format
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Days before the email need to be sent
        /// </summary>
        public int DaysBefore { get; set; }

        /// <summary>
        /// Duration type as "week" or "day"
        /// </summary>
        public string DurationType { get; set; }

        /// <summary>
        /// Assignment date
        /// </summary>
        public DateTime AssignmentDate { get; set; }

        /// <summary>
        /// Assignment id
        /// </summary>
        public string AssignmentId { get; set; }
    }
}
