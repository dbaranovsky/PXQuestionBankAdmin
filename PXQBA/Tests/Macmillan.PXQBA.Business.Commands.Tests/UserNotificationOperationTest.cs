using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.SQLOperations;
using Macmillan.PXQBA.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class UserNotificationOperationTest
    {
        private IDatabaseManager databaseManager;
        private IContext businessContext;

        private IUserNotificationOperation userNotificationOperation;

        [TestInitialize]
        public void TestInitialize()
        {
            databaseManager = Substitute.For<IDatabaseManager>();
            businessContext = Substitute.For<IContext>();

            userNotificationOperation = new UserNotificationOperation(databaseManager, businessContext);
        }


        [TestMethod]
        public void GetNotShownNotification_NoParameters_UserNotShownNotification()
        {
            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetUserNotShownNotifications"))
               .Returns(new List<DatabaseRecord>() { GetUserNotShownNotificationRecord() });

            businessContext.CurrentUser.Returns(new UserInfo() { Username = "123" });

            var notifications = userNotificationOperation.GetNotShownNotification();
            var notificationsArray = notifications.ToArray();

            Assert.IsTrue(notificationsArray.Length==1);
            Assert.IsTrue(notificationsArray[0].UserId == "123");
            Assert.IsTrue(notificationsArray[0].NotificationType == NotificationType.NewDraftForAvailableToInstructors);
        }

        [TestMethod]
        public void CreateNotShownNotification_SaveAndPublishDraft_SqlInvokedWithUserId()
        {
            const string userName = "123";
            bool invokedWithUserId = false;
            databaseManager.When(
                dm =>
                    dm.ExecuteNonQuery(Arg.Is<DbCommand>(c => c.CommandText == "dbo.CreateUserNotShownNotification" &&
                                                        c.Parameters[0].ParameterName == "@userId" &&
                                                        c.Parameters[0].Value.ToString() == userName)))
                        .Do(d=> { invokedWithUserId = true; });
            businessContext.CurrentUser.Returns(new UserInfo() { Username = userName });

            userNotificationOperation.CreateNotShownNotification(NotificationType.SaveAndPublishDraft);

            Assert.IsTrue(invokedWithUserId);
        }

        [TestMethod]
        public void CreateNotShownNotification_EditInPlaceQuestionInProgress_SqlInvokedWithRigthNotificationType()
        {
            const string userName = "123";
            const NotificationType notificationType = NotificationType.EditInPlaceQuestionInProgress;
            bool invokedWithRigthNotificationType = false;

            databaseManager.When(
                dm =>
                    dm.ExecuteNonQuery(Arg.Is<DbCommand>(c => c.CommandText == "dbo.CreateUserNotShownNotification" &&
                                                        c.Parameters[1].ParameterName == "@notificationType" &&
                                                        (NotificationType)c.Parameters[1].Value == notificationType)))
                        .Do(d => { invokedWithRigthNotificationType = true; });
            businessContext.CurrentUser.Returns(new UserInfo() { Username = userName });

            userNotificationOperation.CreateNotShownNotification(notificationType);

            Assert.IsTrue(invokedWithRigthNotificationType);
        }


        private DatabaseRecord GetUserNotShownNotificationRecord()
        {
            var record = new DatabaseRecord();

            record["UserId"] = "123";
            record["NotificationType"] = NotificationType.NewDraftForAvailableToInstructors.ToString();

            return record;
        }
    }
}
