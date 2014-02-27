using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides a common container for all context information required by the business layer
    /// </summary>
    public interface IBusinessContext
    {
        /// <summary>
        /// returns the Product Course ID of the URL passed in
        /// </summary>
        /// <param name="url">the url of the course ex. -standard-/dashboard/eportfolio/</param>
        /// <returns>Product Master Course Id for URL</returns>
        String GetProductCourseId(String course, String url);

        /// <summary>
        /// Gets the logger.
        /// </summary>
        Bfw.Common.Logging.ILogger Logger { get; }

        /// <summary>
        /// Gets the tracer.
        /// </summary>
        Bfw.Common.Logging.ITraceManager Tracer { get; }

        /// <summary>
        /// Gets the cache provider.
        /// </summary>
        Bfw.Common.Caching.ICacheProvider CacheProvider { get; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        UserInfo CurrentUser { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is anonymous.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is anonymous; otherwise, <c>false</c>.
        /// </value>
        bool IsAnonymous { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is public view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is public view; otherwise, <c>false</c>.
        /// </value>
        bool IsPublicView { get; }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        Domain Domain { get; }

        /// <summary>
        /// Gets the dashbaord course id.
        /// </summary>
        string DashboardCourseId { get; }

        /// <summary>
        /// Gets the entity ID.
        /// </summary>
        string EntityId { get; }

        /// <summary>
        /// Gets or sets the enrollment ID.
        /// </summary>
        string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the enrollment status.
        /// </summary>
        string EnrollmentStatus { get; set; }

        /// <summary>
        /// Gets the environment URL.
        /// </summary>
        string EnvironmentUrl { get; }

        /// <summary>
        /// Gets the app domain URL.
        /// </summary>
        string AppDomainUrl { get; }

        /// <summary>
        /// Gets the external resource base URL.
        /// </summary>
        string ExternalResourceBaseUrl { get; }

        /// <summary>
        /// Gets the proxy URL.
        /// </summary>
        string ProxyUrl { get; }

        /// <summary>
        /// Gets the BrainHoney URL.
        /// </summary>
        string BrainHoneyUrl { get; }

        /// <summary>
        /// Gets the discussion prefix.
        /// </summary>
        string DiscussionPrefix { get; }

        /// <summary>
        /// Gets or sets the product course.
        /// </summary>
        Course Product { get; set; }

        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        Course Course { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current course is a product course.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the current course is a product course; otherwise, <c>false</c>.
        /// </value>
        bool CourseIsProductCourse { get; }

        /// <summary>
        /// Gets or sets the access level.
        /// </summary>
        AccessLevel AccessLevel { get; set; }

        /// <summary>
        /// A flag which indicates whether a course needs to be browsed in a read only mode
        /// </summary>
        bool IsCourseReadOnly { get; set; }

        /// <summary>
        /// A flag which indicates whether a course is being browsed in a shared view mode
        /// </summary>
        bool IsSharedCourse { get; set; }

        /// <summary>
        /// <c>true</c> if the user is logged in as an instructor but in 'student view' mode.
        /// </summary>
        bool ImpersonateStudent { get; }

        /// <summary>
        /// The key for the student view cookie.
        /// </summary>
        string StudentViewCookieKey { get; }

        ///// <summary>
        ///// The key for system message cookie.
        ///// </summary>
        //string SystemMessageCookieKey { get; }

        /// <summary>
        /// The key for the preview as visitor cookie.
        /// </summary>
        string PreviewAsVisitorCookieKey { get; }


        /// <summary>
        /// Gets or sets the access level (adopter, etc.).
        /// </summary>
        AccessType AccessType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user can create course.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this user can create course; otherwise, <c>false</c>.
        /// </value>
        bool CanCreateCourse { get; set; }

        /// <summary>
        /// Gets or sets the course ID.
        /// </summary>
        string CourseId { get; set; }

        /// <summary>
        /// Gets or sets the product course id.
        /// </summary>
        string ProductCourseId { get; set; }

        /// <summary>
        /// Gets or sets the student dashboard id.
        /// </summary>
        string StudentDashboardId { get; set; }

        /// <summary>
        /// Gets or sets the site ID.
        /// </summary>
        string SiteID { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        string URL { get; set; } // change the name.

        /// <summary>
        /// Gets the RA base URL.
        /// </summary>
        string RABaseUrl { get;}

        /// <summary>
        /// The name of the product the requrest is using
        /// </summary>
        string ProductType { get; set; }

        /// <summary>
        /// Allows the implementation to set any necessary
        /// values
        /// </summary>
        void Initialize();

        /// <summary>
        /// Produces a new Item Id. The Id must be unique per domain
        /// </summary>
        /// <returns>item id</returns>
        string NewItemId();

        /// <summary>
        /// Determines the correct sequence string for an item that is between min and max
        /// </summary>
        /// <param name="min">item sequenced "above" the item</param>
        /// <param name="max">item sequenced "below" the item</param>
        /// <returns></returns>
        string Sequence(string min, string max);

        /// <summary>
        /// What kind of questions are allowed.  Map from question type name to description.
        /// </summary>
        Dictionary<string, string> GetQuestionTypes();

        /// <summary>
        /// What kind of documents are allowed for download only.
        /// </summary>
        List<string> GetDownloadOnlyDocuments();

        /// <summary>
        /// Gets or sets the BrainHoney auth cookie value.
        /// </summary>
        string BhAuthCookieValue { get; set; }

        /// <summary>
        /// Returns a list of all the courses that the current RA user is logged into, across domains.
        /// </summary>
        /// <param name="isSingleDomain"></param>
        /// <returns></returns>
        IEnumerable<Course> FindAllEnrolledCoursesForRAUser(bool isSingleDomain);

        /// <summary>
        /// Returns list of all courses that the user is enrolled in
        /// </summary>
        IEnumerable<Course> FindCoursesByUserEnrollment(string userId, string domainId);

        /// <summary>
        /// Returns list of all courses that the user is enrolled in
        /// </summary>
        /// <param name="parentId">If this value is not null, then limit courses to only those with this parent ID</param>
        IEnumerable<Course> FindCoursesByUserEnrollment(string userId, string domainId, string parentId);

        /// <summary>
        /// Given a domain id, set the current user object to be the user with the current ref id, but
        /// in the given domain.
        /// </summary>
        /// <param name="domainId">The id of the domain the user is in</param>
        void UpdateCurrentUser(string domainId);

        /// <summary>
        /// Returns a list of ids for all domains this RA user belongs to.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Domain> GetRaUserDomains();

        /// <summary>
        /// Clear the course cache and get the new course definition.
        /// </summary>
        void RefreshCourse();

        string GetSamlAuthenticationBHComponentUrl();

        /// <summary>
        /// Gets the correct user information to use when a new user account in DLAP need to
        /// be created.
        /// </summary>
        /// <returns>UserInfo for the new user account in DLAP.</returns>
        UserInfo GetNewUserData();

		/// <summary>
		/// Initializes the domains.
		/// </summary>
	    void InitializeDomains();

        /// <summary>
        /// Batch the user and domains to find courses
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="domainIds"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        IEnumerable<Course> FindCoursesByUserEnrollmentBatch(List<UserInfo> userInfo, string productCourseId, bool titleAndIdOnly = false);
    }
}
