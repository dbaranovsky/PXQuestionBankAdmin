using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Course = Macmillan.PXQBA.Business.Models.Course;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Manages logic for product courses
    /// </summary>
    public interface IProductCourseManagementService
    {
        /// <summary>
        /// Loads product course
        /// </summary>
        /// <param name="productCourseId">Course id</param>
        /// <param name="requiredQuestionBankRepository">Determines if question bank repository is necessary to load</param>
        /// <returns>Product Course</returns>
        Course GetProductCourse(string productCourseId, bool requiredQuestionBankRepository = false);

        /// <summary>
        /// Loads the list of available for current user courses without question bank repository value
        /// </summary>
        /// <returns>List of courses</returns>
        IEnumerable<Course> GetAvailableCourses();

        /// <summary>
        /// Loads the list of courses for current user with question bank repository value
        /// </summary>
        /// <returns>List of courses</returns>
        IEnumerable<Course> GetCourseList();

        /// <summary>
        /// Updates course xml after metadata was amended
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        Course UpdateMetadataConfig(Course course);

        /// <summary>
        /// Loads the list of all courses in QBA
        /// </summary>
        /// <returns>Course list</returns>
        IEnumerable<Course> GetAllCourses();

        /// <summary>
        /// Creates new draft course
        /// </summary>
        /// <param name="title">Course title</param>
        void CreateNewDraftCourse(string title);

        /// <summary>
        /// Adds reference to the existing site builder course inside QBA
        /// </summary>
        /// <param name="url">Url of the product course</param>
        /// <returns>Course id</returns>
        string AddSiteBuilderCourse(string url);

        /// <summary>
        /// Removes list of question related resources from product course
        /// </summary>
        /// <param name="getTemporaryCourseId">Product course id</param>
        /// <param name="getQuestionRelatedResources">Question related resources</param>
        void RemoveResources(string getTemporaryCourseId, List<string> getQuestionRelatedResources);

        /// <summary>
        /// Saves question related resources
        /// </summary>
        /// <param name="resources">Question resources</param>
        void PutResources(List<Resource> resources);
    }
}