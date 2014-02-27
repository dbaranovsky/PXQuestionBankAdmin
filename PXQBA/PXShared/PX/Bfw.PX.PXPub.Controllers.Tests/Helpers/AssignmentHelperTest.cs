using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    
    
    /// <summary>
    ///This is a test class for WritingTabControllerTest and is intended
    ///to contain all WritingTabControllerTest Unit Tests
    ///</summary>
    [TestClass]
    public class AssignmentHelperTest
    {
        private IBusinessContext _context;
        private ISessionManager _session;
        private IDocumentConverter _documentConverter;
        private IDatabaseManager _databaseManager;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _session = Substitute.For<ISessionManager>();
            _documentConverter = Substitute.For<IDocumentConverter>();
            _databaseManager = Substitute.For<IDatabaseManager>();
        }

        /// <summary>
        /// If pass in null assignment setting to GetGraceDueDate(), it should return min date time
        /// </summary>
        [TestCategory("Assignments"), TestMethod]
        public void AssignmentHelper_GetGraceDueDate_IfAssignmentSettingIsNull_ReturnMinDate()
        {
            var graceDueDate = AssignmentHelper.GetGraceDueDate(null);
            Assert.AreEqual(graceDueDate, DateTime.MinValue);
        }

        [TestCategory("Assignments"), TestMethod]
        public void AssignmentHelper_GetGraceDueDate_ExpectReturnOriginalDueDate()
        {
            var settings = new AssignmentSettings { LateGraceDurationType = null, DueDate = DateTime.MinValue};
            var graceDueDate = AssignmentHelper.GetGraceDueDate(settings);
            Assert.AreEqual(graceDueDate, DateTime.MinValue);
        }

        [TestCategory("Assignments"), TestMethod]
        public void AssignmentHelper_GetGraceDueDate_ExpectReturnOneMinuteGraceDueDate()
        {
            var settings = new AssignmentSettings { LateGraceDurationType = "minute", DueDate = DateTime.MinValue, LateGraceDuration = 1};
            var graceDueDate = AssignmentHelper.GetGraceDueDate(settings);
            Assert.AreEqual(graceDueDate, DateTime.MinValue.AddMinutes(1));
        }

        [TestCategory("Assignments"), TestMethod]
        public void AssignmentHelper_GetGraceDueDate_ExpectReturnOneHourGraceDueDate()
        {
            var settings = new AssignmentSettings { LateGraceDurationType = "hour", DueDate = DateTime.MinValue, LateGraceDuration = 1 };
            var graceDueDate = AssignmentHelper.GetGraceDueDate(settings);
            Assert.AreEqual(graceDueDate, DateTime.MinValue.AddHours(1));
        }

        [TestCategory("Assignments"), TestMethod]
        public void AssignmentHelper_GetGraceDueDate_ExpectReturnOneDayGraceDueDate()
        {
            var settings = new AssignmentSettings { LateGraceDurationType = "day", DueDate = DateTime.MinValue, LateGraceDuration = 1 };
            var graceDueDate = AssignmentHelper.GetGraceDueDate(settings);
            Assert.AreEqual(graceDueDate, DateTime.MinValue.AddDays(1));
        }

        [TestCategory("Assignments"), TestMethod]
        public void AssignmentHelper_GetGraceDueDate_ExpectReturnOneWeekGraceDueDate()
        {
            var settings = new AssignmentSettings { LateGraceDurationType = "week", DueDate = DateTime.MinValue, LateGraceDuration = 1 };
            var graceDueDate = AssignmentHelper.GetGraceDueDate(settings);
            Assert.AreEqual(graceDueDate, DateTime.MinValue.AddDays(7));
        }

        /// <summary>
        /// If grace period is set to infinite, maximum date should be returned
        /// </summary>
        [TestCategory("AssignmentHelper"), TestMethod]
        public void GetGraceDueDate_GracePeriodIsInfinite_ExpectMaxDueDate()
        {
            var settings = new AssignmentSettings { LateGraceDurationType = "infinite", DueDate = DateTime.MinValue, LateGraceDuration = 1 };
            var graceDueDate = AssignmentHelper.GetGraceDueDate(settings);
            Assert.AreEqual(DateTime.MaxValue, graceDueDate);
        }
    }
}
