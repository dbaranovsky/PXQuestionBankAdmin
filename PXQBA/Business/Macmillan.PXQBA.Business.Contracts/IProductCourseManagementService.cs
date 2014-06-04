using System.Collections;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IProductCourseManagementService
    {
        Course GetProductCourse(string productCourseId, bool requiredQuestionBankRepository = false);
        IEnumerable<Course> GetAvailableCourses();

        IEnumerable<Course> GetCourseList();
        Course UpdateMetadataConfig(Course course);
    }
}