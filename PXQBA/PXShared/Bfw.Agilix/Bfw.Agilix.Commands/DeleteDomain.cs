using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Commands
{
    public class DeleteDomain : DlapCommand
    {
        public Domain Domain { get; set; }

        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>()
            };
            request.Parameters.Add("cmd", "deletedomain");
            request.Parameters.Add("domainid", Domain.Id);
            request.AppendData(Domain.ToEntity());

            return request;
        }

        public override void ParseResponse(Dlap.DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("DeleteUsers request failed with response code {0}", response.Code));
            }
        }
    }
}
