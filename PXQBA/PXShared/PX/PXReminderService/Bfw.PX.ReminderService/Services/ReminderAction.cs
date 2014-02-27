using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.XPath;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Bfw.Common.OAuth;
using Bfw.PX.ReminderService.DataContracts;
using Microsoft.Practices.ServiceLocation;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.ReminderService.Helpers;
using Bfw.Common.Patterns.Unity;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;

namespace Bfw.PX.ReminderService
{
    /// <summary>
    /// Contains all the DB and agilx interaction methods
    /// </summary>
    public sealed class ReminderAction
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        private static volatile ReminderAction instance;        
        private static object syncRoot = new Object();

        /// <summary>
        /// Creating a private object of EventLog to be used
        /// </summary>
        private static EventLog eventLog = new EventLog(logName: "PxReminder", machineName: ".", source: "PxReminderService");

        /// <summary>
        /// Factory method property to create the singleton instance of reminder service class
        /// </summary>
        public static ReminderAction GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ReminderAction();
                    }
                }
            }

            return instance;
        }

        #region Properties

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
        /// Mailer service
        /// </summary>
        [Dependency] 
        public IMailerService Mailer { get; set; }

        /// <summary>
        /// DB interaction helper
        /// </summary>
        [Dependency]
        public IDBService DB { get; set; }

        /// <summary>
        /// DLAP interaction Helper
        /// </summary>
        [Dependency]
        public IDLAPService DLAP { get; set; }

        /// <summary>
        /// RA interaction Helper
        /// </summary>
        [Dependency]
        public IRAService RA { get; set; }

        #endregion

        #region Constructors and configuration

        /// <summary>
        /// private constructor to enforce singleton
        /// </summary>       
        private ReminderAction()
        {

        }

        /// <summary>
        /// public constructor for unity
        /// </summary>
        /// <param name="action"></param>
        public ReminderAction(ReminderAction action)
        {
            instance = action;
        }

        #endregion

        public void Run()
        {
            eventLog.WriteEntry("ReminderService: PX Reminder server timer elapsed", EventLogEntryType.Information);

            ReminderInterval = DB.ReminderInterval;
            MailerUserName = DB.MailerUserName;
            MailerPassword = DB.MailerPassword;

            var emails = DB.GetReminderMails();
            FilterOutPastDueReminders(emails);
            emails.AddRange(GetDroppedStudentList());

            if (emails.Any())
            {
                eventLog.WriteEntry(string.Format("Reminder service: Sending {0} reminders to {1} students", emails.Count, emails.Sum(em => em.Senders.Count)), EventLogEntryType.Information);

                foreach (var mail in emails)
                {
                    var successEmails = new List<String>();
                    var failureEmails = new List<String>();

                    foreach (var email in mail.ToEmails())
                    {
                        try
                        {
                            Mailer.SendMail(email, MailerUserName, MailerPassword);
                            successEmails.Add(email.To.First().Address);

                            // The DLAP is the source of events
                            if (mail.EmailId == 0)
                            {
                                DB.AddEmailTracking(mail.CourseId, mail.ProductId, mail.InstructorId, DateTime.Now, "sent", mail.NotificationType,
                                    mail.ItemId, mail.Subject, mail.TemplateBodyHTML, 3, mail.ToList);
                            }
                        }
                        catch
                        {
                            failureEmails.Add(email.To.First().Address);
                            eventLog.WriteEntry(string.Format("ReminderService: Failed to send email to address {0}", email.To.First().Address), EventLogEntryType.Warning);

                            // The DLAP is the source of events
                            if (mail.EmailId == 0)
                            {
                                DB.AddEmailTracking(mail.CourseId, mail.ProductId, mail.InstructorId, DateTime.Now, "add", mail.NotificationType,
                                    mail.ItemId, mail.Subject, mail.TemplateBodyHTML, 3, mail.ToList);
                            }
                        }
                    }

                    // The database is the source of events
                    if (mail.EmailId > 0)
                    {
                        if (successEmails.Count > 0 || failureEmails.Count > 0)
                        {
                            DB.UpdateStatus(mail.EmailId, successEmails, failureEmails);
                        }
                        else
                        {
                            /*PX-4012 - Due to system failure, emails were not send and there is no information. These emails should not be marked as "Sent". 
                                * It should be failed with unknown reason. Marking unkonwn reason as "ErrorInGettingEmailId"  */
                            failureEmails.Add("ErrorInGettingEmailId");
                            DB.UpdateStatus(mail.EmailId, successEmails, failureEmails);
                        }
                    }
                }

                eventLog.WriteEntry("Reminder service: Finished sending reminders", EventLogEntryType.Information);
            }
            else
            {
                eventLog.WriteEntry("ReminderService: No reminders to send", EventLogEntryType.Information);
            }
        }

        public bool ClearEnrollmentCache(EnrollmentSignal enrollment)
        {
            string serviceBaseUrl = ConfigurationManager.AppSettings["PXWebAPIBaseUrl"];
            if (enrollment == null || serviceBaseUrl == null) return false;
            string raUserid = enrollment.Enrollment.User.UserName;
            string courseid = enrollment.Enrollment.CourseId;
            var dictionary = new Dictionary<string, string>()
                    {
                        {"raUserid", raUserid},
                        {"courseid", courseid}
                    };

            var uri = new Uri(serviceBaseUrl + "enrollment/ClearEnrollment" + String.Format("?raUserid={0}&courseid={1}", raUserid, courseid));
            var authHeaders = OAuth.GeneratePXAuthHeader(dictionary, uri, "POST");
            var webClient = new WebClient();
            webClient.Headers.Add(authHeaders);
            var responseBytes = webClient.UploadValues(uri.ToString(), "POST", ToNameValueCollection(dictionary));

            dynamic response = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(responseBytes));

            return response.results;
        }
        private NameValueCollection ToNameValueCollection<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> dict)
        {
            var nameValueCollection = new NameValueCollection();
            foreach (var kvp in dict)
            {
                nameValueCollection.Add(kvp.Key.ToString(), Convert.ToString(kvp.Value));
            }
            return nameValueCollection;
        }

        /// <summary>
        /// Gets list of students with dropped enrollments
        /// </summary>
        /// <returns></returns>
        private List<ReminderEmail> GetDroppedStudentList()
        {
            var result = new List<ReminderEmail>();
            var droppedSignals = new List<Signal>();
            var signals = DLAP.GetSignalList(DB.GetLastSignalId((int)EnrollmentStatus.Withdrawn), ConfigurationManager.AppSettings.Get("droppedStudentSignalType"));

            if (!signals.IsNullOrEmpty())
            {
                foreach (var signal in signals.Where(s => s.NewStatus > 0))
                {
                    if (signal.NewStatus == EnrollmentStatus.Withdrawn)
                    {
                        droppedSignals.Add(signal);
                    }
                }
            }

            if (droppedSignals.Count > 0)
            {
                var enrollments = DLAP.GetDroppedEnrollmentList(droppedSignals);

                IEnumerable<KeyValuePair<string, string>> courses = new List<KeyValuePair<string, string>>();
                var courseIds = enrollments.Select(c => c.Enrollment.Course.ToCourse().ProductCourseId).Distinct();

                if (!courseIds.IsNullOrEmpty())
                {
                    courses = RA.GetCourseList(courseIds);
                }

                if (!enrollments.IsNullOrEmpty())
                {
                    var emailTemplate = DB.GetEmailTemplate(4);

                    foreach (var enrollment in enrollments)
                    {
                        ClearEnrollmentCache(enrollment);

                        var email = new ReminderEmail()
                        {
                            NotificationType = 4,
                            InstructorId = Int32.Parse(enrollment.Instructor.Id),
                            ItemId = enrollment.Signal.SignalId,
                            CourseTitle = enrollment.Enrollment.Course.Title,
                            CourseId = enrollment.Enrollment.CourseId,
                            ProductId = enrollment.Enrollment.Course.ToCourse().ProductCourseId,
                            Subject = string.Format("You have been removed from {0}.", enrollment.Enrollment.Course.Title),                           
                            TemplateBodyText = emailTemplate.TemplateText,
                            TemplateBodyHTML = emailTemplate.TemplateHtml,
                            ToList = enrollment.Enrollment.User.Email,
                            Senders = new List<Sender>() 
                            { 
                                new Sender 
                                {
                                    Id = Int32.Parse(enrollment.Instructor.Id),
                                    FirstName = enrollment.Instructor.FirstName,
                                    LastName = enrollment.Instructor.LastName,
                                    Email = enrollment.Instructor.Email
                                }
                            }
                        };

                        email.TemplateBodyText = EmailHelper.SetDroppedEmailBody(email, enrollment, courses, email.TemplateBodyText);
                        email.TemplateBodyHTML = EmailHelper.SetDroppedEmailBody(email, enrollment, courses, email.TemplateBodyHTML);

                        result.Add(email);
                    }
                }
            }
            else
            {
                var lastSignalId = signals.Select(s => s.SignalId).LastOrDefault();

                if (lastSignalId != null && lastSignalId != "0")
                {
                    // adding fake record to keep track of last processed signal
                    DB.AddEmailTracking("0", "0", 0, DateTime.Now, "sent", 4,
                        lastSignalId, string.Empty, string.Empty, 3, string.Empty);
                }
            }

            return result;
        }

        /// <summary>
        /// gets the course name
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public string CourseTitle(ReminderEmail e)
        {
            var course = DLAP.GetCourse(e.CourseId);

            if (course != null)
            {
                return course.Title;
            }

            return string.Empty;
        }

        /// <summary>
        /// gets the product type
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public string ProductType(ReminderEmail e)
        {
            var course = DLAP.GetCourse(e.CourseId);
            var bfwProperty = course.Data.XPathSelectElement("//bfw_product_type[@name='bfw_property']");

            if (bfwProperty != null)
            {
                return bfwProperty.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// gets the assignment title
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public string AssignmentTitle(ReminderEmail e)
        {
            var content = DLAP.GetContent(e.CourseId, e.ItemId);

            return content.Title;
        }

        /// <summary>
        /// gets the assignment date
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public string AssignmentDate(ReminderEmail e)
        {
            var content = DLAP.GetContent(e.CourseId, e.ItemId);

            var dueDate = content.DueDate;
            if (null != dueDate)
            {
                return dueDate.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// gets the allow late submission value
        /// </summary>
        /// <param name="e"></param>
        /// <returns>yes/no string</returns>
        public string AllowLateSubmission(ReminderEmail e)
        {
            var content = DLAP.GetContent(e.CourseId, e.ItemId);

            var allowLateSubmission = content.Data.Element("allowlatesubmission");
            if (null != allowLateSubmission && allowLateSubmission.Value == "true")
            {
                return "yes";
            }

            return "no";
        }

        /// <summary>
        /// get the max points for the assignment
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public string ItemPoints(ReminderEmail e)
        {
            var content = DLAP.GetContent(e.CourseId, e.ItemId);

            return content.MaxPoints.ToString();
        }

        private void FilterOutPastDueReminders(List<ReminderEmail> reminders)
        {
            var assignmentReminders = reminders.Where(e => e.NotificationType == 1).ToList();
            if (assignmentReminders.Count > 0)
            {
                var list = DLAP.GetPastDueItems(assignmentReminders);
                if (!list.IsNullOrEmpty())
                {
                    foreach (var item in list)
                    {
                        var target = reminders.Single(r => r.ItemId == item.Id);
                        reminders.Remove(target);
                    }
                }
            }
        }
    }
}
