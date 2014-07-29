using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Course = Macmillan.PXQBA.Business.Models.Course;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    /// <summary>
    /// Represents the list of operations available for product courses
    /// </summary>
    public interface IProductCourseOperation
    {
        /// <summary>
        /// Loads product course from dlap
        /// </summary>
        /// <param name="productCourseId">Course id to load</param>
        /// <param name="requiredQuestionBankRepository">Determines if question bank repository is necessary</param>
        /// <returns>Product course</returns>
        Course GetProductCourse(string productCourseId, bool requiredQuestionBankRepository = false);

        /// <summary>
        /// Load the list of all the available courses for current user
        /// </summary>
        /// <param name="requiredQuestionBankRepository">Determines if question bank repository is necessary</param>
        /// <returns>Course list</returns>
        IEnumerable<Course> GetUserAvailableCourses(bool requiredQuestionBankRepository = false);

        /// <summary>
        /// Loads the list of courses by ids provided
        /// </summary>
        /// <param name="courseIds">Ids of the courses to load</param>
        /// <param name="requiredQuestionBankRepository">Determines if question bank repository is necessary</param>
        /// <returns>List of courses</returns>
        IEnumerable<Course> GetCoursesByCourseIds(IEnumerable<string> courseIds,
            bool requiredQuestionBankRepository = false);

        /// <summary>
        /// Updates course data in dlap
        /// </summary>
        /// <param name="course">Course to update</param>
        /// <returns>Updated course</returns>
        Course UpdateCourse(Course course);

        /// <summary>
        /// Loads all courses available in QBA
        /// </summary>
        /// <returns>Course list</returns>
        IEnumerable<Course> GetAllCourses();

        /// <summary>
        /// Creates new course marked as draft in dlap
        /// </summary>
        /// <param name="title">Product course title</param>
        /// <returns>Created draft course</returns>
        Course CreateDraftCourse(string title);

        /// <summary>
        /// Adds existing in dlap product course to the list of courses available in QBA
        /// </summary>
        /// <param name="url">Url of the existing course</param>
        /// <returns>Course id</returns>
        string AddSiteBuilderCourseToQBA(string url);

        /// <summary>
        /// Deletes question related resources from product course storage
        /// </summary>
        /// <param name="itemId">Id of the item where resources are located</param>
        /// <param name="questionRelatedResources">List of resource urls to delete</param>
        void RemoveResources(string itemId, List<string> questionRelatedResources);

        /// <summary>
        /// Saves resources to dlap
        /// </summary>
        /// <param name="resources">Resources to save</param>
        void PutResources(List<Resource> resources);
    }
}
