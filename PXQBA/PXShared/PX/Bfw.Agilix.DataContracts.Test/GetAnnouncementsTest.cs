using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Commands;

namespace Bfw.Agilix.DataContracts.Test
{
    [TestClass]
    public class GetAnnouncementsTest : BaseTest
    {
        #region Before and After each Test methods
        
        private const String
            path = @"GetAnnouncementsTest\",
            outputDir = "GetAnnouncementsTest";

        [TestInitialize]
        public void BeforeEachTest()
        {
        }

        [TestCleanup]
        public void AfterEachTest()
        {
        }

        #endregion

        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void GetOneAnnoucementUsingEntityId()
        {
            var cmd = new GetAnnouncements() { SearchParameters = new AnnouncementSearch() { EntityId = "1" } };

            ProcessCommand(cmd, getFileLocator("SingleAnnoucementRequested"));
            var returnedAnnouncements = cmd.Announcements;

            Assert.IsNotNull(returnedAnnouncements, "Announcement Array");
            Assert.AreEqual(1, returnedAnnouncements.Count, "Number of Announcements returned");

            var expected = new Announcement()
            {
                EntityId = "1",
                Path = "9b4b7d2b782c4dea9a8ef8d1690b8ebb",
                Title = "Announcement",
                StartDate = DateTime.Parse("2012-06-12T19:31:50.3917028Z"),
                EndDate = DateTime.Parse("9999-12-31T23:59:59.9999999Z"),
                CreationDate = DateTime.Parse("2012-06-12T19:31:54.65Z")
            };
            expected.Assert_AreEqual(returnedAnnouncements[0], null);
        }

        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void GetTwoAnnoucementUsingEntityId()
        {
            var cmd = new GetAnnouncements() { SearchParameters = new AnnouncementSearch() { EntityId = "2" } };

            ProcessCommand(cmd, getFileLocator("MultipleAnnoucementRequested"));
            var returnedAnnouncements = cmd.Announcements;

            Assert.IsNotNull(returnedAnnouncements, "Announcement Array");
            Assert.AreEqual(2, returnedAnnouncements.Count, "Number of Announcements returned");


            var expected1 = new Announcement()
            {
                EntityId = "1",
                Path = "9b4b7d2b782c4dea9a8ef8d1690b8ebb",
                Title = "Announcement",
                StartDate = DateTime.Parse("2012-06-12T19:31:50.3917028Z"),
                EndDate = DateTime.Parse("9999-12-31T23:59:59.9999999Z"),
                CreationDate = DateTime.Parse("2012-06-12T19:31:54.65Z")
            };
            expected1.Assert_AreEqual(returnedAnnouncements[0], null);

            var expected2 = new Announcement()
            {
                EntityId = "2",
                Path = "9b4b7d2b782c4dea9a8ef8d1690b8ebc",
                Title = "Announcement2",
                StartDate = DateTime.Parse("2012-06-12T19:31:50.3917029Z"),
                EndDate = DateTime.Parse("9999-12-31T23:59:59.9999998Z"),
                CreationDate = DateTime.Parse("2012-06-12T19:31:54.66Z")
            };
            expected2.Assert_AreEqual(returnedAnnouncements[1], null);
        }

        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void GetOneAnnoucementUsingEntityIdAndPath()
        {
            var cmd = new GetAnnouncements() { SearchParameters = new AnnouncementSearch() { EntityId = "1", Path = "9b4b7d2b782c4dea9a8ef8d1690b8ebb" } };

            ProcessCommand(cmd, getFileLocator("SingleAnnoucementUsingEntityIdAndPath"), "meta.xml");
            var returnedAnnouncements = cmd.Announcements;
            
            var expected = new Announcement()
            {
                EntityId = "1",
                Title = "Announcement",
                Path = "9b4b7d2b782c4dea9a8ef8d1690b8ebb",
                StartDate = DateTime.Parse("2012-06-13T11:45:32.630877-04:00"),
                EndDate = DateTime.Parse("9999-12-31T23:59:59.9999999"),
                CreationDate = DateTime.Parse("2012-06-13T11:45:32.630877-04:00"),
                Html = "Sample Body",
                PrimarySortOrder = "a",
                PinSortOrder = "b"
            };
            expected.Assert_AreEqual(returnedAnnouncements[0], null);
        }
      
    }
}
