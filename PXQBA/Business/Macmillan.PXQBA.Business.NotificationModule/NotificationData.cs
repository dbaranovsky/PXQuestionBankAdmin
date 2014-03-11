using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.NotificationModule
{ /// <summary>
    /// Notification data for email template
    /// </summary>
    public class NotificationData
    {
        public NotificationData()
        {
            Placeholders = new PlaceholderMapper();
            Recipients = new List<string>();
        }

        public IEnumerable<string> Recipients { get; set; }

        public PlaceholderMapper Placeholders { get; set; }

        public string Template { get; set; }
    }
}
