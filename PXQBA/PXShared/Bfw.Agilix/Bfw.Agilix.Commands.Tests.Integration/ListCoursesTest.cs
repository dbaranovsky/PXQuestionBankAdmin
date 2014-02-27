using System.Linq;
using Bfw.PX.PXPub.Components.Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands.Tests.Integration
{
    [TestClass]
    public class ListCoursesTest
    {
        #region TestInitialize
        [TestInitialize]
        public void TestInitialize()
        {

        }
        #endregion TestInitialize


        /// <summary>
        /// Expect at least one course is returned
        /// </summary>
        [TestCategory("ListCoursesIntergationTest"), TestMethod]
        public void ListCoursesIntergationTest_ListCourses_ExpectResultBack()
        {
            var command = new ListCourses
            {
                SearchParameters = new CourseSearch
                {
                    DomainId = "0",
                    Query = "/meta-bfw_course_type='Eportfolio'",
                    Limit = 0
                }
            };
            DlapConnectionHelper.ExecuteDlapCommand(command);

            Assert.IsTrue(command.Courses.Count() > 1);

        }

        /// <summary>
        /// All courses returned should be in domain 8841
        /// </summary>
        [TestCategory("ListCoursesIntergationTest"), TestMethod]
        public void ListCoursesIntergationTest_ListCourses_ExpectAllCoursesAreInDomain8841()
        {
            var command = new ListCourses
            {
                SearchParameters = new CourseSearch
                {
                    DomainId = "8841",
                    Query = "/meta-bfw_course_type='Eportfolio'",
                    Limit = 0
                }
            };
            DlapConnectionHelper.ExecuteDlapCommand(command);

            Assert.IsTrue(command.Courses.Count() > 1);

            Assert.IsFalse(command.Courses.Any(c => c.Domain.Id != "8841"));
        }

        /// <summary>
        /// This should return 10 courses back
        /// </summary>
        [TestCategory("ListCoursesIntergationTest"), TestMethod]
        public void ListCoursesIntergationTest_ListCourses_ExpectTenCoursesBack()
        {
            const int expectedCoursesNumber = 10;
            var command = new ListCourses
            {
                SearchParameters = new CourseSearch
                {
                    DomainId = "0",
                    Query = "/meta-bfw_course_type='Eportfolio'",
                    Limit = expectedCoursesNumber
                }
            };
            DlapConnectionHelper.ExecuteDlapCommand(command);

            Assert.AreEqual(expectedCoursesNumber, command.Courses.Count());

        }
    }
}
