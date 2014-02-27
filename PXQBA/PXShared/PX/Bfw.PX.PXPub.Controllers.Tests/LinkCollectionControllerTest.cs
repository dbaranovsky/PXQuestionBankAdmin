using System;
using System.Web.Mvc;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.ContentTypes;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TestHelper;
using System.Collections.Generic;
using System.Linq;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class LinkCollectionControllerTest
    {
        private IBusinessContext context;
        private IContentActions contentActions;
        private IContentHelper contentHelper;
        private IAssignmentCenterHelper assignmentHelper;

        private LinkCollectionController controller;

        [TestInitialize]
        public void TestInitialize()
        { 
            context = Substitute.For<IBusinessContext>();
            contentActions = Substitute.For<IContentActions>();
            contentHelper = Substitute.For<IContentHelper>();
            assignmentHelper = Substitute.For<IAssignmentCenterHelper>();

            controller = new LinkCollectionController(context, contentActions, contentHelper, assignmentHelper);
        }

        [TestMethod]
        public void LinkList_Should_Return_Model_If_Collection_With_Links_Is_Provided()
        {
            var collection = new LinkCollection() { Id = "collectionId", Links = new List<Link>() { new Link() { Id = "linkId" } } };
            contentHelper.LoadContentView(collection.Id, ContentViewMode.Edit, true, "syllabusfilter").Returns(new ContentView() { Content = collection });

            var result = controller.LinkList(collection, collection.Id);

            Assert.AreEqual(collection.Id, ((result as PartialViewResult).Model as LinkCollection).Id);
            Assert.AreEqual(collection.Links.First().Id, ((result as PartialViewResult).Model as LinkCollection).Links.First().Id);
        }

        [TestMethod]
        public void LinkList_Should_Return_Model_If_CollectionId_Is_Provided()
        {
            var collection = new LinkCollection() { Id = "collectionId" };
            contentHelper.LoadContentView(collection.Id, ContentViewMode.Edit, true, "syllabusfilter").Returns(new ContentView() { Content = collection });

            var result = controller.LinkList(null, collection.Id);

            Assert.AreEqual(collection.Id, ((result as PartialViewResult).Model as LinkCollection).Id);
        }

    }
}
