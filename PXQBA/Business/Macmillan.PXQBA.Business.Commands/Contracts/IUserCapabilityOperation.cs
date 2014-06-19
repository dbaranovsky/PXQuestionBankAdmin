using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IUserCapabilityOperation
    {
        IEnumerable<Role> GetRolesForCourse(string courseId);
        IEnumerable<Role> GetUserRoles(string userId, string courseId);

        IEnumerable<CourseUser> GetUsersInCourse(string courseId);

        void UpdateRolesCapabilities(string courseId, IEnumerable<Role> role);
        void DeleteRole(int roleId);
        Role GetRoleWithCapabilities(int roleId);
    }
}