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
    }
}