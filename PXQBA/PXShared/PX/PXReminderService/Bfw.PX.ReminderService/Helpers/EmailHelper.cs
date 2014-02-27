using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Bfw.Common.Database;
using Bfw.PX.ReminderService.DataContracts;

namespace Bfw.PX.ReminderService.Helpers
{
    /// <summary>
    /// contains email objects extension methods
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Creating a private object of EventLog to be used
        /// </summary>
        private static EventLog eventLog = new EventLog(logName: "PxReminder", machineName: ".", source: "PxReminderService");

        /// <summary>
        /// List of params that vary for each student in a email tracking record
        /// with its calling function
        /// </summary>
        private static Dictionary<string, Func<Sender, string>> studentParamFunctions = new Dictionary<string, Func<Sender, string>>
        { 
            {"studentname", e => string.Format("{0} {1}", e.FirstName, e.LastName)},
            {"studentemail", e => e.Email},
            {"itemattempts", e => string.Format("{0} {1}", e.FirstName, e.LastName)}
        };
        /// <summary>
        /// List of params that vary for each email tracking record
        /// with its calling function
        /// </summary>
        private static Dictionary<string, Func<ReminderEmail, string>> emailParamFunctions = new Dictionary<string, Func<ReminderEmail, string>>
        {
            {"instructorname", e => e.Senders.Where(s => s.Id == e.InstructorId).Select(s => string.Format("{0} {1}", s.FirstName, s.LastName)).First()},
            {"instructoremail", e => e.Senders.Find(s => s.Id == e.InstructorId).Email},
            {"coursetitle", Reminder.CourseTitle},
            {"courseid", e => e.CourseId},
            {"courseurl", Reminder.CourseTitle},
            {"producttype", Reminder.ProductType},
            {"itemtitle", Reminder.AssignmentTitle},
            {"itemduedate", Reminder.AssignmentDate},
            {"itempoints", Reminder.ItemPoints},
            {"itemallowedlate", Reminder.AllowLateSubmission},
            {"customBody", e => e.UserBody}, 
        };

        /// <summary>
        /// Reminder email object
        /// </summary>
        private static ReminderAction Reminder { get { return ReminderAction.GetInstance(); } }
        
        /// <summary>
        /// Sets body of email for dropped student enrollment
        /// </summary>
        /// <param name="email"></param>
        /// <param name="templateBody"></param>
        /// <returns></returns>
        public static string SetDroppedEmailBody(ReminderEmail email, 
            EnrollmentSignal enrollmentSignal, 
            IEnumerable<KeyValuePair<string, string>> courses, 
            string templateBody)
        {
            var result = templateBody;

            var producturl = courses.SingleOrDefault(c => c.Key.Equals(email.ProductId)).Value ?? string.Empty;
            
            result = result.Replace("{studentfirstname}", enrollmentSignal.Enrollment.User.FirstName);
            result = result.Replace("{coursetitle}", email.CourseTitle);
            result = result.Replace("{courseurl}",  string.Format("http://{0}/{1}", producturl, email.CourseId));
            result = result.Replace("{producturl}", string.Format("http://{0}", producturl));
            result = result.Replace("{instructorname}", string.Format("{0} {1}", enrollmentSignal.Instructor.FirstName, enrollmentSignal.Instructor.LastName));
            result = result.Replace("{instructoremail}", enrollmentSignal.Instructor.Email);

            return result;
        }
        
        /// <summary>
        /// Gives the list of emails to be sent based on the reminder email object
        /// </summary>
        /// <param name="reminderEmail"></param>
        /// <returns></returns>
        public static List<MailMessage> ToEmails(this ReminderEmail reminderEmail)
        {
            //regex to find and replace body text
            var regEx = new Regex(@"{(\w+)}");

            var emailParams = new Dictionary<string, string>();
            var emails = new List<MailMessage>();

            //get the value for reminder email level varying params and its value
            foreach (Match match in regEx.Matches(reminderEmail.TemplateBodyHTML))
            {
                var param = match.Groups[1].Value;
                if (!emailParams.ContainsKey(param) && emailParamFunctions.ContainsKey(param))
                {
                    try
                    {
                        emailParams.Add(param, emailParamFunctions[param](reminderEmail));
                    }
                    catch (Exception ex)
                    {
                        eventLog.WriteEntry(string.Format("EmailHelper: Error in ToEmails() method - RegEx replacement block {0} ::::::  {1}", ex.Message, ex.StackTrace), EventLogEntryType.Error);
                        eventLog.WriteEntry(string.Format("ReminderEmail Information -- {0} ", reminderEmail.ToString()), EventLogEntryType.Information);
                        eventLog.WriteEntry(string.Format("ReminderEmail TemplateBodyHTML -- {0} ", reminderEmail.TemplateBodyHTML.ToString()), EventLogEntryType.Warning);
                    }
                }
            }

            if (string.IsNullOrEmpty(reminderEmail.ToList))
            {
                //generate individual email object for each student (No batch email sending)
                IEnumerable<Sender> senderList = null;
                if (reminderEmail.NotificationType == 3)
                {
                    senderList = reminderEmail.Senders.Where(e => e.Flags.Contains("GradeExam")).ToList();
                    if (senderList == null || senderList.Count() == 0)
                    {
                        senderList = reminderEmail.Senders.Where(e => e.Flags.Contains("UpdateCourse")).ToList();
                    }
                }
                else
                {
                    senderList = reminderEmail.Senders.Where(e => e.Flags.Contains("Participate"));
                }

                foreach (var user in senderList)
                {
                    try
                    {
                        MatchEvaluator replace = delegate(Match match)
                        {
                            //Anonymous delegate to get the value for the matched expression to replace
                            var key = match.Groups[1].Value;

                            if (emailParams.ContainsKey(key))
                            {
                                return emailParams[key];
                            }
                            else if (studentParamFunctions.ContainsKey(key))
                            {
                                return studentParamFunctions[key](user);
                            }

                            return string.Empty;
                        };
                        var bodyHtml = regEx.Replace(reminderEmail.TemplateBodyHTML, replace);
                        var bodyText = regEx.Replace(reminderEmail.TemplateBodyText, replace);
                        var subject = regEx.Replace(reminderEmail.Subject, replace);

                        var message = new MailMessage();
                        message.To.Add(new MailAddress(user.Email, string.Format("{0} {1}", user.FirstName, user.LastName)));
                        if (reminderEmail.NotificationType == 3)
                        {
                            message.From = new MailAddress(emailParamFunctions["instructoremail"](reminderEmail), "Macmillan Media");
                        }
                        else
                        {
                            message.From = new MailAddress(emailParamFunctions["instructoremail"](reminderEmail), emailParamFunctions["instructorname"](reminderEmail));
                        }
                        message.Subject = subject;
                        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, null, MediaTypeNames.Text.Plain));
                        //message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHtml, null, MediaTypeNames.Text.Html));
                        message.Body = bodyHtml;
                        message.IsBodyHtml = true;

                        emails.Add(message);
                    }
                    catch (Exception ex) {
                        eventLog.WriteEntry(string.Format("EmailHelper: Error in ToEmails method - Creating email when reminderEmail.ToList has null value - Error Message {0} ::::::  Trace {1}", ex.Message, ex.StackTrace), EventLogEntryType.Error);
                        eventLog.WriteEntry(string.Format("ReminderEmail Information -- {0} ", reminderEmail.ToString()), EventLogEntryType.Information);
                    }
                }
            }
            else
            {
                var emailAddressList = new List<string>(reminderEmail.ToList.Split(','));

                MatchEvaluator replace = delegate(Match match)
                {
                    //Anonymous delegate to get the value for the matched expression to replace
                    var key = match.Groups[1].Value;

                    if (emailParams.ContainsKey(key))
                    {
                        return emailParams[key];
                    }

                    return string.Empty;
                };
                var bodyHtml = regEx.Replace(reminderEmail.TemplateBodyHTML, replace);
                var bodyText = regEx.Replace(reminderEmail.TemplateBodyText, replace);
                var subject = regEx.Replace(reminderEmail.Subject, replace);

                foreach (var email in emailAddressList)
                {
                    try
                    {
                        var message = new MailMessage();
                        message.To.Add(new MailAddress(email, email));
                        message.From = new MailAddress(emailParamFunctions["instructoremail"](reminderEmail), emailParamFunctions["instructorname"](reminderEmail));
                        message.Subject = subject;

                        //message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHtml, null, MediaTypeNames.Text.Html));
                        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, null, MediaTypeNames.Text.Plain));

                        message.Body = bodyHtml;
                        message.IsBodyHtml = true;

                        emails.Add(message);
                    }
                    catch (Exception ex)
                    {
                        // In case emailParamFunction does not has right value, it should still work.
                        eventLog.WriteEntry(string.Format("EmailHelper: Error in ToEmails method - Creating email id when reminderEmail.ToList has some value - Error Message {0} ::::::  Trace {1}", ex.Message, ex.StackTrace), EventLogEntryType.Error);
                        eventLog.WriteEntry(string.Format("ReminderEmail Information -- {0} ", reminderEmail.ToString()), EventLogEntryType.Information);
                    }
                }

            }

            return emails;
        }
    }
}
