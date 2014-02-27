using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class UpdateCoursesTest
    {
        private UpdateCourses courses;

        [TestInitialize]
        public void TestInitialize()
        {
            this.courses = new UpdateCourses();
        }

        [TestMethod]
        public void CourseList_Should_Accept_Single_Course()
        {
            this.courses.Add(new Course());

            Assert.IsTrue(this.courses.Courses.Count > 0);
        }

        [TestMethod]
        public void CourseList_Should_Accept_List_Of_Courses()
        {
            var courseList = new List<Course>();
            courseList.Add(new Course());
            courseList.Add(new Course());

            this.courses.Add(courseList);

            Assert.IsTrue(this.courses.Courses.Count == 2);
        }

        [TestMethod]
        public void CourseList_Should_Be_Empty()
        {
            this.courses.Add(new Course());
            this.courses.Clear();

            Assert.IsTrue(this.courses.Courses.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Response_Should_Throw_Exception_If_Response_Not_OK()
        {
            this.courses.ParseResponse(new DlapResponse() { Code = DlapResponseCode.Error });
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Request_Should_Throw_Exception_If_CourseList_Is_Empty()
        {
            this.courses.ToRequest();
        }

        [TestMethod]
        public void Request_Should_Be_Initialized()
        {
            var course = new Course();
            course.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
            this.courses.Add(course);

            var request = this.courses.ToRequest();

            Assert.AreEqual("updatecourses", request.Parameters["cmd"]);
        }
    }
}
