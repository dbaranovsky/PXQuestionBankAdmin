using System;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class QuestionAdminActionsTest
    {
        private IBusinessContext context;
        private ISessionManager sessionManager;
        private ICourseActions courseActions;
        private IContentActions contentActions;
        private IUserActions userActions;
        private IEnrollmentActions enrollmentActions;
        private IItemQueryActions itemQueryActions;
        private IQuestionActions questionActions;

        private QuestionAdminActions actions;

        [TestInitialize]
        public void TestInitialize()
        { 
            context = Substitute.For<IBusinessContext>();
            sessionManager = Substitute.For<ISessionManager>();
            courseActions = Substitute.For<ICourseActions>();
            contentActions = Substitute.For<IContentActions>();
            userActions = Substitute.For<IUserActions>();
            enrollmentActions = Substitute.For<IEnrollmentActions>();
            itemQueryActions = Substitute.For<IItemQueryActions>();
            questionActions = Substitute.For<IQuestionActions>();

            context.CurrentUser = new DataContracts.UserInfo();

            actions = new QuestionAdminActions(context, sessionManager, courseActions, contentActions, userActions, enrollmentActions, questionActions, itemQueryActions);
        }
    }
}
