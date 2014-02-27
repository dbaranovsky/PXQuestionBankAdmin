using System;
using System.Web.Mvc;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class RSSFeedWidgetControllerTest
    {
        private RSSFeedWidgetController controller;

        private IBusinessContext context;
        private IRSSFeedActions feedActions;
        private IPageActions pageActions;

        [TestInitialize]
        public void TestInitialize()
        { 
            context = Substitute.For<IBusinessContext>();
            feedActions = Substitute.For<IRSSFeedActions>();
            pageActions = Substitute.For<IPageActions>();

            controller = new RSSFeedWidgetController(context, feedActions, pageActions);
        }

        [TestMethod]
        public void CompactViewFPStart_Should_Return_Model_With_No_Feeds()
        {
            var result = controller.CompactViewFPStart(new Models.Widget() { Id = "1", Title = "Scientific American" });
            var model = (Bfw.PX.PXPub.Models.RSSFeedWidget)(result as ViewResult).Model;

            Assert.AreEqual(null, model.RSSFeeds);
        }

        [TestMethod]
        public void PartialListView_Should_Return_Model_With_Feeds()
        {
            string title = string.Empty;
            int totalArchivedArticles = 0;
            string templateId = "PX_ScientificAmerican_RSS_Feed_Compact";
            string feedDescription = string.Empty;
            string imageURL = string.Empty;

            feedActions.ListRssFeeds("1", 0, out title, out totalArchivedArticles, out templateId, out feedDescription, out imageURL, 5).
                ReturnsForAnyArgs(x => {
                    x[4] = templateId;
                    return new List<Biz.DataContracts.RSSFeed>() { new Biz.DataContracts.RSSFeed() };
                });

            var result = controller.PartialListView("1", 0);
            var model = (Bfw.PX.PXPub.Models.RSSFeedWidget)(result as ViewResult).Model;

            Assert.AreEqual(1, model.RSSFeeds.Count);
        }
    }
}
