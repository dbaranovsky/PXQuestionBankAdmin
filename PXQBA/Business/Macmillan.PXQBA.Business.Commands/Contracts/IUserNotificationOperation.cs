using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IUserNotificationOperation
    {
        IEnumerable<UserNotShownNotification> GetNotShownNotification();
        void CreateNotShownNotification(NotificationType notificationType);
    }
}