using System;
using System.Linq;
using System.Web;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Helpers
{
    /// <summary>
    /// Represents helper for course selected
    /// </summary>
    public class CourseHelper
    {
        private const string CourseParamName = "current_course";
        private readonly IProductCourseManagementService productCourseManagementService;

        public CourseHelper(IProductCourseManagementService productCourseManagementService)
        {
            this.productCourseManagementService = productCourseManagementService;
        }

        /// <summary>
        /// Loads requested course by id
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <returns>Loaded course</returns>
        public Course GetCourse(string courseId)
        {
            var course = HttpContext.Current.Session[CourseParamName] as Course;
            if (course != null)
            {
                if (course.ProductCourseId == courseId)
                {
                    return course;
                }
            }

            course = productCourseManagementService.GetProductCourse(courseId, true);
            HttpContext.Current.Session[CourseParamName] = course;

            return course;
        }

        /// <summary>
        /// Gets course id from cache
        /// </summary>
        /// <returns>Course id</returns>
        public string GetChachedCourseId()
        {
            var course = HttpContext.Current.Session[CourseParamName] as Course;
            if (course == null)
            {
                return String.Empty;
            }

            return course.ProductCourseId;
        }

        /// <summary>
        /// Clear cache for course
        /// </summary>
        public void ClearCache()
        {
            HttpContext.Current.Session[CourseParamName] = null;
        }
        
        /// <summary>
        /// Checks if it's necessary to reset course cache
        /// </summary>
        /// <param name="courseFilterDescriptor"></param>
        /// <returns></returns>
        public bool IsResetParameterNeeded(FilterFieldDescriptor courseFilterDescriptor)
        {
            var courseId = GetChachedCourseId();
            if (String.IsNullOrEmpty(courseId))
            {
                return false;
            }

            return courseFilterDescriptor.Values.First() != courseId;
        }
    }
}