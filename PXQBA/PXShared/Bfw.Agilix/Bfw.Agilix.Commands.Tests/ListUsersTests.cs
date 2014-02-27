using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using TestHelper;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    /// <summary>
    /// Provides tests for the ListUsers DLAP command.
    /// </summary>
    [TestClass]
    public class ListUsersTests
    {
        /// <summary>
        /// Confirms ability to search for users by email address.
        /// </summary>
        [TestMethod]
        public void CanSearchByEmail()
        {
            // arrange
            var cmd = new ListUsers
            {
                Query = "/email='inigomontoya@prepare2die.org'"
            };

            // act
            var request = cmd.ToRequest();

            // assert
            Assert.AreEqual("0", request.Parameters["domainid"]);
            Assert.AreEqual("/email='inigomontoya@prepare2die.org'", request.Parameters["query"]);
        }

        /// <summary>
        /// Confirms ability to parse user data contained in a DlapResponse.
        /// </summary>
        [TestMethod]
        public void CanParseResponseContainingOnlyUsers()
        {
            // arrange
            var xmlResponse = @"
<response code=""OK"">
  <users>
    <user id=""1258"" firstname=""Arthur"" lastname=""Admin"" reference=""112233""
          guid=""e41614be-5659-4d99-b03f-7615fe876f9e"" username=""admin"" 
          email=""admin@myschool.edu"" domainid=""24"" flags=""0"" createdby=""10"" 
          creationdate=""2007-11-12T23:04:48.11Z"" modifiedby=""10"" 
          modifieddate=""2007-11-12T23:04:48.11Z"" version=""1"" 
          lastlogindate=""2007-11-13T23:04:48.11Z""/>
    <user id=""27"" firstname=""Tiger"" lastname=""Jones"" reference=""111222"" 
          guid=""9e3b3650-37da-4324-bf96-716c851c8daa"" username=""teacher"" 
          email=""tiger.jones@myschool.edu"" domainid=""24"" flags=""0"" createdby=""10"" 
          creationdate=""2007-06-07T17:17:46.3Z"" modifiedby=""10"" 
          modifieddate=""2007-10-21T23:10:01.14Z"" version=""5"" 
          lastlogindate=""2007-11-12T13:51:44.29Z""/>
  </users>
</response>";

            var response = new DlapResponse(XDocument.Parse(xmlResponse));
            response.Code = DlapResponseCode.OK;

            var cmd = new ListUsers
            {
                Query = "/email='inigomontoya@prepare2die.org'"
            };

            // act
            cmd.ParseResponse(response);

            // assert
            Assert.AreEqual(2, cmd.Users.Count());
            Assert.AreEqual("Arthur", cmd.Users[0].FirstName);
            Assert.AreEqual("teacher", cmd.Users[1].UserName);
            Assert.AreEqual("tiger.jones@myschool.edu", cmd.Users[1].Email);
        }

        [TestMethod]
        [ExpectedException(typeof(Bfw.Agilix.Dlap.DlapException))]
        public void ThrowExceptionWhenDLAPCallErrorsOut()
        {
            var response = new DlapResponse();
            response.Code = DlapResponseCode.NullReference;

            var cmd = new ListUsers
            {
                Query = "/email='inigomontoya@prepare2die.org'"
            };

            // act
            cmd.ParseResponse(response);

            // assert
        }

        [TestMethod]
        public void CanParseEmptyResponseWhenThereAreNoMatches()
        {
            // arrange
            var xmlResponse = @"
<response code=""OK"">
  <users />
</response>";

            var response = new DlapResponse(XDocument.Parse(xmlResponse));
            response.Code = DlapResponseCode.OK;

            var cmd = new ListUsers
            {
                Query = "/email='inigomontoya@prepare2die.org'"
            };

            // act
            cmd.ParseResponse(response);

            // assert
            Assert.AreEqual(0, cmd.Users.Count());
        }

        /// <summary>
        /// Confirms ability to search for users by email address.
        /// </summary>
        [TestMethod]
        public void CanSearchSpecificDomain()
        {
            // arrange
            var cmd = new ListUsers
            {
                DomainId = "1234",
                Query = "/email='inigomontoya@prepare2die.org'"
            };

            // act
            var request = cmd.ToRequest();

            // assert
            Assert.AreEqual("1234", request.Parameters["domainid"]);
            Assert.AreEqual("/email='inigomontoya@prepare2die.org'", request.Parameters["query"]);
        }
        
        /// <summary>
        /// Confirms an exception will be thrown if there is no DomainId specified.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void WillThrowExceptionIfDomainIdIsMissing()
        {
            // arrange
            var cmd = new ListUsers
            {
                DomainId = null,
                Query = "/email='inigomontoya@prepare2die.org'"
            };

            // act
            var request = cmd.ToRequest();

            // assert
        }
    }
}
