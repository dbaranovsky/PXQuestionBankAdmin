using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    public class UserNotificationViewModel
    {
         public string Message { get; set; }

         public bool IsShown { get; set; }

         public int NotificationTypeId { get; set; }
    }
}
