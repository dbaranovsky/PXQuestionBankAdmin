using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using NSubstitute;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

using Bfw.PX.Biz.ServiceContracts;

using Bfw.PXWebAPI.Helpers;

namespace Bfw.PXWebAPI.Tests
{
    [TestFixture]
    public class IApiContentActionsTests
    {
        [Test]
        public void ExpectListOfContentToSync()
        {
            // arrange
            var sessionManager = Substitute.For<ISessionManager>();
            var businessContext = Substitute.For<IBusinessContext>();
            IApiContentActions contentApi = new ApiContentActions(sessionManager, businessContext);
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

        [Test]
        public void ExpectEmptyListOfContentToSync()
        {
            // arrange
            var sessionManager = Substitute.For<ISessionManager>();
            var businessContext = Substitute.For<IBusinessContext>();
            IApiContentActions contentApi = new ApiContentActions(sessionManager, businessContext);
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

        [Test]
        public void ExpectErrorDueToCourseIdNotExisting()
        {
            // arrange
            var sessionManager = Substitute.For<ISessionManager>();
            var businessContext = Substitute.For<IBusinessContext>();
            IApiContentActions contentApi = new ApiContentActions(sessionManager, businessContext);
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

        [Test]
        public void ExpectErrorDueToMissingCreationDate()
        {
            // arrange
            var sessionManager = Substitute.For<ISessionManager>();
            var businessContext = Substitute.For<IBusinessContext>();
            IApiContentActions contentApi = new ApiContentActions(sessionManager, businessContext);
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

        [Test]
        public void ExpectErrorDueToMissingModifiedDate()
        {
            // arrange
            var sessionManager = Substitute.For<ISessionManager>();
            var businessContext = Substitute.For<IBusinessContext>();
            IApiContentActions contentApi = new ApiContentActions(sessionManager, businessContext);
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

        [Test]
        public void ExpectModifiedStatus()
        {
            // arrange
            var sessionManager = Substitute.For<ISessionManager>();
            var businessContext = Substitute.For<IBusinessContext>();
            IApiContentActions contentApi = new ApiContentActions(sessionManager, businessContext);
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

        [Test]
        public void ExpectCreatedStatus()
        {
            // arrange
            var sessionManager = Substitute.For<ISessionManager>();
            var businessContext = Substitute.For<IBusinessContext>();
            IApiContentActions contentApi = new ApiContentActions(sessionManager, businessContext);
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

        [Test]
        public void ExpectDeletedStatus()
        {
            // arrange
            var sessionManager = Substitute.For<ISessionManager>();
            var businessContext = Substitute.For<IBusinessContext>();
            IApiContentActions contentApi = new ApiContentActions(sessionManager, businessContext);
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
    }
}
