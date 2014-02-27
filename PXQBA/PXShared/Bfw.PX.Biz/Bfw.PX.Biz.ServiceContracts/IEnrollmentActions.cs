using System;
using System.Collections.Generic;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that manipulate enrollments.
    /// </summary>
    public interface IEnrollmentActions
    {
        /// <summary>
        /// Sets the enrollment of the user specified to inactive
        /// </summary>
        /// <param name="enrollment">enrollment object that will be deactivated</param>
        void InactiveEnrollment(Enrollment enrollment);

        /// <summary>
        /// Updates the enrollment of the user with new attributes
        /// </summary>
        /// <param name="enrollment">enrollment object that will be updated</param>
        /// <returns>Success or failure as true / false</returns>
        bool UpdateEnrollment(Enrollment enrollment);

        /// <summary>
        /// Updates a list of enrollment
        /// </summary>
        /// <param name="enrollments"></param>
        /// <returns>Success or failure as true / false</returns>
        bool UpdateEnrollments(IEnumerable<Enrollment> enrollments);

        /// <summary>
        /// Deep load of related course data for enrollment collection
        /// </summary>
        /// <param name="enrollments"></param>
        /// <returns></returns>
        List<Enrollment> LoadEnrollmentCourses(List<Enrollment> enrollments, bool getEnrollmentCount = false);

        /// <summary>
        /// Gets a list of user enrollments from agilix based on the External/Reference ID across domains.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <returns>List of Enrollment objects.</returns>
        List<Enrollment> ListEnrollments(string referenceId);

        /// <summary>
        /// Gets a list of user enrollments from agilix that are derrived from a specific product.
        /// </summary>
        /// <param name="referenceId">External/Reference id of the user across domains</param>
        /// <param name="productId">Id of the produect course to return enrollments for</param>
        /// <returns></returns>
        List<Enrollment> ListEnrollments(string referenceId, string productId, bool loadCourses = true, bool getEnrollmentCount = false);

        /// <summary>
        /// Gets a list of user enrollments from agilix that matches the query and reference id.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <returns>List of Enrollment objects.</returns>
        List<Enrollment> ListEnrollmentsMatchQuery(string referenceId, string query);

        /// <summary>
        /// Gets a list of enrollments from agilix that matches the query and user ids.
        /// </summary>
        /// <param name="userIds">user id</param>
        /// <param name="flags">flags</param>
        /// <param name="query">query used to filter the list of courses</param>
        /// <returns></returns>
        List<Enrollment> ListUsersEnrollments(IEnumerable<string> userIds, string flags, string query);

        /// <summary>
        /// Gets an enrollment record.
        /// </summary>
        /// <param name="enrollmentId">enrollment id</param>
        /// <returns></returns>
        Enrollment GetEnrollment(string enrollmentId);

        /// <summary>
        /// Gets an enrollment record.
        /// </summary>
        /// <param name="userId">ID of the user for which to get the enrollment.</param>
        /// <param name="entityId">ID of the course or section.</param>
        /// <returns></returns>
        Enrollment GetEnrollment(string userId, string entityId);

        /// <summary>
        ///  Get the inactive enrollment for the course.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Enrollment GetInactiveEnrollment(string userId, string entityId);

        /// <summary>
        /// Switches user id in course enrollments based on new domain user account.
        /// </summary>
        /// <param name="oldUserId"></param>
        /// <param name="newUserId"></param>
        /// <param name="courseId"></param>
        /// <param name="newDomainId"></param>
        /// <returns></returns>
        List<Enrollment> SwitchUserEntityEnrollment(string oldUserId, string courseId, UserInfo userInfo);

        /// <summary>
        /// Gets the enrollment settings resource file for the current user.
        /// Enrollment file is currently stored as a resource using the following naming/path convention:
        /// Templates/Data/XmlResources/Settings/Enrollment/{EntityId}/{EnrollmentId}.pxres
        /// </summary>
        /// <returns></returns>
        Resource GetEnrollmentSettings();

        /// <summary>
        /// Saves the enrollment settings resource file in the system for the current user.
        /// </summary>
        /// <param name="settingsFile">The settings resource file to save.</param>
        void SaveEnrollmentSettings(Resource settingsFile);

        /// <summary>
        /// Gets a collection of enrollments for the requested entity/course ID.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <returns></returns>
        IEnumerable<Enrollment> GetEntityEnrollments(string entityId);

        /// <summary>
        /// Gets a collection of enrollments with grades for the requested entity/course ID.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <returns></returns>
        IEnumerable<DataContracts.Enrollment> GetEntityEnrollmentsWithGrades(string entityId);

        /// <summary>
        /// Gets the list of users enrolled in the specified entity filtered by user type.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        IEnumerable<Enrollment> GetEntityEnrollments(string entityId, UserType userType);

        /// <summary>
        /// Gets the list of users enrolled in the specified entity via an Admin login.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <returns></returns>
        List<Enrollment> GetEntityEnrollmentsAsAdmin(string entityId);

        /// <summary>
        /// Gets the list of all users enrolled in the specified entity via an Admin login.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <returns></returns>
        List<Enrollment> GetAllEntityEnrollmentsAsAdmin(string entityId);

        /// <summary>
        /// Gets a list of users enrolled in any of the specified entities in the list
        /// </summary>
        /// <param name="entityIds"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        List<Enrollment> GetEnrollmentsBatch(List<string> entityIds, UserType userType);

        /// <summary>
        /// Creates the enrollments.
        /// See http://gls.agilix.com/Docs/Command/CreateEnrollments.
        /// </summary>
        /// <param name="domainId">The ID of the domain to create the enrollment in.</param>
        /// <param name="userId">The ID of the user to enroll.</param>
        /// <param name="entityId">The ID of the course or section in which to enroll the user.</param>
        /// <param name="flags">Bitwise OR of RightsFlags to grant to the user.</param>
        /// <param name="status">EnrollmentStatus for the user.</param>
        /// <param name="startDate">Date that the enrollment begins.</param>
        /// <param name="endDate">Date that the enrollment ends.</param>
        /// <param name="reference">Optional field reserved for any data the caller wishes to store. Used to store the RA ID.</param>
        /// <param name="schema">An optional parameter that specifies how to interpret flags in agilix.</param>
        /// <param name="disallowduplicates">If true then there will be no duplicate enrollment</param>
        /// <returns></returns>
        List<Enrollment> CreateEnrollments(string domainId, string userId, string entityId, string flags, string status, DateTime startDate, DateTime endDate, string reference, string schema, bool disallowduplicates = false);

        /// <summary>
        /// Gets the user enrollment ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entityId"></param>
        /// <param name="allStatus"></param>
        /// <returns></returns>
        String GetUserEnrollmentId(string userId, string entityId, bool allStatus = false);


        /// <summary>
        /// Retruns a list of all domains that a user can enroll in.
        /// </summary>
        /// <param name="userReferenceId">Reference ID of the user that wants to be enrolled.</param>
        /// <param name="parentDomainId">ID of the parent domain to restring domain list to.</param>
        /// <param name="forceIfMember">If populated with a list of domain ids, then these domains will be in the result if the user is a member of the domain.</param>
        /// <returns>Distinct list of domains that the user may enroll in.</returns>
        IEnumerable<Domain> GetEnrollableDomains(string userReferenceId, string parentDomainId, IEnumerable<string> forceIfMember = null);
    }
}
