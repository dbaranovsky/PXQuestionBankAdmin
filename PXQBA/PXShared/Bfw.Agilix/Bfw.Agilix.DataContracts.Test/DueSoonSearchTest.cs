using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.DataContracts.Tests
{
    [TestClass]    
    public class DueSoonSearchTest
    {
        [TestMethod]
        public void DueSoonSearch_Should_Be_Initilaized()
        {
            var instance = new DueSoonSearch();

            Assert.AreEqual(false, instance.ShowCompleted);
            Assert.AreEqual(false, instance.ShowPastDue);
            Assert.AreEqual(14, instance.Days);            
        }
    }
}
