namespace Bfw.Agilix.Commands.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using Bfw.Agilix.DataContracts;
    using Bfw.Agilix.Dlap;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestHelper;

    [TestClass]
    public class GetAnnouncementsTests
    {
        #region Constants and Fields

        private GetAnnouncements _getGetAnnouncements;
        private AnnouncementSearch _announcementSearch;

        #endregion

        #region Public Methods and Operators

        [TestInitialize]
        public void TestInitialize()
        {
            this._announcementSearch = new AnnouncementSearch { EntityId = "130247", Limit = 0, Date = DateTime.Now, Path = "e362a72809af4dd882797522d9db6c61.zip" };
            this._getGetAnnouncements = new GetAnnouncements { SearchParameters = this._announcementSearch };
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetAnnouncementsTests_Type_Is_Get()
        {
            var request = this._getGetAnnouncements.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Get);
            Assert.AreEqual(request.Parameters["cmd"], "getannouncement");
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetAnnouncementsTests_Type_Is_GetAnnounementList()
        {
            var search = new AnnouncementSearch { EntityId = "130247" };
            var request = new GetAnnouncements { SearchParameters = search }.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Get);
            Assert.AreEqual(request.Parameters["cmd"].ToString().ToLowerInvariant(), "getannouncementlist");
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetAnnouncementsTests_With_AnnouncementXml_Data_Sample1()
        {
            const string testString =
                @"<response code=""OK""><announcements><announcement entityid=""130247"" path=""e362a72809af4dd882797522d9db6c61.zip"" title=""sample announcement"" version=""2"" startdate=""2013-08-12T06:00:00Z"" enddate=""9999-12-31T00:00:00Z"" recurse=""true"" hasurl=""false"" creationdate=""2013-08-12T20:35:58.823Z"" modifieddate=""2013-08-12T20:36:56.423Z"" /><announcement entityid=""130247"" path=""e362a72809af4dd882797522d9db6c62.zip"" title=""sample announcement"" version=""1"" startdate=""2013-08-12T06:00:00Z"" enddate=""9999-12-31T00:00:00Z"" recurse=""true"" hasurl=""false"" creationdate=""2013-08-12T20:37:07.827Z"" modifieddate=""2013-08-12T20:37:07.827Z"" /></announcements></response>";
            var response = new DlapResponse(XDocument.Parse(testString));
            this._getGetAnnouncements.ParseResponse(response);

            Assert.AreEqual(this._getGetAnnouncements.Announcements[0].EntityId, _announcementSearch.EntityId);
            Assert.IsNull(this._getGetAnnouncements.Announcements[0].Html);
            Assert.AreEqual(this._getGetAnnouncements.Announcements[0].StartDate, DateTime.Parse("2013-08-12T06:00:00Z"));
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetAnnouncementsTests_With_Single_Announcement_As_Stream()
        {
            string testString = "This is a test string";
            Zipper zipper = new Zipper("e362a72809af4dd882797522d9db6c61.zip");
            zipper.CreateZipFromByte(CreateAnnouncementWrapper(testString));

            var byteArray = File.ReadAllBytes(string.Format(".\\{0}", zipper.ZipFileName));
            var response = new DlapResponse { Code = DlapResponseCode.OK, ResponseStream = new MemoryStream(byteArray) };
            _getGetAnnouncements.ParseResponse(response);

            if (File.Exists(zipper.ZipFileName))
            {
                File.Delete(zipper.ZipFileName);
            }

            Assert.AreEqual(_getGetAnnouncements.Announcements[0].Html, testString);
        }

        private byte[] CreateAnnouncementWrapper(string testAnnouncement)
        {
            StringBuilder an = new StringBuilder().Append(@"<?xml version=""1.0""?>")
                .Append(@"<announcement title=""sample announcement"" recurse=""true"" enddate=""9999-12-31T00:00:00Z"" startdate=""2013-08-12T06:00:00Z"" entityid=""").Append(this._announcementSearch.EntityId).Append(@""" to=""Some University"">")
                .Append("<body>").Append(testAnnouncement).Append("</body>")
                .Append("</announcement>");

            return Helper.GetBytes(an.ToString());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._getGetAnnouncements = null;
            this._announcementSearch = null;
        }


        #endregion
    }
}
