using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IProductCourseOperation
    {
        Course GetProductCourse(string productCourseId, bool requiredQuestionBankRepository = false);

        IEnumerable<Course> GetUserAvailableCourses(bool requiredQuestionBankRepository = false);

        IEnumerable<Course> GetCoursesByCourseIds(IEnumerable<string> courseIds,
            bool requiredQuestionBankRepository = false);

        Course UpdateCourse(Course course);

        IEnumerable<Course> GetAllCourses();
    }
}
