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
	public class PutAnnouncementTest
	{
		private Bfw.Agilix.Commands.PutAnnoucement putAnnouncement;

		private static readonly Announcement DUMMY_Announcement =
		 new Bfw.Agilix.DataContracts.Announcement(){CreationDate = Convert.ToDateTime("2013-04-24T09:58:15.75Z"), 
													 EndDate =Convert.ToDateTime("2013-04-25T23:59:00Z"), 
													 EntityId = "6153",
													 Html = "<b>Final Exam moved to Friday</b>", 
													 Path = "02e839940ac34dd3ae466b42f61e6418.zip",
													 PinSortOrder = "1", 
													 PrimarySortOrder = "1", 
													 StartDate = Convert.ToDateTime("2013-04-24T00:00:00Z"), 
													 Title = "Final Exam moved to Friday", 
													 Version = "2"};
        

		[TestInitialize]
		public void TestInitialize()
		{
			this.putAnnouncement = new PutAnnoucement();
			putAnnouncement.Announcement = DUMMY_Announcement;
			putAnnouncement.DomainId = "6153";
			putAnnouncement.Path = "02e839940ac34dd3ae466b42f61e6418.zip";
		}


		[TestMethod]
		public void PutAnnoucementTest_Request_DlapRequest_Type_Should_Be_PostRequest()
		{
			var request = putAnnouncement.ToRequest();
			Assert.AreEqual(request.Type, DlapRequestType.Post);
		}
	
		[TestMethod]
		public void PutAnnoucementTest_Request_DlapRequest_Parameters_Should_Have_Command_PutAnnoucement()
		{

			var request = putAnnouncement.ToRequest();
			Assert.AreEqual(request.Parameters["cmd"], "putannouncement");
		}

		[TestMethod]
		public void PutAnnoucementTest_ToRequest()
		{
			var request = putAnnouncement.ToRequest();
			Assert.IsNotNull(request);
		}

		[TestMethod]
		public void PutAnnoucementTest_BuildRequestStream()
		{
			Type putAnnouncementType = typeof(Bfw.Agilix.Commands.PutAnnoucement);
			PropertyInfo AnnouncementPropertyInfo = putAnnouncementType.GetProperty("Announcement");
			MethodInfo m = putAnnouncementType.GetMethod("BuildRequestStream", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);

			var putAnnouncementObj = Activator.CreateInstance(putAnnouncementType);

			AnnouncementPropertyInfo.SetValue(putAnnouncementObj, putAnnouncement.Announcement, null);

			var actualResult = m.Invoke(putAnnouncementObj, new object[] { });

			Assert.IsNotNull(actualResult);
			Assert.AreEqual(actualResult.GetType().ToString(), "System.IO.MemoryStream");	

		}

		[TestMethod]
		[ExpectedException(typeof(DlapException), "PutAnnoucement request failed with response code Error.")]
		public void PutAnnoucementTest_ParseResponse_Should_Throw_Exception_If_Response_Code_Is_Not_OK()
		{
			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.Error
			};
			putAnnouncement.ParseResponse(dlapResponse);
		}

		[TestMethod]
		public void PutAnnoucementTest_ParseResponse_OK()
		{

			var dlapResponse = new DlapResponse()
			{
				Code = DlapResponseCode.OK,
				ResponseXml = XDocument.Parse("<announcement><version>2</version></announcement>")
			};
			
			putAnnouncement.ParseResponse(dlapResponse);

			Assert.IsNotNull(dlapResponse);
		}




	}
}
