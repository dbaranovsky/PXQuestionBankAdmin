using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;
using System.Collections;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that create and manage courses.
    /// </summary>
    public interface ICourseActions
    {
        /// <summary>
        /// This method will be used to set the Instructor Email in Reminder Service after course activation
        /// </summary>
        /// <param name="email">Email address of instructor</param>
        /// <param name="courseTitle">Course Title</param>
        /// <param name="courseId">Course Id Passed to Service, It should not be picked from context.</param>
        /// <param name="productCourseId">Product course id for the passed course id</param>
        /// <param name="instructorId">Agilix Instructor Id, this can be different from Context User Id, if it is in different domain (like from Dashboard)</param>
        void SendInstructorEmail(string email, string courseTitle, string courseId, string productCourseId, string instructorId);

        /// <summary>
        /// Sets up the currently logged in user as the instructor. This fixes the problem were the admin
        /// user is set as the instructor. Also, set up the instructor's shadow student user.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <param name="courseOwner">Agilix Course ID (within current domain / institute) for Owner of the course.</param>
        void SetCourseEnrollments(IEnumerable<Course> courses, string courseOwner, bool bShowStudentView=true);

        /// <summary>
        /// returns the application type no matter which course type was passed in
        /// </summary>
        /// <param name="courseType">The course type of the dlap course object</param>
        /// <returns></returns>
        String getApplicationType(String courseType);
        
        /// <summary>
        /// Gets an enrollment for the given user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="entityId">The entity ID.</param>
        /// <returns></returns>
        Enrollment GetEnrollment(string userId, string entityId);

        /// <summary>
        /// Gets a course by course ID.
        /// </summary>
        /// <param name="courseId">The course ID.</param>
        /// <returns></returns>
        Course GetCourseByCourseId(string courseId);

        /// <summary>
        /// Gets a list of courses by a list of ids
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<Course> GetCoursesByCourseIds(IEnumerable<string> courseIds);

        /// <summary>
        /// Gets a set of courses by title match.
        /// </summary>
        /// <param name="titlePart">The title part.</param>
        /// <returns></returns>
        IEnumerable<Course> GetCoursesByTitleMatch(string titlePart);
                
        /// <summary>
        /// Gets the academic terms by domain.
        /// </summary>
        /// <param name="domainId">The domain id.</param>
        /// <returns></returns>
        List<CourseAcademicTerm> GetAcademicTermsByDomain(string domainId);

        /// <summary>
        /// Lists all courses.
        /// </summary>
        /// <returns></returns>
        List<Course> ListCourses();

        /// <summary>
        /// Updates a course.
        /// </summary>
        /// <param name="courses">The course.</param>
        /// <returns></returns>
        Course UpdateCourse(Course course);
     
        /// <summary>
        /// Updates a set of courses.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        List<Course> UpdateCourses(List<Course> courses);

        /// <summary>
        /// Creates a set of courses.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        List<Course> CreateCourses(List<Course> courses);

        /// <summary>
        /// Deletes a set of courses.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        List<Course> DeleteCourses(List<Course> courses);

        /// <summary>
        /// Enrolls user in given courses
        /// </summary>
        /// <param name="courses"></param>
        /// <param name="owner"></param>
        /// <param name="enableStudentView"></param> 
        void EnrollCourses(List<Course> courses, string owner = null, bool enableStudentView=true);

        /// <summary>
        /// Copies a set of courses to a new domain.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <param name="newDomainId">The new domain ID.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        List<Course> CopyCourses(List<Course> courses, string newDomainId, string method);

        /// <summary>
        /// Gets generic course detail. In case Id is missing, create Generic course in Generic Domain 
        /// and then create generic course from course sought. Update Product course id flag and return the newly created generic course.
        /// </summary>
        /// <param name="genericCourseId">Give course details for related Generic course id</param>
        /// <returns></returns>
        Course GetGenericCourse(string genericCourseId, out string parentDomainId);

    	/// <summary>
    	/// Loads data for a specified container
    	/// </summary>
    	/// <param name="course"></param>
    	/// <param name="containerName"> </param>
    	/// <param name="subcontainerId"> </param>
    	/// <returns></returns>
    	List<ContentItem> LoadContainerData(Course course, string containerName, string subcontainerId, string toc = "");

    	/// <summary>
    	/// Loads data for a specified container
    	/// </summary>
    	/// <param name="courseId"></param>
    	/// <param name="containerName"></param>
    	/// <param name="subcontainerId"> </param>
    	/// <returns></returns>
    	List<ContentItem> LoadContainerData(string courseId, string containerName, string subcontainerId, string toc = "");

        /// <summary>
        /// Activates a course.
        /// </summary>
        /// <param name="course">The course.</param>
        void ActivateCourse(Course course);

        /// <summary>
        /// Deactivates a course.
        /// </summary>
        /// <param name="course">The course.</param>
        void DeactivateCourse(Course course);

        /// <summary>
        /// Creates a derived course.
        /// </summary>
        /// <param name="deriveFrom">The course to derive from.</param>
        /// <param name="newDomainId">The domain ID for the new course.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        Course CreateDerivedCourse(Course deriveFrom, string newDomainId, string copymode = "derivative", string userId = null);

        /// <summary>
        /// Cache Launchpad data
        /// </summary>
        /// <param name="preBuiltItems"></param>
        void CacheLaunchPadData(List<ContentItem> preBuiltItems);

        /// <summary>
        /// Retrieve the launch pad data
        /// </summary>
        /// <returns></returns>
        List<ContentItem> FetchLaunchPadData();

        /// <summary>
        /// returns the list of course that matches the search course types
        /// </summary>
        /// <param name="courseType">a list of course type</param>
        /// <returns>list of matching courses</returns>
        IEnumerable<Course> ListCoursesByCourseTypes(List<CourseType> courseTypes);

        /// <summary>
        /// returns the list of course that matches the search query.
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        IEnumerable<Course> ListCoursesMatchQuery(string query);

        /// <summary>
        /// returns the list of course that matches the search course type and academicterm
        /// </summary>
        /// <param name="courseType">course type</param>
        /// <param name="academicTerm">academic term id</param>
        /// <returns>list of matching courses</returns>
        IEnumerable<Course> ListCourses(CourseType courseType, string academicTerm, bool includeSubtypeInSearch = false);

        IEnumerable<Course> ListCourses(CourseType courseType, List<string> academicTerms);

        /// <summary>
        /// Gets the list of academic terms
        /// </summary>
        /// <returns></returns>
        IEnumerable<CourseAcademicTerm> ListAcademicTerms();

        /// <summary>
        /// Gets the list of academic terms
        /// </summary>
        /// <param name="domainId">domain id</param>
        /// <returns></returns>
        IEnumerable<CourseAcademicTerm> ListAcademicTerms(string domainId);

        /// <summary>
        /// Get the current academic term
        /// </summary>
        /// <returns>current academic term</returns>
        CourseAcademicTerm CurrentAcademicTerm();

        /// <summary>
        /// Returns full course information for the requested courses
        /// </summary>
        /// <param name="courses"></param>
        /// <returns></returns>
        IEnumerable<Course> GetCourseListInformation(IEnumerable<Course> courses);

        /// <summary>
        /// Returns course information for given domain and academic term
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="termId"></param>
        /// <param name="applicationType"></param>
        /// <returns></returns>
        IEnumerable<Course> GetCourseByDomainTerm(string domainId, string termId, string applicationType);

        /// <summary>
        /// Get Domain [Schools / Institute] information from Onyx Web services
        /// </summary>
        /// <param name="searchType">Search type can be "City" or "Zip"</param>
        /// <param name="city">Name of city to search</param>
        /// <param name="regionCode">Region Code / State or Province ID</param>
        /// <param name="countryCode">Country code id "US" / "CA"</param>
        /// <param name="instituteType">Institute Type {"0" both, "1" School, "2" College} </param>
        /// <param name="zipCode">Search based on Zip code</param>
        /// <returns>Dictionary of Onyx school id and School name</returns>
        IDictionary<string, string> FindDomainFromOnyx(string searchType, string city, string regionCode, string countryCode, string instituteType, string zipCode);


        /// <summary>
        /// Finds the active sandbox course
        /// </summary>
        /// <param name="courseType">course type</param>
        /// <param name="domainId">domain</param>
        /// <returns>sandbox course</returns>
        Course FindSandboxCourse(CourseType courseType, string domainId, string productCourseId);


        /// <summary>
        /// Merge the deltas from a derivative course into its immediate master course.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        void MergeCourses(Course course);

        /// <summary>
        /// Checks if Course is updated (Checks Items, Questions, Resources for updates)
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        bool CourseUpdateFlag(Course course);
    }
}