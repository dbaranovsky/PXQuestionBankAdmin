using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    /// <summary>
    /// User notification
    /// </summary>
    public class UserNotificationViewModel
    {
        /// <summary>
        /// Notification message
        /// </summary>
         public string Message { get; set; }

        /// <summary>
        /// Indicates if this notification should be shown for current user
        /// </summary>
         public bool IsShown { get; set; }

        /// <summary>
        /// Notification type id
        /// </summary>
         public int NotificationTypeId { get; set; }
    }
}
