using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Task as documented in the http://dev.dlap.bfwpub.com/Docs/Command/RunTask command.
    /// </summary>
    [DataContract]
    public class Task
    {
        /// <summary>
        /// Id of the task.
        /// </summary>
        [DataMember]
        public String TaskId { get; set; }

        /// <summary>
        /// Command that represents the task to run.
        /// </summary>
        [DataMember]
        public String Command { get; set; }

        /// <summary>
        /// Time, in minutes, to between runs of the task.
        /// </summary>
        [DataMember]
        public int PeriodMinutes { get; set; }

        /// <summary>
        /// Date and time the task should start running.
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date and time task was created.
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Date and time task last ran.
        /// </summary>
        [DataMember]
        public DateTime LastRunStartDate { get; set; }

        /// <summary>
        /// Date and time task last completed.
        /// </summary>
        [DataMember]
        public DateTime LastRunEndDate { get; set; }

        /// <summary>
        /// True if the task has finished.
        /// </summary>
        [DataMember]
        public Boolean Finished { get; set; }

        /// <summary>
        /// Number between 0.0 and 1.0 that tells how much of task is complete.
        /// </summary>
        [DataMember]
        public Double PortionComplete { get; set; }

        /// <summary>
        /// Most recently reported status of the task.
        /// </summary>
        [DataMember]
        public String CurrentItem { get; set; }

        /// <summary>
        /// Textual description of any error that has ocured while running task.
        /// </summary>
        [DataMember]
        public String Error { get; set; }

        /// <summary>
        /// Textual result of the task when run compelted successfully.
        /// </summary>
        [DataMember]
        public String Success { get; set; }
    }
}
