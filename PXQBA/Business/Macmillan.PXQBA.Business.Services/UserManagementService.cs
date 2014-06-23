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
        private readonly IRoleOperation roleOperation;

        public UserManagementService(IUserNotificationOperation userNotificationOperation, IRoleOperation roleOperation)
        {
            this.userNotificationOperation = userNotificationOperation;
            this.roleOperation = roleOperation;
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
            var courseRoles = roleOperation.GetRolesForCourse(courseId);
            if (!courseRoles.Any())
            {
                roleOperation.UpdateRolesCapabilities(courseId, PredefinedRoleHelper.GetPredefinedRoles());
                courseRoles = roleOperation.GetRolesForCourse(courseId);
            }
            return courseRoles;
        }

        public void DeleteRole(int roleId)
        {
            try
            {
                roleOperation.DeleteRole(roleId);
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
                return roleOperation.GetRoleWithCapabilities(roleId.Value);
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
                roleOperation.UpdateRolesCapabilities(courseId, new List<Role>() { role });
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("UpdateRole: courseId = {0}, roleid={1}", courseId, role.Id), ex);
                throw;
            }
        }

        public IEnumerable<QBAUser> GetUsers(int startingRecordNumber, int recordsCount)
        {
            return roleOperation.GetQBAUsers(startingRecordNumber, recordsCount);
        }

        public QBAUser GetUser(string userId)
        {
            try
            {
                return roleOperation.GetUserWithRoles(userId);
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
                roleOperation.UpdateUserRoles(user);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("UpdateUserRoles: userId = {0}", user.Id), ex);
                throw;
            }
        }

        public IEnumerable<Capability> GetUserCapabilities(string courseId)
        {
            try
            {
                return roleOperation.GetUserCapabilities(courseId);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("GetUserCapabilities: courseId = {0}", courseId), ex);
                throw;
            }
        }
    }
}