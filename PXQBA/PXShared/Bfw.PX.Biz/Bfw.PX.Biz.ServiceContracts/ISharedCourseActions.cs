using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISharedCourseActions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharedCourseId"></param>
        /// <param name="sharedUserId"></param>
        /// <returns></returns>
        List<SharedCourseDefinition> getSharedCoursesBy(String sharedCourseId, String sharedUserId, bool onlyActive = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharingUserId"></param>
        /// <returns></returns>
        List<SharedCourseDefinition> getSharedCoursesBy(String sharingUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharedCourseId"></param>
        /// <param name="sharedUserId"></param>
        /// <returns></returns>
        SharedCourseDefinition getSharedCourseDefintion(String sharedCourseId, String sharedUserId, bool onlyActive = true);

        /// <summary>
        /// Gets the Shared Course details given an EnrollmentID
        /// </summary>
        /// <param name="sharedEnrollmentId"></param>
        /// <returns></returns>
        SharedCourseDefinition getSharedCourseDef(String sharedEnrollmentId, String sharedCourseId);

        /// <summary>
        /// Gets the list of Users with whom the course has been shared
        /// </summary>
        /// <param name="sharedCourseId">shared courseid</param>
        /// <returns></returns>
        IEnumerable<UserInfo> getSharedToUsers(String sharedCourseId);

        /// <summary>
        /// Gets the list of users whose eportfolio has been shared
        /// </summary>
        /// <returns></returns>
        IEnumerable<TocCategory> getSharedUsers(String sharedCourseId);

        /// <summary>
        /// Gets the list of items shared
        /// </summary>
        /// <param name="sharedCourseId"></param>
        /// <param name="sharedUserId"></param>
        /// <returns></returns>
        IList<string> getSharedItems(String sharedEnrollmentId);
                /// <summary>
        /// Get the shared note for a given user
        /// </summary>
        /// <param name="sharedEnrollmentId"></param>
        /// <returns></returns>
        String getSharedNote(String sharedEnrollmentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CourseId"></param>
        /// <returns></returns>
        IList<String> getListOfSharedUserIds(String sharedCourseId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CourseId"></param>
        /// <returns></returns>
        IList<UserInfo> getListOfSharedUsers(String sharedCourseId, Bfw.PX.Biz.ServiceContracts.IUserActions userActions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="CourseId"></param>
        /// <returns></returns>
        bool IsCourseSharedWithCurrentUser(String sharedCourseId, String sharedUserId);

        /// <summary>
        /// Gets the list of courses which are shared with a user
        /// </summary>
        /// <param name="sharingUserId"></param>
        /// <returns></returns>
        IEnumerable<DashboardItem> getSharedCourses(String sharingUserId, Boolean onlyActive);

        /// <summary>
        /// Generates a unique anonymous name for the shared user
        /// </summary>
        /// <returns></returns>
        String getAnonymousName(string sharedCourseId, string anonymousPrefix);

        /// <summary>
        /// Get the userid given the enrollmentid of the user for the current course
        /// </summary>
        /// <param name="sharedEnrollmentId"></param>
        /// <returns></returns>
        String GetUserId(String sharedEnrollmentId, string sharedCourseId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharedCourseDefinition"></param>
        void Store(SharedCourseDefinition sharedCourseDefinition);

    }
}
