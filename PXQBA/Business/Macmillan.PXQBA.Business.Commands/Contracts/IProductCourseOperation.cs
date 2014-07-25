using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Course = Macmillan.PXQBA.Business.Models.Course;

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
        Course CreateDraftCourse(string title);
        string AddSiteBuilderCourseToQBA(string url);
        void RemoveResources(string itemId, List<string> questionRelatedResources);

        void PutResources(List<Resource> resources);
    }
}
