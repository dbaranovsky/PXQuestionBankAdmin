using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.NotificationModule
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationTemplate
    {
        public IList<string> RecipientContacts { get; set; }

        public string Message { get; set; }

        public string Subject { get; set; }

        public string Sender { get; set; }
    }
}
