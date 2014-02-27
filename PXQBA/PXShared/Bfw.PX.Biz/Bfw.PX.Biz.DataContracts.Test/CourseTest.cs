using Bfw.PX.Biz.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Px.Biz.DataContracts.Test
{
    [TestClass]
    public class CourseTest
    {
        [TestMethod, TestCategory("Course")]
        public void CourseTest_CheckIfLmsId_Required_Exists()
        {
            var oCourse = new Course() {Id = "3334", LmsIdRequired = true, Title = "New Course"};
            Assert.AreEqual(true, oCourse.LmsIdRequired);
        }

        [TestMethod, TestCategory("Course")]
        public void CourseTest_CheckIfLmsLabel_Exists()
        {
            var oCourse = new Course() { Id = "3794", LmsIdLabel = "LMS Id", Title = "New Course" };
            Assert.AreEqual("LMS Id", oCourse.LmsIdLabel);
        }

        [TestMethod, TestCategory("Course")]
        public void CourseTest_CheckIfLmsIdPrompt_Exists()
        {
            var oCourse = new Course() { Id = "3254", LmsIdPrompt = "Please enter LMS Id", Title = "New Course" };
            Assert.AreEqual("Please enter LMS Id", oCourse.LmsIdPrompt);
        }

    }
}
