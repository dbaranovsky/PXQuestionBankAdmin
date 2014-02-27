using Bfw.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Xml.Linq;
using System.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetUsersTest
    {
        private GetUsers getUsers;

        [TestInitialize]
        public void TestInitialize()
        {
            this.getUsers = new GetUsers();
        }

        [TestMethod]
        public void GetUsersTest_Request_Type_Should_Be_Get()
        {
            getUsers.SearchParameters = new UserSearch() { Id = "99999" };            
            var request = getUsers.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Get);
        }

        [TestMethod]
        public void GetUsersTest_Request_Parameter_Should_Have_Command_GetUser_If_UserId_Is_Not_Null()
        {
            getUsers.SearchParameters = new UserSearch() { Id = "99999" };
            var request = getUsers.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "getuser");
        }

        [TestMethod]
        public void GetUsersTest_Request_Parameter_Should_Have_Command_GetUserList_If_UserId_Is_Null()
        {
            getUsers.SearchParameters = new UserSearch() { Id = null };
            var request = getUsers.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "getuserlist");
        }

        [TestMethod]
        public void GetUsersTest_Request_Username_Parameter_Should_Have_Same_Username()
        {
            getUsers.SearchParameters = new UserSearch() { ExternalId = "2000" };
            var request = getUsers.ToRequest();
            Assert.AreEqual(request.Parameters["username"], "2000");
        }

        [TestMethod]
        public void GetUsersTest_Request_Username_Parameter_Should_Be_Null_If_Not_Provided_In_SearchParameter()
        {
            getUsers.SearchParameters = new UserSearch() { Username = "" };            
            var request = getUsers.ToRequest();
            Assert.AreEqual(request.Parameters.ContainsKey("username"), false);
        }

        [TestMethod]
        public void GetUsersTest_Request_Username_Parameter_Should_Be_Null_If_UserId_Is_Not_Null()
        {
            getUsers.SearchParameters = new UserSearch() { Id = "99999" , Username = "root" };
            var request = getUsers.ToRequest();
            Assert.AreEqual(request.Parameters.ContainsKey("username"), false);
        }

        [TestMethod]
        public void GetUsersTest_Request_Parameters_Has_Same_Values_as_SearchParameter()
        {
            getUsers.SearchParameters = new UserSearch();
            getUsers.SearchParameters.Id = null;
            getUsers.SearchParameters.Username = "pxmigration";
            getUsers.SearchParameters.Name = "admin";
            getUsers.SearchParameters.DomainId = "1000";
            getUsers.SearchParameters.ExternalId = "2000";
            var request = getUsers.ToRequest();
            Assert.AreEqual(request.Parameters["username"], "2000");
            Assert.AreEqual(request.Parameters["name"], "admin");
            Assert.AreEqual(request.Parameters["domainid"], "1000");
        }

        [TestMethod]
        public void GetUsersTest_ParseResponse_Users_Should_Have_Single_User()
        {                    
            string responseString = string.Format("<response code=\"OK\"><user userid=\"99999\" firstname=\"pxmigration\" lastname=\"user\" domainid=\"1000000\" username=\"pxmigration\"><data></data></user></response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));
           
            getUsers.ParseResponse(dlapResponse);
            Assert.AreEqual(getUsers.Users.Count(), 1);
        }

        [TestMethod]
        public void GetUsersTest_ParseResponse_Users_Count_Match_With_Response()
        {
            string responseString = string.Format("<response code=\"OK\">" +
                            "<users>" + 
                            "<user userid=\"99999\" firstname=\"pxmigration\" lastname=\"user\" domainid=\"1000000\" username=\"pxmigration\"><data></data></user>" +
                            "<user userid=\"88888\" firstname=\"test\" lastname=\"testuser\" domainid=\"1000000\" username=\"test\"><data></data></user>" +
                            "</users>" + 
                            "</response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));

            getUsers.ParseResponse(dlapResponse);
            Assert.AreEqual(getUsers.Users.Count(), 2);
        }

        [TestMethod]
        public void GetUsersTest_ParseResponse_Users_Should_Have_Empty_User_When_AccessDenied()
        {
            string responseString = string.Format("<response code=\"AccessDenied\"></response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));

            getUsers.ParseResponse(dlapResponse);
            Assert.AreEqual(getUsers.Users.First(), null);
        }

        [TestMethod]
        public void GetUsersTest_ParseResponse_Users_Should_Have_Empty_User_When_BadRequest()
        {
            string responseString = string.Format("<response code=\"BadRequest\" message=\"Bad ExtendedId\"></response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));

            getUsers.ParseResponse(dlapResponse);
            Assert.AreEqual(getUsers.Users.First(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(BadDlapResponseException), "GetUsers expected a response element of 'user' or 'users', but got test instead.")]
        public void GetUsersTest_ParseResponse_Should_Throw_Exception_If_Root_Value_Is_Diffrent()
        {
            string responseString = string.Format("<response code=\"OK\"><test></test></response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));
            getUsers.ParseResponse(dlapResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "GetUsers command failed with code Format")]
        public void GetUsersTest_ParseResponse_Should_Throw_Exception_If_Response_Code_Is_Not_Ok()
        {
            string responseString = string.Format("<response code=\"Format\"><user></user></response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));
            getUsers.ParseResponse(dlapResponse);
        }

    }
}
