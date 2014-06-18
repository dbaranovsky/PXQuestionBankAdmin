using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserNotificationOperation userNotificationOperation;
        private readonly IUserCapabilityOperation userCapabilityOperation;

        public UserManagementService(IUserNotificationOperation userNotificationOperation, IUserCapabilityOperation userCapabilityOperation)
        {
            this.userNotificationOperation = userNotificationOperation;
            this.userCapabilityOperation = userCapabilityOperation;
        }

        public IEnumerable<UserNotShownNotification> GetNotShownNotification()
        {
            return userNotificationOperation.GetNotShownNotification();
        }

        public void CreateNotShownNotification(NotificationType notificationType)
        {
            userNotificationOperation.CreateNotShownNotification(notificationType);
        }

        public IEnumerable<Role> GetRolesForCourse(string courseId)
        {
            var courseRoles  = userCapabilityOperation.GetRolesForCourse(courseId);
            if (!courseRoles.Any())
            {
                var predefinedRoles = PredefinedRoleHelper.GetPredefinedRoles();
                userCapabilityOperation.UpdateRolesCapabilities(courseId, predefinedRoles);
                courseRoles = userCapabilityOperation.GetRolesForCourse(courseId);
            }
            return courseRoles;
        }
    }
}