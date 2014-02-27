using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetItemsTest
    {
        private GetItems items;

        [TestInitialize]
        public void TestInitialize()
        {
            this.items = new GetItems();
            this.items.SearchParameters = new ItemSearch()
            {
                ItemId = "something",
                EntityId = "something"
            };
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "Invalid parameters for generating item search request.")]
        public void Request_Should_Throw_Exception_If_EntityId_Is_Null()
        {
            items.SearchParameters.EntityId = null;
            
            items.ToRequest();
        }

        [TestMethod]
        public void Request_DlapRequest_Should_Be_GetRequest()
        {
            var request = items.ToRequest();

            Assert.AreEqual(request.Type, DlapRequestType.Get);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "Response is not OK.")]
        public void Response_Should_Throw_Exception_If_Not_OK()
        {
            DlapResponse response = new DlapResponse() { Code = DlapResponseCode.Error };
            
            items.ParseResponse(response);
        }

        [TestMethod]
        public void Request_Should_Be_GetItemList_If_Query_Not_Empty()
        {
            items.SearchParameters.Query = "something";
            
            var request = items.ToRequest();

            Assert.AreEqual("getitemlist", request.Parameters["cmd"]);
        }

        [TestMethod]
        public void Request_Should_Be_GetItemList_If_SearchDepth_Not_0()
        {
            items.SearchParameters.Query = string.Empty;
            items.SearchParameters.Depth = 1;

            var request = items.ToRequest();
            
            Assert.AreEqual("getitemlist", request.Parameters["cmd"]);
        }

        [TestMethod]
        public void Request_Should_Be_GetManifestItem_If_AncestorId_Not_Empty()
        {
            items.SearchParameters.Query = string.Empty;
            items.SearchParameters.AncestorId = "1";

            var request = items.ToRequest();
            
            Assert.AreEqual("getmanifestitem", request.Parameters["cmd"]);
        }

        [TestMethod]
        [ExpectedException(typeof(BadDlapResponseException), "Invalid root element of response.")]
        public void Response_Should_Throw_Exception_If_Root_Is_Invalid()
        {            
            items.SearchParameters.Type = DlapItemType.Custom;
            var xml = new System.Xml.Linq.XDocument();
            xml.Add(new XElement("something"));

            items.ParseResponse(new DlapResponse() { ResponseXml = xml });
        }

        [TestMethod]
        public void Response_Should_Be_Single_Item()
        {
            items.SearchParameters.Type = DlapItemType.CustomActivity;
            var xml = Helper.GetResponse(Entity.Item, "GenericItem");
            xml.Root.Attribute("id").Value = "something";

            items.ParseResponse(new DlapResponse() { ResponseXml = xml });

            Assert.AreEqual(1, items.Items.Count);
        }

        [TestMethod]
        public void Response_Should_Be_List_Of_Items()
        {
            items.SearchParameters.Type = DlapItemType.CustomActivity;
            var xmlGeneric = Helper.GetResponse(Entity.Item, "GenericItem");
            var xmlNotGeneric = Helper.GetResponse(Entity.Item, "NotGenericItem");

            var xml = new XDocument(new XElement("items"));
            xml.Element("items").Add(xmlGeneric.Elements());
            xml.Element("items").Add(xmlNotGeneric.Elements());

            items.ParseResponse(new DlapResponse() { ResponseXml = xml });

            Assert.AreEqual(2, items.Items.Count);
        }
    }
}
