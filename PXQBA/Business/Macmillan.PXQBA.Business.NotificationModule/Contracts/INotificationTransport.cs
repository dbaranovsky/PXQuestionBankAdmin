using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.NotificationModule.Contracts
{
    /// <summary>
    /// Base interface for notification transport
    /// </summary>
    public interface INotificationTransport
    {
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="notificationData">Notification data</param>
        void Send(NotificationData notificationData);
    }
}
