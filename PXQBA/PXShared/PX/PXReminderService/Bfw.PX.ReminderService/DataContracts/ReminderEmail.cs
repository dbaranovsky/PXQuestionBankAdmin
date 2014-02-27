using System;
using System.Collections.Generic;
using System.Text;

namespace Bfw.PX.ReminderService.DataContracts
{
    /// <summary>
    /// Reminder email deatisl
    /// </summary>
    public class ReminderEmail
    {
        /// <summary>
        /// tracking id
        /// </summary>
        public int EmailId { get; set; }

        /// <summary>
        /// reminder itm id
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// course title
        /// </summary>
        public string CourseTitle { get; set; }

        /// <summary>
        /// course id
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// product id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// instructor id
        /// </summary>
        public int InstructorId { get; set; }

        /// <summary>
        /// subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// user defined body
        /// </summary>
        public string UserBody { get; set; }

        /// <summary>
        /// email template HTML body
        /// </summary>
        public string TemplateBodyHTML { get; set; }

        /// <summary>
        /// email template text body
        /// </summary>
        public string TemplateBodyText { get; set; }

        /// <summary>
        /// list of students or instructor to send email for
        /// </summary>
        public List<Sender> Senders { get; set; }

        /// <summary>
        /// sent status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// email to list
        /// </summary>
        public string ToList { get; set; }

        /// <summary>
        /// Gets or sets notification type [Defined in PXPub, Web.Config]
        /// 1 - AssignmentReminderEmailEventType
        /// 2 - PresentationShareEmailEventType
        /// 3 - CourseActivationEmailEvent
        /// 4 - DroppedStudentEmailEvent
        /// 
        /// For Notification 1 and 2 -- Senders will be student
        /// For Notification 3 and 4 -- Sender will be instructor
        /// </summary>
        public int NotificationType { get; set; }

        /// <summary>
        /// Return back current information
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder reminderEmail = new StringBuilder();
            reminderEmail.Append("Traking ID / Email Id = ").Append(EmailId).Append(Environment.NewLine);
            if (!string.IsNullOrEmpty(ItemId))
            {
                reminderEmail.Append("Item Id = ").Append(ItemId).Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(CourseId))
            {
                reminderEmail.Append("Course Id = ").Append(CourseId).Append(Environment.NewLine);
            }
            
            reminderEmail.Append("Insutructor Id = ").Append(InstructorId).Append(Environment.NewLine);

            /*if (!string.IsNullOrEmpty(Subject))
            {
                reminderEmail.Append("Subject = ").Append(Subject).Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(UserBody))
            {
                reminderEmail.Append("User Body = ").Append(UserBody).Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(TemplateBodyHTML))
            {
                reminderEmail.Append("TemplateBodyHtml = ").Append(TemplateBodyHTML).Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(TemplateBodyText))
            {
                reminderEmail.Append("TemplateBodyText = ").Append(TemplateBodyText).Append(Environment.NewLine);
            }*/
            if (!string.IsNullOrEmpty(Status))
            {
                reminderEmail.Append("Status = ").Append(Status).Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(ToList))
            {
                reminderEmail.Append("ToList = ").Append(ToList).Append(Environment.NewLine);
            }
            if (Senders != null && Senders.Count > 0)
            {
                reminderEmail.Append("Sender = { ").Append(Environment.NewLine);
                foreach (var s in Senders)
                {
                    reminderEmail.Append(s.ToString()).Append(Environment.NewLine);
                }
                reminderEmail.Append("}");
            }
            return reminderEmail.ToString();
        }
    }
}
