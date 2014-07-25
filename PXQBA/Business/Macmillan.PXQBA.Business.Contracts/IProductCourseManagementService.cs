using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Course = Macmillan.PXQBA.Business.Models.Course;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IProductCourseManagementService
    {
        Course GetProductCourse(string productCourseId, bool requiredQuestionBankRepository = false);
        IEnumerable<Course> GetAvailableCourses();

        IEnumerable<Course> GetCourseList();
        Course UpdateMetadataConfig(Course course);
        IEnumerable<Course> GetAllCourses();
        void CreateNewDraftCourse(string title);
        string AddSiteBuilderCourse(string url);
        void RemoveResources(string getTemporaryCourseId, List<string> getQuestionRelatedResources);
        void PutResources(List<Resource> resources);
    }
}