using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IUserManagementService
    {
        IEnumerable<UserNotShownNotification> GetNotShownNotification();
        void CreateNotShownNotification(NotificationType notificationType);
        IEnumerable<Role> GetRolesForCourse(string courseId);
        void DeleteRole(int roleId);

        Role GetRole(string courseId, int? roleId);
        void UpdateRole(string courseId, Role role);
        PagedCollection<QBAUser> GetUsers(int startingRecordNumber, int recordsCount);
        QBAUser GetUser(string userId);
        void UpdateUserRoles(QBAUser user);

        IEnumerable<Capability> GetUserCapabilities(string courseId);
    }
}