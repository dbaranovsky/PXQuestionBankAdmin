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
	public class GetUsersBatchTest
	{
	  private const string DUMMY_GetUsersBatchXml = "<users>" +
	                                                "<user userid='1' username='test1' email='test1@tst.com' domainid='1' domainname='Root' />" +
													"<user userid='2' username='test2' email='test2@tst.com' domainid='1' domainname='Root' />" +
													"<user userid='3' username='test3' email='test3@tst.com' domainid='1' domainname='Root' />" +
	                                                "</users>";

	  private const string DUMMY_OneUserBatchXml = "<user userid='1' username='test1' email='test1@tst.com' domainid='1' domainname='Root' />";

	  private const string DUMMY_NoUserBatchXml = "<nouser/>";

	  private Bfw.Agilix.Commands.GetUsersBatch getUsersBatch;
	  private readonly List<UserSearch> lstUserSearch = new List<UserSearch>()
		                                         {
													new UserSearch() {DomainId = "1", ExternalId = "7", Id = "1", Name = "test1", Username = "test1@tst.com"},
													new UserSearch() {DomainId = "1", ExternalId = "8", Id = "2", Name = "test2", Username = "test2@tst.com"},
													new UserSearch() {DomainId = "1", ExternalId = "9", Id = "3", Name = "test3", Username = "test3@tst.com"}
												 };
		
		[TestInitialize]
		public void TestInitialize()
		{
			this.getUsersBatch = new GetUsersBatch();
			getUsersBatch.SearchParameters = lstUserSearch;
		}

		[TestMethod]
		public void GetUsersBatchTest_Request_DlapRequest_Type_Should_Be_ModeBatch()
		{
			var request = getUsersBatch.ToRequest();
			Assert.AreEqual(request.Mode, DlapRequestMode.Batch);

			var xBody = request.GetXmlRequestBody();
			Assert.IsNotNull(xBody);
			Assert.IsNotNull(xBody.Root);
			Assert.AreEqual(xBody.Root.Name, "batch");
		}

		[TestMethod]
		public void GetUsersBatchTest_Request_DlapRequest_Type_Should_Be_PostRequest()
		{

			var request = getUsersBatch.ToRequest();
			Assert.AreEqual(request.Type, DlapRequestType.Post);
		}


		[TestMethod]
		public void GetUsersBatchTest_ToRequest()
		{
			var request = getUsersBatch.ToRequest();
			Assert.IsNotNull(request);
			var xBody = request.GetXmlRequestBody();
			Assert.IsNotNull(xBody);
			Assert.IsNotNull(xBody.Root);
			Assert.AreEqual(xBody.Root.Name,"batch");
			Assert.AreEqual(xBody.Root.Elements().Count(), 3);
		}


		[TestMethod]
		[ExpectedException(typeof(DlapException), "GetUsers batch request failed with response code Error")]
		public void GetUsersBatchTest_ParseResponse_Should_Throw_Exception_Response_Code_Is_Error()
		{
			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.Error
			};
			getUsersBatch.ParseResponse(dlapResponse);
		}

		[TestMethod]
		public void GetUsersBatchTest_ParseResponse_Response_Code_Is_BadRequest_With_BadExtendedId()
		{
			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.OK
			};

			var dlapResponseType = typeof(DlapResponse);
			var BatchPropertyInfo = dlapResponseType.GetProperty("Batch");
			var batch = new[] { new DlapResponse() { Code = DlapResponseCode.BadRequest, 
			                                         ResponseXml = XDocument.Parse(DUMMY_NoUserBatchXml),
													 Message = "Bad ExtendedId"} };

			BatchPropertyInfo.SetValue(dlapResponse, batch, null);

			getUsersBatch.ParseResponse(dlapResponse);

			Assert.IsNotNull(dlapResponse);

			Assert.IsNotNull(getUsersBatch.Failures);
			Assert.AreEqual(getUsersBatch.Failures.Count, 1);
			Assert.AreEqual(getUsersBatch.Failures[0].Reason, "Bad ExtendedId");
		}


		[TestMethod]
		public void GetUsersBatchTest_ParseResponse_Response_Code_Is_AccessDenied_()
		{
			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.OK
			};

			var dlapResponseType = typeof(DlapResponse);
			var BatchPropertyInfo = dlapResponseType.GetProperty("Batch");
			var batch = new[] { new DlapResponse() { Code = DlapResponseCode.AccessDenied, 
			                                         ResponseXml = XDocument.Parse(DUMMY_NoUserBatchXml)} };

			BatchPropertyInfo.SetValue(dlapResponse, batch, null);

			getUsersBatch.ParseResponse(dlapResponse);
			
			Assert.IsNotNull(dlapResponse);

			Assert.IsNotNull(getUsersBatch.Failures);
			Assert.AreEqual(getUsersBatch.Failures.Count, 1);
			Assert.AreEqual(getUsersBatch.Failures[0].Reason, "AccessDenied");
		}

		[TestMethod]
		public void GetUsersBatchTest_ParseResponse_OK()
		{

			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.OK
			};

			getUsersBatch.ParseResponse(dlapResponse);

			Assert.IsNotNull(dlapResponse);

			Assert.IsNotNull(getUsersBatch.Failures);
			Assert.AreEqual(getUsersBatch.Failures.Count, 0);

		}

		[TestMethod]
		public void GetUsersBatchTest_ParseResponse_UsersBatch_WhenResponse_OK()
		{

			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.OK
			};

			var dlapResponseType = typeof(DlapResponse);
			var BatchPropertyInfo = dlapResponseType.GetProperty("Batch");
			var batch = new[]
				{new DlapResponse() {Code = DlapResponseCode.OK, ResponseXml = XDocument.Parse(DUMMY_GetUsersBatchXml)}};				

			BatchPropertyInfo.SetValue(dlapResponse, batch, null);

			getUsersBatch.ParseResponse(dlapResponse);

			Assert.IsNotNull(dlapResponse);
			Assert.IsNotNull(getUsersBatch.Failures);
			Assert.AreEqual(getUsersBatch.Failures.Count, 0);

			Assert.IsNotNull(getUsersBatch.Users);
			Assert.AreEqual(getUsersBatch.Users.Count,1);

			var agilixUsersList = getUsersBatch.Users[0];

			Assert.IsNotNull(agilixUsersList);
			Assert.AreEqual(agilixUsersList.Count, 3);
		}

		[TestMethod]
		public void GetUsersBatchTest_ParseResponse_UsersBatch_WhenResponse_OK_ForSingleUser()
		{

			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.OK
			};

			var dlapResponseType = typeof(DlapResponse);
			var BatchPropertyInfo = dlapResponseType.GetProperty("Batch");
			var batch = new[] { new DlapResponse() { Code = DlapResponseCode.OK, ResponseXml = XDocument.Parse(DUMMY_OneUserBatchXml) } };

			BatchPropertyInfo.SetValue(dlapResponse, batch, null);

			getUsersBatch.ParseResponse(dlapResponse);

			Assert.IsNotNull(dlapResponse);
			Assert.IsNotNull(getUsersBatch.Failures);
			Assert.AreEqual(getUsersBatch.Failures.Count, 0);

			Assert.IsNotNull(getUsersBatch.Users);
			Assert.AreEqual(getUsersBatch.Users.Count, 1);

			var agilixUsersList = getUsersBatch.Users[0];

			Assert.IsNotNull(agilixUsersList);
			Assert.AreEqual(agilixUsersList.Count, 1);
		}

		[TestMethod]
		[ExpectedException(typeof(BadDlapResponseException), "GetUsers expected a response element of 'user' or 'users', but got nouser instead.")]
		public void GetUsersBatchTest_ParseResponse_WhenResponse_BadDlapResponseException()
		{

			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.OK,
				ResponseXml = XDocument.Parse(DUMMY_NoUserBatchXml)
			};

			var dlapResponseType = typeof(DlapResponse);
			var BatchPropertyInfo = dlapResponseType.GetProperty("Batch");
			var batch = new[] { new DlapResponse() { Code = DlapResponseCode.None,  ResponseXml = XDocument.Parse(DUMMY_NoUserBatchXml) }  };

			BatchPropertyInfo.SetValue(dlapResponse, batch, null);

			getUsersBatch.ParseResponse(dlapResponse);

		}


	}
}
