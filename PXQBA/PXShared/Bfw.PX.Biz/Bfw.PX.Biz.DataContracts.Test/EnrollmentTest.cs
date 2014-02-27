using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.Biz.DataContracts.Test
{
    [TestClass]
    public class EnrollmentTest
    {
        [TestMethod]
        public void Enrollment_Should_Set_Get_OverallGrade()
        {
            var enrollment = new Enrollment()
            {
                OverallAchieved = 5,
                OverallPossible = 10,
                OverallGrade = "50%",
            };

            Assert.AreEqual(5, enrollment.OverallAchieved);
            Assert.AreEqual(10, enrollment.OverallPossible);
            Assert.AreEqual("50%", enrollment.OverallGrade);
        }

        [TestMethod]
        public void Enrollment_Should_Set_Get_CategoryGrades()
        {
            var enrollment = new Enrollment()
            {
                CategoryGrades = new List<CategoryGrade>() 
                { 
                    new CategoryGrade()
                    {
                        Id = "1",
                        Name = "category"
                    }
                }
            };

            Assert.AreEqual("1", enrollment.CategoryGrades.ToList().First().Id);
            Assert.AreEqual("category", enrollment.CategoryGrades.ToList().First().Name);
        }
    }
}
