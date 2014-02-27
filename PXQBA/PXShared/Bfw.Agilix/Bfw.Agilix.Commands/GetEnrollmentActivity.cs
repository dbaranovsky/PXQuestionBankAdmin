using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using System.Text;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

using Bfw.Common.Collections;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands
{

    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetEnrollmentActivity command.
    /// </summary>
    public class GetEnrollmentActivity : DlapCommand
    {
        /// <summary>
        /// Gets or sets the search parameter.
        /// </summary>
        public EnrollmentActivitySearch SearchParameter { get; set; }

        /// <summary>
        /// Gets or sets the enrollment activity document.
        /// </summary>
        public XDocument EnrollmentActivity { get; protected set; }

        /// <summary>
        /// Activity from command response.
        /// </summary>
        public IEnumerable<ItemActivity> Activity { get; set; }

        #region Overrides from DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/.
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "GetEnrollmentActivity" },
                    { "enrollmentid", SearchParameter.EnrollmentId }
                }
            };

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (response.ResponseXml != null)
            {
                EnrollmentActivity = ParseXmlResponse(response);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Parses the XML response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        protected XDocument ParseXmlResponse(DlapResponse response)
        {
            if (response.Code != DlapResponseCode.OK)
            {
                throw new DlapException(response.Message);
            }

            var doc = response.ResponseXml;
            var nodes = doc.XPathSelectElements("//activity");

            if (!nodes.IsNullOrEmpty())
            {
                Activity = from n in nodes
                           select new ItemActivity()
                           {
                               EnrollmentId = SearchParameter.EnrollmentId,
                               ItemId = n.Attribute("itemid").Value,
                               StartTime = DateTime.Parse(n.Attribute("date").Value),
                               Seconds = int.Parse(n.Attribute("seconds").Value)
                           };
            }
            else
            {
                Activity = new List<ItemActivity>();
            }

            var listElm = response.ResponseXml.Root;

            return doc;
        }

        #endregion
    }
}
