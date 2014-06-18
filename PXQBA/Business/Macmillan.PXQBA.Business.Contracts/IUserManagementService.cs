using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IUserManagementService
    {
        IEnumerable<UserNotShownNotification> GetNotShownNotification();
        void CreateNotShownNotification(NotificationType notificationType);
        IEnumerable<Role> GetRolesForCourse(string courseId);
    }
}