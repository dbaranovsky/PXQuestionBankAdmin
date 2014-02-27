using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a DLAP task. See http://gls.agilix.com/Docs/Concept/Tasks.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Gets or sets the task ID.
        /// </summary>
        [DataMember]
        public String TaskId { get; set; }

        /// <summary>
        /// The task command-line including parameters, if any.
        /// </summary>
        [DataMember]
        public String Command { get; set; }

        /// <summary>
        /// The number of minutes between task start times. Must be greater than 0.
        /// </summary>
        [DataMember]
        public int PeriodMinutes { get; set; }

        /// <summary>
        /// A date and time to start the task. If startdate is in the past, the task starts tomorrow at the specified time. If omitted, the task starts as soon as possible.
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date the task was created.
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Last date and time the task was executed.
        /// </summary>
        [DataMember]
        public DateTime LastRunStartDate { get; set; }

        /// <summary>
        /// Last date and time the task completed.
        /// </summary>
        [DataMember]
        public DateTime LastRunEndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Task"/> is finished.
        /// </summary>
        /// <value>
        ///   <c>true</c> if finished; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public Boolean Finished { get; set; }

        /// <summary>
        /// When finished is false, contains a number between 0.0 and 1.0 indicating the fraction of the task that has completed.
        /// </summary>
        [DataMember]
        public Double PortionComplete { get; set; }

        /// <summary>
        /// When finished is false, contains end-user-reportable text of the current task's progress.
        /// </summary>
        [DataMember]
        public String CurrentItem { get; set; }

        /// <summary>
        /// An error string returned by the task when it has finished with an error.
        /// </summary>
        [DataMember]
        public String Error { get; set; }

        /// <summary>
        /// A success string returned by the task when it has finished successfully.
        /// </summary>
        [DataMember]
        public String Success { get; set; }        
    }
}
 