using System.Collections.Generic;
using System.Configuration;
using Bfw.Common.JqGridHelper;
using Bfw.PX.Biz.DataContracts;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using System.Web.Mvc;
using NSubstitute;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    /// <summary>
    ///This is a test class for ImageUploadControllerTest and is intended
    ///to contain all ImageUploadControllerTest Unit Tests
    ///</summary>
    [TestClass]
    public class ImageUploadControllerTest
    {
        private ImageUploadController _controller;
        private IBusinessContext _context;
        private IContentActions _contActions;
        private IResourceMapActions _resourceMapActions;
        private IServiceLocator _serviceLocator;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            
            _contActions = Substitute.For<IContentActions>();
            _resourceMapActions = Substitute.For<IResourceMapActions>();
           
            _context = Substitute.For<IBusinessContext>();
            _serviceLocator = Substitute.For<IServiceLocator>();
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);

            _controller = new ImageUploadController(_context, _contActions, _resourceMapActions, null); 
            
        }

        /// <summary>
        ///Test to get image when config setting "AllowImageType" is null
        ///Expect to get 2 images back
        ///</summary>
        [TestCategory("ImageUpload"), TestMethod]
        public void ImageGridData_GetTwoImagesWithSameType()
        {
            _context.ExternalResourceBaseUrl.Returns("/Assests");
            _context.EnrollmentId.Returns("117043");
            _contActions.ListResources("117043", "Assets/*.jpg", "").Returns(new List<Resource> { new Resource { Name = "Test resource 1" }, new Resource { Name = "Test resource 2" } });
            JsonResult gridData = (JsonResult)_controller.ImageGridData("", "", 1, 117043);
            Assert.IsNotNull(gridData.Data);
            JqGridModel gridModel = (JqGridModel)gridData.Data;
            Assert.AreEqual(gridModel.Data.Records, 2);

        }

        /// <summary>
        ///Test to get image when config setting "AllowImageType" is set "jpg,png".
        ///Expect to get 2 images back
        ///</summary>
        [TestCategory("ImageUpload"), TestMethod]
        public void ImageGridData_GetTwoImagesWithDifferentTypes()
        {
            ConfigurationManager.AppSettings["AllowImageType"] = "jpg,png";

            _context.ExternalResourceBaseUrl.Returns("/Assests");
            _context.EnrollmentId.Returns("117043");
            _contActions.ListResources("117043", "Assets/*.jpg", "").Returns(new List<Resource> { new Resource { Name = "Test resource 1" } });
            _contActions.ListResources("117043", "Assets/*.png", "").Returns(new List<Resource> { new Resource { Name = "Test resource 1" } });

            JsonResult gridData = (JsonResult)_controller.ImageGridData("", "", 1, 117043);
            JqGridModel gridModel = (JqGridModel)gridData.Data;
            Assert.AreEqual(gridModel.Data.Records, 2);

        }
    }
}
