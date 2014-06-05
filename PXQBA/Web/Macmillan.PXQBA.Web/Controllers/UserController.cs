using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.ViewModels.User;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class UserController : MasterController
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Keeps the session alive.
        /// </summary>
        /// <param name="data">Random value.</param>
        /// <returns></returns>
        public ActionResult KeepSessionAlive(string data)
        {
            return new JsonResult { Data = "Success" };
        }

        /// <summary>
        /// Returns availible notification for current user
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrentUserNotificationForUrl()
        {
            return  JsonCamel(GetAllNotification());
        }

        /// <summary>
        /// Forbid notification showing for current user
        /// </summary>
        /// <param name="type">notification type</param>
        /// <returns></returns>
        public ActionResult DontShowForCurrentUser(NotificationType type)
        {
            return JsonCamel(new {isSuccess = true});
        }

        private IEnumerable<UserNotificationViewModel> GetAllNotification()
        {
           return (from notification in (NotificationType[]) Enum.GetValues(typeof (NotificationType))
                   select new UserNotificationViewModel()
                       {
                           IsShown = true, 
                           Message = EnumHelper.GetEnumDescription(notification), 
                           NotificationTypeId = (int) notification
                       }).ToList();
        }
	}
}