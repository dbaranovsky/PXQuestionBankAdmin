using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    public class GetStatus : DlapCommand
    {
        public XDocument Status { get; set; }

        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest
            {
                Type = DlapRequestType.Get,
                Mode = DlapRequestMode.Single,
                Parameters = new Dictionary<string, object> {
                    { "cmd", "getstatus" }
                }
            };

            return request;
        }

        public override void ParseResponse(DlapResponse response)
        {
            if (response.Code != DlapResponseCode.OK)
                throw new DlapException("Could not get status from DLAP Server");

            Status = response.ResponseXml;
        }
    }
}
