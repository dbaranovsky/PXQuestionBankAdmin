using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Database;
using Bfw.PX.ReminderService.DataContracts;

namespace Bfw.PX.ReminderService.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the reminder email from the database record
        /// </summary>
        /// <param name="record">databse record</param>
        /// <returns>reminder email</returns>
        public static ReminderEmail ToReminderEmail(this DatabaseRecord record, IDBService dbHelper, IDLAPService dlapHelper)
        {
            var emailId = record.Int32("EmailId");
            var sentEmails = dbHelper.GetEmailSentHistory(emailId);
            var userList = dlapHelper.GetStudentList(record.String("CourseId"));
            List<Sender> senderList = userList.Where(e => !sentEmails.Contains(e.Email)).ToList();

            return new ReminderEmail
            {
                EmailId = emailId,
                CourseId = record.String("CourseId"),
                ItemId = record.String("ItemId"),
                InstructorId = record.Int32("InstructorId"),
                Subject = record.String("InstructorSubject"),
                UserBody = record.String("InstructorBody"),
                TemplateBodyHTML = record.String("TemplateHtml"),
                TemplateBodyText = record.String("TemplateText"),
                Status = record.String("Status"),
                NotificationType = record.Int32("NotificationType"),
                Senders = senderList,
                ToList = record.String("ToList")
            };
        }
    }
}
