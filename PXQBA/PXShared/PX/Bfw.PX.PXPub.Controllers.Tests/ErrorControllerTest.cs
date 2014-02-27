using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TestHelper;
using Bfw.PX.PXPub.Controllers;
using Bfw.Common.Exceptions;
using System;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class ErrorControllerTest
    {
        private IBusinessContext _context;
        private IPageActions _pageActions;
        private IUserActions _userActions;
        private ErrorPageController _errorPageController;

        /// <summary>
        /// Basic init steps needed for all test methods as part of the arrangement 
        /// to help setup request context to act as helper for each test method
        /// </summary>

        [TestInitialize]
        public void InitializeControllerContext()
        {
            //Arrange
            _context = Substitute.For<IBusinessContext>();
            _pageActions = Substitute.For<IPageActions>();
            _userActions = Substitute.For<IUserActions>();


            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();
            
        }

        [TestMethod]
        public void ErrorControllerTest_Verify_Correct_Error_Is_Shown_To_PM()
        {            
            _errorPageController = new ErrorPageController();
            var exception = new ProgramManagerNotFoundException();
            System.Configuration.ConfigurationManager.AppSettings["ProgramManagerNotFoundException_Message"] = "Test";
            
            //Act
            var result = _errorPageController.DisplayError(exception) as ViewResult;

            //Assert
            Assert.AreEqual("Test", ((ErrorData)result.Model).DisplayMessage);
        }

        [TestMethod]
        public void ErrorControllerTest_Verify_Default_Error_Is_Shown_To_PM_If_PM_Message_Is_Empty()
        {
            _errorPageController = new ErrorPageController();
            var exception = new ProgramManagerNotFoundException();
            System.Configuration.ConfigurationManager.AppSettings["ProgramManagerNotFoundException_Message"] = "";
            System.Configuration.ConfigurationManager.AppSettings["Default_Message"] = "Default";

            //Act
            var result = _errorPageController.DisplayError(exception) as ViewResult;

            //Assert
            Assert.AreEqual("Default", ((ErrorData)result.Model).DisplayMessage);
        }

        [TestMethod]
        public void ErrorControllerTest_Verify_Default_Error_Is_Shown_To_Exception()
        {
            _errorPageController = new ErrorPageController();
            var exception = new Exception();            
            
            System.Configuration.ConfigurationManager.AppSettings["Default_Message"] = "Default";

            //Act
            var result = _errorPageController.DisplayError(exception) as ViewResult;

            //Assert
            Assert.AreEqual("Default", ((ErrorData)result.Model).DisplayMessage);
        }

       
    }
}
