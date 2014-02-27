using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.PXPub.Models.Test
{
    [TestClass]
    public class CourseHeaderTest
    {
        [TestMethod]
        public void CourseHeaderTest_Displayed_InstructorName_Should_Be_None_For_Generic_Courses()
        {
            var ch = new CourseHeader {CourseSubType="generic",InstructorName = "test"};
            Assert.AreEqual("None", ch.DisplayedInstructorName);
        }

        [TestMethod]
        public void CourseHeaderTest_Displayed_InstructorName_Should_Not_Be_None_For_Non_Generic_Courses()
        {
            var ch = new CourseHeader { CourseSubType = "abc", InstructorName = "test" };
            Assert.AreNotSame("None", ch.DisplayedInstructorName);
        }

        [TestMethod]
        public void CourseHeaderTest_Displayed_InstructorName_Should_Be_Blank_If_Instructor_Name_Is_Null_For_Non_Generic_Courses()
        {
            var ch = new CourseHeader { CourseSubType = "abc", InstructorName = null };
            Assert.AreEqual("", ch.DisplayedInstructorName);
        }
    }
}
