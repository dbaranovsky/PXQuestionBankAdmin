using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class AssignmentCenterItemMapperTest
    {
        [TestCategory("Mapping"), TestMethod]
        public void AssignmentCenterItemMapperTest_ModelItemToBizItem_SetsContainerProperly()
        {
            var toc1 = "syllabusfilter";
            var container1 = "foo";
            var toc2 = "assignmentfilter";
            var container2 = "bar;";

            AssignmentCenterItem item = new AssignmentCenterItem()
            {
                Containers = new List<Container>()
                {
                    new Container(toc1, container1),
                    new Container(toc2, container2)
                }
            };

            var bizitem = item.ToAssignmentCenterItem();
            Assert.AreEqual(2, bizitem.Containers.Count);
            Assert.IsTrue(bizitem.Containers.Any(c => c.Toc.Equals(toc1)));
            Assert.IsTrue(bizitem.Containers.Any(c => c.Toc.Equals(toc2)));
            Assert.AreEqual(container1, bizitem.Containers.FirstOrDefault(c => c.Toc.Equals(toc1)).Value);
            Assert.AreEqual(container2, bizitem.Containers.FirstOrDefault(c => c.Toc.Equals(toc2)).Value);
        }

        [TestCategory("Mapping"), TestMethod]
        public void AssignmentCenterItemMapperTest_ModelItemToBizItem_SetsSubContainerProperly()
        {
            var toc1 = "syllabusfilter";
            var subcontainer1 = "foo";
            var toc2 = "assignmentfilter";
            var subcontainer2 = "bar;";

            AssignmentCenterItem item = new AssignmentCenterItem()
            {
               SubContainerIds = new List<Container>()
                {
                    new Container(toc1, subcontainer1),
                    new Container(toc2, subcontainer2)
                }
            };

            var bizitem = item.ToAssignmentCenterItem();
            Assert.AreEqual(2, bizitem.SubContainerIds.Count);
            Assert.IsTrue(bizitem.SubContainerIds.Any(c => c.Toc.Equals(toc1)));
            Assert.IsTrue(bizitem.SubContainerIds.Any(c => c.Toc.Equals(toc2)));
            Assert.AreEqual(subcontainer1, bizitem.SubContainerIds.FirstOrDefault(c => c.Toc.Equals(toc1)).Value);
            Assert.AreEqual(subcontainer2, bizitem.SubContainerIds.FirstOrDefault(c => c.Toc.Equals(toc2)).Value);
        }
    }
}
