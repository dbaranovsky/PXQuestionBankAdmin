using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides access to the xml response for any GET command
    /// </summary>
    public class GetRawResponse : DlapCommand
    {
        /// <summary>
        /// Add all the parameters required for this command
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Access the response
        /// </summary>
        public XDocument XmlResponse { get; private set; }

        public override Dlap.DlapRequest ToRequest()
        {
            var request = new DlapRequest();
            request.Parameters = Parameters;
            request.Type = DlapRequestType.Get;
            return request;
        }

        public override void ParseResponse(Dlap.DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("GetRawResponse command failed with code {0}", response.Code));
            }
            XmlResponse = response.ResponseXml;
        }
    }
}
