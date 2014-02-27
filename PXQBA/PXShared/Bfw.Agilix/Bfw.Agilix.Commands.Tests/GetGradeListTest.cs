using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Agilix.DataContracts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetGradeListTest
    {
        private GetGradeList _command;

        [TestInitialize]
        public void TestInitialize()
        {
            _command = new GetGradeList();
        }

        [TestCategory("GradeList"), TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetGradeListActions_ToRequest_WithNoSearchParams_ThrowsArgsException()
        {
            _command.SearchParameters = new GradeListSearch();
            _command.ToRequest();
        }

        [TestCategory("GradeList"), TestMethod]
        public void GetGradeListActions_ToRequest_WithParams_ShouldMapParamsCorrectToCommand()
        {
            var enrollmentid = "enrollmentid";
            var itemid = "itemid";

            _command.SearchParameters = new GradeListSearch();
            _command.SearchParameters.EnrollmentId = enrollmentid;
            _command.SearchParameters.ItemId = itemid;

            var request = _command.ToRequest();

            Assert.AreEqual(itemid, request.Parameters["itemid"]);
            Assert.AreEqual(enrollmentid, request.Parameters["enrollmentid"]);
        }

        [TestCategory("GradeList"), TestMethod]
        public void GetGradeListActions_ParseResponse_WithInvalidResponse_DoesNothing()
        {
            _command.ParseResponse(new Dlap.DlapResponse());

            //Just make sure no exception is thrown
            Assert.IsTrue(true);
        }

        [TestCategory("GradeList"), TestMethod]
        public void GetGradeListActions_ParseResponse_WithValidResponse_ParsesProperly()
        {
            var status = "status";
            var responseversion = "responseversion";
            var submittedDate = DateTime.Today;
            var seconds = "seconds";
            var submittedversion = "submittedversion";

            XDocument doc =
                XDocument.Parse(
                    "<grade status=\"" + status + "\" seconds=\"" + seconds + "\" responseversion=\"" + responseversion + 
                    "\" submitteddate=\"" + submittedDate.ToString() + "\" submittedversion=\"" + submittedversion + "\"/>");
            _command.ParseResponse(new Dlap.DlapResponse(doc));

            Assert.AreEqual(status, _command.GradeList.Status);
            Assert.AreEqual(responseversion, _command.GradeList.Responseversion);
            Assert.AreEqual(submittedDate, _command.GradeList.SubmittedDate);
            Assert.AreEqual(seconds, _command.GradeList.Seconds);
            Assert.AreEqual(submittedversion, _command.GradeList.Submittedversion);
        }
    }
}
