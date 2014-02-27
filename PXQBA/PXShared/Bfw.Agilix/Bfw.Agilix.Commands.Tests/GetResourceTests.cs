namespace Bfw.Agilix.Commands.Tests
{
    using System.IO;
    using System.Xml.Linq;
    using Bfw.Agilix.Dlap;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestHelper;

    [TestClass]
    public class GetResourceTests
    {
        private GetResource _getResource;

        [TestInitialize]
        public void TestInitialize()
        {
            this._getResource = new GetResource { EntityId = "entityid", MetadataOnly = true, ResourcePath = "http://www.whfreeman.com/resourceuri" };
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetResourceCommand_Type_Is_Get()
        {
            var request = _getResource.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Get);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetResourceCommand_No_Parse_With_Some_ResponseXml()
        {
            var response = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = new XDocument() };
            _getResource.ParseResponse(response);
            Assert.IsNull(_getResource.Resource);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetResourceCommand_Parse_With_Null_ResponseXml()
        {
            var response = new DlapResponse { Code = DlapResponseCode.OK };
            _getResource.ParseResponse(response);
            Assert.IsNotNull(_getResource.Resource);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetResourceCommand_With_Memory_StreamResponse()
        {
            const string testString = "This is a test data.";
            string finalResult;
            var byteArray = Helper.GetBytes(testString);
            var response = new DlapResponse { Code = DlapResponseCode.OK, ResponseStream = new MemoryStream(byteArray) };
            _getResource.ParseResponse(response);

            using (var result = _getResource.Resource.GetStream())
            {
                var byteArrayResult = new byte[result.Length];
                result.Read(byteArrayResult, 0, 40);
                finalResult = Helper.GetString(byteArrayResult);
            }

            Assert.AreEqual(testString, finalResult);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._getResource = null;
        }
    }
}
