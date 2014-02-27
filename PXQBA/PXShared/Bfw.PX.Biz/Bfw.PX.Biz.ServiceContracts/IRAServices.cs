using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using Bfw.Common;
using Bfw.PX.Biz.DataContracts;
using Bfw.Common.Collections;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    public interface IRAServices
    {
        /// <summary>
        /// Gets the RA Users from a list of DLAP users
        /// </summary>
        /// <param name="dlapUsersList">List of UserInfo objects</param>
        /// <returns>List of UserInfo Objects</returns>
        UserProfileResponse GetUserProfile(IEnumerable<UserInfo> dlapUsersList);


        /// <summary>
        /// Gets the RA Users from a list of user reference ids
        /// </summary>
        /// <param name="user_reference_ids">String Array of user reference ids</param>
        /// <returns>List of UserInfo Objects</returns>
        UserProfileResponse GetUserProfile(String[] user_reference_ids);

        UserAuthResponse AuthenticateUser(string user, string password);

        RAAccessInfo GetAccessLevelByBaseUrl(string raUserId, string baseUrl);

        RASiteInfo GetSiteListByBaseUrl(string baseUrl);

        /// <summary>
        /// Sets a user profile image for a specific user and returns the RAWebServiceResponse object
        /// </summary>
        /// <param name="userRefId"> </param>
        /// <param name="fileType">File Extention</param>
        /// <param name="file">Posted file Stream object</param>
        /// <returns>RAWebServiceResponse Object</returns>
        UserProfileResponse SetUserProfileImage(string userRefId, string fileType, Stream file);

        /// <summary>
        /// Sets or updates a users email, first and last names by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userRefId"> </param>
        /// <param name="email">E-Mail</param>
        /// <param name="firstName">First Name</param>
        /// <param name="lastName">Last Name</param>
        /// <returns>RAWebServiceResponse Object</returns>
        UserProfileResponse SetUserProfileInfo(string userId, string userRefId, string email, string firstName,
            string lastName);
    }
}
