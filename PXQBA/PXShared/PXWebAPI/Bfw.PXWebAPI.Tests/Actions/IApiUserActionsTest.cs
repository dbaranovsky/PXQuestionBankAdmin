using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.PXWebAPI.Tests.Actions
{
    [TestClass]
    public class IApiUserActionsTest
    {
        private ISessionManager sessionManager;
        private ISession session;
        private IUserActions userActions;
        private IBusinessContext context;

        private ApiUserActions apiUserActions;

        [TestInitialize]
        public void TestInitialize()
        { 
            sessionManager = Substitute.For<ISessionManager>();
            session = Substitute.For<ISession>();
            userActions = Substitute.For<IUserActions>();
            context = Substitute.For<IBusinessContext>();
            sessionManager.CurrentSession = session; 

            apiUserActions = new ApiUserActions(sessionManager, userActions, context);
        }

        [TestMethod]
        public void UpdateUsers_Should_Update_List_Of_Users()
        {
            sessionManager.CurrentSession.WhenForAnyArgs(o => o.Execute(Arg.Any<GetUsers>())).Do(o =>
            {
                var command = (o.Arg<GetUsers>() as GetUsers);
                command.GetType().GetProperty("Users").SetValue(command,
                new List<Agilix.DataContracts.AgilixUser>()
                {
                    new Agilix.DataContracts.AgilixUser()
                    {
                         Reference = "12345"
                    },
                    new Agilix.DataContracts.AgilixUser()
                    {
                         Reference = "67890"
                    }

                }, null);
            });

            apiUserActions.UpdateUsers("userId", true, new Models.EditUser() { FirstName = "Family", LastName = "Guy", Email = "family@guy.com" });

            userActions.Received(2);
        }
    }
}
