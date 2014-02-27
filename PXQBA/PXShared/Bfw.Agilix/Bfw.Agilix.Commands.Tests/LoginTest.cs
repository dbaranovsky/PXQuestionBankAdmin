using System;
using System.Globalization;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Bfw.Agilix.Commands.Tests
{
	[TestClass]
	public class LoginTest
	{
		private Bfw.Agilix.Commands.Login login;

		private const string DUMMY_LoginXml = "<user userid='7' username='testUser' firstname='' lastname='' email='test@test.com' domainid='1' domainname='Root' userspace='Root' token='^H^q8O^ZLcN|qZ8XlaJxB5RmqS*zJD75iB' />";


		[TestInitialize]
		public void TestInitialize()
		{
			this.login = new Login {Username = "test@test.com", Password = "testPassword"};
		}


		[TestMethod]
		public void LoginTest_Request_DlapRequest_Type_Should_Be_PostRequest()
		{
			var request = login.ToRequest();
			Assert.AreEqual(request.Type, DlapRequestType.Post);
		}

		[TestMethod]
		public void LoginTest_Request_DlapRequest_Parameters_Should_Have_Command_Login()
		{
			var request = login.ToRequest();
			Assert.AreEqual(request.Attributes["cmd"], "login");
		}

		[TestMethod]
		public void LoginTest_ToRequest()
		{
			var request = login.ToRequest();
			Assert.IsNotNull(request);
		}


		[TestMethod]
		[ExpectedException(typeof(DlapAuthenticationException), "Could not authenticate user test@test.com")]
		public void LoginTest_ParseResponse_Should_Throw_Exception_If_Response_Code_Is_Not_OK()
		{
			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.Error
			};
			login.ParseResponse(dlapResponse);
		}

		[TestMethod]
		public void LoginTest_ParseResponse_OK()
		{

			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.OK,
				ResponseXml = XDocument.Parse(DUMMY_LoginXml)
			};

			login.ParseResponse(dlapResponse);

			Assert.IsNotNull(dlapResponse);
			Assert.IsNotNull(login.User);
			Assert.AreEqual(login.User.Email, login.Username);
			Assert.AreEqual(login.User.Id, "7");
			Assert.AreEqual(login.User.Domain.Id, "1");
			Assert.AreEqual(login.User.Domain.Name, "Root");
		}




	}
}
