using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.Biz.DataContracts.Test
{
    [TestClass]
    public class CategoryGradeTest
    {
        [TestMethod]
        public void CategoryGrade_Should_Set_Get_Properties()
        {
            CategoryGrade grade = new CategoryGrade()
            {
                Id = "1",
                Name = "name",
                Achieved = 10,
                Possible = 20,
                Letter = "C"
            };

            Assert.AreEqual("1", grade.Id);
            Assert.AreEqual("name", grade.Name);
            Assert.AreEqual(10, grade.Achieved);
            Assert.AreEqual(20, grade.Possible);
            Assert.AreEqual("C", grade.Letter);
        }
    }
}
