using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Bfw.Agilix.DataContracts.Tests
{
    [TestClass]
    public class SignalTest
    {
        [TestMethod]
        public void ParseEntity_Should_Parse_Response()
        {
            var signal = new Signal();
            var response = XElement.Parse(Helper.GetContent(Entity.Signal));

            signal.ParseEntity(response);

            Assert.AreEqual("525863068", signal.SignalId);
            Assert.AreEqual("66159", signal.DomainId);
            Assert.AreEqual("1.2", signal.Type);
            Assert.AreEqual("163426", signal.EntityId);
            Assert.AreEqual(DateTime.Parse("2013-12-31T19:24:46.623Z"), signal.CreationDate);
            Assert.AreEqual("116589", signal.CreationBy);
            Assert.AreEqual(EnrollmentStatus.Active, signal.OldStatus);
            Assert.AreEqual(EnrollmentStatus.Withdrawn, signal.NewStatus);
        }
    }
}
