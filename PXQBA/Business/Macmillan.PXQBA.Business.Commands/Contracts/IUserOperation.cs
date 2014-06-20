using System.Collections;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IUserOperation
    {
        UserInfo GetUser(string agilixUserId);

        IEnumerable<UserInfo> GetUsers(IEnumerable<string> userNames);
    }
}