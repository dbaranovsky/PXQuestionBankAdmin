using System;
using System.Linq;
using System.Web;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Helpers
{
    public class CourseHelper
    {
        private const string CourseParamName = "current_course";
        private readonly IProductCourseManagementService productCourseManagementService;

        public CourseHelper(IProductCourseManagementService productCourseManagementService)
        {
            this.productCourseManagementService = productCourseManagementService;
        }

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

        public string GetChachedCourseId()
        {
            var course = HttpContext.Current.Session[CourseParamName] as Course;
            if (course == null)
            {
                return String.Empty;
            }

            return course.ProductCourseId;
        }

        public void ClearCache()
        {
            HttpContext.Current.Session[CourseParamName] = null;
        }
        
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