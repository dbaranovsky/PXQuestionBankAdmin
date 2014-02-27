using System;
using System.Xml.Linq;
using DlapShip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DlapShipTest
{
    [TestClass]
    public class DeleteDlapItem
    {
        private const string DeleteDlapItemFile1 = "../../../TestScripts/DeleteDlapItem1.xml";
        private const string BadDeleteDlapItemFile1 = "../../../TestScripts/BadDeleteDlapItem1.xml";

        [TestMethod]
        public void DeleteAction_IfValidFile_ParsedProperly()
        {
            PrivateType pt = new PrivateType(typeof(Program));
            Program.DeleteItemsFilePath = DeleteDlapItemFile1;
            bool retval = (bool)pt.InvokeStatic("TryParseDeleteDlapItemsFile", new object[] { null });

            Assert.IsTrue(retval);
        }

        [TestMethod]
        public void DeleteAction_IfInvalidFile_ParseFails()
        {
            PrivateType pt = new PrivateType(typeof(Program));
            Program.DeleteItemsFilePath = BadDeleteDlapItemFile1;
            bool retval = (bool)pt.InvokeStatic("TryParseDeleteDlapItemsFile", new object[] { null });

            Assert.IsFalse(retval);
        }
    }
}
