using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class Assign
    {

        /// <summary>
        /// assign item id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// assignmnet due date
        /// </summary>
        public DateTime DueDate { get; set; }

        ///// <summary>
        ///// behaviour if the action is assign or unassign
        ///// </summary>
        //public string Behavior { get; set; }

        /// <summary>
        /// completion trigger
        /// </summary>
        public CompletionTrigger CompletionTrigger
        {
            get; 
            set;
        }

        /// <summary>
        /// gradebook category
        /// </summary>
        public string GradebookCategory { get; set; }

        /// <summary>
        /// syllabus filter
        /// </summary>
        public string SyllabusFilter { get; set; }

        /// <summary>
        /// points
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// runric id
        /// </summary>
        public string RubricId { get; set; }

        /// <summary>
        /// flag gradable
        /// </summary>
        public bool IsGradeable { get; set; }

        /// <summary>
        /// flag late submission
        /// </summary>
        public bool IsAllowLateSubmission { get; set; }

        /// <summary>
        /// flag send reminder
        /// </summary>
        public bool IsSendReminder { get; set; }

        /// <summary>
        /// reminder duration
        /// </summary>
        public int ReminderDurationCount { get; set; }

        /// <summary>
        /// reminder duration type (days/week)
        /// </summary>
        public string ReminderDurationType { get; set; }

        /// <summary>
        /// reminder subject
        /// </summary>
        public string ReminderSubject { get; set; }

        /// <summary>
        /// reminder body
        /// </summary>
        public string ReminderBody { get; set; }

        /// <summary>
        /// include gbb trigger
        /// </summary>
        public int IncludeGbbScoreTrigger { get; set; }

        /// <summary>
        /// flag highlight late submission
        /// </summary>
        public bool IsHighlightLateSubmission { get; set; }

        /// <summary>
        /// flag allow grace period
        /// </summary>
        public bool IsAllowLateGracePeriod { get; set; }

        /// <summary>
        /// Is Allow Extra Credit
        /// </summary>
        public bool IsAllowExtraCredit { get; set; }

        /// <summary>
        /// late submission grace duration
        /// </summary>
        public long LateGraceDuration { get; set; }
        
        /// <summary>
        /// late submission grace duration type
        /// </summary>
        public string LateGraceDurationType { get; set; }

        /// <summary>
        /// calculation type trigger
        /// </summary>
        public SubmissionGradeAction? SubmissionGradeAction { get; set; }

        /// <summary>
        /// Grade Rule
        /// </summary>
        public string GradeRule { get; set; }
    }
}
