using System;
using System.Linq;
using NSubstitute;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PXWebAPI.Tests
{
    [TestClass]
    public class IApiContentActionsTests
    {
        private ISessionManager sessionManager;
        private IBusinessContext businessContext;
        private IItemQueryActions itemQueryActions;
        private IContentActions contentActions;

        private IApiContentActions contentApi;

        [TestInitialize]
        public void TestInitialize()
        {
            sessionManager = Substitute.For<ISessionManager>();
            businessContext = Substitute.For<IBusinessContext>();
            itemQueryActions = Substitute.For<IItemQueryActions>();
            contentActions = Substitute.For<IContentActions>();

            contentApi = new ApiContentActions(sessionManager, businessContext, itemQueryActions, contentActions);
        }

        [TestMethod]
        public void ExpectListOfContentToSync()
        {
            // arrange
            var courseId = "1234";
            var date = DateTime.Parse("2013-03-17T16:49:57.627Z").AddDays(-4);

            sessionManager.CurrentSession.Send(new DlapRequest(), true).ReturnsForAnyArgs(new DlapResponse(
                System.Xml.Linq.XDocument.Parse(@"
                    <response code=""OK"">
                        <items>
                            <item id=""Assignment12"" version=""8"" 
                                  resourceentityid=""63237,0,0"" actualentityid=""6651"" 
                                  creationdate=""2011-06-17T19:23:05.89Z"" creationby=""2"" 
                                  modifieddate=""2013-03-17T16:49:57.627Z"" modifiedby=""7"">
                                <data>
                                <type>Assignment</type>
                                <parent>DEFAULT</parent>
                                <sequence>a</sequence>
                                <title>Assignment 12</title>
                                <href>Assets/assignment12.htm</href>
                                </data>
                            </item>
                        </items>
                    </response>
                "))
            );

            // act
            var itemsToSync = contentApi.GetItemsToSync(from: courseId, since: date);

            // assert
            Assert.AreNotEqual(0, itemsToSync.Count);
            Assert.AreEqual(Bfw.PXWebAPI.Models.SyncStatus.Modified, itemsToSync.SyncItems.First().Status);
        }

        [TestMethod]
        public void ExpectEmptyListOfContentToSync()
        {
            // arrange
            var courseId = "1234";
            var date = DateTime.Parse("2013-03-17T16:49:57.627Z").AddDays(-4);

            sessionManager.CurrentSession.Send(new DlapRequest(), true).ReturnsForAnyArgs(new DlapResponse(            
                System.Xml.Linq.XDocument.Parse(@"
                    <response code=""OK"">
                        <items />
                    </response>
                "))
            );

            // act
            var itemsToSync = contentApi.GetItemsToSync(from: courseId, since: date);

            // assert
            Assert.AreEqual(0, itemsToSync.Count);
        }

        [TestMethod]
        public void ExpectErrorDueToCourseIdNotExisting()
        {
            // arrange
            var courseId = "1234";
            var date = DateTime.Parse("2013-03-17T16:49:57.627Z").AddDays(-4);

            sessionManager.CurrentSession.Send(new DlapRequest(), true).ReturnsForAnyArgs(new DlapResponse(           
                System.Xml.Linq.XDocument.Parse(@"
                    <response code=""DoesNotExist"" message=""Course /24 not found."">
                      <detail>GoCourseServer.DataModel.DoesNotExistException: Course /24 not found.
                       at GoCourseServer.DataModel.Course.GetCourse(Int64 courseId)
                       at GoCourseServer.DataModel.Enrollment.GetEntity()
                       at GoCourseServer.DataModel.Enrollment.GetSchema()
                       at GoCourseServer.ManifestRequestHandler.GetItemList(DlapContext context)
                       at GoCourseServer.Dlap.ProcessRequest(HttpContext context)
                    agent='Desktop' url='http://dev.dlap.bfwpub.com/Dlap.ashx?getitemlist&amp;entityid=234' site='dlap' method='GET' referer='http://dev.dlap.bfwpub.com/CallMethod.aspx' process='828' userid='2'</detail>
                    </response>
                "))
            );

            // act
            var itemsToSync = contentApi.GetItemsToSync(from: courseId, since: date);

            // assert
            Assert.AreEqual(true, itemsToSync.Error);
            Assert.AreNotEqual(string.Empty, itemsToSync.Message);
            Assert.AreEqual(0, itemsToSync.Count);
        }

        [TestMethod]
        public void ExpectErrorDueToMissingCreationDate()
        {
            // arrange
            var courseId = "1234";
            var date = DateTime.Parse("2013-03-17T16:49:57.627Z").AddDays(-4);

            sessionManager.CurrentSession.Send(new DlapRequest(), true).ReturnsForAnyArgs(new DlapResponse(
                System.Xml.Linq.XDocument.Parse(@"
                    <response code=""OK"">
                        <items>
                            <item id=""Assignment12"" version=""8"" 
                                  resourceentityid=""63237,0,0"" actualentityid=""6651"" 
                                  creationby=""2"" 
                                  modifieddate=""2013-03-17T16:49:57.627Z"" modifiedby=""7"">
                                <data>
                                <type>Assignment</type>
                                <parent>DEFAULT</parent>
                                <sequence>a</sequence>
                                <title>Assignment 12</title>
                                <href>Assets/assignment12.htm</href>
                                </data>
                            </item>
                        </items>
                    </response>
                "))
            );

            // act
            var itemsToSync = contentApi.GetItemsToSync(from: courseId, since: date);

            // assert
            Assert.AreEqual(true, itemsToSync.Error);
            Assert.AreNotEqual(string.Empty, itemsToSync.Message);
            Assert.AreEqual(0, itemsToSync.Count);
        }

        [TestMethod]
        public void ExpectErrorDueToMissingModifiedDate()
        {
            // arrange
            var courseId = "1234";
            var date = DateTime.Parse("2013-03-17T16:49:57.627Z").AddDays(-4);

            sessionManager.CurrentSession.Send(new DlapRequest(), true).ReturnsForAnyArgs(new DlapResponse(
                System.Xml.Linq.XDocument.Parse(@"
                    <response code=""OK"">
                        <items>
                            <item id=""Assignment12"" version=""8"" 
                                  resourceentityid=""63237,0,0"" actualentityid=""6651"" 
                                  creationdate= ""2011-06-17T19:23:05.89Z"" creationby=""2"" 
                                  modifiedby=""7"">
                                <data>
                                <type>Assignment</type>
                                <parent>DEFAULT</parent>
                                <sequence>a</sequence>
                                <title>Assignment 12</title>
                                <href>Assets/assignment12.htm</href>
                                </data>
                            </item>
                        </items>
                    </response>
                "))
            );

            // act
            var itemsToSync = contentApi.GetItemsToSync(from: courseId, since: date);

            // assert
            Assert.AreEqual(true, itemsToSync.Error);
            Assert.AreNotEqual(string.Empty, itemsToSync.Message);
            Assert.AreEqual(0, itemsToSync.Count);
        }

        [TestMethod]
        public void ExpectModifiedStatus()
        {
            // arrange
            var courseId = "1234";
            var date = DateTime.Parse("2013-03-17T16:49:57.627Z").AddDays(-4);

            sessionManager.CurrentSession.Send(new DlapRequest(), true).ReturnsForAnyArgs(new DlapResponse(
                System.Xml.Linq.XDocument.Parse(@"
                    <response code=""OK"">
                        <items>
                            <item id=""Assignment12"" version=""8"" 
                                  resourceentityid=""63237,0,0"" actualentityid=""6651"" 
                                  creationdate=""2011-06-17T19:23:05.89Z"" creationby=""2"" 
                                  modifieddate=""2013-03-17T16:49:57.627Z"" modifiedby=""7"">
                                <data>
                                <type>Assignment</type>
                                <parent>DEFAULT</parent>
                                <sequence>a</sequence>
                                <title>Assignment 12</title>
                                <href>Assets/assignment12.htm</href>
                                </data>
                            </item>
                        </items>
                    </response>
                "))
            );

            // act
            var itemsToSync = contentApi.GetItemsToSync(from: courseId, since: date);

            // assert
            Assert.AreNotEqual(0, itemsToSync.Count);
            Assert.AreEqual(Bfw.PXWebAPI.Models.SyncStatus.Modified, itemsToSync.SyncItems.First().Status);
        }

        [TestMethod]
        public void ExpectCreatedStatus()
        {
            // arrange
            var courseId = "1234";
            var date = DateTime.Parse("2013-03-17T16:49:57.627Z").AddDays(-4);

            sessionManager.CurrentSession.Send(new DlapRequest(), true).ReturnsForAnyArgs(new DlapResponse(
                System.Xml.Linq.XDocument.Parse(@"
                    <response code=""OK"">
                        <items>
                            <item id=""Assignment12"" version=""8"" 
                                  resourceentityid=""63237,0,0"" actualentityid=""6651"" 
                                  creationdate=""2013-03-17T16:49:57.627Z"" creationby=""2"" 
                                  modifieddate=""2013-03-17T16:49:57.627Z"" modifiedby=""7"">
                                <data>
                                <type>Assignment</type>
                                <parent>DEFAULT</parent>
                                <sequence>a</sequence>
                                <title>Assignment 12</title>
                                <href>Assets/assignment12.htm</href>
                                </data>
                            </item>
                        </items>
                    </response>
                "))
            );

            // act
            var itemsToSync = contentApi.GetItemsToSync(from: courseId, since: date);

            // assert
            Assert.AreNotEqual(0, itemsToSync.Count);
            Assert.AreEqual(Bfw.PXWebAPI.Models.SyncStatus.Created, itemsToSync.SyncItems.First().Status);
        }

        [TestMethod]
        public void ExpectDeletedStatus()
        {
            // arrange
            var courseId = "1234";
            var date = DateTime.Parse("2013-03-17T16:49:57.627Z").AddDays(-4);

            sessionManager.CurrentSession.Send(new DlapRequest(), true).ReturnsForAnyArgs(new DlapResponse(
                System.Xml.Linq.XDocument.Parse(@"
                    <response code=""OK"">
                        <items>
                            <item id=""Assignment12"" version=""8"" 
                                  resourceentityid=""63237,0,0"" actualentityid=""6651"" 
                                  creationdate=""2013-03-17T16:49:57.627Z"" creationby=""2"" 
                                  modifieddate=""2013-03-17T16:49:57.627Z"" modifiedby=""7"">
                                <data>
                                <type>Assignment</type>
                                <parent>PX_DELETED</parent>
                                <sequence>a</sequence>
                                <title>Assignment 12</title>
                                <href>Assets/assignment12.htm</href>
                                </data>
                            </item>
                        </items>
                    </response>
                "))
            );

            // act
            var itemsToSync = contentApi.GetItemsToSync(from: courseId, since: date);

            // assert
            Assert.AreNotEqual(0, itemsToSync.Count);
            Assert.AreEqual(Bfw.PXWebAPI.Models.SyncStatus.Deleted, itemsToSync.SyncItems.First().Status);
        }

        [TestMethod]
        public void GetTableofContents_Returns_Children()
        {
            var courseid = "1234";
            var topParentId = "topParentId";
            var parentId = "parentId";
            contentActions.ListChildren(courseid, topParentId, 1, "syllabusfilter").Returns(new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Id = "noChildrenId",
                        Type = "Assignment",
                        Containers = new List<Container>() { new Container("syllabusfilter", "Launchpad") },
                        AssignmentSettings = new AssignmentSettings()
                    },
                    new ContentItem()
                    {
                        Id = parentId,
                        Type = "Folder",
                        Containers = new List<Container>() { new Container("syllabusfilter", "Launchpad") },
                        AssignmentSettings = new AssignmentSettings()
                    }
                });
            contentActions.ListChildren(courseid, parentId, 1, "syllabusfilter").Returns(new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Id = "itemId",
                        Type = "Assignment",
                        Containers = new List<Container>() { new Container("syllabusfilter", "Launchpad") },
                        AssignmentSettings = new AssignmentSettings()
                    }
                });

            var result = contentApi.GetTableofContents(topParentId, courseid, 2);

            Assert.IsTrue(result.Any(i => i.ItemId.Equals("noChildrenId")));
            Assert.IsTrue(result.Any(i => i.ItemId.Equals(parentId)));
            Assert.IsTrue(result.Any(i => i.ItemId.Equals("itemId")));
        }

        [TestMethod]
        public void GetTableofContents_Returns_SortedChildren()
        {
            var courseid = "1234";
            var topParentId = "topParentId";
            var parentId = "parentId";
            contentActions.ListChildren(courseid, topParentId, 1, "syllabusfilter").Returns(new List<ContentItem>()
                {
                    new ContentItem
                    {
                        Id = "child1",
                        Type = "Assignment",
                        Categories = new List<TocCategory>
                        {
                            new TocCategory {Id = "syllabusfilter", Sequence = "m"},
                            new TocCategory {Id = "bfw_toc_contents", Sequence = "1"}
                        },
                        Containers = new List<Container>() { new Container("syllabusfilter", "Launchpad") },
                        AssignmentSettings = new AssignmentSettings()
                    },
                    new ContentItem
                    {
                        Id = "child2",
                        Type = "Assignment",
                        Categories = new List<TocCategory>
                        {
                            new TocCategory {Id = "bfw_toc_contents", Sequence = "2"},
                            new TocCategory {Id = "syllabusfilter", Sequence = "t"}
                        },
                        Containers = new List<Container>() { new Container("syllabusfilter", "Launchpad") },
                        AssignmentSettings = new AssignmentSettings()
                    },
                    new ContentItem
                    {
                        Id = parentId,
                        Type = "Folder",
                        Categories = new List<TocCategory>
                        {
                            new TocCategory {Id = "bfw_toc_contents", Sequence = "3"},
                            new TocCategory {Id = "syllabusfilter", Sequence = "j"}
                        },
                        Containers = new List<Container>() { new Container("syllabusfilter", "Launchpad") },
                        AssignmentSettings = new AssignmentSettings()
                    }
                });
            contentActions.ListChildren(courseid, parentId, 1, "syllabusfilter").Returns(new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Id = "itemId",
                        Type = "Assignment",
                        Sequence = "a",
                        Containers = new List<Container>() { new Container("syllabusfilter", "Launchpad") },
                        AssignmentSettings = new AssignmentSettings()
                    }
                });

            var result = contentApi.GetTableofContents(topParentId, courseid, 2);

            Assert.IsTrue(result.Skip(0).First().Sequence == "j");
            Assert.IsTrue(result.Skip(1).First().Sequence == "a");
            Assert.IsTrue(result.Skip(2).First().Sequence == "m");
            Assert.IsTrue(result.Skip(3).First().Sequence == "t");
        }

    }
}
