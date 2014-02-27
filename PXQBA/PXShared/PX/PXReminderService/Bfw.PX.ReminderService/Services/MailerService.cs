using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Bfw.PX.ReminderService
{
    public class MailerService : IMailerService
    {
        /// <summary>
        /// send grid web api send mail for <see cref="http://docs.sendgrid.com/documentation/api/web-api/mail/"/>
        /// </summary>
        string sendMailAPI = @"https://sendgrid.com/api/mail.send.xml?api_user={0}&api_key={1}&to={2}&toname={3}&subject={4}&text={5}&html={6}&from={7}";

        /// <summary>
        /// Creating a private object of EventLog to be used
        /// </summary>
        private static EventLog eventLog = new EventLog(logName: "PxReminder", machineName: ".", source: "PxReminderService");

        /// <summary>
        /// public constructor
        /// </summary>
        public MailerService()
        {

        }

        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="email">mail message to send</param>
        public void SendMail(MailMessage email, string userName, string userPassword)
        {
            var from = string.IsNullOrEmpty(email.From.DisplayName) ? 
                HttpUtility.UrlEncode(email.From.Address) :
                HttpUtility.UrlEncode(string.Format("\"{0}\"<{1}>", email.From.DisplayName, email.From.Address));
            var to = HttpUtility.UrlEncode(email.To[0].Address);
            var toName = HttpUtility.UrlEncode(email.To[0].DisplayName);
            var subject = HttpUtility.UrlEncode(email.Subject);
            var body = HttpUtility.UrlEncode(email.Body);
            
            var av = email.AlternateViews.First();                     
            var dataStream = av.ContentStream;
            var byteBuffer = new byte[dataStream.Length];
            var encoding = Encoding.GetEncoding(av.ContentType.CharSet);            
            var textBody = encoding.GetString(byteBuffer, 0, dataStream.Read(byteBuffer, 0, byteBuffer.Length));

            // We should not send both Html and Text view of data
            if (!string.IsNullOrEmpty(body) && body.Length > 1900) { textBody = string.Empty; }

            //temporary mail send using web api as the smtp port is blcoked. need to be reverted later.
            //var request = WebRequest.Create(string.Format(sendMailAPI, "newman.de", "4M@cMill@n", to, toName, subject, textBody, body, from));
            var request = WebRequest.Create(string.Format(sendMailAPI, userName, userPassword, to, toName, subject, textBody, body, from));
            try
            {
                request.Timeout = 15000;
                var response = request.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {                
                eventLog.WriteEntry(string.Format("MailService: Error in SendMail method - Error Message {0} ::::::  Trace {1}", ex.Message, ex.StackTrace), EventLogEntryType.Error);

                StringBuilder emailInformation = EmailInformation(email);

                eventLog.WriteEntry(string.Format("Email Information - {0}", emailInformation.ToString()), EventLogEntryType.Information);

                throw ex;
            }
            finally
            {
                request.Abort();
            }
        }

        private static StringBuilder EmailInformation(MailMessage email)
        {
            StringBuilder emailInformation = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(email.From.Address))
                {
                    emailInformation.Append("From : ").Append(email.From.Address).Append(Environment.NewLine);
                }
                if (!string.IsNullOrEmpty(email.To[0].Address))
                {
                    emailInformation.Append("To : ").Append(email.To[0].Address).Append(Environment.NewLine);
                }
                if (!string.IsNullOrEmpty(email.To[0].DisplayName))
                {
                    emailInformation.Append("To Name : ").Append(email.To[0].DisplayName).Append(Environment.NewLine);
                }
                if (!string.IsNullOrEmpty(email.Subject))
                {
                    emailInformation.Append("Subject : ").Append(email.Subject).Append(Environment.NewLine);
                }
                if (!string.IsNullOrEmpty(email.Body))
                {
                    emailInformation.Append("Body : ").Append(email.Body).Append(Environment.NewLine);
                }
            }
            finally { }

            return emailInformation;
            
        }
    }
}
