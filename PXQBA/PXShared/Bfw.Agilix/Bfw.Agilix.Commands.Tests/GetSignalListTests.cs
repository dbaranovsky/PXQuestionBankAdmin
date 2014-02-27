namespace Bfw.Agilix.Commands.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Bfw.Agilix.DataContracts;
    using Bfw.Agilix.Dlap;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GetSignalListTests
    {
        #region Constants and Fields

        private GetSignalList _getSignalList;

        private SignalSearch _signalSearch;

        #endregion

        #region Public Methods and Operators

        [TestMethod]
        [ExpectedException(typeof(DlapException),
            "GetSignalList expected a signal element, but got bad response")]
        // ReSharper disable once InconsistentNaming
        public void GetSignalListTests_No_Parse_With_Some_ResponseXml()
        {
            var response = new DlapResponse { Code = DlapResponseCode.BadRequest, ResponseXml = new XDocument() };
            this._getSignalList.ParseResponse(response);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetSignalListTests_Parse_With_Null_ResponseXml()
        {
            var response = new DlapResponse { Code = DlapResponseCode.OK };
            this._getSignalList.ParseResponse(response);
            Assert.AreEqual(0, this._getSignalList.SignalList.Count);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetSignalListTests_Type_Is_Get()
        {
            var request = this._getSignalList.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Get);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetSignalListTests_With_SignalList_Data_Sample1()
        {
            const string testString =
                @"<response code=""OK""><signals><signal signalid=""486598036"" domainid=""88332"" type=""1.1"" entityid=""116160"" creationdate=""2013-07-05T06:48:28.877Z"" creationby=""116160"" /><signal signalid=""486598037"" domainid=""8"" type=""1.1"" entityid=""48"" creationdate=""2013-07-05T06:48:42.127Z"" creationby=""48"" /></signals></response>";
            var response = new DlapResponse(XDocument.Parse(testString));
            this._getSignalList.ParseResponse(response);

            var signalfound =
                (from el in this._getSignalList.SignalList 
                    where el.DomainId == "88332"
                    select el).FirstOrDefault();

            Assert.IsNotNull(signalfound);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void GetSignalListTests_With_SignalList_Data_Sample2()
        {
            const string testString =
                @"<response code=""OK""><signals><signal signalid=""486600966"" domainid=""90377"" type=""4.4"" entityid=""109884"" creationdate=""2013-07-05T17:02:04.517Z"" creationby=""7""><data entitytype=""C"" questionid=""9fcafa97_5b1f_40d4_8610_bde9f118f895"" /></signal><signal signalid=""486601666"" domainid=""90377"" type=""4.4"" entityid=""109884"" creationdate=""2013-07-05T20:29:24.967Z"" creationby=""102950""><data entitytype=""C"" questionid=""aa0f5241_6ac1_4f6a_83fd_b40964526622"" /></signal></signals></response>";
            var response = new DlapResponse(XDocument.Parse(testString));
            this._getSignalList.ParseResponse(response);

            var signalfound =
                (from el in this._getSignalList.SignalList
                    where el.QuestionId == "9fcafa97_5b1f_40d4_8610_bde9f118f895"
                    select el).FirstOrDefault();

            Assert.IsNotNull(signalfound);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._getSignalList = null;
            this._signalSearch = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this._signalSearch = new SignalSearch { LastSignalId = "0", SignalType = "" };
            this._getSignalList = new GetSignalList { SearchParameter = this._signalSearch };
        }

        #endregion
    }
}