using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that create and manage courses.
    /// </summary>
    public interface IDashboardActions
    {


        /// <summary>
        /// Creates the dashboard course if it does not exist
        /// </summary>
        /// <param name="parentCourseId">Course that dashboard will derive from</param>
        /// <param name="ownerId">Who owns the dashboard</param>
        /// <param name="title">Title of course</param>
        /// <param name="selectedDerivativeDomain">Domain that dashboard will derive from</param>
        /// <param name="academicTerm">Acadmenic term course thats currently taking place</param>
        /// <param name="courseTimeZone">Time zone of course</param>
        /// <returns></returns>
        string CreateDashboard(string parentCourseId, string title, string targetDomain, string academicTerm, string courseTimeZone, string dashboard_type, bool createProgramDashboard = false);

        /// <summary>
        /// Gets dashboard data
        /// </summary>
        /// <returns></returns>
        DashboardData GetDashboardData(bool getChildrenCourses, bool getOtherCourses);

        /// <summary>
        /// Gets Program dashboard data
        /// </summary>
        /// <returns></returns>
        DashboardData GetProgramDashboardData();

        /// <summary>
        /// Get the Dashboard Id
        /// </summary>
        /// <param name="refId"></param>
        /// <param name="domainId"></param>
        /// <returns></returns>
        string GetDashboardId(string courseProductId, string dashboard_type, string ref_id);

        /// <summary>
        /// Get the Dashboard Id
        /// </summary>
        /// <param name="refId"></param>
        /// <param name="domainId"></param>
        /// <returns></returns>
        string GetProgramDashboardId(string domainId, string courseType, string productCourseId);

        /// <summary>
        /// Create all necessary dashboard courses.
        /// </summary>
        /// <param name="targetDomain"></param>
        /// <param name="courseSubType"></param>
        /// <returns></returns>
        Course CreateDashboardCourses(string targetDomain, string courseSubType);

        /// <summary>
        /// Insert the personal Dashboard Data
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refId"></param>
        /// <param name="domainId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        void InsertDashboardData(string userId, string refId, string domainId, string courseId, bool programDashboard = false);

        /// <summary>
        /// Deletes Course specified by ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refId"></param>
        /// <param name="domainId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        bool DeleteCourses(string courseId);

        /// <summary>
        /// Gets all dashboard courses.
        /// </summary>
        /// <param name="dashboardCourseId">The dashboard course id.</param>
        /// <returns></returns>
        List<Course> GetAllDashboardCourses(string dashboardCourseId);


        /// <summary>
        /// Gets the dashboard courses.
        /// </summary>
        /// <param name="referenceId">The reference id.</param>
        /// <returns></returns>
        IList<Course> GetDashboardCoursesSpecificToProduct(string referenceId, string productId, string termId);


    }
}