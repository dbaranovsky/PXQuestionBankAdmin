using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Bfw.Common.Database;
using Bfw.Common.Collections;
using Bfw.PX.ReminderService.DataContracts;
using Bfw.PX.ReminderService.Helpers;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.ReminderService
{
    public class DBService : IDBService
    {
        /// <summary>
        /// connection string for the reminder deatails DB
        /// </summary>
        public string ReminderConnectionString { get; set; }

        /// <summary>
        /// Frequency interval in milliseconds
        /// </summary>
        public double ReminderInterval { get; set; }

        /// <summary>
        /// Authentication mailer user name
        /// </summary>
        public string MailerUserName { get; set; }

        /// <summary>
        /// Authentication mailer password
        /// </summary>
        public string MailerPassword { get; set; }

        /// <summary>
        /// Gets or sets the dlap helper.
        /// </summary>
        /// <value>
        /// The dlap halper.
        /// </value>
        private static IDLAPService dlapService { get; set; }

        /// <summary>
        /// Creating a private object of EventLog to be used
        /// </summary>
        private static EventLog eventLog = new EventLog(logName: "PxReminder", machineName: ".", source: "PxReminderService");

        public DBService()
        {
            dlapService = ServiceLocator.Current.GetInstance<IDLAPService>();
            GetReminderConfiguration();
        }

        /// <summary>
        /// Gets email template based on id
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public Structs.EmailTemplate GetEmailTemplate(int templateId)
        {
            var result = new Structs.EmailTemplate();

            var db = new DatabaseManager(ReminderConnectionString, "System.Data.SqlClient");

            try
            {
                db.StartSession();
                var records = db.Query("GetEmailTemplate @0", templateId);

                if (records.Count() > 0)
                {
                    result.id = records.First().Int32("Id");
                    result.TemplateText = records.First().String("TemplateText");
                    result.TemplateHtml = records.First().String("TemplateHtml");
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(string.Format("ReminderAction: Error in Method GetEmailTemplate {0}", ex.StackTrace), EventLogEntryType.Warning);
            }
            finally
            {
                db.EndSession();
            }

            return result;
        }

        /// <summary>
        /// Gets last processed DLAP signal from db history
        /// </summary>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        public string GetLastSignalId(int notificationType)
        {
            var result = "0";
            var db = new DatabaseManager(ReminderConnectionString, "System.Data.SqlClient");

            try
            {
                db.StartSession();
                var records = db.Query("GetLastSignalId @0", notificationType);

                if (records.Count() > 0)
                {
                    result = records.First().String("LastSignalId");
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(string.Format("ReminderAction: Error in Method GetLastSignalId {0}", ex.StackTrace), EventLogEntryType.Warning);
            }
            finally
            {
                db.EndSession();
            }

            return result;
        }

        /// <summary>
        /// Adds new entry to the Email Tracking (if event is DLAP based)
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="productId"></param>
        /// <param name="instructorId"></param>
        /// <param name="sendOnDate"></param>
        /// <param name="status"></param>
        /// <param name="notificationType"></param>
        /// <param name="itemId"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="templateId"></param>
        /// <param name="toList"></param>
        public void AddEmailTracking(string courseId, string productId, int instructorId, DateTime sendOnDate, string status, 
            int notificationType, string itemId, string subject, string body, int templateId, string toList)
        {
            var db = new DatabaseManager(ReminderConnectionString, "System.Data.SqlClient");

            try
            {
                db.StartSession();
                var records = db.ExecuteNonQuery("AddEmailTracking @0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10", 
                    courseId, productId, instructorId, sendOnDate, status,
                    notificationType, itemId, subject, body, templateId, toList);
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(string.Format("ReminderAction: Error in Method AddEmailTracking {0}", ex.StackTrace), EventLogEntryType.Warning);
            }
            finally
            {
                db.EndSession();
            }
        }

        /// <summary>
        /// Updates the status of the emails sent
        /// </summary>
        /// <param name="trackingEmailId">tacking id</param>
        /// <param name="successEmailIds">successfully sent emailsids</param>
        /// <param name="failureEmailIds">failed emailids</param>
        public void UpdateStatus(int trackingEmailId, List<string> successEmailIds, List<string> failureEmailIds)
        {
            var db = new DatabaseManager(ReminderConnectionString, "System.Data.SqlClient");

            try
            {
                db.StartSession();
                var records = db.ExecuteNonQuery("UpdateReminderEmailStatus @0, @1, @2", trackingEmailId,
                    string.Format("<emails>{0}{1}{2}</emails>", successEmailIds.Count > 0 ? "<email>" : "",
                        string.Join("</email><email>", successEmailIds.ToArray()), successEmailIds.Count > 0 ? "</email>" : ""),
                    string.Format("<emails>{0}{1}{2}</emails>", failureEmailIds.Count > 0 ? "<email>" : "",
                        string.Join("</email><email>", failureEmailIds.ToArray()), failureEmailIds.Count > 0 ? "</email>" : ""));
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(string.Format("ReminderAction: Error in Method UpdateStatus {0}", ex.StackTrace), EventLogEntryType.Warning);
            }
            finally
            {
                db.EndSession();
            }
        }

        /// <summary>
        /// Get the list of emails sent for a partially sent reminder
        /// </summary>
        /// <param name="trackingEmailId"></param>
        /// <returns></returns>
        public List<string> GetEmailSentHistory(int trackingEmailId)
        {
            var emailsSent = new List<string>();

            var db = new DatabaseManager(ReminderConnectionString, "System.Data.SqlClient");

            try
            {
                db.StartSession();
                var records = db.Query("GetEmailSentHistory @0", trackingEmailId);

                if (records != null && records.Count() > 0)
                {
                    emailsSent = records.Map(e => e.String("EmailAddress")).ToList();
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(string.Format("ReminderAction: Error in Method GetEmailSentHistory {0}", ex.StackTrace), EventLogEntryType.Warning);
            }
            finally
            {
                db.EndSession();
            }

            return emailsSent;
        }

        /// <summary>
        /// Gets the reminder service config settings
        /// </summary>
        public void GetReminderConfiguration()
        {
            var db = new DatabaseManager("PXDATA");

            try
            {
                db.StartSession();
                var records = db.Query("GetReminderEmailConfiguration");

                if (records != null && records.Count() > 0)
                {
                    var record = records.First();
                    ReminderConnectionString = record.String("ConnectionString");
                    ReminderInterval = record.Int64("ReminderInterval");
                    MailerUserName = record.String("MailerUserName");
                    MailerPassword = record.String("MailerPassword");
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(string.Format("ReminderAction: Error in Method GetReminderConfiguration {0}", ex.StackTrace), EventLogEntryType.Warning);
            }
            finally
            {
                db.EndSession();
            }
        }

        /// <summary>
        /// Gets the list of reminder emails to be sent based on the current date
        /// </summary>
        /// <returns>list od reminder emails that need to be sent</returns>
        public List<ReminderEmail> GetReminderMails()
        {
            var emailsToSend = new List<ReminderEmail>();

            var db = new DatabaseManager(ReminderConnectionString, "System.Data.SqlClient");

            try
            {
                db.StartSession();
                var records = db.Query("GetReminderEmails");

                if (records != null && records.Count() > 0)
                {
                    emailsToSend = records.Map(e => e.ToReminderEmail(this, dlapService)).ToList();
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(string.Format("ReminderAction: Error in Method GetReminderMails {0}", ex.StackTrace), EventLogEntryType.Warning);
            }
            finally
            {
                db.EndSession();
            }

            return emailsToSend;
        }
    }
}
