using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    /// <summary>
    /// Represents list of operations for user notification
    /// </summary>
    public interface IUserNotificationOperation
    {
        /// <summary>
        /// Loads information about notifications that are not shown to the user (if user clicks 'Do not show again' option)
        /// </summary>
        /// <returns>List of notifications</returns>
        IEnumerable<UserNotShownNotification> GetNotShownNotification();

        /// <summary>
        /// Creates record in the database that particular notification should be shown to the current user any more
        /// </summary>
        /// <param name="notificationType">Notification that shouldn't be shown any more</param>
        void CreateNotShownNotification(NotificationType notificationType);
    }
}