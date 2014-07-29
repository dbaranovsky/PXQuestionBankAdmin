using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Represents service that manages business logic for users and roles
    /// </summary>
    public interface IUserManagementService
    {
        /// <summary>
        /// Loads the list of 'Do not show again' notifications for QBA users
        /// </summary>
        /// <returns>List of notifications</returns>
        IEnumerable<UserNotShownNotification> GetNotShownNotification();

        /// <summary>
        /// When user checks 'Do not show again' checkbox on UI notification, the record in the database is created
        /// </summary>
        /// <param name="notificationType">Type of notification</param>
        void CreateNotShownNotification(NotificationType notificationType);

        /// <summary>
        /// Loads the list of roles for particular course
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <returns>List of roles</returns>
        IEnumerable<Role> GetRolesForCourse(string courseId);

        /// <summary>
        /// Deletes role
        /// </summary>
        /// <param name="roleId">Role id to delete</param>
        void DeleteRole(int roleId);

        /// <summary>
        /// Loads role with capabilities for a course or creates new role template if not specified
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <param name="roleId">Role id</param>
        /// <returns>Role with capabilities</returns>
        Role GetRole(string courseId, int? roleId);

        /// <summary>
        /// Adds or updates role for the course
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <param name="role">Role id</param>
        void UpdateRole(string courseId, Role role);

        /// <summary>
        /// Loads recordsCount number of users starting from specified value (for a particular page)
        /// </summary>
        /// <param name="startingRecordNumber">Starting user number</param>
        /// <param name="recordsCount">Users count</param>
        /// <returns>Page of users</returns>
        PagedCollection<QBAUser> GetUsers(int startingRecordNumber, int recordsCount);

        /// <summary>
        /// Loads user with its roles
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>QBA user</returns>
        QBAUser GetUser(string userId);

        /// <summary>
        /// Updates roles granted for the user
        /// </summary>
        /// <param name="user">User to update roles for</param>
        void UpdateUserRoles(QBAUser user);

        /// <summary>
        /// Loads the list of user capabilities in a particular course
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <returns>List of capabilities</returns>
        IEnumerable<Capability> GetUserCapabilities(string courseId);
    }
}