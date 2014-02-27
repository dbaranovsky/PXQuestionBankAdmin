using System;
using System.Linq;
using Bfw.Common;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.ContentTypes;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Models;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Bfw.PX.Biz.DataContracts;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class DocumentCollectionControllerTest
    {
        private IBusinessContext context;
        private IContentActions contentActions;
        private IContentHelper contentHelper;
        private IEnrollmentActions enrollmentActions;       
        private DocumentCollectionController controller;
        private IServiceLocator serviceLocator;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            contentActions = Substitute.For<IContentActions>();
            contentHelper = Substitute.For<IContentHelper>();
            enrollmentActions = Substitute.For<IEnrollmentActions>();
            serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<IEnrollmentActions>().Returns(enrollmentActions);          
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            var user = new UserInfo { Id = "116774", DomainId = "66159", ReferenceId = "6669368" };
            context.CurrentUser.Returns(user);
            controller = new DocumentCollectionController(context, contentActions, contentHelper);
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };
            controller.HttpContext.Request.Form.Returns(new System.Collections.Specialized.NameValueCollection());
        }        
    }
}
