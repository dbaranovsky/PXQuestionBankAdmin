using System;
using System.Collections.Generic;
using System.Linq;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public class CourseHelper : ICourseHelper
    {
        /// <summary>
        /// Gets or sets enrollment actions
        /// </summary>
        private BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets course actions
        /// </summary>
        private BizSC.ICourseActions CourseActions { get; set; }

        public CourseHelper(BizSC.IEnrollmentActions enrollmentActions,BizSC.ICourseActions courseActions)
        {
            EnrollmentActions = enrollmentActions;
            CourseActions = courseActions;
        }

        /// <summary>
        /// List all learning curve courses
        /// </summary>
        /// <param name="userReferenceId"> user reference id</param>
        /// <param name="productId"> product course id</param>
        /// <param name="includeGenericCourse">include generic courses in the list</param>
        /// <param name="includeDashboard">include dashboard courses in the list</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Course> ListCourses(string userReferenceId, string productId, bool includeGenericCourse, bool includeDashboard, string sProductType)
        {
            string courseType="";
            if (sProductType.Equals(CourseType.LearningCurve.ToString()))
            {
                courseType = CourseType.LearningCurve.ToString();
            }
            else if (sProductType.Equals(CourseType.XBOOK.ToString()))
            {
                courseType = CourseType.XBOOK.ToString();
            }
            else if (sProductType.Equals(CourseType.FACEPLATE.ToString()))
            {
                courseType = CourseType.FACEPLATE.ToString();
            }

            string query = "/meta-bfw_course_type='" + courseType + "'";

            if (!string.IsNullOrEmpty(productId)) query += " AND /meta-product-course-id='" + productId + "'";

            var enrolledCourseIds = EnrollmentActions.ListEnrollmentsMatchQuery(userReferenceId, query).Select(e => e.Course.Id).Distinct();

            var courses = CourseActions.GetCoursesByCourseIds(enrolledCourseIds);

            if (!includeGenericCourse && !includeDashboard)
                courses.RemoveAll(i => !i.CourseSubType.Equals("regular", StringComparison.InvariantCultureIgnoreCase));
            else if (!includeGenericCourse)
                courses.RemoveAll(i => (!i.CourseSubType.Equals("regular", StringComparison.InvariantCultureIgnoreCase) && !i.CourseSubType.Equals("instructor_dashboard", StringComparison.InvariantCultureIgnoreCase)));
            else if (!includeDashboard)
                courses.RemoveAll(i => (!i.CourseSubType.Equals("regular", StringComparison.InvariantCultureIgnoreCase) && !i.CourseSubType.Equals("generic", StringComparison.InvariantCultureIgnoreCase)));
            else
                courses.RemoveAll(i => (!i.CourseSubType.Equals("regular", StringComparison.InvariantCultureIgnoreCase) && !i.CourseSubType.Equals("instructor_dashboard", StringComparison.InvariantCultureIgnoreCase) && !i.CourseSubType.Equals("generic", StringComparison.InvariantCultureIgnoreCase)));

            return courses;
        }

        
    }
}
