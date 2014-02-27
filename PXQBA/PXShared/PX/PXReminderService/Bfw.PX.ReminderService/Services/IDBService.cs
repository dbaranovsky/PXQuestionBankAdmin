using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.ReminderService.DataContracts;
using Bfw.PX.ReminderService.Helpers;

namespace Bfw.PX.ReminderService
{
    public interface IDBService
    {
        double ReminderInterval { get; set; }
        string MailerUserName { get; set; }
        string MailerPassword { get; set; }

        void GetReminderConfiguration();

        List<ReminderEmail> GetReminderMails();

        void AddEmailTracking(string courseId, string productId, int instructorId, DateTime sendOnDate, string status,
            int notificationType, string itemId, string subject, string body, int templateId, string toList);

        void UpdateStatus(int trackingEmailId, List<string> successEmailIds, List<string> failureEmailIds);

        Structs.EmailTemplate GetEmailTemplate(int templateId);

        string GetLastSignalId(int notificationType);

        List<string> GetEmailSentHistory(int trackingEmailId);
    }
}
