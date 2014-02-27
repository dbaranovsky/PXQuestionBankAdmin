using System;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;
using Bfw.PX.PXPub.Controllers.Contracts;
using System.Web.Mvc;

namespace Bfw.PX.PXPub.Controllers.Tests.Widgets
{
    [TestClass]
    public class StartWelcomeWdigetControllerTest
    {
        private IPageActions _pageActions;

        private StartWelcomeWidgetController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _pageActions = Substitute.For<IPageActions>();

            _controller = new StartWelcomeWidgetController(null, _pageActions, null, null, null);
        }

        /// <summary>
        /// If item has "meta-content-type", it should be passed to AssignedItem.ContentType
        /// </summary>
        [TestCategory("StartWelcomeWdiget"), TestMethod]
        public void WhenSave_ExpectWidgetZoneIDIsInJsonResponse()
        {
            var model = new StartWelcomeWidget{Title="Test welcome widget"};
            JsonResult result = (JsonResult)_controller.Save(model, "PX_HOME_FACEPLATE_START", "PX_HOME_FACEPLATE_START_ZONE_TOP",
                "PX_FacePlate_StartWelcome_Widget", "PX_FacePlate_StartWelcome_Widget");
            var jsonString = result.Data.ToString();
            Assert.IsTrue(jsonString.Contains("Result = Success"));
            Assert.IsTrue(jsonString.Contains("Mode = EDIT"));
            Assert.IsTrue(jsonString.Contains("OldWidgetID = PX_FacePlate_StartWelcome_Widget"));
            Assert.IsTrue(jsonString.Contains("WidgetZoneID = PX_HOME_FACEPLATE_START_ZONE_TOP"));
            Assert.IsTrue(jsonString.Contains("WidgetTemplateID = PX_FacePlate_StartWelcome_Widget"));
            Assert.IsTrue(jsonString.Contains("WidgetId = PX_FacePlate_StartWelcome_Widget"));



        }

       

    }
}
