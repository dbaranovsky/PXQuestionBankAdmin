using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using DlapShip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DlapShipTest
{
    [TestClass]
    public class DeleteDlapItemIntegrationTest
    {
        private const string DeleteDlapItemFile1 = "../../../TestScripts/DeleteDlapItem1.xml";
        private string _authToken = string.Empty;
        /// <summary>
        /// Gets the authentication token for making dlap requests against the dev dalp instance
        /// </summary>
        /// <returns>Dev DLAP authentication token</returns>
        public string GetAuthToken()
        {
            string retval = string.Empty;

            //Get an auth cookie
            HttpWebRequest request = WebRequest.CreateHttp("http://dev.dlap.bfwpub.com/dlap.ashx");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string content = "cmd=login&username=root%2Fpxmigration&password=Px-Migration-123&submit=Login";
            byte[] contentbytes = Encoding.ASCII.GetBytes(content);
            request.ContentLength = contentbytes.Length;
            Stream contentStream = request.GetRequestStream();
            contentStream.Write(contentbytes, 0, contentbytes.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            XmlDocument responseDoc = new XmlDocument();
            responseDoc.Load(response.GetResponseStream());
            XmlNode node = responseDoc.SelectSingleNode("/response/user");
            XmlAttribute token = node.Attributes["token"];
            retval = token.Value;

            return retval;
        }

        /// <summary>
        /// Gets the XML string of a getitem request again dev DLAP for <paramref name="itemdid"/> and <paramref name="entityid"/>
        /// </summary>
        /// <param name="itemid">id of the item to get the response for</param>
        /// <param name="entityid">id of the course to get the response for</param>
        /// <returns>Response to a getitem dlap request using itemid and entityid</returns>
        public string GetItemResponse(string itemid, string entityid)
        {
            HttpWebRequest request = WebRequest.CreateHttp(string.Format("http://dev.dlap.bfwpub.com/dlap.ashx?getitemlist&itemid={0}&entityid={1}", itemid, entityid));
            request.Method = "GET";
            Cookie authcookie = new Cookie("AZT", _authToken, string.Empty, "dev.dlap.bfwpub.com");
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(authcookie);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            XmlDocument responseDoc = new XmlDocument();
            responseDoc.Load(response.GetResponseStream());
            return responseDoc.InnerXml;
        }

        /// <summary>
        /// Inserts a test item into the dev dlap instance that should be removed in DeleteItem1 test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _authToken = GetAuthToken();

            //Use cookie to add item to dlap.
            HttpWebRequest putrequest = WebRequest.CreateHttp("http://dev.dlap.bfwpub.com/dlap.ashx?putitems");
            putrequest.Method = "POST";
            putrequest.ContentType = "text/xml; charset=UTF-8";
            putrequest.CookieContainer = new CookieContainer();
            Cookie authcookie = new Cookie("AZT", _authToken, string.Empty, "dev.dlap.bfwpub.com");
            putrequest.CookieContainer.Add(putrequest.RequestUri, authcookie);
            byte[] putdata = Encoding.ASCII.GetBytes(@"<requests>
  <item itemid=""test1"" entityid=""128393"">
    <data>
      <parent>TESTING</parent>
      <type>CustomActivity</type>
    </data>
  </item>
</requests>");
            putrequest.ContentLength = putdata.Length;
            Stream dataStream = putrequest.GetRequestStream();
            dataStream.Write(putdata, 0, putdata.Length);
            HttpWebResponse putresponse = (HttpWebResponse)putrequest.GetResponse();
            XmlDocument putresponseDoc = new XmlDocument();
            putresponseDoc.Load(putresponse.GetResponseStream());
            XmlNode putnode = putresponseDoc.SelectSingleNode("/response/responses/response");
            XmlAttribute code = putnode.Attributes["code"];

        }

        [TestMethod]
        public void DeleteDlapItemAction_DeleteItem1()
        {
            PrivateType pt = new PrivateType(typeof(Program));
            pt.InvokeStatic("Main", new object[] 
            { 
                new string[] 
                { 
                    string.Format("/d:{0}", DeleteDlapItemFile1),
                    "/e:dev",
                    "/app:xbook",
                    "/id:128393"
                } 
            });

            string xmlresponse = GetItemResponse("test", "128393");

            Assert.IsTrue(xmlresponse.Contains("ResourceNotFound"));
        }
    }
}
