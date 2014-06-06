using System.Collections.Generic;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserNotificationOperation userNotificationOperation;

        public UserManagementService(IUserNotificationOperation userNotificationOperation)
        {
            this.userNotificationOperation = userNotificationOperation;
        }

        public IEnumerable<UserNotShownNotification> GetNotShownNotification()
        {
            return userNotificationOperation.GetNotShownNotification();
        }

        public void CreateNotShownNotification(NotificationType notificationType)
        {
            userNotificationOperation.CreateNotShownNotification(notificationType);
        }
    }
}