using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.ReminderService.DataContracts;
using System.Net.Mail;

namespace Bfw.PX.ReminderService
{
    public interface IMailerService
    {
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="email">mail message to send</param>
        void SendMail(MailMessage email, string userName, string userPassword);
    }
}
