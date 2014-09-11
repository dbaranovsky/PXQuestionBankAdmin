using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Common.Helpers.Tests
{
    [TestClass]
    public class EnumHelperTest
    {

        [TestMethod]
        public void Equals_QuestionStatus_True()
        {
            var result = EnumHelper.Equals("Deleted", QuestionStatus.Deleted);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_WrongQuestionStatus_False()
        {
            var result = EnumHelper.Equals("Deleted", QuestionStatus.InProgress);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetEnumValues_NotEnums_Exception()
        {
            var values = EnumHelper.GetEnumValues(typeof(object));
        }
    }
}
