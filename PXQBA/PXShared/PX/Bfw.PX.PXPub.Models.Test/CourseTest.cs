using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.PXPub.Models.Test
{
    [TestClass]
    public class CourseTest
    {
        [TestMethod]
        public void CourseTest_IfLmsId_Required_Exists()
        {
            var course = new Bfw.PX.PXPub.Models.Course()
            {
                Id = "3445",
                Title = "New Course",
                LmsIdRequired = true
            };

            Assert.AreEqual(true, course.LmsIdRequired);
        }

        [TestMethod]
        public void CourseTest_IfLmsId_Label_Exists()
        {
            var course = new Bfw.PX.PXPub.Models.Course()
            {
                Id = "35585",
                Title = "Test Course",
                LmsIdLabel = "LMS Id"
            };

            Assert.AreEqual("LMS Id", course.LmsIdLabel);            
        }

        [TestMethod]
        public void CourseTest_IfLmsId_Prompt_Exists()
        {
            var course = new Bfw.PX.PXPub.Models.Course()
            {
                Id = "3445",
                Title = "New Course",
                LmsIdPrompt  = "Please enter LMS Id"
            };

            Assert.AreEqual("Please enter LMS Id", course.LmsIdPrompt);            
        }
    }
}
