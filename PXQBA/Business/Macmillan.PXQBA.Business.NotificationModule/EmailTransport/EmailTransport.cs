using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.NotificationModule.Contracts;

namespace Macmillan.PXQBA.Business.NotificationModule.EmailTransport
{
    /// <summary>
    /// Represents transport notification by email
    /// </summary>
    public class EmailTransport : INotificationTransport
    {
        private readonly EmailBuilder builder;

        public EmailTransport(EmailBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Execute sending email notification
        /// </summary>
        /// <param name="notificationData">Mail template</param>
        public void Send(NotificationData notificationData)
        {
            var emailTemplate = builder.BuildEmail(notificationData);

            //TODO: Create connection with the server and send mail
        }
    }
}
