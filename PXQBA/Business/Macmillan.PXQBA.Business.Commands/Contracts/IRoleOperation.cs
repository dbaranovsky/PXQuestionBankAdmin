using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    /// <summary>
    /// Represents list of operations available for user roles
    /// </summary>
    public interface IRoleOperation
    {
        /// <summary>
        /// Loads list of roles available for particular course
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <returns>List of roles</returns>
        IEnumerable<Role> GetRolesForCourse(string courseId);

        /// <summary>
        /// Loads QBA users for particular page
        /// </summary>
        /// <param name="startingRecordNumber">User number to start from</param>
        /// <param name="recordsCount">Count of users to return</param>
        /// <returns></returns>
        PagedCollection<QBAUser> GetQBAUsers(int startingRecordNumber, int recordsCount);

        /// <summary>
        /// Updates capabilities for provided roles in a particular course
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <param name="role">Roles list to update capabilities for</param>
        void UpdateRolesCapabilities(string courseId, IEnumerable<Role> role);

        /// <summary>
        /// Deletes role from database
        /// </summary>
        /// <param name="roleId">Role id to delete</param>
        void DeleteRole(int roleId);

        /// <summary>
        /// Loads role with the list of capabilities available for that role
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <returns>Role</returns>
        Role GetRoleWithCapabilities(int roleId);

        /// <summary>
        /// Loads QBA user with the roles of this user for courses
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User</returns>
        QBAUser GetUserWithRoles(string userId);

        /// <summary>
        /// Updates user roles for courses
        /// </summary>
        /// <param name="user">User</param>
        void UpdateUserRoles(QBAUser user);

        /// <summary>
        /// Loads the list of user capabilities in the course
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <returns>List of capabilities</returns>
        IEnumerable<Capability> GetUserCapabilities(string courseId);

        /// <summary>
        /// Set a particular role for current user in specified product course
        /// </summary>
        /// <param name="role">Role to set</param>
        /// <param name="productCourseId">Product course id</param>
        void GrantPredefinedRoleToCurrentUser(PredefinedRole role, string productCourseId);
    }
}