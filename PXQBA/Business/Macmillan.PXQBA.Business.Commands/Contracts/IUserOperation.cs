using System.Collections;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    /// <summary>
    /// Represents list of operations that are available for user object
    /// </summary>
    public interface IUserOperation
    {
        /// <summary>
        /// Loads user information from dlap
        /// </summary>
        /// <param name="agilixUserId">Agilix user id</param>
        /// <returns>Loaded user info</returns>
        UserInfo GetUser(string agilixUserId);

        /// <summary>
        /// Loads the list of users and their information from dlap
        /// </summary>
        /// <param name="userNames">User names to load</param>
        /// <returns>Loaded users</returns>
        IEnumerable<UserInfo> GetUsers(IEnumerable<string> userNames);
    }
}