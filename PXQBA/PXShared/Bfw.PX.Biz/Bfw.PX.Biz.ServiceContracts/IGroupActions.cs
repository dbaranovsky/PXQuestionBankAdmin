using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that manipulate groups.
    /// </summary>
    public interface IGroupActions
    {
        /// <summary>
        /// Lists groups in the specified course.
        /// </summary>
        /// <param name="courseId">ID of the owning course to list groups for.</param>
        /// <param name="setId">Group set ID by which to filter the list.</param>
        IEnumerable<Group> ListGroups(string courseId, int setId = 0);

        /// <summary>
        /// Deletes a group set and all its child groups.
        /// </summary>
        /// <param name="courseId">ID of the owning course.</param>
        /// <param name="setId">ID of the group set to delete.</param>
        void DeleteGroupSet(string courseId, int setId);

        /// <summary>
        /// Loads all group sets for a specified course including groups.
        /// </summary>
        /// <param name="courseId">ID of the owning course.</param>
        /// <returns></returns>
        IDictionary<GroupSet, IList<Group>> GetAllGroupSetsWithGroups(string courseId);

        /// <summary>
        /// Creates a group in the specified owner course.
        /// </summary>
        /// <param name="ownerId">ID of the course to which this group belongs. </param>
        /// <param name="setId">The ID of the group set within the owning entity to which this group belongs.</param>
        /// <param name="groupName">Title for the group.</param>
        /// <param name="domainId">ID of the domain to create the group in.</param>
        /// <returns></returns>
        string CreateGroup(string ownerId, string setId, string groupName, string domainId);

        /// <summary>
        /// Adds member enrollment to an existing group.
        /// </summary>
        /// <param name="groupid">ID of the group to add members to.</param>
        /// <param name="enrollmentId">ID of the enrollment to add to the group.</param>
        void AddGroupMember(string groupid, string enrollmentId);

        /// <summary>
        /// Flushes the cache after saving a group
        /// </summary>
        void FlushCourseFromCache();
    }
}