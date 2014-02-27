using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Bfw.PX.Biz.ServiceContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace Bfw.PX.PXPub.Controllers.Tests.Mappers
{
    [TestClass]
    public class ContentItemMapperTest
    {
        private IBusinessContext _context;
        private IContentActions _contentActions;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _contentActions = Substitute.For<IContentActions>();

            _context.CourseId = "courseId";
        }

        [TestCategory("Mapping"), TestMethod]
        public void ContentItemMapperTest_ToBaseContentItem_SetContainers()
        {
            var toc1 = "syllabusfilter";
            var container1 = "foo";
            var toc2 = "assignmentfilter";
            var container2 = "bar;";

            BizDC.ContentItem bizitem = new BizDC.ContentItem()
            {
                Containers = new List<BizDC.Container>()
                {
                    new BizDC.Container(toc1, container1),
                    new BizDC.Container(toc2, container2)
                }
            };

            ContentItem item = new ContentItem();
            item.ToBaseContentItem(bizitem, _contentActions);

            Assert.AreEqual(2, item.Containers.Count);
            Assert.IsTrue(item.Containers.Any(c => c.Toc.Equals(toc1)));
            Assert.IsTrue(item.Containers.Any(c => c.Toc.Equals(toc2)));
            Assert.AreEqual(container1, item.Containers.FirstOrDefault(c => c.Toc.Equals(toc1)).Value);
            Assert.AreEqual(container2, item.Containers.FirstOrDefault(c => c.Toc.Equals(toc2)).Value);
        }

        [TestCategory("Mapping"), TestMethod]
        public void ContentItemMapperTest_ToBaseContentItem_SetSubContainer()
        {
            var toc1 = "syllabusfilter";
            var subcontainer1 = "foo";
            var toc2 = "assignmentfilter";
            var subcontainer2 = "bar;";

            BizDC.ContentItem bizitem = new BizDC.ContentItem()
            {
                SubContainerIds = new List<BizDC.Container>()
                {
                    new BizDC.Container(toc1, subcontainer1),
                    new BizDC.Container(toc2, subcontainer2)
                }
            };

            ContentItem item = new ContentItem();
            item.ToBaseContentItem(bizitem, _contentActions);

            Assert.AreEqual(2, item.SubContainerIds.Count);
            Assert.IsTrue(item.SubContainerIds.Any(c => c.Toc.Equals(toc1)));
            Assert.IsTrue(item.SubContainerIds.Any(c => c.Toc.Equals(toc2)));
            Assert.AreEqual(subcontainer1, item.SubContainerIds.FirstOrDefault(c => c.Toc.Equals(toc1)).Value);
            Assert.AreEqual(subcontainer2, item.SubContainerIds.FirstOrDefault(c => c.Toc.Equals(toc2)).Value);
        }

        /// <summary>
        ///ToContentItem mapper should set the AgilixType property of the model
        /// Types:
        // Resource,
        //Assignment,
        //Assessment,
        //Discussion,
        //Folder,
        //CustomActivity,
        //AssetLink,
        //RssFeed,
        //Survey,
        //Shortcut
        //Custom
        ///</summary>
        [TestCategory("Mapping"),  TestMethod]
        public void ToContentItem_ShouldSetAgilixType()
        {
            TypesAreEqual("Resource");
            TypesAreEqual("Assignment");
            TypesAreEqual("Discussion");
            TypesAreEqual("Folder");
            TypesAreEqual("CustomActivity");
            TypesAreEqual("AssetLink");
            TypesAreEqual("RssFeed");
            TypesAreEqual("Survey");
            TypesAreEqual("Survey");
            TypesAreEqual("Survey");
            TypesAreEqual("Shortcut");
            TypesAreEqual("Custom");
            var bizitem = new BizDC.ContentItem() { Type = "ASDKASKLDAS" }; //junk types should resolve to Custom
            var modelItem = bizitem.ToContentItem(null);
            Assert.AreEqual("custom", modelItem.AgilixType.ToString().ToLowerInvariant());
        }

        [TestCategory("Mapping"), TestMethod]
        public void ToContentItem_Should_Populate_Grades_For_Dropbox()
        {
            var bizItem = new BizDC.ContentItem() { Type = "Dropbox", Subtype = "Dropbox" };
            var contentItem = bizItem.ToContentItem(_contentActions, true);

            _contentActions.GetGradesPerItem(new List<BizDC.ContentItem>() { bizItem }, _context.CourseId).Received();
        }

        [TestCategory("Mapping"), TestMethod]
        public void ContentItemMapperAction_ToContentItem_ShouldMapToHtmlQuiz()
        {
            var bizItem = new BizDC.ContentItem()
            {
                Type = "Assessment", 
                Subtype = "",
                ExamTemplate = "examtemplate"
            };
            var contentItem = bizItem.ToContentItem(_contentActions, true);
            Assert.IsInstanceOfType(contentItem, typeof (HtmlQuiz));
        }

        private static void TypesAreEqual(string type)
        {
            var bizitem = new BizDC.ContentItem() {Type = type};
            var modelItem = bizitem.ToContentItem(null);
            Assert.AreEqual(bizitem.Type.ToLowerInvariant(), modelItem.AgilixType.ToString().ToLowerInvariant());
        }
    }
}
