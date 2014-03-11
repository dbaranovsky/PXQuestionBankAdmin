using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.NotificationModule.Contracts;

namespace Macmillan.PXQBA.Business.NotificationModule
{
    public class NotificationService: INotificationService
    {
        private readonly INotificationTransport transport;

        /// <summary>
        /// Creates notification service instance injecting notification transport
        /// </summary>
        /// <param name="transport">Notification transport</param>
        public NotificationService(INotificationTransport transport)
        {
            this.transport = transport;
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="notificationData">Mail template</param>
        public void SendNotification(NotificationData notificationData)
        {
            transport.Send(notificationData);
        }
    }
}
