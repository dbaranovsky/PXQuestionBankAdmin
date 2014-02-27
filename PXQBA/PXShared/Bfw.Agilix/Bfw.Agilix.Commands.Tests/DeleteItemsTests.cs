using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    /// <summary>
    /// Summary description for DeleteItemsTests
    /// </summary>
    [TestClass]
    public class DeleteItemsTests
    {
        public DeleteItemsTests()
        {
            
        }

        [TestCategory("DeleteItems"), TestMethod]
        [ExpectedException(typeof(DlapException), "Cannot create a DeleteItems request if there are not items in the Items collection.")]
        public void DeleteItems_HasNoItem_RequestExpectException()
        {
            DeleteItems deleteItems = new DeleteItems();
            deleteItems.ToRequest();
            
            
        }

        [TestCategory("DeleteItems"), TestMethod]
        public void DeleteItems_HasItem_RequestOk()
        {
            var itemList = new List<Item> { new Item {Id="test1"}};
            DeleteItems deleteItems = new DeleteItems { Items = itemList };
            DlapRequest request = deleteItems.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "deleteitems");
        }

        [TestCategory("DeleteItems"), TestMethod]
        public void DeleteItems_ShouldNot_Throw_Exception_WithDropBoxType_AndNullDataObject()
        {            
            var itemList = new List<Item> { new Item { Id = "test1" } };
            itemList.First().DropBoxType = DropBoxType.Url;
            itemList.First().Data = null;
            DeleteItems deleteItems = new DeleteItems { Items = itemList };
            DlapRequest request = deleteItems.ToRequest();                
        }

        [TestCategory("DeleteItems"), TestMethod]
        public void DeleteItems_ItemExistCourseExist_ResponseOk()
        {
            string responseString = "<response code=\"OK\"><responses><response code=\"OK\" /></responses></response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));

            DeleteItems deleteItems = new DeleteItems();
            deleteItems.ParseResponse(response);
            Assert.AreEqual(deleteItems.Failures.Count(), 0);

        }

        [TestCategory("DeleteItems"), TestMethod]
        public void DeleteItems_ItemNotExistCourseNotExist_ResponseError()
        {

            string responseString = "<response code=\"OK\"><responses>" +
                                    "<response code=\"DoesNotExist\" message=\"Entity /109512312328 not found.\">" +
                                    "<detail>GoCourseServer.DataModel.DoesNotExistException: Entity /109512312328 not found.    at GoCourseServer.EntityRef.GetEntityType(DlapContext context, Int64 entityId)    at GoCourseServer.EntityRef.get_EntityType()    " +
                                    "at GoCourseServer.ManifestRequestHandler.DeleteItems(DlapContext context) agent='Firefox21.0' url='http://dev.dlap.bfwpub.com/Dlap.ashx?deleteitems' site='dlap' method='POST' referer='http://dev.dlap.bfwpub.com/CallMethod.aspx' process='2284' userid='7'" +
                                    "</detail>     </response>  </responses> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            var itemList = new List<Item> { new Item { Id = "DSDFsafdasfasdfsdfasdfklasdfkasasdfasdfasdfj", EntityId = "109512312328" } };
            DeleteItems deleteItems = new DeleteItems { Items = itemList };
            deleteItems.ParseResponse(response);
            Assert.AreEqual(deleteItems.Failures.Count(), 1);
        }

        [TestCategory("DeleteItems"), TestMethod]
        [ExpectedException(typeof(DlapException), "DeleteItems request failed with response code 'NoAuthentication'.")]
        public void DeleteItems_NoAuthentication_ResponseError()
        {

            string responseString = "<response code=\"NoAuthentication\" message=\"No Authentication: action='deleteitems'\">" +
                                    "<detail>GoCourseServer.NoAuthenticationException: No Authentication: action='deleteitems'    at GoCourseServer.DlapContext.RequireAuthentication()    at GoCourseServer.Dlap.DlapCommand.Invoke(DlapContext dlapContext)    at GoCourseServer.Dlap.ProcessRequest(HttpContext context) agent='Firefox21.0' url='http://dev.dlap.bfwpub.com/Dlap.ashx?deleteitems' site='dlap' method='POST' referer='http://dev.dlap.bfwpub.com/CallMethod.aspx' process='2284'"+
                                    "</detail> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            var itemList = new List<Item> { new Item { Id = "DSDFsafdasfasdfsdfasdfklasdfkasasdfasdfasdfj", EntityId = "109512312328" } };
            DeleteItems deleteItems = new DeleteItems { Items = itemList };
            deleteItems.ParseResponse(response);
            Assert.AreEqual(deleteItems.Failures.Count(), 1);
        }
    }
}
