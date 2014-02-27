using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Bfw.Agilix.Dlap;
    using Bfw.Common.Collections;

    [TestClass]
    public class GetResourceListTests
    {
        private const string TestString = "This is a test data.";

        private const string ResponseString = @"<response code=""OK""><resources><resource entityid=""4378"" path=""imsmanifest.xml"" version=""1"" size=""4430"" flags=""2"" creationdate=""2008-04-10T06:52:22.587Z"" modifieddate=""2008-04-20T06:52:22.587Z""/><resource entityid=""4378"" path=""Templates/Data/HTMLDoc/Text.htm"" version=""1"" size=""56"" flags=""2"" creationdate=""2008-04-20T06:52:23.26Z"" modifieddate=""2008-04-20T06:52:23.26Z""/></resources></response>";

        private GetResourceList _getResource;

        private XDocument _responsElement;

        [TestInitialize]
        public void TestInitialize()
        {
            this._getResource = new GetResourceList { EntityId = "entityid", ResourcePath = "http://www.whfreeman.com/resourceuri" };
            this._responsElement = XDocument.Parse(ResponseString);
        }

        [TestMethod]
        public void GetResourceListCommand_Type_Is_Get()
        {
            var request = _getResource.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Get);
        }

        [TestMethod]
        public void GetResourceListCommand_WithQuery_Is_GetAndWithQuery()
        {
            _getResource.Query = "PX/academicterms.xml";
            var request = _getResource.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Get);
            Assert.IsNotNull(request.BuildQuery());
        }

        [TestMethod]
        public void GetResourceListCommand_Parse_With_Some_ResponseXml()
        {
            var response = new DlapResponse(_responsElement);
            _getResource.ParseResponse(response);
            Assert.IsNotNull(_getResource.Resources);
        }

        [TestMethod]
        public void GetResourceListCommand_No_Parse_With_Null_ResponseXml()
        {
            var response = new DlapResponse { Code = DlapResponseCode.OK };
            _getResource.ParseResponse(response);
            Assert.IsNull(_getResource.Resources);
        }

        [TestMethod]
        public void GetResourceListCommand_With_Resources()
        {
            var response = new DlapResponse(_responsElement);
            _getResource.ParseResponse(response);
            if (!_getResource.Resources.IsNullOrEmpty())
            {
                var resourceInfo = _getResource.Resources.OrderByDescending(r => r.ModifiedDate).First();
                Assert.AreEqual(resourceInfo.ModifiedDate, DateTime.Parse("2008-04-20T06:52:23.26Z"));
            }
            foreach (var resource in _getResource.Resources)
            {
                Assert.IsNotNull(resource.EntityId);
            }
        }
        
        [TestCleanup]
        public void TestCleanup()
        {
            this._getResource = null;
            this._responsElement = null;
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
