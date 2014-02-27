using System;
using System.Collections.Specialized;
using System.Linq;
using NSubstitute;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.DataContracts;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class DownloadControllerTest
    {
        private IBusinessContext _context;
        private IContentActions _contentActions;
        private IGradeActions gradeActions;
        private IDocumentConverter documentConverter;
        private IEnrollmentActions enrollmentActions;
        private DownloadController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _contentActions = Substitute.For<IContentActions>();
            gradeActions = Substitute.For<IGradeActions>();
            documentConverter = Substitute.For<IDocumentConverter>();
            enrollmentActions = Substitute.For<IEnrollmentActions>();
            _context.CurrentUser = new UserInfo { Id = "34567" };
            _context.EntityId.Returns("45678");
            _controller = new DownloadController(_context, _contentActions, gradeActions, documentConverter, enrollmentActions);
        }

        [TestMethod]
        public void CanDownloadStudentUploadDocumentUsingEntityId()
        {
            var itemId = "9999999";
            var documentId = "8888888";
            _context.Course = new Course { CourseType = "Launchpad" };
            _context.EnrollmentId.Returns("22222");
            _context.EntityId.Returns("11111");
            var ci = new ContentItem { };
            var resource = new Resource { Url = "Assets/Test.docx", Extension = "docx" };
            var stream = resource.GetStream();
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = Encoding.ASCII.GetBytes("Some content for the word document.");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            ci.Resources = new List<Resource> { resource };
            _contentActions.GetContent(_context.EnrollmentId, itemId, true).Returns(new ContentItem());
            _contentActions.GetContent(_context.EntityId, itemId, true).Returns(ci);
            var file = _controller.Document(itemId, "TestDoc", documentId, null);
            Assert.IsNotNull(file);
            Assert.IsNotNull(file.FileStream);
        }

        /// <summary>
        /// This is a test of the fallback to using EnrollmentId.
        /// The code change was to use EntityId to store the document uploads.
        /// </summary>
        [TestMethod]
        public void CanDownloadStudentUploadDocumentUsingEnrollmentId()
        {
            var itemId = "9999999";
            var documentId = "8888888";
            _context.Course = new Course { CourseType = "Launchpad" };
            _context.EnrollmentId.Returns("22222");
            _context.EntityId.Returns("11111");
            var ci = new ContentItem { };
            var resource = new Resource { Url = "Assets/Test.docx", Extension = "docx" };
            var stream = resource.GetStream();
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = Encoding.ASCII.GetBytes("Some content for the word document.");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            ci.Resources = new List<Resource> { resource };
            _contentActions.GetContent(_context.EntityId, itemId, true).Returns(new ContentItem());
            _contentActions.GetContent(_context.EnrollmentId, itemId, true).Returns(ci);
            var file = _controller.Document(itemId, "TestDoc", documentId, null);
            Assert.IsNotNull(file);
            Assert.IsNotNull(file.FileStream);
        }

        /// <summary>
        /// If resource does not contain html tags, the result should be prepend with <html> and append with </html>
        /// </summary>
        [TestCategory("DownloadController"), TestMethod]
        public void DownloadController_ResourceDocuments_ExpectHtmlAdded()
        {
            _context.EntityId.Returns("entityid");
            _contentActions.GetContent(null, null).ReturnsForAnyArgs(new ContentItem());
            string resourceData = "<div>test data</div>";
            var resourceDataStream = new MemoryStream(Encoding.UTF8.GetBytes(resourceData));
            _contentActions.ListResources("resourceId", "entityid").ReturnsForAnyArgs(new List<Resource> { new TestResouce { TestResourceStream = resourceDataStream, Name = "TestResource" } });
            documentConverter.ConvertDocuments(Arg.Any<List<DocumentConversion>>())
                .Returns(c => c.Arg<List<DocumentConversion>>().First().DataStream);

            
            //documentConverter.ConvertDocuments()
            var file = _controller.ResourceDocuments("itemid", "", "docx", "docId", "CM");
            using (StreamReader sr = new StreamReader(file.FileStream, Encoding.UTF8))
            {
                var fileData = sr.ReadToEnd();

                Assert.AreEqual(fileData, "<html><div>test data</div></html>");
            }

        }

        /// <summary>
        /// If resource contains html tags, the result should not be modified
        /// </summary>
        [TestCategory("DownloadController"), TestMethod]
        public void DownloadController_ResourceDocuments_ExpectNoHtmlAdded()
        {
            _context.EntityId.Returns("entityid");
            _contentActions.GetContent(null, null).ReturnsForAnyArgs(new ContentItem());
            string resourceData = "<html><div>test data</div></html>";
            var resourceDataStream = new MemoryStream(Encoding.UTF8.GetBytes(resourceData));
            _contentActions.ListResources("resourceId", "entityid").ReturnsForAnyArgs(new List<Resource> { new TestResouce { TestResourceStream = resourceDataStream, Name = "TestResource" } });
            documentConverter.ConvertDocuments(Arg.Any<List<DocumentConversion>>())
                .Returns(c => c.Arg<List<DocumentConversion>>().First().DataStream);


            //documentConverter.ConvertDocuments()
            var file = _controller.ResourceDocuments("itemid", "", "docx", "docId", "CM");
            using (StreamReader sr = new StreamReader(file.FileStream, Encoding.UTF8))
            {
                var fileData = sr.ReadToEnd();

                Assert.AreEqual(fileData, "<html><div>test data</div></html>");
            }

        }
        /// <summary>
        /// private class used for testing Resouce
        /// </summary>
        private class TestResouce : Resource
        {
            /// <summary>
            /// Set the protected property ResourceStream in Resource
            /// </summary>
            public MemoryStream TestResourceStream
            {
                set
                {
                    ResourceStream = value;
                }
            }
        }

    }

    
}
