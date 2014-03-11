using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.NotificationModule.EmailTransport
{
    /// <summary>
    /// Create Email notification
    /// </summary>
    public class EmailBuilder
    {
        /// <summary>
        /// Execute email creating
        /// </summary>
        /// <param name="notificationData">Notification data </param>
        public NotificationTemplate BuildEmail(NotificationData notificationData)
        {
           
            return new NotificationTemplate
            {
                Sender = "", //ConfigurationHelper.GetSMTPUser(),
                Message = "",//notificationData.Placeholders.Mappings.Aggregate(template.Body, (current, value) => current.Replace(value.Key, value.Value)),
                Subject = "",//notificationData.Placeholders.Mappings.Aggregate(template.Subject, (current, value) => current.Replace(value.Key, value.Value)),
                RecipientContacts = new List<string>()//notificationData.Recipients.ToList()
            };
        }
    }
}
