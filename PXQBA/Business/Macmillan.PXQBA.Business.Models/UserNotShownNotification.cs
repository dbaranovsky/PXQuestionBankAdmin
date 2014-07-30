namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Notification that user chose 'Do not show again' option for
    /// </summary>
    public class UserNotShownNotification
    {
        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Type of notification
        /// </summary>
        public NotificationType NotificationType { get; set; }
    }
}