using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IRoleOperation
    {
        IEnumerable<Role> GetRolesForCourse(string courseId);

        PagedCollection<QBAUser> GetQBAUsers(int startingRecordNumber, int recordsCount);

        void UpdateRolesCapabilities(string courseId, IEnumerable<Role> role);
        void DeleteRole(int roleId);
        Role GetRoleWithCapabilities(int roleId);

        QBAUser GetUserWithRoles(string userId);
        void UpdateUserRoles(QBAUser user);



        IEnumerable<Capability> GetUserCapabilities(string courseId);
        void GrantPredefinedRoleToCurrentUser(PredefinedRole author, string productCourseId);
    }
}