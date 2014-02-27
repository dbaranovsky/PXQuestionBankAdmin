using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// settings for the assign tab used to assign the content items
    /// </summary>
    public class AssignTabSettings
    {
        /// <summary>
        /// Shows 'Make Gradable' checkbox in the assign tab
        /// </summary>
        public Boolean ShowMakeGradeable { get; set; }

        /// <summary>
        ///  shows "Allow Late Submission" checkbox
        /// </summary>
        public Boolean ShowAllowLateSubmissions { get; set; }

        public Boolean ShowScheduleReminder { get; set; }

        /// <summary>
        /// shows a checkbox that allows the user to make the system send a reminder
        /// </summary>
        public Boolean ShowSendReminder { get; set; }

        public Boolean ShowSubContentCreation { get; set; }

        /// <summary>
        /// Shows the other assingments scheduled for the same day
        /// </summary>
        public Boolean ShowAssignedSameDay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Boolean ShowGradebookCategory { get; set; }

        public Boolean ShowSyllabusCategory { get; set; }

        public Boolean ShowPointsPossible { get; set; }

        /// <summary>
        ///  shows the dropdown "Include score in gradebook"
        /// </summary>
        public Boolean ShowIncludeScore { get; set; }

        public Boolean ShowCalculationType { get; set; }

        /// <summary>
        /// show/hide Show Mark As Complete
        /// </summary>
        public Boolean ShowMarkAsComplete { get; set; }

        /// <summary>
        /// Toggle showing of the completion category
        /// </summary>
        public Boolean ShowCompletionCategory { get; set; }

    }
}
