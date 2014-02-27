using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.DataContracts;
using Bfw.Agilix.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that manipulate users.
    /// </summary>
    public interface IUserActions
    {
        /// <summary>
        /// Logs the user in as the anonymous user.
        /// </summary>
        /// <returns></returns>
        UserInfo Login();

        /// <summary>
        /// Uses the credentials specified to login the user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserInfo Login(string username, string password);

        /// <summary>
        /// Logsout the currently logged in user.
        /// </summary>
        void Logout();

        /// <summary>
        /// Gets the currently logged in user.
        /// </summary>
        /// <returns></returns>
        UserInfo CurrentUser();

        /// <summary>
        /// Gets a list of specific user ids from dlap
        /// Initial usage of method is to get the formatted name of specific users
        /// </summary>
        /// <param name="userIds">A list of userids</param>
        /// <returns>A list of UserInfo objects for the list of user ids</returns>
        IList<UserInfo> ListUsers(IEnumerable<String> userIds);

        /// <summary>
        /// Lists all users in the system.
        /// </summary>
        /// <returns></returns>
        List<UserInfo> ListUsers();

        /// <summary>
        /// Gets the user with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserInfo GetUser(string id);

        /// <summary>
        /// Get User by RA Id
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        UserInfo GetUserByRAId(string RAId);

        /// <summary>
        /// Gets the user Info by (RA) Reference Id and Domain Id
        /// </summary>
        /// <param name="referenceId">This is RA reference id / user id in RA</param>
        /// <param name="domainId">Agilix Domain Id</param>
        /// <returns></returns>
        UserInfo GetUserByReferenceAndDomainId(string referenceId, string domainId);

        /// <summary>
        /// Lists users that match the search criteria.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="domainId"></param>
        /// <returns></returns>
        List<UserInfo> ListUsersLike(UserInfo example);

        /// <summary>
        /// Accepts list of UserSearch as parameter and returns List of List of UserInfo in batch execution mode.
        /// </summary>
        /// <param name="userSearchs"></param>
        /// <param name="failures">List containing failure collections</param>
        /// <returns></returns>
        List<List<UserInfo>> ListUsersLike(List<UserSearch> userSearchs, out List<ItemStorageFailure> failures);

        /// <summary>
        /// Creates the user with the given information.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="pwdQuestion">The password question.</param>
        /// <param name="pwdAnswer">The password answer.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="email">The email.</param>
        /// <param name="domainId">The domain ID.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="referenceId">The reference ID.</param>
        /// <returns></returns>
        UserInfo CreateUser(string username, string password, string pwdQuestion, string pwdAnswer, string firstName, string lastName, string email, string domainId, string domainName, string referenceId);

        /// <summary>
        /// Creates the user with the given information
        /// </summary>
        /// <param name="users">The users.</param>
        /// <returns></returns>
        List<UserInfo> CreateUsers(IList<AgilixUser> users);

		/// <summary>
		/// Returns a list of RA users based on DLAP users
		/// </summary>
        /// <returns>RAWebServiceResponse Object</returns>
        UserProfileResponse LoadUserProfile();

    	/// <summary>
    	/// Sets an image or avatar for the user in RA
    	/// </summary>
    	/// <param name="userId">User ID</param>
    	/// <param name="userRefId"> </param>
    	/// <param name="fileType">File extention</param>
    	/// <param name="file">File Stream</param>
    	/// <returns>RAWebServiceResponse Object</returns>
    	UserProfileResponse SetUserProfileImage(string userRefId, string fileType, Stream file);

    	/// <summary>
    	/// Updates user information in RA
    	/// </summary>
    	/// <param name="userId"> </param>
    	/// <param name="userRefId">User ID</param>
    	/// <param name="email">User E-Mail</param>
    	/// <param name="firstName">First Name</param>
    	/// <param name="lastName">Last Name</param>
    	/// <returns>RAWebServiceResponse Object</returns>
        UserProfileResponse SetUserProfileInfo(string userId, string userRefId, string email, string firstName, string lastName);
	
        /// <summary>
        /// Invalidate user
        /// </summary>
        /// <param name="userRefId"></param>
        void invalidateUser(string userRefId);

		/// <summary>
        /// Updates the user with passed information.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="domainId">The domain ID.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="username">The username.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="email">The email.</param>
        /// <param name="referenceId">The reference ID.</param>
        /// <returns></returns>
        bool UpdateUser(string userId, string domainId, string domainName, string username, string firstName, string lastName, string email, string referenceId);

        /// <summary>
        /// Update the user info
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool UpdateUser(UserInfo user);

        /// <summary>
        /// Update list of user info objects
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        bool UpdateUsers(IList<UserInfo> users);

        /// <summary>
        /// Gets the List of Instructors associated with a program
        /// </summary>
        /// <param name="search">search</param>
        /// <returns></returns>
        IEnumerable<UserInfo> ListProgramInstructors(UserProgramSerach search);

        /// <summary>
        /// Get all instructors in the domain in any academic Term
        /// </summary>
        /// <param name="application_type">course type</param>
        /// <param name="domainId">domain id</param>
        /// <returns></returns>
        IEnumerable<UserInfo> ListInstructorsForDomain(String application_type, string domainId);

        /// <summary>
        /// Get all instructors in the domain
        /// </summary>
        /// <param name="application_type">course type</param>
        /// <param name="domainId">domain id</param>
        /// <param name="academicTerm">academic term</param>
        /// <returns></returns>
        IEnumerable<UserInfo> ListInstructorsForDomain(String application_type, string domainId, string academicTerm);

        IEnumerable<Bfw.PX.Biz.DataContracts.Course> FindCoursesByInstructor(String application_type, string instructorId, string domainId, string academicTerm, string parentId);

		/// <summary>
		/// Gets the User Profile Image URL. The profile image stored in BrainHoney can be of various types (jpeg, gif, png etc.)
		/// So, the image with the latest modified date is supposed to be the the latest profile image.
		/// </summary>
		/// <param name="UserProfile"></param>
		/// <returns></returns>
		string GetUserAvatarUrl(UserInfo UserProfile);

        /// <summary>
        /// Returns the pxmigration/user user ID, which is "7" across DLAP
        /// </summary>
        string PxMigrationUserId { get; }

        /// <summary>
        /// Only to be used for integration testing purposes
        /// </summary>
        /// <param name="user"></param>
        void DeleteUsers(params UserInfo[] users);
    }
}