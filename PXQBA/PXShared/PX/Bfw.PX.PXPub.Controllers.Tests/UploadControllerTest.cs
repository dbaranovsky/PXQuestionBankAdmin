using System;
using System.Linq;
using Bfw.PX.PXPub.Models;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.IO;
using System.Collections.Specialized;
using Bfw.PX.Biz.DataContracts;
using Bfw.Common.Custom;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using Course = Bfw.PX.Biz.DataContracts.Course;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class UploadControllerTest
    {
        private IBusinessContext context;
        private IContentHelper contentHelper;
        private IGradeActions gradeActions;
        private IResourceMapActions resourceMapActions;
        private IContentActions contentActions;
        private IDocumentConverter documentConverter;        
        private UploadController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            contentHelper = Substitute.For<IContentHelper>();
            gradeActions = Substitute.For<IGradeActions>();
            resourceMapActions = Substitute.For<IResourceMapActions>();
            contentActions = Substitute.For<IContentActions>();
            documentConverter = Substitute.For<IDocumentConverter>();            
            controller = new UploadController(context, contentHelper, gradeActions, resourceMapActions, contentActions, documentConverter);
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };
            controller.Request.Form.Returns(new System.Collections.Specialized.NameValueCollection { { "UploadTitle", "Test.docx" } });
        }

       [TestMethod]
        public void UploadHasCorrectEntityId()
        {
            var entityId = "999999";
            var enrollmentId = "888888";
            var parentId = "28a20b38d94a4bdfa96d6f303c1b4e85";
                      
            context.Course.Returns(new Course { CourseType = "Eportfolio" });
            context.EntityId.Returns(entityId);
            context.EnrollmentId.Returns(enrollmentId);
            
            var postedFile = Substitute.For<HttpPostedFileBase>();
            postedFile.InputStream.Returns(new MemoryStream());

            var result = controller.ImageUpload(new Models.Upload
            {
                ParentId = null,
                UploadType = Models.UploadType.Default,
                UploadFile = postedFile,
                UploadTitle = "Test.docx",
                AddToResourceMap = false,
                RetainOriginalFile = false,
                UploadFileType = UploadFileType.Any
            }) as PartialViewResult;
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<UploadResponse>(result.Model.ToString());
            
            //Assert
            Assert.IsNotNull(result);
            contentHelper.Received().StoreDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<System.Web.HttpPostedFileBase>(), Arg.Is(entityId));
        }
    }
}
