using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.NotificationModule.Contracts
{
      /// <summary>
    /// Base interface for notofication service
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Send notofication
        /// </summary>
        /// <param name="notificationData">Notification template</param>
        void SendNotification(NotificationData notificationData);
    }
}
