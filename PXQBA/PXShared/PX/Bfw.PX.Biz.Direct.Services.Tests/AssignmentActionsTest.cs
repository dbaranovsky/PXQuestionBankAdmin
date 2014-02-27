using System;
using System.Web.UI.WebControls;
using Bfw.Common.Database;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class AssignmentActionsTest
    {
        // ReSharper disable InconsistentNaming
        private IBusinessContext context;
        private IContentActions contentActions;
        private IDatabaseManager databaseManager;
        private AssignmentActions assignmentActions;
        private string _defaultToc;
        private IServiceLocator serviceLocator;  

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            contentActions = Substitute.For<IContentActions>();
            databaseManager = Substitute.For<IDatabaseManager>();
            assignmentActions = new AssignmentActions(context, contentActions);
            _defaultToc = "syllabusfilter";
            serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<IDatabaseManager>().Returns(databaseManager);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        [TestMethod]
        public void CalculateDayBeforeFromReminderEmailForZeroValue()
        {
            // Assign
            var reminderEmail = new ReminderEmail
            {
                AssignmentDate = new DateTime(year: 2013, month: 08, day: 08),
                DurationType = "minute",
                DaysBefore = 0
            };

            // Act
            var sendOnDate = assignmentActions.CalculateSendOnDate(reminderEmail);

            // Assert
            Assert.AreEqual(new DateTime(year: 2013, month: 08, day: 08), sendOnDate, "send on date on same day and time in Assignment Action is failing.");
        }

        [TestMethod]
        public void CalculateDayBeforeFromReminderEmail()
        {
            // Assign
            var reminderEmail = new ReminderEmail
            {
                AssignmentDate = new DateTime(year: 2013, month: 08, day: 08),
                DurationType = "day",
                DaysBefore = 1
            };

            // Act
            var sendOnDate = assignmentActions.CalculateSendOnDate(reminderEmail);

            // Assert
            Assert.AreEqual(new DateTime(year: 2013, month: 08, day: 07), sendOnDate, "send on date before a day in Assignment Action is failing.");
        }

        [TestMethod]
        public void CalculateWeekBeforeFromReminderEmail()
        {
            // Assign
            var reminderEmail = new ReminderEmail
            {
                AssignmentDate = new DateTime(year: 2013, month: 08, day: 08),
                DurationType = "week",
                DaysBefore = 1
            };

            // Act
            var sendOnDate = assignmentActions.CalculateSendOnDate(reminderEmail);

            // Assert
            Assert.AreEqual(new DateTime(year: 2013, month: 08, day: 01), sendOnDate, "send on date before a week in Assignment Action is failing.");
        }

        [TestMethod]
        public void CalculateHoursBeforeFromReminderEmail()
        {
            // Assign
            var reminderEmail = new ReminderEmail
            {
                AssignmentDate = new DateTime(year: 2013, month: 08, day: 08, hour: 23, minute:59, second: 0),
                DurationType = "hour",
                DaysBefore = 1
            };

            // Act
            var sendOnDate = assignmentActions.CalculateSendOnDate(reminderEmail);

            // Assert
            Assert.AreEqual(new DateTime(year: 2013, month: 08, day: 08, hour: 22, minute: 59, second: 0), sendOnDate, "send on date before a hour in Assignment Action is failing.");
        }

        [TestMethod]
        public void CalculateDaysFromReminderEmail()
        {
            // Assign
            var reminderEmail = new ReminderEmail
            {
                AssignmentDate = new DateTime(year: 2013, month: 08, day: 08, hour: 23, minute: 59, second: 0),
                DurationType = "minute",
                DaysBefore = 1
            };

            // Act
            var sendOnDate = assignmentActions.CalculateSendOnDate(reminderEmail);

            // Assert
            Assert.AreEqual(new DateTime(year: 2013, month: 08, day: 08, hour: 23, minute: 58, second: 0), sendOnDate, "send on date before a minute in Assignment Action is failing.");
        }

        /// <summary>
        /// If IsAllowLateGracePeriod.IsAllowLateGracePeriod is true, then item.IsAllowLateGracePeriod should be true
        /// </summary>
        [TestCategory("Assignments"), TestMethod]
        public void AssignmentActionsTest_Assign_ExpectIsAllowLateGracePeriodIsTrue()
        {
            //create and mock neccessary model and method
            var testIsAllowLateGracePeriod = false;
            AssignedItem assignItem = new AssignedItem { IsAllowLateGracePeriod  = true};
            contentActions.GetContent(null, null).ReturnsForAnyArgs(new ContentItem {AssignmentSettings = new AssignmentSettings()});

            //When contentActions.StoreContent get called, check ContentItem.AssignmentSettings.IsAllowLateGracePeriod
            contentActions.WhenForAnyArgs(f => f.StoreContent(null, null, false)).Do(x =>
            {
                testIsAllowLateGracePeriod = ((ContentItem) x[0]).AssignmentSettings.IsAllowLateGracePeriod;
            });
            assignmentActions.Assign(assignItem);
            //Expect ContentItem.AssignmentSettings.IsAllowLateGracePeriod is true
            Assert.IsTrue(testIsAllowLateGracePeriod);

        }

        /// <summary>
        /// If IsAllowLateGracePeriod.IsAllowLateGracePeriod is false, then item.IsAllowLateGracePeriod should be false
        /// </summary>
        [TestCategory("Assignments"), TestMethod]
        public void AssignmentActionsTest_Assign_ExpectIsAllowLateGracePeriodIsFalse()
        {

            var testIsAllowLateGracePeriod = true;
            AssignedItem assignItem = new AssignedItem { IsAllowLateGracePeriod = false };
            contentActions.GetContent(null, null).ReturnsForAnyArgs(new ContentItem { AssignmentSettings = new AssignmentSettings() });

            //When contentActions.StoreContent get called, check ContentItem.AssignmentSettings.IsAllowLateGracePeriod
            contentActions.WhenForAnyArgs(f => f.StoreContent(null, null, false)).Do(x =>
            {
                testIsAllowLateGracePeriod = ((ContentItem)x[0]).AssignmentSettings.IsAllowLateGracePeriod;
            });
            assignmentActions.Assign(assignItem);

            //Expect ContentItem.AssignmentSettings.IsAllowLateGracePeriod is false
            Assert.IsFalse(testIsAllowLateGracePeriod);

        }

        /// <summary>
        /// If IsAllowLateGracePeriod.IsAllowLateSubmission is false, then item.Properties[allowlatesubmissiongrace] should be false
        /// </summary>
        [TestCategory("ActivityMapper"), TestMethod]
        public void AssignmentActionsTest_Assign_ExpectAllowLateSubmissionGraceIsFalse()
        {
            var testIsAllowLateGracePeriod = true;
            AssignedItem assignItem = new AssignedItem { IsAllowLateGracePeriod = false };
            contentActions.GetContent(null, null).ReturnsForAnyArgs(new ContentItem { AssignmentSettings = new AssignmentSettings() });

            //When contentActions.StoreContent get called, check ContentItem.Properties["bfw_allowlatesubmissiongrace"]
            contentActions.WhenForAnyArgs(f => f.StoreContent(null, null, false)).Do(x =>
            {
                testIsAllowLateGracePeriod = bool.Parse(((ContentItem)x[0]).Properties["bfw_allowlatesubmissiongrace"].Value.ToString());
            });
            assignmentActions.Assign(assignItem);

            //Expect ContentItem.Properties["bfw_allowlatesubmissiongrace"] is false
            Assert.IsFalse(testIsAllowLateGracePeriod);

        }

        [TestMethod]
        public void EnsureGracePeriod_IsSaved_FirstTime()
        {
            var itemId = "7ca22b488c364fc88db78eb822f71bb5";
            var entityId = "133289";
            var groupId = entityId;
            var lateGraceDuration = 15;
            var lateGraceDurationType = "Minute";
            context.EntityId.Returns(entityId);
            //contentItem does not have values for grace period the first time it's set
            contentActions.GetContent(entityId, itemId).Returns(new ContentItem { Id = itemId, AssignmentSettings = new AssignmentSettings() });
            AssignedItem assignItem = new AssignedItem { Id = itemId, GroupId = groupId, IsAllowLateSubmission = true, IsAllowLateGracePeriod = true, LateGraceDuration = lateGraceDuration, LateGraceDurationType = lateGraceDurationType };
            assignmentActions.Assign(assignItem);
            //Assert
            contentActions.Received().StoreContent(Arg.Is<ContentItem>(c => c.AssignmentSettings.LateGraceDuration == lateGraceDuration
                                                                            && c.AssignmentSettings.LateGraceDurationType == lateGraceDurationType
                                                                            && c.AssignmentSettings.IsAllowLateGracePeriod
                                                                            && (long)c.Properties["bfw_latesubmissiongraceduration"].Value == lateGraceDuration
                                                                            && c.Properties["bfw_latesubmissiongracedurationtype"].Value as String == lateGraceDurationType
                                                                            && (bool)c.Properties["bfw_allowlatesubmissiongrace"].Value == true),
                                                                       groupId, false);
        }
    }
}
