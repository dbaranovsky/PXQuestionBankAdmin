using System;
using System.Reflection;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.PXPub.Models;
using NSubstitute;
using System.Xml;
using System.Linq;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class SettingsViewControllerTest
    {
        private IBusinessContext context;
        private IContentActions contentActions;

        private SettingsViewController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            contentActions = Substitute.For<IContentActions>();

            controller = new SettingsViewController(context, contentActions);
        }

        [TestMethod]
        public void SaveContentSettings_Will_Save_Item_As_Visible_To_Student()
        {
            contentActions.GetContent("", "").ReturnsForAnyArgs(new Biz.DataContracts.ContentItem() 
            {
                Id = "1"
            });

            controller.SaveContentSettings(new ContentSettings() { Visibility = "visibletotudent", Id = "1", DueDate = new DateTime(2023, 10, 15) });
            var calls = contentActions.ReceivedCalls();
            var contentItem = calls.Last().GetArguments().ToArray()[0];

            Assert.AreEqual("<bfw_visibility>\r\n  <roles>\r\n    <instructor />\r\n    <student />\r\n  </roles>\r\n</bfw_visibility>", (contentItem as Biz.DataContracts.ContentItem).Visibility);
        }

        [TestMethod]
        public void SaveContentSettings_Will_Save_Item_As_Invisible_To_Student()
        {
            contentActions.GetContent("", "").ReturnsForAnyArgs(new Biz.DataContracts.ContentItem()
            {
                Id = "1"
            });

            controller.SaveContentSettings(new ContentSettings() { Visibility = "hidefromstudent", Id = "1", DueDate = new DateTime(2023, 10, 15) });
            var calls = contentActions.ReceivedCalls();
            var contentItem = calls.Last().GetArguments().ToArray()[0];

            Assert.AreEqual("<bfw_visibility>\r\n  <roles>\r\n    <instructor />\r\n  </roles>\r\n</bfw_visibility>", (contentItem as Biz.DataContracts.ContentItem).Visibility);
        }

        [TestMethod]
        public void SaveContentSettings_Will_Save_Item_As_RestrictedByDate_To_Student()
        {
            contentActions.GetContent("", "").ReturnsForAnyArgs(new Biz.DataContracts.ContentItem()
            {
                Id = "1"
            });

            controller.SaveContentSettings(new ContentSettings() { Visibility = "restrictedbydate", Id = "1", DueDate = new DateTime(2023, 10, 15) });
            var calls = contentActions.ReceivedCalls();
            var contentItem = calls.Last().GetArguments().ToArray()[0];

            Assert.AreEqual("<bfw_visibility>\r\n  <roles>\r\n    <instructor />\r\n    <student>\r\n      <restriction>\r\n        <date endate=\"2023-10-15T04:00:00Z\" />\r\n      </restriction>\r\n    </student>\r\n  </roles>\r\n</bfw_visibility>", (contentItem as Biz.DataContracts.ContentItem).Visibility);
        }
    }
}
