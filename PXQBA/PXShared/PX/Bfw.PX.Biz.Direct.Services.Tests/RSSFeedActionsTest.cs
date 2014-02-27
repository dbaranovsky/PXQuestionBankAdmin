using System;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using TestHelper;
using System.Linq;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class RSSFeedActionsTest
    {
        private IBusinessContext context;
        private ISessionManager sessionManager;

        private RSSFeedActions actions;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            sessionManager = Substitute.For<ISessionManager>();

            context.Course = new DataContracts.Course()
            {
                CourseTimeZone = "Eastern Standard Time"
            };

            actions = new RSSFeedActions(context, sessionManager);
        }

        [TestMethod]
        public void ParseRssFeedItems_Should_StripOut_Html_Symbols()
        {
            var items = new List<CodeForce.Utilities.RssItem>() 
            { 
                new CodeForce.Utilities.RssItem()
                {
                    Link = "http://link.com",
                    Title = "This is a test <b>title</b>",
                    Description ="This is a test <b>description</b>"
                }
            };
            var method = actions.GetType().GetMethod("ParseRssFeedItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var expected = new CodeForce.Utilities.RssItem()
                {
                    Link = "http://link.com",
                    Title = "This is a test title",
                    Description = "This is a test description",
                    Date = DateTime.Parse("01/02/2023 07:00 PM")
                };

            var result = method.Invoke(actions, new object[] { 1, "feedURL", null, items, 0 });

            Assert.AreEqual("This is a test title", (result as List<RSSFeed>).First().LinkTitle);
            Assert.AreEqual("This is a test description", (result as List<RSSFeed>).First().LinkDescription);
        }
    }
}
