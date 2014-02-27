using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class GradeActionsTest
    {
        private GradeActions _grades;

        private IBusinessContext _context;
        private ISessionManager _sessionManager;
        private ISession _session; 
        private IDocumentConverter _converter;
        private IContentActions _content;
        private IEnrollmentActions _enrollmentActions;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _sessionManager = Substitute.For<ISessionManager>();
            _session = Substitute.For<ISession>();
            _converter = Substitute.For<IDocumentConverter>();
            _content = Substitute.For<IContentActions>();
            _enrollmentActions = Substitute.For<IEnrollmentActions>();

            _sessionManager.CurrentSession.Returns(_session);

            _grades = new GradeActions(_context, _sessionManager, _converter, _content, _enrollmentActions);
        }

        [TestCategory("GradeActions"), TestMethod]
        public void GetDueSoonItemsWithGrades_Should_Return_List_Of_Grades()
        {
            _sessionManager.CurrentSession.WhenForAnyArgs(s => s.ExecuteAsAdmin(Arg.Any<GetDueSoonList>())).Do(c => c.Arg<GetDueSoonList>().Grades = new List<Grade>() { new Grade() });

            var result = _grades.GetDueSoonItemsWithGrades("1", -240);

            Assert.AreNotEqual(null, result);
        }

        /// <summary>
        /// When item query is less than 1950, expect to use item id in the query
        /// </summary>
        [TestCategory("GradeActions"), TestMethod]
        public void GetGradesByEnrollment_ItemIdQueryIsLessThan1950_ExpectToUseItemIdInQuery()
        {
            IEnumerable<string> itemQuery = null;
            const string itemId = "1234567890";
            List<string> itemIds = new List<string>();
            for (int i = 0; i < 149; i++)
            {
                itemIds.Add(itemId);
            }

            _sessionManager.CurrentSession.WhenForAnyArgs(s => s.Execute(Arg.Any<GetGrades>())).Do(
                c =>
                {
                    itemQuery = c.Arg<GetGrades>().SearchParameters.ItemIds;
                    c.Arg<GetGrades>().ParseResponse(new DlapResponse());
                   
                });

            _grades.GetGradesByEnrollment("testEnrollment", itemIds);

            Assert.IsFalse(itemQuery.IsNullOrEmpty());
            Assert.IsTrue(149 == itemQuery.Count());
        }

        /// <summary>
        /// When item query is 1950, expect to use item id in the query
        /// </summary>
        [TestCategory("GradeActions"), TestMethod]
        public void GetGradesByEnrollment_ItemIdQueryIsEqualTo1950_ExpectToUseItemIdInQuery()
        {
            IEnumerable<string> itemQuery = null;
            const string itemId = "1234567890";
            List<string> itemIds = new List<string>();
            for (int i = 0; i < 150; i++)
            {
                itemIds.Add(itemId);
            }

            _sessionManager.CurrentSession.WhenForAnyArgs(s => s.Execute(Arg.Any<GetGrades>())).Do(
                c =>
                {
                    itemQuery = c.Arg<GetGrades>().SearchParameters.ItemIds;
                    c.Arg<GetGrades>().ParseResponse(new DlapResponse());

                });

            _grades.GetGradesByEnrollment("testEnrollment", itemIds);

            Assert.IsFalse(itemQuery.IsNullOrEmpty());
            Assert.IsTrue(150 == itemQuery.Count());
        }

        /// <summary>
        /// When item query is 1950, expect to use * in the query
        /// </summary>
        [TestCategory("GradeActions"),TestMethod]
        public void GetGradesByEnrollment_ItemIdQueryIsLongerThan1950_ExpectToUseStarInQuery()
        {
            IEnumerable<string> itemQuery = null;
            const string itemId = "1234567890";
            List<string> itemIds = new List<string> { "12345678900"};
            for (int i = 0; i < 149; i++)
            {
                itemIds.Add(itemId);
            }

            _sessionManager.CurrentSession.WhenForAnyArgs(s => s.Execute(Arg.Any<GetGrades>())).Do(
                c =>
                {
                    itemQuery = c.Arg<GetGrades>().SearchParameters.ItemIds;
                    c.Arg<GetGrades>().ParseResponse(new DlapResponse());

                });

            _grades.GetGradesByEnrollment("testEnrollment", itemIds);

            Assert.IsFalse(itemQuery.IsNullOrEmpty());
            Assert.IsTrue(1 == itemQuery.Count());
            Assert.IsTrue("*" == itemQuery.First());
        }

        [TestCategory("GradeActions"), TestMethod]
        public void GetGradeList_WithEmptyEnrollmentId_ReturnsBlankStatus()
        {
            var result = _grades.GetGradeList(string.Empty, "itemId");
            Assert.IsNull(result.Status);
        }

        [TestCategory("GradeActions"), TestMethod]
        public void GetGradeList_WithEmptyItemId_ReturnsBlankStatus()
        {
            var result = _grades.GetGradeList("enrollmentId", string.Empty);
            Assert.IsNull(result.Status);
        }
    }
}
