using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class PutItemsTest
    {
        private PutItems putItems;

        private static List<Item> DUMMY_PARENT_ITEMS =
        new List<Item>
        {
            new Item(){ Id="1", ActualEntityid="something",  Category="parent"},          
            new Item(){ Id="2", ActualEntityid="something",  Category="parent"},           
            new Item(){ Id="3", ActualEntityid="something",  Category="parent"},           
            new Item(){ Id="4", ActualEntityid="something",  Category="parent"} ,          
            new Item(){ Id="5", ActualEntityid="something",  Category="parent"} ,          
            new Item(){ Id="6", ActualEntityid="something",  Category="parent"} ,          
        };

        private List<Item> DUMMY_CHILD_ITEMS =
        new List<Item>
        {
           new Item(){ Id="Child_1", ActualEntityid="something",  Category="child"}          
        };

        [TestInitialize]
        public void TestInitialize()
        {
            this.putItems = new PutItems();            
        }
        

        [TestMethod]
        public void PutItemsTest_Request_DlapRequest_Type_Should_Be_PostRequest()
        {
            putItems.Items.AddRange(DUMMY_PARENT_ITEMS);
            var request = putItems.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void PutItemsTest_Request_DlapRequest_Mode_Should_Be_BatchRequest()
        {
            putItems.Items.AddRange(DUMMY_PARENT_ITEMS);
            var request = putItems.ToRequest();
            Assert.AreEqual(request.Mode, DlapRequestMode.Batch);
        }

        [TestMethod]
        public void PutItemsTest_Request_DlapRequest_Parameters_Should_Have_Command_PutItems()
        {
            putItems.Items.AddRange(DUMMY_PARENT_ITEMS);
            var request = putItems.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "putitems");
        }      

        


        [TestMethod]
        [ExpectedException(typeof(DlapException), "PutItems request failed with response code Error.")]
        public void PutItemsTest_ParseResponse_Should_Throw_Exception_If_Response_Code_Is_Not_OK()
        {
            var dlapResponse = new DlapResponse()
            {
                Code = DlapResponseCode.Error
            };
            putItems.ParseResponse(dlapResponse);            
        }

        [TestMethod]
        public void PutItemsTest_Failures_List_Should_Have_Data_If_Any_Batch_Response_Fails()
        {
            string responseString = "<response code=\"OK\"><responses>" +
                                    "<response code=\"DoesNotExist\" message=\"Entity /109512312328 not found.\">" +
                                    "<detail>GoCourseServer.DataModel.DoesNotExistException:</detail>" +                                   
                                    "</response>  </responses> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            putItems.Items.AddRange(DUMMY_PARENT_ITEMS);   
            putItems.ParseResponse(response);
            Assert.AreEqual(putItems.Failures.Count(), 1);
            
        }
       
    }
}
