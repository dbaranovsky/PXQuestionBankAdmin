using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Services.DLAP
{
    public class UserOperation : IUserOperation
    {
        private readonly IContext businessContext;

        public UserOperation(IContext businessContext)
        {
            this.businessContext = businessContext;
        }

        public UserInfo GetUser(string agilixUserId)
        {
            var search = new GetUsers
            {
                SearchParameters = new UserSearch()
                {
                    Id = agilixUserId,
                }
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(search);

            return Mapper.Map<UserInfo>(search.Users.FirstOrDefault());
        }

        public IEnumerable<UserInfo> GetUsers(IEnumerable<string> userNames)
        {
            var cmd = new GetUsersBatch()
            {
                SearchParameters = userNames.Select(u => new UserSearch(){Username = u}).ToList()
            };

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            return Mapper.Map<IEnumerable<UserInfo>>(cmd.Users.Where(u => !u.IsNullOrEmpty()).Select(u => u.First()));
        }
    }
}