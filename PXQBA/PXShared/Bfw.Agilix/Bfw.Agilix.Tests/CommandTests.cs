using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Commands;

using NUnit.Framework;

namespace Bfw.Agilix.Tests
{
    [TestFixture]
    public class CommandTests
    {
        [Test]
        public void GenerateLoginRequest()
        {
            //arrange
            var cmd = new Login
            {
                Username = "foo@bar.com",
                Password = "123456"
            };

            //act
            var request = cmd.ToRequest();

            //assert
            Assert.AreEqual("text/xml", request.ContentType);
            Assert.IsTrue(request.Attributes.ContainsKey("username"));
            Assert.IsTrue(request.Attributes.ContainsKey("password"));
            Assert.AreEqual("foo@bar.com", request.Attributes["username"]);
            Assert.AreEqual("123456", request.Attributes["password"]);
        }
    }
}
