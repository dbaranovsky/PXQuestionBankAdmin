using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetDueSoonListTest
    {
        private GetDueSoonList items;

        [TestInitialize]
        public void TestInitialize()
        {
            this.items = new GetDueSoonList();
            this.items.SearchParameters = new DueSoonSearch()
            {

            };
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "Invalid parameters for generating search request.")]
        public void Request_Should_Throw_Exception_If_UserId_And_EnrollmentId_Is_Missing()
        {
            items.ToRequest();
        }

        [TestMethod]
        public void Request_DlapRequest_Should_Be_GetRequest()
        {
            items.SearchParameters.EnrollmentId = "1";
            var request = items.ToRequest();

            Assert.AreEqual(request.Type, DlapRequestType.Get);
        }

        [TestMethod]
        public void Request_DlapRequest_Should_Be_By_Enrollment()
        {
            items.SearchParameters.EnrollmentId = "1";
            var request = items.ToRequest();

            Assert.AreEqual(request.Parameters.Contains(new KeyValuePair<string, object>("userid", "1")), false);
            Assert.AreEqual(request.Parameters.Contains(new KeyValuePair<string, object>("enrollmentid", "1")), true);
        }

        [TestMethod]
        public void Request_DlapRequest_Should_Be_By_User()
        {
            items.SearchParameters.UserId = "1";
            var request = items.ToRequest();

            Assert.AreEqual(request.Parameters.Contains(new KeyValuePair<string, object>("userid", "1")), true);
            Assert.AreEqual(request.Parameters.Contains(new KeyValuePair<string, object>("enrollmentid", "1")), false);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "Invalid request.")]
        public void Response_Should_Throw_Exception_If_Response_Code_Is_Not_Ok()
        {
            items.ParseResponse(new DlapResponse() { Code = DlapResponseCode.BadRequest });
        }

        [TestMethod]
        [ExpectedException(typeof(BadDlapResponseException), "Invalid root of response.")]
        public void Response_Should_Throw_Exception_If_Response_Does_Not_Have_Valid_Root()
        {
            var response = new XDocument();
            response.Add(new XElement("root"));

            items.ParseResponse(new DlapResponse() { ResponseXml = response });
        }

        [TestMethod]
        public void Response_Should_Populate_List_Of_Grades()
        {
            var expectedList = GetGrades();

            items.SearchParameters.EnrollmentId = "1";
            var response = new DlapResponse() { ResponseXml = XDocument.Parse(Helper.GetContent(Entity.GetDueSoonList)) };

            items.ParseResponse(response);

            foreach (var expected in expectedList)
            {
                var actual = items.Grades.First(i => i.Item.Id.Equals(expected.Item.Id));
                Assert.IsTrue(ObjectComparer.AreObjectsEqual(expected, actual, new[] { "Data" }));
            }
        }

        private IEnumerable<Grade> GetGrades()
        {
            var result = new List<Grade>();

            var expected = new Grade();
            var type = expected.GetType();

            var expected1 = new Grade()
            {
                Seconds = 43,
                Status = GradeStatus.Completed,
                ScoredDate = DateTime.Parse("2013-09-19T16:47:38.837Z"),
                SubmittedDate = DateTime.Parse("2013-09-19T16:47:38.837Z")
            };
            type.GetProperty("Letter").SetValue(expected1, "A", null);
            type.GetProperty("Achieved").SetValue(expected1, 10, null);
            type.GetProperty("Possible").SetValue(expected1, 10, null);
            type.GetProperty("ScoredVersion").SetValue(expected1, 1, null);
            type.GetProperty("SubmittedVersion").SetValue(expected1, 1, null);
            type.GetProperty("Item").SetValue(expected1, new Item() { Id = "588f3efd2dc142d1a19fd97d704279a0", Title = "Untitled Link", IsGradable = true, DueDate = DateTime.Parse("2013-09-23T03:59:01Z") }, null);
            result.Add(expected1);

            var expected2 = new Grade()
            {
                Seconds = 377,
                Status = GradeStatus.None
            };
            type.GetProperty("Item").SetValue(expected2, new Item() { Id = "bsi__7788BE50__039E__418B__8730__13FEBC053D70", Title = "LearningCurve: Production and Cost", IsGradable = true, DueDate = DateTime.Parse("2013-09-24T03:59:01Z") }, null);
            result.Add(expected2);

            var expected3 = new Grade()
            {

            };
            type.GetProperty("Item").SetValue(expected3, new Item() { Id = "ANGEL_econportal__stoneecon2__master_37D1C2B6E52008A5CC2FCE1D65A70000", Title = "Chapter 07 Homework Quiz", IsGradable = true, DueDate = DateTime.Parse("2013-09-24T03:59:01Z") }, null);
            result.Add(expected3);

            var expected4 = new Grade()
            {

            };
            type.GetProperty("Item").SetValue(expected4, new Item() { Id = "bsi__98FD1B5C__0526__4473__B21F__DF96B72E45F9", Title = "LearningCurve: Supply and Demand", IsGradable = true, DueDate = DateTime.Parse("2013-09-25T03:59:01Z") }, null);
            result.Add(expected4);

            var expected5 = new Grade()
            {
                Seconds = 89,
                Status = GradeStatus.Completed | GradeStatus.ShowScore | GradeStatus.Released,
                ScoredDate = DateTime.Parse("2013-09-19T16:56:04.687Z"),
                SubmittedDate = DateTime.Parse("2013-09-19T16:56:04.607Z")
            };
            type.GetProperty("Letter").SetValue(expected5, "F", null);
            type.GetProperty("Achieved").SetValue(expected5, 0.05, null);
            type.GetProperty("RawAchieved").SetValue(expected5, 1, null);
            type.GetProperty("Possible").SetValue(expected5, 1, null);
            type.GetProperty("RawPossible").SetValue(expected5, 20, null);
            type.GetProperty("ScoredVersion").SetValue(expected5, 3, null);
            type.GetProperty("SubmittedVersion").SetValue(expected5, 3, null);
            type.GetProperty("Attempts").SetValue(expected5, 3, null);
            type.GetProperty("Item").SetValue(expected5, new Item() { Id = "ANGEL_econportal__stoneecon2__master_58D5775BED6FFE88171008BA7E550000", Title = "Chapter 03 Homework Quiz", IsGradable = true, DueDate = DateTime.Parse("2013-09-25T03:59:01Z") }, null);
            result.Add(expected5);

            var expected6 = new Grade()
            {

            };
            type.GetProperty("Item").SetValue(expected6, new Item() { Id = "bsi__76C9438C__60C2__4770__BC9F__6A4E3DD857BD", Title = "LearningCurve: Monopolistic Competition, Oligopoly, and Game Theory", IsGradable = true, DueDate = DateTime.Parse("2013-09-26T03:59:01Z") }, null);
            result.Add(expected6);

            var expected7 = new Grade()
            {

            };
            type.GetProperty("Item").SetValue(expected7, new Item() { Id = "ANGEL_econportal__stoneecon2__master_83E2BCF8848A57299032369B09060000", Title = "Chapter 10 Homework Quiz", IsGradable = true, DueDate = DateTime.Parse("2013-09-26T03:59:01Z") }, null);
            result.Add(expected7);

            return result;
        }
    }
}
