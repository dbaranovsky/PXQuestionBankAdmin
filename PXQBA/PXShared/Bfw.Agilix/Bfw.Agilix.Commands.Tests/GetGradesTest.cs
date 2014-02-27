using System;
using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetGradesTest
    {
        private GetGrades grades;

        [TestInitialize]
        public void TestInitialize()
        {
            this.grades = new GetGrades();
            this.grades.SearchParameters = new GradeSearch
            {
                EnrollmentId = "something",
                UserId = "something",
                EntityId = "something"
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Request_Should_Throw_Exception_If_No_Params()
        {
            this.grades.SearchParameters = new GradeSearch();

            this.grades.ToRequest();
        }

        [TestMethod]
        public void Request_Should_Be_For_All_Items()
        {
            DlapRequest request = this.grades.ToRequest();

            Assert.AreEqual("*", request.Parameters["itemid"] = "*");
        }

        [TestMethod]
        public void Request_Should_Be_For_List_Of_Items()
        {
            this.grades.SearchParameters.EnrollmentId = null;
            this.grades.SearchParameters.UserId = null;
            var ids = new string[] { "1", "2" };            
            this.grades.SearchParameters.ItemIds = ids;

            var request = this.grades.ToRequest();

            Assert.AreEqual("1|2", request.Parameters["itemid"]);
        }

        [TestMethod]
        public void Request_Should_Have_EntityId()
        {
            this.grades.SearchParameters.EnrollmentId = null;

            var request = this.grades.ToRequest();

            Assert.AreEqual(this.grades.SearchParameters.EntityId, request.Parameters["entityid"]);
        }

        [TestMethod]
        public void Response_For_GetEnrollmentGradebook2_Should_Be_Single_Enrollment()
        {
            string command = "GetEnrollmentGradebook2";
            this.grades.SearchParameters.CommandRequested = command;
            DlapResponse response = new DlapResponse();
            response.ResponseXml = Helper.GetResponse(Entity.Enrollment, command);

            this.grades.ParseResponse(response);

            Assert.AreEqual(1, (this.grades.Enrollments as List<Enrollment>).Count);
        }

        [TestMethod]
        public void Response_For_GetUserGradebook2_Should_Be_Single_Enrollment()
        {
            string command = "GetUserGradebook2";
            this.grades.SearchParameters.EnrollmentId = null;
            this.grades.SearchParameters.CommandRequested = command;
            DlapResponse response = new DlapResponse();
            response.ResponseXml = Helper.GetResponse(Entity.Enrollment, command);

            this.grades.ParseResponse(response);

            Assert.AreEqual(1, (this.grades.Enrollments as List<Enrollment>).Count);
        }
    }
}
