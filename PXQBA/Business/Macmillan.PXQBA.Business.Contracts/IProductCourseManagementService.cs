using System.Collections;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IProductCourseManagementService
    {
        Course GetProductCourse(string productCourseId);
        IEnumerable<Course> GetAvailableCourses();
    }
}