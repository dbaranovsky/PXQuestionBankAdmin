using System.ComponentModel.Design.Serialization;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetEnrollmentAcitvityTest
    {
        private GetEnrollmentActivity _getEnrollmentActivity;

        [TestInitialize]
        public void TestInitialize()
        {
            _getEnrollmentActivity = new GetEnrollmentActivity();
            _getEnrollmentActivity.SearchParameter = new EnrollmentActivitySearch();
        }

        [TestMethod]
        public void Verify_DlapRequest_Type_Is_A_Get_Request()
        {
            //arrange
            _getEnrollmentActivity.SearchParameter.EnrollmentId = "11111";
            
            //act
            DlapRequest request = _getEnrollmentActivity.ToRequest();
            
            //assert
            Assert.AreEqual(DlapRequestType.Get, request.Type);
        }

        [TestMethod]
        public void Verify_DlapRequest_Command_Is_GetEnrollmentActivity()
        {
            //arragne
            _getEnrollmentActivity.SearchParameter.EnrollmentId = "11111";
            
            //act
            DlapRequest request = _getEnrollmentActivity.ToRequest();

            //assert
            Assert.AreEqual("GetEnrollmentActivity", request.Parameters["cmd"]);
        }

        [TestMethod]
        public void Verify_ParseResponse_Returns_EnrollmentActivity()
        {
            //arrage
            DlapResponse response = new DlapResponse() { ResponseXml = new XDocument()};

            //act
            _getEnrollmentActivity.ParseResponse(response);

            //assert
            Assert.IsNotNull(_getEnrollmentActivity.EnrollmentActivity);
        }

        [TestMethod]
        public void Verify_ParseResponse_Returns_EnrollmentActivity_When_Nodes_Is_Not_Null_Or_Empty()
        {
            //arrage
            string fakeXml = "<root><activity itemid=\"2222\" date=\"01/01/2013\" second=\"20\">one</activity><activity itemid=\"3333\" date=\"01/02/2013\" second=\"30\">two</activity></root>";

            _getEnrollmentActivity.SearchParameter.EnrollmentId = "11111";

            DlapResponse response = new DlapResponse() { ResponseXml = XDocument.Parse(fakeXml) };
            
            //act
            _getEnrollmentActivity.ParseResponse(response);

            //assert
            Assert.IsNotNull(_getEnrollmentActivity.EnrollmentActivity);
        }
        
        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Throw_Exception_If_Response_Code_Is_Error()
        {
            //arrange
            DlapResponse response = new DlapResponse(){ Code = DlapResponseCode.Error, ResponseXml = new XDocument()};
            
            //act
            _getEnrollmentActivity.ParseResponse(response);

        }


    }
}
