using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using TestHelper;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class CopyCoursesTest
    {
        private CopyCourses courses;

        [TestInitialize]
        public void TestInitialize()
        {
            this.courses = new CopyCourses();
        }

        [TestMethod]
        public void CopyCourses_Accept_Single_Course()
        {
            this.courses.Add(new Course());

            Assert.IsTrue(this.courses.Courses.Count > 0);
        }

        [TestMethod]
        public void CopyCourses_Accept_List_Of_Courses()
        {
            var courseList = new List<Course>();
            courseList.Add(new Course());
            courseList.Add(new Course());

            this.courses.Add(courseList);

            Assert.IsTrue(this.courses.Courses.Count == 2);
        }

        [TestMethod]
        public void CopyCourses_Remove_All_Courses_From_List()
        {
            this.courses.Add(new Course());
            this.courses.Clear();

            Assert.IsTrue(this.courses.Courses.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CopyCourses_Raise_Exception_When_DLAP_Return_Error()
        {
            this.courses.ParseResponse(new DlapResponse() { Code = DlapResponseCode.Error });
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CopyCourses_Raise_Exception_When_CourseList_Empty()
        {
            this.courses.ToRequest();
        }

        [TestMethod]
        public void CopyCourses_Initialize_Request()
        {
            var course = new Course();
            course.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
            this.courses.Add(course);
            var request = this.courses.ToRequest();
            Assert.AreEqual("copycourses", request.Parameters["cmd"]);
        }
    }
}
