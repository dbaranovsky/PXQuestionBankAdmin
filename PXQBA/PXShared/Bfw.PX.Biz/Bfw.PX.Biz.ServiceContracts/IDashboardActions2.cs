using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that create and manage courses.
    /// </summary>
    public interface IDashboardActions2
    {
        /// <summary>
        /// Gets list of courses for instructor dashboard
        /// </summary>
        /// <param name="getChildrenCourses">Include children or not</param>
        /// <returns>Bdc.DashboardData</returns>
        DashboardData GetDashboardData(bool getChildrenCourses);

        /// <summary>
        /// Deletes Course specified by ID
        /// </summary>
        /// <param name="courseId">string</param>
        /// <returns></returns>
        bool DeleteCourse(string courseId);

        /// <summary>
        /// Gets the dashboard courses.
        /// This should be new one to use to get DashboardCourses
        /// </summary>
        /// <param name="referenceId">The reference id.</param>
        /// <param name="productId">The product Id.</param>
        /// <param name="termId">The term id.</param>
        /// <returns>IList<Course></returns>
        IList<Course> GetDashboardCoursesSpecificToProduct(string referenceId, string productId, string termId);
    }
}