using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetRawItemTests
    {
        private GetRawItem _getRawItem;

        [TestInitialize]
        public void TestInitialize()
        {
            this._getRawItem = new GetRawItem { ItemId = "something", EntityId = "something" };
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "Invalid parameters for generating item search request.")]
        public void GetRawItemTest_Should_ThrowException_If_EntityId_IsNull()
        {
            _getRawItem.EntityId = null;
            _getRawItem.ToRequest();
        }

        [TestMethod]
        public void GetRawItemTest_DlapRequest_Should_Be_PostRequest()
        {
            var request = _getRawItem.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void GetRawItemTest_Parse_Response_NotNull()
        {
            var response = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = new XDocument() };
            _getRawItem.ParseResponse(response);
            Assert.IsNotNull(_getRawItem.ItemDocument);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "Invalid parameters for generating item search request.")]
        public void GetRawItemTest_Parse_Response_With_Exception()
        {
            var response = new DlapResponse { Code = DlapResponseCode.AccessDenied, ResponseXml = new XDocument() };
            _getRawItem.ParseResponse(response);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._getRawItem = null;
        }
    }
}
