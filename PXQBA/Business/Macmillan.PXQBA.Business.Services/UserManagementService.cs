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
           return userCapabilityOperation.GetRolesForCourse(courseId);
        }

        public void DeleteRole(int roleId)
        {
            userCapabilityOperation.DeleteRole(roleId);
        }

        public Role GetRole(string courseId, int? roleId)
        {
            if (roleId.HasValue && roleId.Value > 0)
            {
                return userCapabilityOperation.GetRoleWithCapabilities(courseId, roleId.Value);
            }
            return GetNewRoleTemplate();
        }

        private Role GetNewRoleTemplate()
        {
            return new Role();
        }

        public void UpdateRole(string courseId, Role role)
        {
            userCapabilityOperation.UpdateRolesCapabilities(courseId, new List<Role>(){role});
        }
    }
}