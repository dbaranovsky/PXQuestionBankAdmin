using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Logging;

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
            var courseRoles = userCapabilityOperation.GetRolesForCourse(courseId);
            if (!courseRoles.Any())
            {
                userCapabilityOperation.UpdateRolesCapabilities(courseId, PredefinedRoleHelper.GetPredefinedRoles());
                courseRoles = userCapabilityOperation.GetRolesForCourse(courseId);
            }
            return courseRoles;
        }

        public void DeleteRole(int roleId)
        {
            try
            {
                userCapabilityOperation.DeleteRole(roleId);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("UpdateRole: roleid={0}", roleId), ex);
                throw;
            }
        }

        public Role GetRole(string courseId, int? roleId)
        {
            if (roleId.HasValue && roleId.Value > 0)
            {
                return userCapabilityOperation.GetRoleWithCapabilities(roleId.Value);
            }
            return GetNewRoleTemplate();
        }

        private Role GetNewRoleTemplate()
        {
            return new Role {Name = "New Role"};
        }

        public void UpdateRole(string courseId, Role role)
        {
            try
            {
                userCapabilityOperation.UpdateRolesCapabilities(courseId, new List<Role>() { role });
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("UpdateRole: courseId = {0}, roleid={1}", courseId, role.Id), ex);
                throw;
            }
        }

        public IEnumerable<QBAUser> GetUsers(int startingRecordNumber, int recordsCount)
        {
            return userCapabilityOperation.GetQBAUsers(startingRecordNumber, recordsCount);
        }

        public QBAUser GetUser(string userId)
        {
            try
            {
                return userCapabilityOperation.GetUserWithRoles(userId);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("GetUser: userId = {0}", userId), ex);
                throw;
            }
        }

        public void UpdateUserRoles(QBAUser user)
        {
            try
            {
                userCapabilityOperation.UpdateUserRoles(user);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("UpdateUserRoles: userId = {0}", user.Id), ex);
                throw;
            }
        }
    }
}