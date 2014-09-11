
using System;
using System.Collections.Generic;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Common.Helpers.Tests
{
    [TestClass]
    public class EnumerableHelperTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsCollectionEqual_NoParameters_Exception()
        {
            EnumerableHelper.IsCollectionEqual<object>(null, null);
        }


        [TestMethod]
        public void IsCollectionEqual_TheSameCollection_True()
        {
            var objects = new List<object>()
                                   {
                                       new object(),
                                       new object()
                                   };
            var result = objects.IsCollectionEqual(objects);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsCollectionEqual_DifferentCollection_False()
        {
            var objects1 = new List<object>()
                                   {
                                       new object(),
                                       new object()
                                   };

            var objects2 = new List<object>()
                                   {
                                       new object(),
                                       new object()
                                   };

            var result = objects1.IsCollectionEqual(objects2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsCollectionEqual_DifferentLengthCollection_False()
        {
            var objects1 = new List<object>()
                                   {
                                       new object(),
                                       new object()
                                   };

            var objects2 = new List<object>()
                                   {
                                       new object(),
                                       new object(),
                                       new object()
                                   };

            var result = objects1.IsCollectionEqual(objects2);

            Assert.IsFalse(result);
        }
    }
}
